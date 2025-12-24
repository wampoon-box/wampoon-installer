@echo off
REM Wampoon Apache HTTP Server Wrapper
REM This script sets up the environment and runs Apache with proper paths

REM Set WAMPOON_ROOT_DIR to the parent directory (one level up from scripts folder)
set "WAMPOON_ROOT_DIR=%~dp0"

REM Define paths
set "APACHE_DIR=%WAMPOON_ROOT_DIR%\apps\apache"
set "HTTPD_EXE=%APACHE_DIR%\bin\httpd.exe"

REM Check if Apache exists
if not exist "%HTTPD_EXE%" (
    echo Error: Apache not found at %HTTPD_EXE%
    echo Please ensure Wampoon is properly installed.
    exit /b 1
)

REM Set OpenSSL config path (fixes hardcoded C:\Apache24\conf\openssl.cnf issue)
set "OPENSSL_CONF=%APACHE_DIR%\conf\openssl.cnf"

REM Show environment info
echo Wampoon Root Directory: %WAMPOON_ROOT_DIR%
echo Apache Path: %HTTPD_EXE%
echo OpenSSL Config: %OPENSSL_CONF%
echo.

REM Run Apache with all passed arguments
"%HTTPD_EXE%" %*

REM Exit with the same code as Apache
exit /b %ERRORLEVEL%
