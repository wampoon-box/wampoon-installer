## TODO:
# 1. Add a function to check if the software is already installed.
# - Use Test-Path to check if the directories exist.
# - Use Join-Path to create paths.


$appName = "pwampp"

# Software versions and URLs
$apacheVersion = "2.4.63-250207"
$phpVersion = "8.4.6"
$mysqlVersion = "8.0.35"

# Define ports for Apache and MySQL
$serverPort = 8080
$mysqlPort = 3306

# Set base directory to current script location 
$baseDir = Split-Path -Parent $MyInvocation.MyCommand.Path 
# Set the temp directory to the system's temp directory
Write-Output "Base Directory: $baseDir"

# Define URIs for downloading software
$apacheUrl = "https://www.apachelounge.com/download/VS17/binaries/httpd-$apacheVersion-win64-VS17.zip"
$phpUrl = "https://windows.php.net/downloads/releases/php-$phpVersion-Win32-vs17-x64.zip"
$mysqlUrl = "https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-$mysqlVersion-winx64.zip"

# Define paths for Apache, PHP, and MySQL
$installDir = Join-Path -Path $baseDir "pwampp"
# $tempDir = $env:TEMP
$tempDir = "$baseDir\temp"

$logFile = "$baseDir\logs\pwampp-portable.log"
$apachePath = "$installDir\apache"
$phpPath = "$installDir\php"
$mysqlPath = "$installDir\mysql"
$htdocsDir = "$installDir\htdocs"

$phpIniFile = "$phpPath\php.ini"
$mySqlIniFile = "$mysqlPath\bin\my.ini"
$httpdConf = Join-Path $apachePath "conf\httpd.conf"

#TODO: Remove the following variables.
$apacheZip = "$tempDir\apache.zip"
$phpZip = "$tempDir\php.zip"
$mysqlZip = "$tempDir\mysql.zip"


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
        "INFO" { Write-Host $logMessage -ForegroundColor Blue }
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

# Function to extract zip files with progress
function Extract-Zip {
    param (
        [string]$ZipFile,
        [string]$Destination
    )
    
    try {
        Write-Log "Extracting $ZipFile to $Destination..."
        
        # Create destination directory if it doesn't exist
        if (-not (Test-Path $Destination)) {
            New-Item -ItemType Directory -Path $Destination -Force | Out-Null
        }
        
        # Extract zip file
        Expand-Archive -Path $ZipFile -DestinationPath $Destination -Force
        
        Write-Log "Extraction completed successfully" -Level "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Failed to extract $ZipFile. Error: $_" -Level "ERROR"
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

# Function to clean up temporary files
function Remove-TempFiles {
    try {
        Write-Log "Cleaning up temporary files..."
        
        if (Test-Path $tempDir) {
            Remove-Item $tempDir -Recurse -Force
            Write-Log "Temporary files cleaned up successfully" -Level "SUCCESS"
            return $true
        }
        else {
            Write-Log "Temporary directory not found, nothing to clean up" -Level "WARNING"
            return $true
        }
    }
    catch {
        Write-Log "Failed to clean up temporary files. Error: $_" -Level "ERROR"
        return $false
    }
}


function Install-Apache {
    # Download and extract Apache
    Write-Log "Processing Apache..." -Level "INFO"
    if (-not (Test-Path $apachePath)) {
        if (Download-File -Url $apacheUrl -Output $apacheZip) {
            # Create a temporary extraction directory
            $tempExtractPath = "$tempDir\apache_extract"
            if (Extract-Zip -ZipFile $apacheZip -Destination $tempExtractPath) {
                # Find the Apache24 directory in the extracted content
                $apacheDir = Get-ChildItem -Path $tempExtractPath -Filter "Apache24" -Directory
                if ($apacheDir) {
                    # Move to the final destination
                    Copy-Item -Path "$tempExtractPath\Apache24" -Destination $apachePath -Recurse -Force                    
               
                    Write-Log "Apache installed successfully." -Level "SUCCESS"
                    return $true
                }
                else {
                    Write-Log "Could not find Apache24 directory in the extracted files." -Level "ERROR"
                    return $false
                }
            }
        }            
    }
    else {
        Write-Log "Apache already installed, skipping download and extraction" -Level "WARNING"
    }    
}

function Install-PHP {
    # Download and extract PHP
    Write-Log "Processing PHP..." -Level "INFO"
    if (-not (Test-Path $phpPath)) {
        if (Download-File -Url $phpUrl -Output $phpZip) {
            if (-not (Extract-Zip -ZipFile $phpZip -Destination $phpPath)) {
                return $false
            }
        }
        else {
            return $false
        }
    }
    else {
        Write-Log "PHP already installed, skipping download and extraction" -Level "WARNING"
    }    
}

function Install-MySQL {
    # Download and extract MySQL
    Write-Log "Processing MySQL..." -Level "INFO"
    if (-not (Test-Path $mysqlPath)) {
        if (Download-File -Url $mysqlUrl -Output $mysqlZip) {
            if (-not (Extract-Zip -ZipFile $mysqlZip -Destination $installDir)) {
                return $false
            }
        }
        else {
            return $false
        }
    }
    else {
        Write-Log "MySQL already installed, skipping download and extraction" -Level "WARNING"
    }    
}

# Function to install all components
function Install-All {
    try {
        Write-Log "Starting installation process..." -Level "INFO"
        Write-Log "Installing Apache, PHP, and MySQL..." -Level "WARNING"
        
        # Create installation directory
        if (-not (Test-Path $installDir)) {
            New-Item -ItemType Directory -Path $installDir -Force | Out-Null
        }
        
        # Create temp directory
        if (-not (Test-Path $tempDir)) {
            New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
        }
        
        Install-Apache
        Install-PHP
        Install-MySQL
                
        # Configure components
        # if (-not (Configure-Apache)) { return $false }
        # if (-not (Configure-PHP)) { return $false }
        # if (-not (Initialize-MySQL)) { return $false }
        
        # # Create scripts and test file
        # if (-not (Create-StartupScript)) { return $false }
        # if (-not (Create-ShutdownScript)) { return $false }
        # if (-not (Create-TestPHP)) { return $false }
        
        # Clean up
        # if (-not (Remove-TempFiles)) { return $false }
        
        Write-Log "Installation completed successfully!" -Level "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Installation failed. Error: $_" -Level "ERROR"
        return $false
    }
}


# -----------------------------------
# Main script execution logic.
# -----------------------------------

if (-not (Test-Path $tempDir)) {
    Write-Host "Creating directory: $tempDir" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $tempDir | Out-Null
}



Install-All

# $installMySql = Read-Host "Should MySql be installed? (y/n)"


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