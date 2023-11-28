using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace dbd_screenshot
{
    internal class Uploader
    {
        private readonly string requestUrl = "https://api.nightlight.gg/v1/upload";
        
        internal HttpResponseMessage UploadImage(string apiKey, string resizedScreenshotPath)
        { 
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            
            MultipartFormDataContent formContent = new MultipartFormDataContent();
            using (FileStream fileStream = new FileStream(resizedScreenshotPath, FileMode.Open, FileAccess.Read))
            {
                formContent.Add(new StreamContent(fileStream), "file", Path.GetFileName(resizedScreenshotPath));
                
                // Make the synchronous POST request and return the response
                Task<HttpResponseMessage> responseTask = httpClient.PostAsync(requestUrl, formContent);
                responseTask.Wait(); // Wait for the task to complete
                
                return responseTask.Result;
            }
        }
    }
}
