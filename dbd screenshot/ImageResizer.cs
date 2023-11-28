using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dbd_screenshot
{
    internal class ImageResizer
    {
        public string ResizeAtPath(string sourcePath)
        {
            string resizedPath = Path.GetTempPath() + "resized_Screenshot.png";

            using (Image image = Image.FromFile(sourcePath))
            {
                // Calculate the new dimensions while maintaining aspect ratio
                int newWidth = 1920;
                int newHeight = 1080;

                // Resize the image
                using (Bitmap resizedImage = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                    }

                    // Save the resized image to the specified target path
                    resizedImage.Save(resizedPath, ImageFormat.Png);
                }
            }
            return resizedPath;
        }
    }
}
