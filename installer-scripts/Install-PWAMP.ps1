#Requires -Version 5.1

<#
.SYNOPSIS
    PWAMP Installer - Downloads and installs Apache, PHP, MariaDB, and phpMyAdmin
.DESCRIPTION
    Modular PowerShell installer that reads packages.json and allows users to select components to install
.PARAMETER InstallPath
    Base installation directory (default: C:\PWAMP)
.PARAMETER ConfigFile
    Path to packages.json configuration file
.PARAMETER Quiet
    Skip user prompts and install all required packages
#>

[CmdletBinding()]
param(
    [string]$InstallPath = "C:\PWAMP",
    [string]$ConfigFile = "packages.json",
    [switch]$Quiet
)

# Global variables
$Global:Config = $null
$Global:SelectedPackages = @()

#region Utility Functions

function Write-Status {
    param([string]$Message, [string]$Color = "Green")
    Write-Host "[$((Get-Date).ToString('HH:mm:ss'))] $Message" -ForegroundColor $Color
}

function Write-Error-Status {
    param([string]$Message)
    Write-Status $Message -Color "Red"
}

function Write-Warning-Status {
    param([string]$Message)
    Write-Status $Message -Color "Yellow"
}

function Test-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Test-Prerequisites {
    Write-Status "Checking prerequisites..."
    
    if (-not (Test-Administrator)) {
        Write-Error-Status "This script requires administrator privileges"
        Write-Host "Please run PowerShell as Administrator and try again." -ForegroundColor Yellow
        exit 1
    }
    
    if ($PSVersionTable.PSVersion.Major -lt 5) {
        Write-Error-Status "PowerShell 5.1 or higher is required"
        exit 1
    }
    
    Write-Status "Prerequisites check passed"
}

#endregion

#region Configuration Functions

function Read-Configuration {
    param([string]$ConfigPath)
    
    Write-Status "Reading configuration from $ConfigPath"
    
    if (-not (Test-Path $ConfigPath)) {
        Write-Error-Status "Configuration file not found: $ConfigPath"
        exit 1
    }
    
    try {
        $jsonContent = Get-Content $ConfigPath -Raw -ErrorAction Stop
        $config = $jsonContent | ConvertFrom-Json -ErrorAction Stop
        
        Write-Status "Loaded $($config.Count) packages from configuration"
        return $config
    }
    catch {
        Write-Error-Status "Failed to parse configuration file: $($_.Exception.Message)"
        exit 1
    }
}

function Show-PackageInfo {
    param($Package)
    
    $size = [math]::Round($Package.EstimatedSize / 1MB, 1)
    $required = if ($Package.IsRequired) { "(Required)" } else { "(Optional)" }
    
    Write-Host "  $($Package.Name) v$($Package.Version) $required" -ForegroundColor Cyan
    Write-Host "    $($Package.Description)" -ForegroundColor Gray
    Write-Host "    Size: ~$size MB" -ForegroundColor Gray
    
    if ($Package.Dependencies -and $Package.Dependencies.Count -gt 0) {
        Write-Host "    Dependencies: $($Package.Dependencies -join ', ')" -ForegroundColor Gray
    }
    Write-Host ""
}

function Select-Packages {
    param($AllPackages)
    
    if ($Quiet) {
        Write-Status "Quiet mode: selecting all required packages"
        return $AllPackages | Where-Object { $_.IsRequired }
    }
    
    Write-Host "`n=== PWAMP Component Selection ===" -ForegroundColor Yellow
    Write-Host "Available packages:" -ForegroundColor Green
    Write-Host ""
    
    # Show all packages
    for ($i = 0; $i -lt $AllPackages.Count; $i++) {
        Write-Host "[$($i + 1)] " -NoNewline -ForegroundColor White
        Show-PackageInfo $AllPackages[$i]
    }
    
    # Get user selections
    $selected = @()
    
    Write-Host "Select packages to install:" -ForegroundColor Green
    Write-Host "Enter package numbers separated by commas (e.g., 1,2,4)" -ForegroundColor Gray
    Write-Host "Or press Enter to install all packages" -ForegroundColor Gray
    
    $input = Read-Host "Selection"
    
    if ([string]::IsNullOrWhiteSpace($input)) {
        # Install all packages
        $selected = $AllPackages
        Write-Status "Selected all packages"
    }
    else {
        # Parse user input
        try {
            $numbers = $input -split ',' | ForEach-Object { [int]$_.Trim() }
            foreach ($num in $numbers) {
                if ($num -ge 1 -and $num -le $AllPackages.Count) {
                    $selected += $AllPackages[$num - 1]
                }
                else {
                    Write-Warning-Status "Invalid package number: $num"
                }
            }
        }
        catch {
            Write-Error-Status "Invalid input format. Using all packages."
            $selected = $AllPackages
        }
    }
    
    # Ensure required packages are included
    $requiredPackages = $AllPackages | Where-Object { $_.IsRequired }
    foreach ($required in $requiredPackages) {
        if ($required -notin $selected) {
            Write-Status "Adding required package: $($required.Name)"
            $selected += $required
        }
    }
    
    # Resolve dependencies
    $selected = Resolve-Dependencies $selected $AllPackages
    
    Write-Host "`nSelected packages:" -ForegroundColor Green
    foreach ($pkg in $selected) {
        Write-Host "  - $($pkg.Name) v$($pkg.Version)" -ForegroundColor Cyan
    }
    
    return $selected
}

