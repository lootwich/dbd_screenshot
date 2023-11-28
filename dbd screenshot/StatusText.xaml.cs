using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace dbd_screenshot
{
    /// <summary>
    /// Interaction logic for StatusText.xaml
    /// </summary>
    public partial class StatusText : UserControl
    {
        private Status mStatus = Status.NONE;
        public StatusText()
        {
            InitializeComponent();
        }

        public Status StatusImage
        {
            get
            {
                return mStatus;
            }
            set
            {
                mStatus = value;
                image_status.Source = StatusHelper.GetStatusMessage(value);
            }
        }

        public string StatusName
        {
            get
            {
                return "" + status_name.Content.ToString();
            }
            set
            {
                status_name.Content = value;
            }
        }

        public string StatusLog
        {
            get
            {
                return "" + status_log.Content.ToString();
            }
            set
            {
                status_log.Content = value;
            }
        }
    }

    public class StatusWithLog
    {
        public Status status = Status.NONE;
        public string logMessage = "";

        public StatusWithLog(Status status, string logMessage)
        {
            this.status = status;
            this.logMessage = logMessage;
        }

        public StatusWithLog(Status status)
        {
            this.status = status;
            logMessage = "";
        }
    }

    public enum Status
    {
        NONE,
        LOADING,
        ERROR,
        COMPLETE
    }

    public static class StatusHelper
    {
        public static ImageSource? GetStatusMessage(Status status)
        {
            return status switch
            {
                Status.LOADING => fromString(Resources.loading),
                Status.ERROR => fromString(Resources.error),
                Status.COMPLETE => fromString(Resources.complete),
                Status.NONE => fromString(Resources.empty),
                _ => fromString(Resources.empty),
            };
        }

        private static ImageSource? fromString(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            return biImg;
        }
    }
}
