using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VDXRMirror
{
    /// <summary>
    /// Simplified image source for rendering VR frames in WPF
    /// Uses WriteableBitmap for reliable cross-platform frame display
    /// </summary>
    public class DirectXImageSource : IDisposable
    {
        private WriteableBitmap? _bitmap;
        private bool _disposed = false;
        private int _currentWidth = 0;
        private int _currentHeight = 0;

        public ImageSource? ImageSource => _bitmap;

        public DirectXImageSource()
        {
            // Initialize with default size
            CreateBitmap(1920, 1080);
        }

        private void CreateBitmap(int width, int height)
        {
            if (_currentWidth == width && _currentHeight == height && _bitmap != null)
                return;

            // Create WriteableBitmap for frame display
            _bitmap = new WriteableBitmap(
                width, 
                height, 
                96,  // DPI X
                96,  // DPI Y
                PixelFormats.Bgra32, 
                null);

            _currentWidth = width;
            _currentHeight = height;
        }

        public void UpdateFrame(byte[] frameData, int width, int height)
        {
            if (_disposed || frameData == null) return;

            try
            {
                // Recreate bitmap if dimensions changed
                if (_bitmap == null || _currentWidth != width || _currentHeight != height)
                {
                    CreateBitmap(width, height);
                }

                if (_bitmap == null) return;

                // Update bitmap on UI thread
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // Lock the bitmap for writing
                        _bitmap.Lock();

                        // Calculate stride
                        int stride = _bitmap.BackBufferStride;
                        int pixelSize = (_bitmap.Format.BitsPerPixel + 7) / 8;

                        unsafe
                        {
                            // Get pointer to back buffer
                            byte* backBuffer = (byte*)_bitmap.BackBuffer.ToPointer();

                            // Copy frame data to back buffer
                            fixed (byte* srcPtr = frameData)
                            {
                                // Copy row by row to handle stride differences
                                int srcStride = width * pixelSize;
                                
                                for (int row = 0; row < height; row++)
                                {
                                    byte* srcRow = srcPtr + (row * srcStride);
                                    byte* dstRow = backBuffer + (row * stride);
                                    
                                    // Copy the row
                                    for (int col = 0; col < srcStride; col++)
                                    {
                                        dstRow[col] = srcRow[col];
                                    }
                                }
                            }
                        }

                        // Mark the entire bitmap as dirty
                        _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
                    }
                    finally
                    {
                        // Always unlock the bitmap
                        _bitmap.Unlock();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frame update failed: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _bitmap = null;
            _disposed = true;
        }
    }
}