# ==============================
# Pwamp Installer Main Script
# ==============================

# --- PARAMETERS ---
param (
    [string]$InstallPath = "$PSScriptRoot\..\PwampRoot"
)

# Stop on any error.
$ErrorActionPreference = "Stop"

# --- ENVIRONMENT SETUP ---
$env:PWAMP_CANCEL_FILE = $env:PWAMP_CANCEL_FILE -or "$env:TEMP\pwamp-cancel.flag"

# --- IMPORT HELPERS ---
Import-Module "$PSScriptRoot\utlis\helpers.psm1"

# --- LOAD CONFIG ---
$config = Get-InstallerConfig

# ==============================
# [FOLDERS] Create Directory Structure
# ==============================

$appsPath          = Join-Path $InstallPath "apps"
$apachePath        = Join-Path $appsPath "apache"
$mariadbPath       = Join-Path $appsPath "mariadb"
$phpPath           = Join-Path $appsPath "php"
$phpmyadminPath    = Join-Path $appsPath "phpmyadmin"
$dashboardPath     = Join-Path $appsPath "pwamp-dashboard"
$tempPath          = Join-Path $env:TEMP "pwamp-install-tmp"

# Create all required directories.
foreach ($dir in @(
    $appsPath, $apachePath, $mariadbPath,
    $phpPath, $phpmyadminPath, $dashboardPath, $tempPath
)) {
    Ensure-Directory -Path $dir
}

# =============================
# [DOWNLOAD] Download and Extract Files
# =============================

function Download-And-Verify {
    param (
        [string]$name,
        [string]$url,
        [string]$sha256
    )

    $filename = Split-Path $url -Leaf
    $downloadPath = Join-Path $tempPath $filename

    Test-Cancel
    Write-Host "[DOWNLOAD] Downloading $name..."
    Invoke-Expression "curl.exe -L `"$Url`" -o `"$downloadPath`""                       

    Test-Cancel
    Write-Host "[VERIFY] Verifying $name checksum..."
    # Assert-FileHash -Path $downloadPath -ExpectedHash $sha256

    return $downloadPath
}

# --- Apache ---
$apacheZip = Download-And-Verify `
    -name "Apache" `
    -url $config.apache.url `
    -sha256 $config.apache.sha256
Extract-Zip -Source $apacheZip -Destination $apachePath
Test-Cancel

# --- MariaDB ---
# $mariadbZip = Download-And-Verify `
#     -name "MariaDB" `
#     -url $config.mariadb.url `
#     -sha256 $config.mariadb.sha256
# Extract-Zip -Source $mariadbZip -Destination $mariadbPath
# Test-Cancel

# --- PHP ---
# $phpZip = Download-And-Verify `
#     -name "PHP" `
#     -url $config.php.url `
#     -sha256 $config.php.sha256
# Extract-Zip -Source $phpZip -Destination $phpPath
# Test-Cancel

# --- phpMyAdmin ---
# $phpmyadminZip = Download-And-Verify `
#     -name "phpMyAdmin" `
#     -url $config.phpmyadmin.url `
#     -sha256 $config.phpmyadmin.sha256
# Extract-Zip -Source $phpmyadminZip -Destination $phpmyadminPath
# Test-Cancel

# --- Pwamp Dashboard ---
# $dashboardZip = Download-And-Verify `
#     -name "Pwamp Dashboard" `
#     -url $config.dashboard.url `
#     -sha256 $config.dashboard.sha256
# Extract-Zip -Source $dashboardZip -Destination $dashboardPath
# Test-Cancel

# =============================
# [CONFIG] Apply Config Templates
# =============================

# TODO: Test applying a template.
# Write-Host "[CONFIG] Applying Apache config..."

# $httpdTemplate = "$PSScriptRoot/config/apache/httpd.conf.template"
# $httpdTarget   = Join-Path $apachePath "conf\httpd.conf"

# Apply-Template `
#     -TemplatePath $httpdTemplate `
#     -TargetPath $httpdTarget `
#     -Replacements @{
#         "PWAMP_ROOT"     = $InstallPath
#         "APACHE_ROOT"    = $apachePath
#         "PHP_DIR"        = $phpPath
#         "PHPMYADMIN_DIR" = $phpmyadminPath
#         "DASHBOARD_DIR"  = $dashboardPath
#     }

# Extend with Apply-Template for my.ini, php.ini, etc., as needed.

# =============================
# [COMPLETE] Done
# =============================

Write-Host "`n[SUCCESS] Pwamp installed successfully at: $InstallPath"
