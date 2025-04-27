$appName = "pwampp"
# Software versions and URLs
$apacheVersion = "2.4.63-250207"
$phpVersion = "8.2.16"
$mysqlVersion = "8.0.35"

# Set base directory to current script location 
$baseDir = Split-Path -Parent $MyInvocation.MyCommand.Path 
# Set the temp directory to the system's temp directory
Write-Output "Base Directory: $baseDir"

# Define URIs for downloading software
$apacheUrl = "https://www.apachelounge.com/download/VS17/binaries/httpd-$apacheVersion-win64-VS17.zip"
$phpUrl = "https://windows.php.net/downloads/releases/php-$phpVersion-nts-Win32-vs16-x64.zip"
$mysqlUrl = "https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-$mysqlVersion-winx64.zip"

# Define paths for Apache, PHP, and MySQL
$installDir = "$baseDir\pwampp"
# $tempDir = $env:TEMP
$tempDir = "$baseDir\temp"

$logFile = "$baseDir\logs\pwampp-portable.log"
$apachePath = "$installDir\Apache"
$phpPath = "$installDir\php"
$mysqlPath = "$installDir\mysql"
$htdocsDir = "$installDir\htdocs"

$phpIniFile = "$phpPath\php.ini"
$mySqlIniFile = "$mysqlPath\bin\my.ini"
# The path to httpd.conf
$httpdConf = Join-Path $apachePath "conf\httpd.conf"


$apacheZip = "$tempDir\apache.zip"
$phpZip = "$tempDir\php.zip"
$mysqlZip = "$tempDir\mysql.zip"

$serverPort = 8080
$mysqlPort = 3306

# Function to write to log file
function Write-Log {
    param (
        [string]$Message,
        [string]$Level = "INFO"
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    Add-Content -Path $logFile -Value $logMessage
    
    # Also write to console with color based on level
    switch ($Level) {
        "ERROR" { Write-Host $logMessage -ForegroundColor Red }
        "WARNING" { Write-Host $logMessage -ForegroundColor Yellow }
        "SUCCESS" { Write-Host $logMessage -ForegroundColor Green }
        default { Write-Host $logMessage }
    }
}

# Function to download files with progress
function Download-File {
    param (
        [string]$Url,
        [string]$Output
    )    
    Write-Log "Downloading $Url to $Output..." -Level "WARNING"
    try {
        Invoke-Expression "curl.exe -L `"$Url`" -o `"$Output`""                       
        Write-Log "Download completed successfully" -Level "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Failed to download $Url. Error: $_" -Level "ERROR"
        return $false
    }                
}

# Function to check if software is already installed
function Test-Installation {
    $apacheInstalled = Test-Path $apachePath
    $phpInstalled = Test-Path $phpPath
    $mysqlInstalled = Test-Path $mysqlPath
    
    return ($apacheInstalled -and $phpInstalled -and $mysqlInstalled)
}

# Function to display menu
function Show-Menu {
    Clear-Host
    Write-Host "===== XAMPP Portable Manager =====" -ForegroundColor Cyan
    Write-Host "1. Install Apache, PHP, and MySQL"
    Write-Host "2. Start Server"
    Write-Host "3. Stop Server"
    Write-Host "4. Reset MySQL Password"
    Write-Host "5. View Installation Status"
    Write-Host "6. View Server Information"
    Write-Host "7. Exit"
    Write-Host "=================================="
}

# -----------------------------------
# Main script execution logic.
# -----------------------------------

if (-not (Test-Path $tempDir)) {
    Write-Host "Creating directory: $tempDir" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $tempDir | Out-Null
}

Download-File -Url $apacheUrl -Output $apacheZip



# $installMySql = Read-Host "Should MySql be installed? (y/n)"

