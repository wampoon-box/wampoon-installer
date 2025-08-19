@echo off
REM Wampoon MariaDB MySQL Command Line Wrapper
REM This script sets up the environment and runs MariaDB MySQL client

REM Set WAMPOON_ROOT_DIR to the parent directory (one level up from scripts folder)
set "WAMPOON_ROOT_DIR=%~dp0"

REM Define paths to Wampoon MariaDB
set "WAMPOON_MARIADB_DIR=%WAMPOON_ROOT_DIR%\apps\mariadb"
set "MYSQL_EXE=%WAMPOON_MARIADB_DIR%\bin\mysql.exe"

REM Check if MariaDB MySQL client exists
if not exist "%MYSQL_EXE%" (
    echo Error: MariaDB MySQL client not found at %MYSQL_EXE%
    echo Please ensure Wampoon is properly installed.
    exit /b 1
)

REM Show environment info (optional - can be removed for production)
echo Wampoon Root Directory: %WAMPOON_ROOT_DIR%
echo MariaDB MySQL Client Path: %MYSQL_EXE%
echo.

REM Run MariaDB MySQL client with all passed arguments
"%MYSQL_EXE%" %*

REM Exit with the same code as MySQL client
exit /b %ERRORLEVEL%