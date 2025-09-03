# VDXR Mirror

A VR mirror application for Quest 3 with Virtual Desktop, designed for smooth streaming via OBS.

## 🚀 Quick Start

### Download & Run (Easiest)
1. **Download** the latest ZIP from [Releases](../../releases)
2. **Extract** anywhere on your PC
3. **Run** `VDXRMirror.exe` - that's it!

### Build from Source
1. **Clone** this repository
2. **Run** `installer\build-simple.bat` on Windows
3. **Zip** the `dist` folder for distribution

## ✨ Features

- **🎯 Zero-Setup Installation** - Download, extract, run
- **🖥️ Clean Stream Output** - Perfect for OBS capture
- **📱 Quest 3 Optimized** - Works with Virtual Desktop VDXR
- **🎮 Multiple Options** - 720p/1080p, eye selection, smoothing
- **⌨️ Hotkeys** - Ctrl+1/2 (resolution), Ctrl+L/R/B (eyes), Ctrl+S (smoothing)
- **💾 Persistent Settings** - Remembers your preferences
- **🎨 Temporal Smoothing** - Reduces jitter for better streaming

## 📋 Requirements

- **Windows 10/11** PC
- **Quest 3** VR headset  
- **Virtual Desktop** with VDXR runtime enabled

## 🎯 Target Use Case

Optimized for **flight simulation streaming** where natural head movement is desired, but output smoothing improves viewer experience.

## 🏗️ Technical Details

- **Framework**: C# WPF (.NET 8.0)
- **VR Integration**: OpenXR via VDXR runtime
- **Rendering**: WriteableBitmap for maximum compatibility
- **Smoothing**: Temporal frame blending (25%, 50%, 75% strength)
- **Dependencies**: Self-contained with automatic OpenXR loader inclusion

## 🔄 Automatic Builds

Every commit triggers an automatic build on GitHub Actions:
- ✅ **Compiles** the application with all dependencies
- ✅ **Creates** self-contained Windows distribution  
- ✅ **Generates** ZIP file ready for download
- ✅ **Uploads** as artifact (available for 30 days)
- ✅ **Creates release** when you tag a version

### Manual Build Trigger
Visit the [Actions tab](../../actions) and click **"Run workflow"** to trigger a build manually.

## 📦 Distribution Options

### Option 1: GitHub Actions (Automated) ⭐
- Push code → automatic build → download ZIP from Actions/Releases
- No local setup required

### Option 2: Local Build  
- Run `installer\build-simple.bat` on Windows
- Creates `dist` folder to zip manually

## 🐛 Troubleshooting

- **"VDXR runtime not available"** → Start Virtual Desktop with VDXR enabled
- **Black screen** → Check Virtual Desktop connection and VDXR settings
- **No VR connected** → Shows animated test pattern (normal for development)

## 📄 License

This project is open source and available under standard terms.

---

**🎮 Ready to stream your VR gameplay with smooth, professional output!**