using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VDXRMirror
{
    public partial class MainWindow : Window
    {
        private VRCaptureService? _vrCapture;
        private TemporalSmoother? _smoother;
        private DirectXImageSource? _imageSource;
        private System.Windows.Controls.Image? _displayImage;
        private bool _isCapturing = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            RegisterHotkeys();
            InitializeVRCapture();
            
            // Track window position changes
            LocationChanged += MainWindow_LocationChanged;
        }

        private void LoadSettings()
        {
            // Apply saved settings to menu items
            Menu_1080p.IsChecked = AppSettings.Resolution == "1080p";
            Menu_720p.IsChecked = AppSettings.Resolution == "720p";
            
            Menu_LeftEye.IsChecked = AppSettings.EyeSelection == "Left";
            Menu_RightEye.IsChecked = AppSettings.EyeSelection == "Right";
            Menu_BothEyes.IsChecked = AppSettings.EyeSelection == "Both";
            
            Menu_SmoothingEnabled.IsChecked = AppSettings.SmoothingEnabled;
            Menu_Smooth25.IsChecked = AppSettings.SmoothingStrength == 25;
            Menu_Smooth50.IsChecked = AppSettings.SmoothingStrength == 50;
            Menu_Smooth75.IsChecked = AppSettings.SmoothingStrength == 75;
            
            // Apply window size and position from settings
            if (AppSettings.Resolution == "720p")
            {
                Width = 1280;
                Height = 720 + 40; // Dynamic menu height
            }
            else
            {
                Width = 1920;
                Height = 1080 + 40; // Dynamic menu height
            }
            
            // Set window position if saved
            if (AppSettings.WindowX > 0 && AppSettings.WindowY > 0)
            {
                Left = AppSettings.WindowX;
                Top = AppSettings.WindowY;
            }
        }

        private void RegisterHotkeys()
        {
            // Register global hotkeys
            KeyDown += MainWindow_KeyDown;
        }
        
        private void MainWindow_LocationChanged(object? sender, EventArgs e)
        {
            // Save window position
            AppSettings.WindowX = Left;
            AppSettings.WindowY = Top;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle hotkeys
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.D1: // Ctrl+1 - 720p
                        Resolution_720p_Click(null, null);
                        break;
                    case Key.D2: // Ctrl+2 - 1080p
                        Resolution_1080p_Click(null, null);
                        break;
                    case Key.L: // Ctrl+L - Left eye
                        EyeSelection_Left_Click(null, null);
                        break;
                    case Key.R: // Ctrl+R - Right eye
                        EyeSelection_Right_Click(null, null);
                        break;
                    case Key.B: // Ctrl+B - Both eyes
                        EyeSelection_Both_Click(null, null);
                        break;
                    case Key.S: // Ctrl+S - Toggle smoothing
                        Smoothing_Toggle_Click(null, null);
                        break;
                }
            }
        }

        private async void InitializeVRCapture()
        {
            try
            {
                StatusText.Text = "Initializing DirectX...";
                
                // Initialize DirectX image source
                _imageSource = new DirectXImageSource();
                
                // Create image control to display VR frames
                _displayImage = new System.Windows.Controls.Image
                {
                    Source = _imageSource.ImageSource,
                    Stretch = System.Windows.Media.Stretch.Uniform
                };
                
                // Replace status text with image
                DisplayContainer.Children.Clear();
                DisplayContainer.Children.Add(_displayImage);
                
                StatusText.Text = "Initializing VDXR connection...";
                
                // Initialize VR capture service
                _vrCapture = new VRCaptureService();
                _smoother = new TemporalSmoother();
                
                // Subscribe to frame events
                _vrCapture.FrameCaptured += OnFrameCaptured;
                
                // Try to connect to VDXR
                bool connected = await _vrCapture.InitializeVRConnection();
                
                if (connected)
                {
                    StatusText.Text = "VDXR Connected - Starting capture...";
                    
                    // Set initial resolution
                    UpdateVRResolution();
                    
                    // Start capturing
                    _vrCapture.StartCapture();
                    _isCapturing = true;
                    
                    // Hide status text once capture starts
                    await Task.Delay(2000);
                    StatusText.Visibility = Visibility.Hidden;
                }
                else
                {
                    StatusText.Text = "VDXR not available. Start Virtual Desktop with VDXR enabled.";
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }

        #region Menu Event Handlers

        private void Resolution_720p_Click(object sender, RoutedEventArgs e)
        {
            Menu_720p.IsChecked = true;
            Menu_1080p.IsChecked = false;
            AppSettings.Resolution = "720p";
            
            Width = 1280;
            Height = 720 + 40;
            
            UpdateVRResolution();
        }

        private void Resolution_1080p_Click(object sender, RoutedEventArgs e)
        {
            Menu_720p.IsChecked = false;
            Menu_1080p.IsChecked = true;
            AppSettings.Resolution = "1080p";
            
            Width = 1920;
            Height = 1080 + 40;
            
            UpdateVRResolution();
        }

        private void EyeSelection_Left_Click(object sender, RoutedEventArgs e)
        {
            Menu_LeftEye.IsChecked = true;
            Menu_RightEye.IsChecked = false;
            Menu_BothEyes.IsChecked = false;
            AppSettings.EyeSelection = "Left";
        }

        private void EyeSelection_Right_Click(object sender, RoutedEventArgs e)
        {
            Menu_LeftEye.IsChecked = false;
            Menu_RightEye.IsChecked = true;
            Menu_BothEyes.IsChecked = false;
            AppSettings.EyeSelection = "Right";
        }

        private void EyeSelection_Both_Click(object sender, RoutedEventArgs e)
        {
            Menu_LeftEye.IsChecked = false;
            Menu_RightEye.IsChecked = false;
            Menu_BothEyes.IsChecked = true;
            AppSettings.EyeSelection = "Both";
        }

        private void Smoothing_Toggle_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.SmoothingEnabled = Menu_SmoothingEnabled.IsChecked ?? false;
        }

        private void SmoothingStrength_25_Click(object sender, RoutedEventArgs e)
        {
            Menu_Smooth25.IsChecked = true;
            Menu_Smooth50.IsChecked = false;
            Menu_Smooth75.IsChecked = false;
            AppSettings.SmoothingStrength = 25;
        }

        private void SmoothingStrength_50_Click(object sender, RoutedEventArgs e)
        {
            Menu_Smooth25.IsChecked = false;
            Menu_Smooth50.IsChecked = true;
            Menu_Smooth75.IsChecked = false;
            AppSettings.SmoothingStrength = 50;
        }

        private void SmoothingStrength_75_Click(object sender, RoutedEventArgs e)
        {
            Menu_Smooth25.IsChecked = false;
            Menu_Smooth50.IsChecked = false;
            Menu_Smooth75.IsChecked = true;
            AppSettings.SmoothingStrength = 75;
        }

        private void Help_Hotkeys_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Keyboard Shortcuts:\n\n" +
                "Ctrl+1 - Switch to 720p\n" +
                "Ctrl+2 - Switch to 1080p\n" +
                "Ctrl+L - Left eye only\n" +
                "Ctrl+R - Right eye only\n" +
                "Ctrl+B - Both eyes\n" +
                "Ctrl+S - Toggle smoothing\n",
                "VDXR Mirror - Hotkeys",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Help_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "VDXR Mirror v1.0\n\n" +
                "VR mirror application for Quest 3 with Virtual Desktop\n" +
                "Optimized for streaming via OBS\n\n" +
                "Requirements:\n" +
                "- Quest 3 VR headset\n" +
                "- Virtual Desktop with VDXR runtime",
                "About VDXR Mirror",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion

        private void UpdateVRResolution()
        {
            if (_vrCapture == null) return;
            
            var resolution = AppSettings.Resolution;
            if (resolution == "720p")
            {
                _vrCapture.SetResolution(1280, 720);
            }
            else
            {
                _vrCapture.SetResolution(1920, 1080);
            }
        }
        
        private void OnFrameCaptured(object? sender, FrameCapturedEventArgs e)
        {
            if (e.FrameData == null) return;
            
            // Apply temporal smoothing if enabled
            var frameData = e.FrameData;
            if (AppSettings.SmoothingEnabled && _smoother != null)
            {
                frameData = _smoother.ProcessFrame(frameData, e.Width, e.Height, AppSettings.SmoothingStrength);
            }
            
            // Update DirectX image source on UI thread
            Dispatcher.BeginInvoke(() =>
            {
                _imageSource?.UpdateFrame(frameData, e.Width, e.Height);
                
                // Update the image control's source to reflect the new frame
                if (_displayImage != null && _imageSource != null)
                {
                    _displayImage.Source = _imageSource.ImageSource;
                }
            });
        }
        
        protected override void OnClosed(EventArgs e)
        {
            // Save final window position
            AppSettings.WindowX = Left;
            AppSettings.WindowY = Top;
            
            // Clean up resources
            _vrCapture?.Dispose();
            _smoother?.Dispose();
            _imageSource?.Dispose();
            base.OnClosed(e);
        }
    }
}