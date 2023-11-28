namespace dbd_screenshot
{
    internal interface IMainView
    {
        void OnTokenRead(string authToken);
        void UpdateFindDbdStatus(StatusWithLog status);
        void UpdateSnappingScreenshotStatus(StatusWithLog status);
        void UpdateResizingPixelStatus(StatusWithLog status);
        void UpdateUploadStatus(StatusWithLog status);
        void UpdateCleanupStatus(StatusWithLog status);
        void ShowLinkOfMatch(string url);
        void HideLinkOfMatch();
    }
}
