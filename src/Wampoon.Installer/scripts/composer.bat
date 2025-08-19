@echo off
REM Wampoon Composer Command Line Wrapper
REM This script sets up the environment and runs Composer with proper PHP path

REM Set WAMPOON_ROOT_DIR to the parent directory (one level up from scripts folder)
set "WAMPOON_ROOT_DIR=%~dp0"

REM Define paths to Wampoon components
set "WAMPOON_PHP_DIR=%WAMPOON_ROOT_DIR%\apps\php"
set "WAMPOON_COMPOSER_DIR=%WAMPOON_ROOT_DIR%\apps\composer"
set "PHP_EXE=%WAMPOON_PHP_DIR%\php.exe"
set "COMPOSER_PHAR=%WAMPOON_COMPOSER_DIR%\composer.phar"

REM Check if PHP exists
if not exist "%PHP_EXE%" (
    echo Error: PHP not found at %PHP_EXE%
    echo Please ensure Wampoon is properly installed.
    exit /b 1
)

REM Check if Composer exists
if not exist "%COMPOSER_PHAR%" (
    echo Error: Composer not found at %COMPOSER_PHAR%
    echo Please ensure Wampoon is properly installed.
    exit /b 1
)

REM Show environment info (optional - can be removed for production)
echo Wampoon Root Directory: %WAMPOON_ROOT_DIR%
echo PHP Path: %PHP_EXE%
echo Composer Path: %COMPOSER_PHAR%
echo.

REM Run Composer with all passed arguments
"%PHP_EXE%" "%COMPOSER_PHAR%" %*

REM Exit with the same code as Composer
exit /b %ERRORLEVEL%