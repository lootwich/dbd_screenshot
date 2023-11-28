using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace dbd_screenshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainView
    {
        private readonly MainPresenter presenter;

        public MainWindow()
        {
            InitializeComponent();

            presenter = new(this);
            presenter.ReadTokenConfig();
            HideLinkOfMatch();
            label_copy.Content = $"by sldw.de (v idontknow)";
        }

        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            presenter.WriteTokenConfig(textbox_auth.Text);
        }

        private void Button_take_screenshot_Click(object sender, RoutedEventArgs e)
        {
            presenter.StartScreenshotProcess();
        }

        public void Hyperlink_RequestNavigate(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = hyperlink.NavigateUri.ToString(),
                UseShellExecute = true
            });
            e.Handled = true;
        }

        // Presenter Callbacks

        public void OnTokenRead(string authToken)
        {
            textbox_auth.Text = authToken;
        }

        void IMainView.UpdateFindDbdStatus(StatusWithLog status)
        {
            status_get_window.StatusImage = status.status;
            status_get_window.StatusLog = status.logMessage;
        }

        void IMainView.UpdateSnappingScreenshotStatus(StatusWithLog status)
        {
            status_make_screenshot.StatusImage = status.status;
            status_make_screenshot.StatusLog = status.logMessage;
        }

        void IMainView.UpdateResizingPixelStatus(StatusWithLog status)
        {
            status_resize_image.StatusImage = status.status;
            status_resize_image.StatusLog = status.logMessage;
        }

        void IMainView.UpdateUploadStatus(StatusWithLog status)
        {
            status_upload_image.StatusImage = status.status;
            status_upload_image.StatusLog = status.logMessage;
        }

        void IMainView.UpdateCleanupStatus(StatusWithLog status)
        {
            status_cleanup.StatusImage = status.status;
            status_cleanup.StatusLog = status.logMessage;
        }

        public void ShowLinkOfMatch(string url)
        {
            hyperlink_label.Text = url;
            hyperlink.NavigateUri = new Uri(url);
            hyperlink_text_block.Visibility = Visibility.Visible;
        }

        public void HideLinkOfMatch()
        {
            hyperlink_text_block.Visibility = Visibility.Hidden;
        }
    }
}
