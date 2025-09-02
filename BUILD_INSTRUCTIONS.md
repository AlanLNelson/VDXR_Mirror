# Build Instructions

This project creates a Windows application for VR mirroring with Quest 3 and Virtual Desktop.

## Prerequisites

- **Visual Studio 2022** (Community Edition or higher)
- **.NET 8.0 SDK**
- **Windows 10/11** development machine

## Building the Application

### Option 1: Using Visual Studio
1. Open `VDXRMirror.sln` in Visual Studio 2022
2. Restore NuGet packages (should happen automatically)
3. Select **Release** configuration
4. Build > Build Solution (or press Ctrl+Shift+B)
5. The executable will be in `src\VDXRMirror\bin\Release\net8.0-windows\`

### Option 2: Using Command Line
```bash
# Navigate to project root
cd VDXR_Mirror

# Restore packages
dotnet restore

# Build in Release mode
dotnet build -c Release

# Publish self-contained executable (recommended for distribution)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Dependencies

The project uses these NuGet packages (all automatically included):
- **OpenXR.Loader**: Official OpenXR loader DLL (eliminates manual setup)
- **SharpDX**: DirectX integration for hardware-accelerated rendering
- **SharpDX.Direct3D11**: Direct3D 11 support for VR frame processing
- **SharpDX.DXGI**: DirectX Graphics Infrastructure for display management
- **System.Text.Json**: Settings persistence

**Key Feature:** `CopyLocalLockFileAssemblies=true` ensures all NuGet dependencies are automatically copied to output directory.

## Development Status

**Current Status: Complete - Production Ready with Bug Fixes Applied**

### Completed:
✅ Project structure with Visual Studio solution  
✅ WPF application framework  
✅ Menu system with dropdowns (Resolution, Eye Selection, Smoothing)  
✅ Settings persistence system with window positioning  
✅ Hotkey support framework  
✅ Application manifest for Windows compatibility  
✅ **Bug fixes applied**: Removed invalid icon reference, fixed async method, added window positioning, improved menu height handling  
✅ **OpenXR Integration**: Complete OpenXR interop layer for VDXR communication  
✅ **VR Frame Capture**: Async frame capture service optimized for performance  
✅ **Simplified Rendering**: WriteableBitmap-based image display for maximum compatibility  
✅ **Temporal Smoothing**: Frame blending engine for jitter reduction  
✅ **Mock Frame Generator**: Test pattern generation for development without VR hardware  
✅ **Automatic Dependencies**: OpenXR.Loader NuGet package eliminates manual DLL setup  
✅ **Distribution System**: Both ZIP (portable) and MSI installer options  
✅ **Self-Contained Deployment**: Includes .NET runtime and all dependencies  
✅ **Production Bug Fixes**: Fixed compilation errors, removed unused dependencies, optimized rendering pipeline  

## Distribution Options

### Option 1: Portable ZIP (Recommended)
Run `installer\build-simple.bat` to create a portable distribution:
- Users download and extract ZIP file
- Double-click `VDXRMirror.exe` to run immediately
- No installation required
- Can be placed anywhere on system

### Option 2: MSI Installer
Run `installer\build-installer.ps1` (requires WiX Toolset):
- Professional Windows installer experience
- Start Menu and Desktop shortcuts
- Add/Remove Programs integration
- Traditional installation workflow

## Project Structure

```
src/VDXRMirror/
├── VDXRMirror.csproj      # Project file with dependencies
├── App.xaml               # Application entry point
├── App.xaml.cs            # Application logic and settings management
├── MainWindow.xaml        # Main UI with menu system
├── MainWindow.xaml.cs     # UI event handlers and window logic
├── AppSettings.cs         # Settings persistence (JSON)
├── VRCaptureService.cs    # VR frame capture (Step 2)
├── TemporalSmoother.cs    # Frame smoothing engine (Step 3)
└── app.manifest           # Windows compatibility settings
```

## Features Implemented

- **Clean Menu-Driven UI**: All controls in dropdown menus, no on-screen buttons
- **Resolution Support**: 720p and 1080p options with automatic window sizing
- **Eye Selection**: Left, Right, or Both eyes
- **Smoothing Options**: Enable/disable with strength control (25%, 50%, 75%)
- **Hotkey Support**: Ctrl+1/2 (resolution), Ctrl+L/R/B (eye selection), Ctrl+S (smoothing)
- **Persistent Settings**: Automatically saves/restores user preferences
- **Windows Integration**: Proper DPI awareness and Windows 10/11 compatibility
- **Zero-Setup Installation**: Download, extract/install, and run - no manual configuration
- **Self-Contained**: Includes .NET 8.0 runtime and all dependencies
- **Professional Distribution**: Both portable ZIP and MSI installer options