using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

using FallenStrap.UI.ViewModels.Settings;

namespace FallenStrap.UI.Elements.Settings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        private Models.Persistable.WindowState _state => App.State.Prop.SettingsWindow;

        public MainWindow(bool showAlreadyRunningWarning)
        {
            var viewModel = new MainWindowViewModel();

            viewModel.RequestSaveNoticeEvent += (_, _) => SettingsSavedSnackbar.Show();
            viewModel.RequestCloseWindowEvent += (_, _) => Close();

            DataContext = viewModel;
            
            InitializeComponent();

            App.Logger.WriteLine("MainWindow", "Initializing settings window");

            if (showAlreadyRunningWarning)
                ShowAlreadyRunningSnackbar();

            LoadState();
            ApplyCustomBackground();
        }

        public void ApplyCustomBackground()
        {
            var path = App.Settings.Prop.SettingsBackgroundPath;

            // corta cualquier animacion de gif previa antes de cambiar de fondo
            CustomBackgroundImage.BeginAnimation(Image.SourceProperty, null);

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                CustomBackgroundGrid.Visibility = Visibility.Collapsed;
                DefaultBackgroundGrid.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                if (Path.GetExtension(path).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                    PlayAnimatedGif(path);
                else
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    CustomBackgroundImage.Source = bitmap;
                }

                DefaultBackgroundGrid.Visibility = Visibility.Collapsed;
                CustomBackgroundGrid.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("MainWindow", $"No se pudo cargar el fondo personalizado: {ex.Message}");
                CustomBackgroundGrid.Visibility = Visibility.Collapsed;
                DefaultBackgroundGrid.Visibility = Visibility.Visible;
            }
        }

        private void PlayAnimatedGif(string path)
        {
            var decoder = new GifBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            if (decoder.Frames.Count == 0)
                return;

            if (decoder.Frames.Count == 1)
            {
                CustomBackgroundImage.Source = decoder.Frames[0];
                return;
            }

            var keyFrames = new ObjectAnimationUsingKeyFrames();
            var elapsed = TimeSpan.Zero;

            foreach (var frame in decoder.Frames)
            {
                keyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame(frame, elapsed));
                elapsed += GetFrameDelay(frame);
            }

            // sostiene el ultimo frame hasta completar su propio delay antes de repetir
            keyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame(decoder.Frames[0], elapsed));
            keyFrames.Duration = elapsed;
            keyFrames.RepeatBehavior = RepeatBehavior.Forever;

            CustomBackgroundImage.Source = decoder.Frames[0];
            CustomBackgroundImage.BeginAnimation(Image.SourceProperty, keyFrames);
        }

        private static TimeSpan GetFrameDelay(BitmapFrame frame)
        {
            const int defaultDelayMs = 100;

            try
            {
                if (frame.Metadata is System.Windows.Media.Imaging.BitmapMetadata metadata)
                {
                    var query = metadata.GetQuery("/grctlext/Delay");
                    if (query is ushort delayCentiseconds && delayCentiseconds > 0)
                        return TimeSpan.FromMilliseconds(delayCentiseconds * 10);
                }
            }
            catch
            {
                // algunos gifs no exponen este metadato; usamos el delay por defecto
            }

            return TimeSpan.FromMilliseconds(defaultDelayMs);
        }

        public void LoadState()
        {
            if (_state.Left > SystemParameters.VirtualScreenWidth)
                _state.Left = 0;

            if (_state.Top > SystemParameters.VirtualScreenHeight)
                _state.Top = 0;

            if (_state.Width > 0)
                this.Width = _state.Width;

            if (_state.Height > 0)
                this.Height = _state.Height;

            if (_state.Left > 0 && _state.Top > 0)
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                this.Left = _state.Left;
                this.Top = _state.Top;
            }
        }

        private async void ShowAlreadyRunningSnackbar()
        {
            await Task.Delay(500); // wait for everything to finish loading
            AlreadyRunningSnackbar.Show();
        }

        #region INavigationWindow methods

        public Frame GetFrame() => RootFrame;

        public INavigation GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) => RootNavigation.PageService = pageService;

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods

        private void WpfUiWindow_Closing(object sender, CancelEventArgs e)
        {
            if (App.FastFlags.Changed || App.PendingSettingTasks.Any())
            {
                var result = Frontend.ShowMessageBox(Strings.Menu_UnsavedChanges, MessageBoxImage.Warning, MessageBoxButton.YesNo);

                if (result != MessageBoxResult.Yes)
                    e.Cancel = true;
            }
            
            _state.Width = this.Width;
            _state.Height = this.Height;

            _state.Top = this.Top;
            _state.Left = this.Left;

            App.State.Save();
        }

        private void WpfUiWindow_Closed(object sender, EventArgs e)
        {
            if (App.LaunchSettings.TestModeFlag.Active)
                LaunchHandler.LaunchRoblox(LaunchMode.Player);
            else
                App.SoftTerminate();
        }
    }
}
