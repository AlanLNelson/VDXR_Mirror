# PowerShell script to build VDXR Mirror installer
# This creates a complete MSI package with all dependencies

param(
    [string]$Configuration = "Release"
)

Write-Host "Building VDXR Mirror Installer..." -ForegroundColor Green

# Set paths
$SolutionDir = Split-Path -Parent $PSScriptRoot
$ProjectDir = Join-Path $SolutionDir "src\VDXRMirror"
$InstallerDir = $PSScriptRoot
$OutputDir = Join-Path $SolutionDir "dist"

# Ensure output directory exists
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force
}

try {
    # Step 1: Build the main application
    Write-Host "Step 1: Building application..." -ForegroundColor Yellow
    Set-Location $SolutionDir
    
    dotnet restore
    dotnet build -c $Configuration
    
    # Step 2: Publish self-contained with all dependencies
    Write-Host "Step 2: Publishing self-contained application..." -ForegroundColor Yellow
    dotnet publish $ProjectDir -c $Configuration -r win-x64 --self-contained true -p:PublishSingleFile=false -o (Join-Path $OutputDir "publish")
    
    if ($LASTEXITCODE -ne 0) {
        throw "Application build failed"
    }
    
    # Step 3: Check for WiX Toolset
    Write-Host "Step 3: Checking for WiX Toolset..." -ForegroundColor Yellow
    
    $WixPath = Get-Command "candle.exe" -ErrorAction SilentlyContinue
    if (!$WixPath) {
        Write-Host "WiX Toolset not found. Please install WiX Toolset v3.11 or later." -ForegroundColor Red
        Write-Host "Download from: https://github.com/wixtoolset/wix3/releases" -ForegroundColor Red
        Write-Host ""
        Write-Host "Alternative: Use Visual Studio Installer Projects extension" -ForegroundColor Yellow
        exit 1
    }
    
    # Step 4: Build MSI with WiX
    Write-Host "Step 4: Building MSI installer..." -ForegroundColor Yellow
    Set-Location $InstallerDir
    
    # Compile WiX source
    & candle.exe -dVDXRMirror.TargetDir="$(Join-Path $OutputDir "publish")\" -dVDXRMirror.TargetPath="$(Join-Path $OutputDir "publish\VDXRMirror.exe")" -dSolutionDir="$SolutionDir\" VDXRMirror.wxs
    
    if ($LASTEXITCODE -ne 0) {
        throw "WiX compilation failed"
    }
    
    # Link MSI
    & light.exe -o "$OutputDir\VDXRMirrorSetup.msi" VDXRMirror.wixobj
    
    if ($LASTEXITCODE -ne 0) {
        throw "MSI linking failed"
    }
    
    Write-Host "✅ Installer built successfully!" -ForegroundColor Green
    Write-Host "Location: $OutputDir\VDXRMirrorSetup.msi" -ForegroundColor Green
    
    # Display file size
    $MsiFile = Get-Item "$OutputDir\VDXRMirrorSetup.msi"
    $SizeMB = [math]::Round($MsiFile.Length / 1MB, 2)
    Write-Host "Size: $SizeMB MB" -ForegroundColor Cyan
    
} catch {
    Write-Host "❌ Build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    Set-Location $SolutionDir
}

Write-Host ""
Write-Host "🚀 Ready for distribution!" -ForegroundColor Green
Write-Host "Users can now download VDXRMirrorSetup.msi and double-click to install." -ForegroundColor Green