using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace dbd_screenshot
{
    internal class Screenshoter
    {
        private readonly IntPtr window;
        private IntPtr hdcWindow;
        private IntPtr hdcMem;
        private IntPtr hBitmap;
        private IntPtr hOld;

        public Screenshoter(IntPtr window) {
            this.window = window;
        }

        public void TakeScreenshot(string imagePath)
        {
            hdcWindow = WindowScreenshot.GetWindowDC(window);
            hdcMem = WindowScreenshot.CreateCompatibleDC(hdcWindow);

            WindowScreenshot.RECT windowRect;
            WindowScreenshot.GetWindowRect(window, out windowRect);

            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;

            hBitmap = WindowScreenshot.CreateCompatibleBitmap(hdcWindow, width, height);
            hOld = WindowScreenshot.SelectObject(hdcMem, hBitmap);

            WindowScreenshot.BitBlt(hdcMem, 0, 0, width, height, hdcWindow, 0, 0, 0x00CC0020);

            // Create a Bitmap object from the HBITMAP handle
            Bitmap bitmap = Image.FromHbitmap(hBitmap);

            // Save the screenshot as PNG
            bitmap.Save(imagePath, ImageFormat.Png);
        }

        public void Cleanup()
        {
            WindowScreenshot.SelectObject(hdcMem, hOld);
            WindowScreenshot.DeleteObject(hBitmap);
            WindowScreenshot.DeleteObject(hdcMem);
            WindowScreenshot.ReleaseDC(window, hdcWindow);
        }
    }
}
