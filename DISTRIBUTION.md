# Distribution Guide - "Download & Run" Installation

The VDXR Mirror application is designed for **zero-hassle installation** - users simply download and run.

## ✅ What's Included Automatically

- **OpenXR loader DLL** via official NuGet package
- **All DirectX dependencies** (SharpDX libraries)
- **.NET 8.0 Runtime** (self-contained deployment)
- **All application dependencies** 
- **No manual setup required**

## Distribution Options

### Option 1: Simple ZIP Distribution (Recommended)
**Easiest for users - just extract and run**

1. Run `installer\build-simple.bat`
2. Zip the entire `dist` folder
3. Users download, extract, and run `VDXRMirror.exe`
4. Optionally run included "Create Desktop Shortcut.bat"

**Pros:**
- No installer dependencies
- Works on any Windows 10/11 system
- Users can place anywhere
- Instant "portable" application

### Option 2: Professional MSI Installer
**Traditional Windows installer experience**

1. Install WiX Toolset v3.11+
2. Run `installer\build-installer.ps1`
3. Distribute `VDXRMirrorSetup.msi`
4. Users double-click to install with Start Menu shortcuts

**Pros:**
- Professional installation experience
- Start Menu integration
- Proper uninstaller
- Add/Remove Programs entry

## User Experience

### ZIP Distribution:
1. Download `VDXR-Mirror-v1.0.zip`
2. Extract to desired location
3. Double-click `VDXRMirror.exe` 
4. **Application runs immediately**

### MSI Distribution:
1. Download `VDXRMirrorSetup.msi`
2. Double-click to install
3. Launch from Start Menu or Desktop
4. **Application runs immediately**

## What Users Need

**Hardware:**
- Windows 10/11 PC
- Quest 3 VR headset
- Wi-Fi connection for Virtual Desktop

**Software:**
- Virtual Desktop app (on PC and Quest)
- VDXR runtime enabled in Virtual Desktop settings

**That's it!** No manual DLL downloads, no SDK installations, no configuration files.

## Technical Details

The application uses:
- **Self-contained .NET deployment** - includes runtime
- **CopyLocalLockFileAssemblies** - includes all NuGet dependencies
- **OpenXR.Loader NuGet package** - automatic OpenXR DLL inclusion
- **Mock frame generator** - works even without VR for testing

## Recommended Distribution Strategy

For maximum compatibility and ease of use:

1. **Build both versions** (ZIP + MSI)
2. **Feature ZIP prominently** as "Portable Version"
3. **Offer MSI as alternative** for users preferring traditional installation
4. **Include setup video** showing Virtual Desktop VDXR configuration

## File Sizes (Approximate)

- **ZIP Distribution**: ~50-80 MB
- **MSI Installer**: ~50-80 MB

Both include the complete .NET 8.0 runtime and all dependencies for true "download and run" experience.