function Resolve-Dependencies {
    param($SelectedPackages, $AllPackages)
    
    $resolved = $SelectedPackages | ForEach-Object { $_ }
    $processed = @()
    
    do {
        $added = $false
        foreach ($package in $resolved) {
            if ($package.PackageID -in $processed) { continue }
            
            $processed += $package.PackageID
            
            if ($package.Dependencies -and $package.Dependencies.Count -gt 0) {
                foreach ($depId in $package.Dependencies) {
                    $dependency = $AllPackages | Where-Object { $_.PackageID -eq $depId }
                    if ($dependency -and $dependency -notin $resolved) {
                        Write-Status "Adding dependency: $($dependency.Name) (required by $($package.Name))"
                        $resolved += $dependency
                        $added = $true
                    }
                }
            }
        }
    } while ($added)
    
    return $resolved
}

#endregion

#region Download Functions

function New-DownloadsDirectory {
    param([string]$BasePath)
    
    $downloadsPath = Join-Path $BasePath "Downloads"
    if (-not (Test-Path $downloadsPath)) {
        New-Item -ItemType Directory -Path $downloadsPath -Force | Out-Null
        Write-Status "Created downloads directory: $downloadsPath"
    }
    return $downloadsPath
}

function Download-Package {
    param($Package, [string]$DownloadPath)
    
    $fileName = [System.IO.Path]::GetFileName((New-Object System.Uri $Package.DownloadUrl).LocalPath)
    if ([string]::IsNullOrEmpty($fileName)) {
        $fileName = "$($Package.PackageID)-$($Package.Version).zip"
    }
    
    $filePath = Join-Path $DownloadPath $fileName
    
    if (Test-Path $filePath) {
        $fileSize = (Get-Item $filePath).Length
        if ($fileSize -gt 1KB) {
            Write-Status "Package already downloaded: $($Package.Name)"
            return $filePath
        }
        else {
            Write-Warning-Status "Removing corrupted download: $fileName"
            Remove-Item $filePath -Force
        }
    }
    
    Write-Status "Downloading $($Package.Name) v$($Package.Version)..."
    Write-Host "  URL: $($Package.DownloadUrl)" -ForegroundColor Gray
    
    try {
        # Use Invoke-WebRequest with browser-like headers
        $headers = @{
            'User-Agent' = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
            'Accept' = 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8'
            'Accept-Language' = 'en-US,en;q=0.5'
            'Accept-Encoding' = 'gzip, deflate'
            'Connection' = 'keep-alive'
            'Upgrade-Insecure-Requests' = '1'
        }
        
        Invoke-WebRequest -Uri $Package.DownloadUrl -OutFile $filePath -Headers $headers -ErrorAction Stop
        
        # Verify download
        if (Test-Path $filePath) {
            $downloadedSize = (Get-Item $filePath).Length
            if ($downloadedSize -lt 1KB) {
                throw "Downloaded file appears to be corrupted (size: $downloadedSize bytes)"
            }
            
            $sizeMB = [math]::Round($downloadedSize / 1MB, 1)
            Write-Status "Downloaded successfully: $fileName ($sizeMB MB)"
            return $filePath
        }
        else {
            throw "Download failed - file not found after download"
        }
    }
    catch {
        Write-Error-Status "Download failed for $($Package.Name): $($_.Exception.Message)"
        Write-Host "  URL: $($Package.DownloadUrl)" -ForegroundColor Red
        throw
    }
}

#endregion

#region Installation Functions

function Install-Package {
    param($Package, [string]$FilePath, [string]$InstallBasePath)
    
    $installPath = Join-Path $InstallBasePath $Package.InstallPath
    
    Write-Status "Installing $($Package.Name) to $installPath..."
    
    # Create installation directory
    if (-not (Test-Path $installPath)) {
        New-Item -ItemType Directory -Path $installPath -Force | Out-Null
    }
    
    try {
        # Extract based on archive format
        switch ($Package.ArchiveFormat.ToLower()) {
            "zip" {
                if ($PSVersionTable.PSVersion.Major -ge 5) {
                    Expand-Archive -Path $FilePath -DestinationPath $installPath -Force
                }
                else {
                    # Fallback for older PowerShell versions
                    Add-Type -AssemblyName System.IO.Compression.FileSystem
                    [System.IO.Compression.ZipFile]::ExtractToDirectory($FilePath, $installPath)
                }
            }
            default {
                throw "Unsupported archive format: $($Package.ArchiveFormat)"
            }
        }
        
        # Verify installation
        $extractedFiles = Get-ChildItem -Path $installPath -Recurse -File
        if ($extractedFiles.Count -eq 0) {
            throw "No files were extracted"
        }
        
        Write-Status "Extracted $($extractedFiles.Count) files to $installPath"
        
        # Post-installation tasks
        Invoke-PostInstallTasks $Package $installPath
        
        Write-Status "$($Package.Name) installed successfully"
    }
    catch {
        Write-Error-Status "Installation failed for $($Package.Name): $($_.Exception.Message)"
        throw
    }
}

