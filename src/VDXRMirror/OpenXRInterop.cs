using System;
using System.Runtime.InteropServices;

namespace VDXRMirror
{
    /// <summary>
    /// OpenXR interop for VDXR frame capture
    /// Based on OpenXR specification 1.0
    /// </summary>
    public static class OpenXRInterop
    {
        private const string OpenXRLibrary = "openxr_loader.dll";

        // OpenXR Result Codes
        public enum XrResult
        {
            XR_SUCCESS = 0,
            XR_ERROR_VALIDATION_FAILURE = -1,
            XR_ERROR_RUNTIME_FAILURE = -2,
            XR_ERROR_OUT_OF_MEMORY = -3,
            XR_ERROR_API_VERSION_UNSUPPORTED = -4,
            XR_ERROR_INITIALIZATION_FAILED = -5,
            XR_ERROR_FUNCTION_UNSUPPORTED = -6,
            XR_ERROR_FEATURE_UNSUPPORTED = -7,
            XR_ERROR_EXTENSION_NOT_PRESENT = -8,
            XR_ERROR_LIMIT_REACHED = -9,
            XR_ERROR_SIZE_INSUFFICIENT = -10,
            XR_ERROR_HANDLE_INVALID = -11,
            XR_ERROR_INSTANCE_LOST = -12,
            XR_ERROR_SESSION_RUNNING = -14,
            XR_ERROR_SESSION_NOT_RUNNING = -16,
            XR_ERROR_SESSION_LOST = -17,
            XR_ERROR_SYSTEM_INVALID = -18,
            XR_ERROR_PATH_INVALID = -19,
            XR_ERROR_PATH_COUNT_EXCEEDED = -20,
            XR_ERROR_PATH_FORMAT_INVALID = -21,
            XR_ERROR_PATH_UNSUPPORTED = -22,
            XR_ERROR_LAYER_INVALID = -23,
            XR_ERROR_LAYER_LIMIT_EXCEEDED = -24,
            XR_ERROR_SWAPCHAIN_RECT_INVALID = -25,
            XR_ERROR_SWAPCHAIN_FORMAT_UNSUPPORTED = -26,
            XR_ERROR_ACTION_TYPE_MISMATCH = -27,
            XR_ERROR_SESSION_NOT_READY = -28,
            XR_ERROR_SESSION_NOT_STOPPING = -29,
            XR_ERROR_TIME_INVALID = -30,
            XR_ERROR_REFERENCE_SPACE_UNSUPPORTED = -31,
            XR_ERROR_FILE_ACCESS_ERROR = -32,
            XR_ERROR_FILE_CONTENTS_INVALID = -33,
            XR_ERROR_FORM_FACTOR_UNSUPPORTED = -34,
            XR_ERROR_FORM_FACTOR_UNAVAILABLE = -35,
            XR_ERROR_API_LAYER_NOT_PRESENT = -36,
            XR_ERROR_CALL_ORDER_INVALID = -37,
            XR_ERROR_GRAPHICS_DEVICE_INVALID = -38,
            XR_ERROR_POSE_INVALID = -39,
            XR_ERROR_INDEX_OUT_OF_RANGE = -40,
            XR_ERROR_VIEW_CONFIGURATION_TYPE_UNSUPPORTED = -41,
            XR_ERROR_ENVIRONMENT_BLEND_MODE_UNSUPPORTED = -42,
            XR_ERROR_NAME_DUPLICATED = -44,
            XR_ERROR_NAME_INVALID = -45,
            XR_ERROR_ACTIONSET_NOT_ATTACHED = -46,
            XR_ERROR_ACTIONSETS_ALREADY_ATTACHED = -47,
            XR_ERROR_LOCALIZED_NAME_DUPLICATED = -48,
            XR_ERROR_LOCALIZED_NAME_INVALID = -49,
            XR_ERROR_GRAPHICS_REQUIREMENTS_CALL_MISSING = -50,
            XR_ERROR_RUNTIME_UNAVAILABLE = -51
        }

        // OpenXR Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct XrApiLayerProperties
        {
            public uint type;
            public IntPtr next;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string layerName;
            public ulong specVersion;
            public uint layerVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string description;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XrApplicationInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string applicationName;
            public uint applicationVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string engineName;
            public uint engineVersion;
            public ulong apiVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XrInstanceCreateInfo
        {
            public uint type;
            public IntPtr next;
            public ulong createFlags;
            public XrApplicationInfo applicationInfo;
            public uint enabledApiLayerCount;
            public IntPtr enabledApiLayerNames;
            public uint enabledExtensionCount;
            public IntPtr enabledExtensionNames;
        }

        // OpenXR Function Imports
        [DllImport(OpenXRLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern XrResult xrEnumerateApiLayerProperties(uint propertyCapacityInput, out uint propertyCountOutput, IntPtr properties);

        [DllImport(OpenXRLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern XrResult xrCreateInstance(ref XrInstanceCreateInfo createInfo, out IntPtr instance);

        [DllImport(OpenXRLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern XrResult xrDestroyInstance(IntPtr instance);

        [DllImport(OpenXRLibrary, CallingConvention = CallingConvention.StdCall)]
        public static extern XrResult xrGetInstanceProperties(IntPtr instance, IntPtr instanceProperties);

        // Constants
        public const uint XR_TYPE_API_LAYER_PROPERTIES = 14;
        public const uint XR_TYPE_INSTANCE_CREATE_INFO = 2;
        public const ulong XR_CURRENT_API_VERSION = 0x0001000000000000UL; // Version 1.0.0

        // Helper method to check if VDXR is available
        public static bool IsVDXRRuntimeAvailable()
        {
            try
            {
                uint layerCount = 0;
                XrResult result = xrEnumerateApiLayerProperties(0, out layerCount, IntPtr.Zero);
                
                if (result == XrResult.XR_SUCCESS && layerCount > 0)
                {
                    // Check for Virtual Desktop runtime presence
                    // This is a simplified check - in practice we'd enumerate layers
                    return true;
                }
                
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}