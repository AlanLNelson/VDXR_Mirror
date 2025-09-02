@echo off
REM Simple batch script to build VDXR Mirror without WiX (using dotnet publish)
REM Creates a self-contained deployment folder that can be zipped for distribution

echo Building VDXR Mirror Self-Contained Distribution...
echo.

cd /d "%~dp0\.."

REM Clean previous builds
if exist "dist" rmdir /s /q "dist"
mkdir "dist"

REM Build and publish self-contained
echo Step 1: Building application...
dotnet restore
dotnet build -c Release

if errorlevel 1 (
    echo ❌ Build failed!
    pause
    exit /b 1
)

echo Step 2: Publishing self-contained application...
dotnet publish src\VDXRMirror -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o "dist\VDXRMirror"

if errorlevel 1 (
    echo ❌ Publish failed!
    pause
    exit /b 1
)

REM Create README for users
echo Creating distribution README...
> "dist\README.txt" (
echo VDXR Mirror - VR Mirror Application
echo.
echo INSTALLATION:
echo 1. Extract this folder to any location ^(e.g., Program Files^)
echo 2. Run VDXRMirror.exe
echo.
echo REQUIREMENTS:
echo - Windows 10/11
echo - Quest 3 VR headset
echo - Virtual Desktop with VDXR runtime enabled
echo.
echo The application includes all required dependencies.
echo No additional downloads needed!
echo.
echo For support: https://github.com/yourusername/VDXR_Mirror
)

REM Create desktop shortcut helper
> "dist\Create Desktop Shortcut.bat" (
echo @echo off
echo echo Creating desktop shortcut...
echo powershell "$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut([Environment]::GetFolderPath('Desktop') + '\VDXR Mirror.lnk'^); $s.TargetPath = '%~dp0VDXRMirror\VDXRMirror.exe'; $s.WorkingDirectory = '%~dp0VDXRMirror'; $s.Description = 'VR mirror application for Quest 3 with Virtual Desktop'; $s.Save(^)"
echo echo Desktop shortcut created!
echo pause
)

echo.
echo ✅ Build completed successfully!
echo.
echo Distribution folder: dist\VDXRMirror\
echo.
echo To distribute:
echo 1. Zip the entire "dist" folder
echo 2. Users extract and run VDXRMirror.exe
echo 3. Optionally run "Create Desktop Shortcut.bat"
echo.
pause