function Invoke-PostInstallTasks {
    param($Package, [string]$InstallPath)
    
    switch ($Package.PackageID) {
        "apache" {
            Write-Status "Configuring Apache..."
            # Add any Apache-specific configuration here
        }
        "php" {
            Write-Status "Configuring PHP..."
            # Add any PHP-specific configuration here
        }
        "mariadb" {
            Write-Status "Configuring MariaDB..."
            # Add any MariaDB-specific configuration here
        }
        "phpmyadmin" {
            Write-Status "Configuring phpMyAdmin..."
            # Add any phpMyAdmin-specific configuration here
        }
    }
}

#endregion

#region Main Installation Function

function Install-PWAMP {
    param(
        [string]$InstallationPath,
        [string]$ConfigurationFile,
        [switch]$QuietMode
    )
    
    Write-Host "`n=== PWAMP Installer ===" -ForegroundColor Yellow
    Write-Status "Starting PWAMP installation"
    Write-Status "Installation path: $InstallationPath"
    
    try {
        # Check prerequisites
        Test-Prerequisites
        
        # Read configuration
        $Global:Config = Read-Configuration $ConfigurationFile
        
        # Select packages
        $Global:SelectedPackages = Select-Packages $Global:Config
        
        if ($Global:SelectedPackages.Count -eq 0) {
            Write-Warning-Status "No packages selected for installation"
            return
        }
        
        # Confirm installation
        if (-not $QuietMode) {
            Write-Host "`nInstallation Summary:" -ForegroundColor Yellow
            Write-Host "  Installation Path: $InstallationPath" -ForegroundColor Gray
            Write-Host "  Selected Packages: $($Global:SelectedPackages.Count)" -ForegroundColor Gray
            $totalSize = ($Global:SelectedPackages | Measure-Object -Property EstimatedSize -Sum).Sum
            Write-Host "  Total Size: ~$([math]::Round($totalSize / 1MB, 1)) MB" -ForegroundColor Gray
            Write-Host ""
            
            $confirm = Read-Host "Proceed with installation? (Y/n)"
            if ($confirm -match '^[Nn]') {
                Write-Status "Installation cancelled by user"
                return
            }
        }
        
        # Create installation directory
        if (-not (Test-Path $InstallationPath)) {
            New-Item -ItemType Directory -Path $InstallationPath -Force | Out-Null
            Write-Status "Created installation directory: $InstallationPath"
        }
        
        # Create downloads directory
        $downloadsPath = New-DownloadsDirectory $InstallationPath
        
        # Download and install each package
        $successCount = 0
        $totalPackages = $Global:SelectedPackages.Count
        
        foreach ($package in $Global:SelectedPackages) {
            try {
                Write-Host "`n--- Processing $($package.Name) ($($successCount + 1)/$totalPackages) ---" -ForegroundColor Cyan
                
                # Download package
                $downloadedFile = Download-Package $package $downloadsPath
                
                # Install package
                Install-Package $package $downloadedFile $InstallationPath
                
                $successCount++
            }
            catch {
                Write-Error-Status "Failed to install $($package.Name): $($_.Exception.Message)"
                
                if (-not $QuietMode) {
                    $continue = Read-Host "Continue with remaining packages? (Y/n)"
                    if ($continue -match '^[Nn]') {
                        break
                    }
                }
            }
        }
        
        # Installation summary
        Write-Host "`n=== Installation Complete ===" -ForegroundColor Green
        Write-Status "Successfully installed $successCount of $totalPackages packages"
        Write-Status "Installation path: $InstallationPath"
        
        if ($successCount -lt $totalPackages) {
            Write-Warning-Status "$($totalPackages - $successCount) packages failed to install"
        }
        
        Write-Host "`nYour PWAMP stack is ready!" -ForegroundColor Green
    }
    catch {
        Write-Error-Status "Installation failed: $($_.Exception.Message)"
        exit 1
    }
}

#endregion

# Main execution
if ($MyInvocation.InvocationName -ne '.') {
    Install-PWAMP -InstallationPath $InstallPath -ConfigurationFile $ConfigFile -QuietMode:$Quiet
}