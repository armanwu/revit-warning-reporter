@echo off
setlocal enabledelayedexpansion

echo ========================================================
echo        Revit Warning Reporter - Install Script
echo        Copyright (c) 2026 Arman Arisman
echo ========================================================
echo.

set "TARGET_DIR=%APPDATA%\Autodesk\Revit\Addins\2027"

rem 1. If src folder exists, build project and populate package folder
if exist "src\RevitWarningReporter.csproj" (
    echo [1/3] Building Revit Add-in project from src directory...
    dotnet build "src\RevitWarningReporter.csproj" -c Release
    if !ERRORLEVEL! NEQ 0 (
        echo.
        echo [ERROR] Project build failed!
        pause
        exit /b !ERRORLEVEL!
    )

    echo.
    echo Updating release package folder...
    if not exist "package\RevitWarningReporter" (
        mkdir "package\RevitWarningReporter"
    )

    copy /Y "src\RevitWarningReporter.addin" "package\RevitWarningReporter.addin" >nul

    if exist "src\bin\Release\net10.0-windows\RevitWarningReporter.dll" (
        xcopy /Y /Q "src\bin\Release\net10.0-windows\*.*" "package\RevitWarningReporter\" >nul
    ) else if exist "src\bin\Release\net8.0-windows\RevitWarningReporter.dll" (
        xcopy /Y /Q "src\bin\Release\net8.0-windows\*.*" "package\RevitWarningReporter\" >nul
    )
)

echo.
echo [2/3] Checking release package...
if not exist "package\RevitWarningReporter.addin" (
    echo [ERROR] package\RevitWarningReporter.addin not found!
    pause
    exit /b 1
)

if not exist "package\RevitWarningReporter" (
    echo [ERROR] package\RevitWarningReporter directory not found!
    pause
    exit /b 1
)

echo.
echo [3/3] Installing add-in to Revit 2027 directory...
if not exist "%TARGET_DIR%" (
    mkdir "%TARGET_DIR%"
)

copy /Y "package\RevitWarningReporter.addin" "%TARGET_DIR%\RevitWarningReporter.addin" >nul
if not exist "%TARGET_DIR%\RevitWarningReporter" (
    mkdir "%TARGET_DIR%\RevitWarningReporter"
)
xcopy /Y /Q /E "package\RevitWarningReporter\*.*" "%TARGET_DIR%\RevitWarningReporter\" >nul

echo.
echo ========================================================
echo [SUCCESS] Revit Warning Reporter successfully installed!
echo Manifest : %TARGET_DIR%\RevitWarningReporter.addin
echo Assembly : %TARGET_DIR%\RevitWarningReporter\
echo.
echo Please start/restart Autodesk Revit 2027.
echo The "Warning Exporter" button is located under the "Add-Ins" tab.
echo ========================================================
echo.
pause
