using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace dbd_screenshot
{
    internal class MainPresenter
    {
        private readonly IMainView view;
        private readonly BackgroundWorker worker = new();
        private readonly ConfigManager configManager = new();

        private readonly ProcessWindowHelper processWindowHelper = new();
        private Screenshoter? screenshoter;
        private readonly ImageResizer resizer = new();
        private readonly Uploader uploader = new();

        private readonly string windowName = "DeadByDaylight-Win64-Shipping";

        public MainPresenter(IMainView view)
        {
            this.view = view;

            worker.ProgressChanged += Worker_ProgressChanged;
            worker.DoWork += Worker_DoWork;
            worker.WorkerReportsProgress = true;
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            IntPtr processHandle = GetDbdWindow();
            if(processHandle != IntPtr.Zero)
            {
                string rawScreenshotPath = TakeDbdScreenshot(processHandle);
                string resizedPath = ResizeImage(rawScreenshotPath);
                NightLightResponse? response = UploadImage(resizedPath);
                if(response?.statusCode == HttpStatusCode.OK)
                    Cleanup(rawScreenshotPath, resizedPath);
            }
        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null) return;

            switch ((ScreenshotSteps)e.ProgressPercentage)
            {
                case ScreenshotSteps.WINDOW:
                    view.UpdateFindDbdStatus((StatusWithLog)e.UserState);
                    break;
                case ScreenshotSteps.SCREENSHOT:
                    view.UpdateSnappingScreenshotStatus((StatusWithLog)e.UserState);
                    break;
                case ScreenshotSteps.RESIZE:
                    view.UpdateResizingPixelStatus((StatusWithLog)e.UserState);
                    break;
                case ScreenshotSteps.UPLOAD:
                    view.UpdateUploadStatus((StatusWithLog)e.UserState);
                    break;
                case ScreenshotSteps.CLEANUP:
                    view.UpdateCleanupStatus((StatusWithLog)e.UserState);
                    break;
                case ScreenshotSteps.MATCH_URL:
                    view?.ShowLinkOfMatch((string)e.UserState);
                    break;
                case ScreenshotSteps.MATCH_URL_HIDE:
                    view?.HideLinkOfMatch();
                    break;
                default:
                    Console.WriteLine("No behaviour handled for this event");
                    break;
            }
        }

        public void ReadTokenConfig()
        {
            view.OnTokenRead(ReadTokenFromConfigFile());
        }

        private string ReadTokenFromConfigFile()
        {
            return configManager.LoadConfig("auth", "");
        }

        public void WriteTokenConfig(string newAuthToken)
        {
            configManager.SaveConfig("auth", newAuthToken);
        }

        internal void StartScreenshotProcess()
        {
            if(!worker.IsBusy)
            {
                StatusWithLog log = new(Status.NONE);

                view.UpdateFindDbdStatus(log);
                view.UpdateSnappingScreenshotStatus(log);
                view.UpdateResizingPixelStatus(log);
                view.UpdateUploadStatus(log);
                view.UpdateCleanupStatus(log);
                view.HideLinkOfMatch();

                worker.RunWorkerAsync();
            }
        }

        private IntPtr GetDbdWindow()
        {
            worker.ReportProgress((int)ScreenshotSteps.WINDOW, getLog(Status.LOADING));
            IntPtr processHandle = processWindowHelper.GetWindowHandleByProcessName(windowName);
            if(processHandle == IntPtr.Zero)
                worker.ReportProgress((int)ScreenshotSteps.WINDOW, getLog(Status.ERROR, "Unable to get dbd window"));
            else
                worker.ReportProgress((int)ScreenshotSteps.WINDOW, getLog(Status.COMPLETE));
            return processHandle;
        }

        private string TakeDbdScreenshot(IntPtr processHandle)
        {
            screenshoter = new(processHandle);
            string path = Path.GetTempPath() + "raw_Screenshot.png";
            worker.ReportProgress((int)ScreenshotSteps.SCREENSHOT, getLog(Status.LOADING));
            screenshoter.TakeScreenshot(path);
            worker.ReportProgress((int)ScreenshotSteps.SCREENSHOT, getLog(Status.COMPLETE));
            return path;
        }

        private string ResizeImage(string rawScreenshotPath)
        {
            worker.ReportProgress((int)ScreenshotSteps.RESIZE, getLog(Status.LOADING));
            string resizedPath = resizer.ResizeAtPath(rawScreenshotPath);
            worker.ReportProgress((int)ScreenshotSteps.RESIZE, getLog(Status.COMPLETE));
            return resizedPath;
        }

        private NightLightResponse? UploadImage(string resizedScreenshotPath)
        {
            worker.ReportProgress((int)ScreenshotSteps.UPLOAD, getLog(Status.LOADING));
            // TODO: api key from config
            HttpResponseMessage res = uploader.UploadImage(ReadTokenFromConfigFile(), resizedScreenshotPath);
            HttpStatusCode statusCode = res.StatusCode;
            NightLightResponse? responseBody = res.Content.ReadFromJsonAsync<NightLightResponse>().Result;
            if (responseBody != null)
            {
                responseBody.statusCode = statusCode;
            }
            if(statusCode == HttpStatusCode.OK)
            {
                worker.ReportProgress((int)ScreenshotSteps.UPLOAD, getLog(Status.COMPLETE, "" + responseBody?.statusCode.ToString()));
                worker.ReportProgress((int)ScreenshotSteps.MATCH_URL, "" + responseBody?.Data?.Url);
            }
            else
            {
                worker.ReportProgress((int)ScreenshotSteps.UPLOAD, getLog(Status.ERROR, responseBody?.statusCode.ToString() + " - " + responseBody?.Error?.Message));
                worker.ReportProgress((int)ScreenshotSteps.MATCH_URL_HIDE); 
            }
            
            return responseBody;
        }

        private void Cleanup(string rawScreenshotPath, string resizedScreenshotPath)
        {
            worker.ReportProgress((int)ScreenshotSteps.CLEANUP, getLog(Status.LOADING));
            screenshoter?.Cleanup();
            File.Delete(rawScreenshotPath);
            File.Delete(resizedScreenshotPath);
            worker.ReportProgress((int)ScreenshotSteps.CLEANUP, getLog(Status.COMPLETE));
        }

        private StatusWithLog getLog(Status status)
        {
            return new(status);
        }

        private StatusWithLog getLog(Status status, string message)
        {
            return new(status, message);
        }
    }

    enum ScreenshotSteps
    {
        WINDOW,
        SCREENSHOT,
        RESIZE,
        UPLOAD,
        CLEANUP,
        MATCH_URL,
        MATCH_URL_HIDE,
    }

    public class NightLightResponse
    {
        public DataItem? Data { get; set; }
        public ErrorItem? Error { get; set; }
        public string Status { get; set; }

        public HttpStatusCode statusCode = HttpStatusCode.BadRequest;
    }

    public class DataItem
    {
        public string? Url { get; set; }
    }

    public class ErrorItem
    {
        public string? Message { get; set; }
        public string? Status { get; set; }
    }
}
