using System;
using System.Collections.Generic;
using System.Linq;

namespace VDXRMirror
{
    /// <summary>
    /// Applies temporal smoothing to VR frames to reduce jitter
    /// Uses frame blending to create smoother motion for streaming
    /// </summary>
    public class TemporalSmoother : IDisposable
    {
        private readonly Queue<byte[]> _frameHistory = new();
        private readonly object _historyLock = new object();
        private byte[]? _blendedFrame;
        private bool _disposed = false;
        
        private const int MaxHistoryFrames = 3; // Keep last 3 frames for blending

        public TemporalSmoother()
        {
            // Initialize with default settings
        }

        public bool IsEnabled { get; set; } = true;
        public int Strength { get; set; } = 50; // 0-100

        /// <summary>
        /// Process frame with temporal smoothing
        /// </summary>
        /// <param name="frameData">Current frame data (BGRA format)</param>
        /// <param name="width">Frame width</param>
        /// <param name="height">Frame height</param>
        /// <param name="strength">Smoothing strength (0-100)</param>
        /// <returns>Smoothed frame data</returns>
        public byte[] ProcessFrame(byte[] frameData, int width, int height, int strength)
        {
            if (!IsEnabled || strength == 0)
            {
                return frameData;
            }

            lock (_historyLock)
            {
                // Add current frame to history
                AddFrameToHistory(frameData);

                // Create blended frame
                return BlendFrames(frameData, width, height, strength);
            }
        }

        private void AddFrameToHistory(byte[] frameData)
        {
            // Clone the frame data to avoid reference issues
            var frameCopy = new byte[frameData.Length];
            Array.Copy(frameData, frameCopy, frameData.Length);
            
            _frameHistory.Enqueue(frameCopy);

            // Keep only the most recent frames
            while (_frameHistory.Count > MaxHistoryFrames)
            {
                _frameHistory.Dequeue();
            }
        }

        private byte[] BlendFrames(byte[] currentFrame, int width, int height, int strength)
        {
            if (_frameHistory.Count < 2)
            {
                // Not enough history for blending
                return currentFrame;
            }

            var pixelCount = width * height * 4; // BGRA
            _blendedFrame ??= new byte[pixelCount];

            // Calculate blend weights based on strength
            float currentWeight = 1.0f - (strength / 100.0f * 0.5f); // 50-100% for current frame
            float historyWeight = strength / 100.0f * 0.5f / (_frameHistory.Count - 1); // Distribute remaining weight

            // Start with current frame
            Array.Copy(currentFrame, _blendedFrame, pixelCount);

            // Blend with previous frames
            var historyFrames = _frameHistory.ToArray();
            
            for (int i = 0; i < pixelCount; i += 4) // Process BGRA pixels
            {
                float blendedB = _blendedFrame[i] * currentWeight;
                float blendedG = _blendedFrame[i + 1] * currentWeight;
                float blendedR = _blendedFrame[i + 2] * currentWeight;
                float blendedA = _blendedFrame[i + 3] * currentWeight;

                // Blend with history frames (excluding the current frame we just added)
                for (int frameIdx = 0; frameIdx < historyFrames.Length - 1; frameIdx++)
                {
                    var historyFrame = historyFrames[frameIdx];
                    if (historyFrame.Length > i + 3)
                    {
                        blendedB += historyFrame[i] * historyWeight;
                        blendedG += historyFrame[i + 1] * historyWeight;
                        blendedR += historyFrame[i + 2] * historyWeight;
                        blendedA += historyFrame[i + 3] * historyWeight;
                    }
                }

                // Store blended result
                _blendedFrame[i] = ClampByte(blendedB);
                _blendedFrame[i + 1] = ClampByte(blendedG);
                _blendedFrame[i + 2] = ClampByte(blendedR);
                _blendedFrame[i + 3] = ClampByte(blendedA);
            }

            return _blendedFrame;
        }

        private static byte ClampByte(float value)
        {
            return (byte)Math.Max(0, Math.Min(255, Math.Round(value)));
        }

        /// <summary>
        /// Clear frame history (useful when switching resolutions or restarting)
        /// </summary>
        public void ClearHistory()
        {
            lock (_historyLock)
            {
                _frameHistory.Clear();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_historyLock)
            {
                _frameHistory.Clear();
            }

            _blendedFrame = null;
            _disposed = true;
        }
    }
}