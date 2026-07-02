@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

set "PROJECT_NAME=ClassIslandCLI"
set "VERSION=1.0.2.0"
set "OUTPUT_DIR=publish"
set "RELEASE_DIR=releases"

echo ==========================================
echo   ClassIslandCLI AOT Build - Windows
echo   Version: %VERSION%
echo ==========================================

if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
if exist "%RELEASE_DIR%" rmdir /s /q "%RELEASE_DIR%"
mkdir "%OUTPUT_DIR%" 2>nul
mkdir "%RELEASE_DIR%" 2>nul

for %%R in (win-x64 win-arm64 win-x86) do (
    echo.
    echo ========================================
    echo   Building %%R ...
    echo ========================================

    dotnet publish -c Release -r %%R -o "%OUTPUT_DIR%\%%R" --self-contained true
    if !errorlevel! neq 0 (
        echo [FAIL] %%R build failed with error !errorlevel!
        exit /b !errorlevel!
    )
    echo [OK] %%R build succeeded.

    echo Packaging %%R ...
    powershell -NoProfile -Command "Compress-Archive -Path '%OUTPUT_DIR%\%%R\*' -DestinationPath '%RELEASE_DIR%\%PROJECT_NAME%-%VERSION%-%%R.zip' -Force"
    if !errorlevel! neq 0 (
        echo [FAIL] %%R packaging failed with error !errorlevel!
        exit /b !errorlevel!
    )
    echo [OK] Package: %RELEASE_DIR%\%PROJECT_NAME%-%VERSION%-%%R.zip
)

echo.
echo ==========================================
echo   All builds complete!
echo ==========================================
echo.
dir "%RELEASE_DIR%" /b
echo.
endlocal
