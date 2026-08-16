@echo off
setlocal enabledelayedexpansion

echo ========================================================
echo       Revit Warning Reporter - Uninstall Script
echo       Copyright (c) 2026 Arman Arisman
echo ========================================================
echo.

set "TARGET_DIR=%APPDATA%\Autodesk\Revit\Addins\2027"

echo Checking Revit Warning Reporter installation at:
echo %TARGET_DIR%
echo.

set "MANIFEST=%TARGET_DIR%\RevitWarningReporter.addin"
set "FOLDER=%TARGET_DIR%\RevitWarningReporter"

set "FOUND=0"

if exist "%MANIFEST%" (
    echo Removing manifest: %MANIFEST%
    del /F /Q "%MANIFEST%"
    set "FOUND=1"
)

if exist "%FOLDER%" (
    echo Removing add-in directory: %FOLDER%
    rmdir /S /Q "%FOLDER%"
    set "FOUND=1"
)

echo.
if "!FOUND!"=="1" (
    echo ========================================================
    echo [SUCCESS] Revit Warning Reporter has been uninstalled!
    echo Please restart Autodesk Revit 2027.
    echo ========================================================
) else (
    echo [INFO] Revit Warning Reporter was not found in Revit 2027 Addins directory.
)

echo.
pause
