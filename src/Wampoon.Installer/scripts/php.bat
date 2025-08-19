@echo off
REM Wampoon PHP Command Line Wrapper
REM This script sets up the environment and runs PHP with proper paths

REM Set WAMPOON_ROOT_DIR to the parent directory (one level up from scripts folder)
set "WAMPOON_ROOT_DIR=%~dp0"

REM Define paths to Wampoon PHP
set "WAMPOON_PHP_DIR=%WAMPOON_ROOT_DIR%\apps\php"
set "PHP_EXE=%WAMPOON_PHP_DIR%\php.exe"

REM Check if PHP exists
if not exist "%PHP_EXE%" (
    echo Error: PHP not found at %PHP_EXE%
    echo Please ensure Wampoon is properly installed.
    exit /b 1
)

REM Show environment info (optional - can be removed for production)
echo Wampoon Root Directory: %WAMPOON_ROOT_DIR%
echo PHP Path: %PHP_EXE%
echo.

REM Run PHP with all passed arguments
"%PHP_EXE%" %*

REM Exit with the same code as PHP
exit /b %ERRORLEVEL%