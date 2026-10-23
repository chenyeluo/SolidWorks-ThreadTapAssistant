@echo off
setlocal enabledelayedexpansion
title SolidWorks ThreadTapAssistant Setup
cd /d "%~dp0"

set REGASM=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe

if not exist "%REGASM%" (
    echo [ERROR] .NET Framework 64-bit RegAsm.exe not found: %REGASM%
    pause
    exit /b
)

echo ========================================================
echo   SolidWorks ThreadTapAssistant - Registering Add-in
echo ========================================================
echo.
"%REGASM%" /codebase "%~dp0ThreadTapAssistant.dll" /tlb

if %errorlevel% equ 0 (
    echo.
    echo ========================================================
    echo  [SUCCESS] ThreadTapAssistant registered successfully!
    echo ========================================================
    echo.
    echo 1. Open SolidWorks;
    echo 2. Go to Tools -^> Add-Ins;
    echo 3. Check both boxes for [ThreadTapAssistant];
    echo 4. Find the icon on the right TaskPane!
    echo.
) else (
    echo.
    echo [NOTICE] If registration failed, please right-click this file and choose "Run as administrator".
    echo Or directly double-click [一键安装.exe].
    echo.
)

pause
