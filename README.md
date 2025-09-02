# VDXR Mirror

A VR mirror application for Quest 3 with Virtual Desktop, designed for smooth streaming via OBS.

## Features

- **Clean Display**: Borderless window showing only VR content (perfect for OBS capture)
- **Multiple Resolutions**: 720p and 1080p support (1080p default)
- **Eye Selection**: Left eye, Right eye, or Both eyes (stereoscopic)
- **Temporal Smoothing**: Reduces jitter for better streaming experience
- **Menu-Driven Interface**: All controls in dropdown menus (no on-screen buttons)
- **Persistent Settings**: Remembers your preferences
- **Hotkey Support**: Quick access to common functions
- **Single-File Installer**: MSI package with all dependencies

## Target Use Case

Optimized for flight simulation streaming where natural head movement is desired, but output smoothing improves viewer experience.

## Technical Architecture

### Core Components
- **VRCaptureService**: Captures frames from VDXR runtime
- **TemporalSmoother**: Applies lightweight frame blending to reduce jitter
- **RenderSurface**: DirectX rendering surface for low-latency display
- **SettingsManager**: Handles configuration persistence
- **MenuSystem**: Clean dropdown-based interface

### Technology Stack
- **Framework**: C# WPF for native Windows performance
- **VR Integration**: VDXR runtime via Virtual Desktop
- **Smoothing**: Simple temporal frame blending (not complex motion interpolation)
- **Rendering**: DirectX for real-time display
- **Installation**: MSI package with .NET runtime included

### Smoothing Approach
Uses lightweight temporal smoothing - blending current frame with 1-2 previous frames to reduce high-frequency jitter while preserving natural head movement for flight simulation.

## Development Plan

1. **Project Setup**: Create C# WPF application structure
2. **VDXR Integration**: Implement frame capture from Virtual Desktop VDXR runtime  
3. **Temporal Smoothing**: Build simple frame blending engine
4. **UI System**: Create clean menu-driven interface
5. **Settings & Hotkeys**: Add configuration persistence and keyboard shortcuts
6. **Installer**: Package as single MSI with all dependencies

## Usage

1. Install from single MSI file
2. Launch Virtual Desktop and connect Quest 3
3. Set Virtual Desktop to use VDXR runtime
4. Launch VDXR Mirror application
5. Configure resolution/eye settings via menu
6. Capture window in OBS for streaming

## Requirements

- Windows 10/11
- Quest 3 VR headset
- Virtual Desktop (with VDXR runtime enabled)
- .NET 8.0 Runtime (included in installer)
