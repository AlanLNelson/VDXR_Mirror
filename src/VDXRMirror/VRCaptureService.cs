using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace VDXRMirror
{
    /// <summary>
    /// Service for capturing VR frames from VDXR runtime
    /// Implements OpenXR-based frame capture with DirectX 11
    /// </summary>
    public class VRCaptureService : IDisposable
    {
        private bool _disposed = false;
        private bool _isCapturing = false;
        private IntPtr _xrInstance = IntPtr.Zero;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _captureTask;
        
        
        // Frame data
        private byte[]? _currentFrameData;
        private int _frameWidth = 1920;
        private int _frameHeight = 1080;
        private readonly object _frameLock = new object();

        public event EventHandler<FrameCapturedEventArgs>? FrameCaptured;

        public VRCaptureService()
        {
        }

        public bool IsConnected { get; private set; } = false;
        public bool IsCapturing => _isCapturing;
        public int FrameWidth => _frameWidth;
        public int FrameHeight => _frameHeight;


        public async Task<bool> InitializeVRConnection()
        {
            try
            {
                // Check if VDXR runtime is available
                if (!OpenXRInterop.IsVDXRRuntimeAvailable())
                {
                    Debug.WriteLine("VDXR runtime not available. Please ensure Virtual Desktop is running with VDXR enabled.");
                    return false;
                }

                // Initialize OpenXR instance
                var appInfo = new OpenXRInterop.XrApplicationInfo
                {
                    applicationName = "VDXR Mirror",
                    applicationVersion = 1,
                    engineName = "VDXR Mirror Engine",
                    engineVersion = 1,
                    apiVersion = OpenXRInterop.XR_CURRENT_API_VERSION
                };

                var createInfo = new OpenXRInterop.XrInstanceCreateInfo
                {
                    type = OpenXRInterop.XR_TYPE_INSTANCE_CREATE_INFO,
                    next = IntPtr.Zero,
                    createFlags = 0,
                    applicationInfo = appInfo,
                    enabledApiLayerCount = 0,
                    enabledApiLayerNames = IntPtr.Zero,
                    enabledExtensionCount = 0,
                    enabledExtensionNames = IntPtr.Zero
                };

                var result = OpenXRInterop.xrCreateInstance(ref createInfo, out _xrInstance);
                
                if (result == OpenXRInterop.XrResult.XR_SUCCESS)
                {
                    IsConnected = true;
                    Debug.WriteLine("OpenXR instance created successfully");
                    return true;
                }
                else
                {
                    Debug.WriteLine($"Failed to create OpenXR instance: {result}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"VR connection initialization failed: {ex.Message}");
                return false;
            }
        }

        public void StartCapture()
        {
            if (_isCapturing || !IsConnected) return;

            _isCapturing = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            // Start capture loop
            _captureTask = Task.Run(async () => await CaptureLoop(_cancellationTokenSource.Token));
            
            Debug.WriteLine("VR frame capture started");
        }

        public void StopCapture()
        {
            if (!_isCapturing) return;

            _isCapturing = false;
            _cancellationTokenSource?.Cancel();
            
            try
            {
                _captureTask?.Wait(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping capture: {ex.Message}");
            }
            
            Debug.WriteLine("VR frame capture stopped");
        }

        private async Task CaptureLoop(CancellationToken cancellationToken)
        {
            var frameInterval = TimeSpan.FromMilliseconds(1000.0 / 90.0); // 90 FPS target
            
            while (!cancellationToken.IsCancellationRequested && IsConnected)
            {
                try
                {
                    await CaptureFrame();
                    await Task.Delay(frameInterval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Frame capture error: {ex.Message}");
                    await Task.Delay(100, cancellationToken); // Brief pause on error
                }
            }
        }

        private async Task CaptureFrame()
        {
            // TODO: Implement actual OpenXR frame capture
            // For now, create a mock frame with gradient pattern for testing
            await CreateMockFrame();
            
            // Notify frame captured
            FrameCaptured?.Invoke(this, new FrameCapturedEventArgs(_currentFrameData, _frameWidth, _frameHeight));
        }

        private async Task CreateMockFrame()
        {
            // Create a test pattern until we implement real OpenXR capture
            await Task.Run(() =>
            {
                lock (_frameLock)
                {
                    var pixelCount = _frameWidth * _frameHeight * 4; // RGBA
                    _currentFrameData ??= new byte[pixelCount];
                    
                    // Create a simple animated gradient for testing
                    var time = Environment.TickCount * 0.001;
                    
                    for (int y = 0; y < _frameHeight; y++)
                    {
                        for (int x = 0; x < _frameWidth; x++)
                        {
                            var index = (y * _frameWidth + x) * 4;
                            
                            // Animated gradient pattern
                            var r = (byte)(128 + 127 * Math.Sin(x * 0.01 + time));
                            var g = (byte)(128 + 127 * Math.Sin(y * 0.01 + time * 1.1));
                            var b = (byte)(128 + 127 * Math.Sin((x + y) * 0.005 + time * 0.8));
                            
                            _currentFrameData[index] = b;     // B
                            _currentFrameData[index + 1] = g; // G
                            _currentFrameData[index + 2] = r; // R
                            _currentFrameData[index + 3] = 255; // A
                        }
                    }
                }
            });
        }

        public void SetResolution(int width, int height)
        {
            if (_frameWidth == width && _frameHeight == height) return;
            
            StopCapture();
            
            _frameWidth = width;
            _frameHeight = height;
            
            
            lock (_frameLock)
            {
                _currentFrameData = null; // Will be recreated on next frame
            }
            
            Debug.WriteLine($"Resolution changed to {width}x{height}");
            
            if (IsConnected)
            {
                StartCapture();
            }
        }

        public byte[]? GetLatestFrame()
        {
            lock (_frameLock)
            {
                return _currentFrameData?.Clone() as byte[];
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            StopCapture();
            
            if (_xrInstance != IntPtr.Zero)
            {
                OpenXRInterop.xrDestroyInstance(_xrInstance);
                _xrInstance = IntPtr.Zero;
            }

            _cancellationTokenSource?.Dispose();
            
            IsConnected = false;
            _disposed = true;
            
            Debug.WriteLine("VRCaptureService disposed");
        }
    }

    public class FrameCapturedEventArgs : EventArgs
    {
        public byte[]? FrameData { get; }
        public int Width { get; }
        public int Height { get; }
        public DateTime Timestamp { get; }

        public FrameCapturedEventArgs(byte[]? frameData, int width, int height)
        {
            FrameData = frameData;
            Width = width;
            Height = height;
            Timestamp = DateTime.UtcNow;
        }
    }
}