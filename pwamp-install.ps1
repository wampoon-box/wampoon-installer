## TODO:
# 1. Add a function to check if the software is already installed.
# - Use Test-Path to check if the directories exist.
# - Use Join-Path to create paths.
# This script updates Apache's httpd.conf and PHP's php.ini files to use relative paths
# for portable deployment on USB drives or other movable media

$appName = "pwampp"

# Software versions and URLs
$apacheVersion = "2.4.63-250207"
$phpVersion = "8.4.6"
$mysqlVersion = "8.0.35"

# Define ports for Apache and MySQL
$serverPort = 80 # Change to 8080 if port 80 is in use
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

$logFile =  "$baseDir\logs\pwampp-portable.log"
$apachePath = "$installDir\apache"
$phpPath = "$installDir\php"
$mysqlPath = "$installDir\mysql"
$htdocsDir = "$installDir\htdocs"

$phpIniFile = "$phpPath\php.ini"
$mySqlIniFile = "$mysqlPath\bin\my.ini"


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
function Get-File {
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
function Expand-Zip {
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



# Function to backup files before modifying them
function Create-Backup {
    param (
        [string]$FilePath
    )
    
    if (Test-Path $FilePath) {
        $BackupPath = "$FilePath.bak"
        Write-Log "Creating backup of $FilePath to $BackupPath"
        Copy-Item -Path $FilePath -Destination $BackupPath -Force
        return $true
    } else {
        Write-Log "Warning: File $FilePath not found!" -Level "WARNING"
        return $false
    }
}

function Install-Apache {
    # Download and extract Apache
    Write-Log "Processing Apache..." -Level "INFO"
    if (-not (Test-Path $apachePath)) {
        if (Get-File -Url $apacheUrl -Output $apacheZip) {
            # Create a temporary extraction directory
            $tempExtractPath = "$tempDir\apache_extract"
            if (Expand-Zip -ZipFile $apacheZip -Destination $tempExtractPath) {
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
        return $false
    }    
}

function Install-PHP {
    # Download and extract PHP
    Write-Log "Processing PHP..." -Level "INFO"
    if (-not (Test-Path $phpPath)) {
        if (Get-File -Url $phpUrl -Output $phpZip) {
            if (-not (Expand-Zip -ZipFile $phpZip -Destination $phpPath)) {
                return $false
            }
        }
        else {
            return $false
        }
    }
    else {
        Write-Log "PHP already installed, skipping download and extraction" -Level "WARNING"
        return $false
    }    
}

function Install-MySQL {
    # Download and extract MySQL
    Write-Log "Processing MySQL..." -Level "INFO"
    if (-not (Test-Path $mysqlPath)) {
        if (Get-File -Url $mysqlUrl -Output $mysqlZip) {
            if (-not (Expand-Zip -ZipFile $mysqlZip -Destination $installDir)) {
                return $false
            }
        }
        else {
            return $false
        }
    }
    else {
        Write-Log "MySQL already installed, skipping download and extraction" -Level "WARNING"
        return $false
    }    
}


#----------------------------
#-- Configuration functions | 
#----------------------------

# Function to update Apache httpd.conf file
function Update-HttpdConf {
    param (
        [string]$HttpdConfPath
    )
    
    if (-not (Create-Backup -FilePath $HttpdConfPath)) {
        return
    }
    $phpDir = "../php"
    Write-Log "Updating httpd.conf to use relative paths..." -Level "INFO"
    
    $content = Get-Content -Path $HttpdConfPath -Raw
    
    # Replace ServerRoot with relative path
    $content = $content -replace '(?m)^ServerRoot\s+"[^"]*"', 'ServerRoot "../"'
    # Update DocumentRoot path
    $content = $content -replace '(?m)^DocumentRoot\s+"[^"]*"', 'DocumentRoot "../htdocs"'
    # Update Directory paths
    $content = $content -replace '<Directory\s+"[^"]*htdocs">', '<Directory "../htdocs">'
    # Update ErrorLog paths
    $content = $content -replace '(?m)^ErrorLog\s+"[^"]*"', 'ErrorLog "./logs/error.log"'
    # Update CustomLog paths
    $content = $content -replace '(?m)^CustomLog\s+"[^"]*"\s+', 'CustomLog "./logs/access.log" '    
    # Update ScriptAlias paths
    $content = $content -replace '(?m)^ScriptAlias\s+/cgi-bin/\s+"[^"]*"', 'ScriptAlias /cgi-bin/ "./cgi-bin/"'
    # Update Directory configuration for CGI
    $content = $content -replace '<Directory\s+"[^"]*cgi-bin">', '<Directory "./cgi-bin">'
    $content = $content -replace '#LoadModule rewrite_module', 'LoadModule rewrite_module'

    # Update PHP module path (assuming PHP is in parent directory)
    $phpModulePattern = 'LoadModule\s+php_module\s+"[^"]*"'
    if ($content -match $phpModulePattern) {
        $content = $content -replace $phpModulePattern, 'LoadModule php_module "../php/php8apache2_4.dll"'
    } else {
        # If no PHP module is defined, add it before the end of LoadModule section
        $lastLoadModule = $content -split "(?m)^LoadModule" | Select-Object -Last 1
        $insertPos = $content.LastIndexOf("LoadModule" + $lastLoadModule)
        $insertPos = $content.IndexOf("`n", $insertPos) + 1
        $content = $content.Insert($insertPos, "LoadModule php_module ""../php/php8apache2_4.dll""`n")
    }
    
    # Update PHPIniDir path
    $phpIniDirPattern = 'PHPIniDir\s+"[^"]*"'
    if ($content -match $phpIniDirPattern) {
        $content = $content -replace $phpIniDirPattern, 'PHPIniDir "../php"'
    } else {
        # If PHPIniDir is not defined, add it after PHP module
        $insertPos = $content.IndexOf("LoadModule php_module")
        $insertPos = $content.IndexOf("`n", $insertPos) + 1
        $content = $content.Insert($insertPos, "PHPIniDir ""../php""`n")
    }
    
    # Make sure PHP file type is configured
    if ($content -notmatch 'AddType\s+application/x-httpd-php\s+\.php') {
        $insertPos = $content.IndexOf("PHPIniDir")
        $insertPos = $content.IndexOf("`n", $insertPos) + 1
        $content = $content.Insert($insertPos, "AddType application/x-httpd-php .php`n")
        $insertPos = $content.IndexOf("`n", $insertPos) + 1
        $content = $content.Insert($insertPos, "DirectoryIndex index.php index.html`n")        
    }
    
    # Save updated file
    $content | Set-Content -Path $HttpdConfPath -Force
    Write-Host "httpd.conf updated successfully." -ForegroundColor Green
}


# function Configure-Apache {
#     try {        
#         Write-Log "Configuring Apache..." -Level "INFO"    
        
#         $httpdConf = Join-Path $apachePath "conf\httpd.conf"
        
#         if (Test-Path $httpdConf) {
#             # Modify Apache config for portable use
#             (Get-Content $httpdConf) | ForEach-Object {
#                 $_ -replace "c:/Apache24", $apachePath.replace('\', '/') `
#                     -replace "ServerRoot .*$", "ServerRoot `"$($apachePath.replace('\', '/'))`"" `
#                     -replace "Listen 80", "Listen $serverPort" `
#                     -replace "#ServerName www.example.com:80", "ServerName localhost:$serverPort" `
#                     -replace "#LoadModule rewrite_module", "LoadModule rewrite_module"
#             } | Set-Content $httpdConf
            
#             # Add PHP configuration
#             Add-Content -Path $httpdConf -Value "`n# PHP Configuration"
#             Add-Content -Path $httpdConf -Value "LoadModule php_module `"$($phpPath.replace('\', '/'))/php8apache2_4.dll`""
#             Add-Content -Path $httpdConf -Value "AddHandler application/x-httpd-php .php"
#             Add-Content -Path $httpdConf -Value "PHPIniDir `"$($phpPath.replace('\', '/'))`""
            
#             Write-Log "Apache configuration completed successfully" -Level "SUCCESS"
#             return $true
#         }
#         else {
#             Write-Log "Apache configuration file not found at $httpdConf" -Level "ERROR"
#             return $false
#         }
#     }
#     catch {
#         Write-Log "Failed to configure Apache. Error: $_" -Level "ERROR"
#         return $false
#     }
# }
function Configure-PHP {
    Write-Log "Configuring PHP..." -Level "INFO"
    # Placeholder for PHP configuration logic
    return $true
}

function Initialize-MySQL {
    Write-Log "Initializing MySQL..." -Level "INFO"
    # Placeholder for MySQL initialization logic
    return $true
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
        # Create htdocs directory
        if (-not (Test-Path $htdocsDir)) {
            Write-Log "Creating directory: $htdocsDir" -ForegroundColor Yellow
            New-Item -ItemType Directory -Path $htdocsDir -Force | Out-Null
        }
        
        # Create temp directory
        if (-not (Test-Path $tempDir)) {
            New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
        }
        
        # if (-not (Install-Apache)) { return $false }
        
        # Install-PHP
        # Install-MySQL
                
        # Configure components
        #  if (-not (Configure-Apache)) { return $false }
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
Update-HttpdConf -HttpdConfPath "$apachePath\conf\httpd.conf"


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