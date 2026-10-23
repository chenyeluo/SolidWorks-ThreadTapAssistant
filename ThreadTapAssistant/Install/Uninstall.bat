@echo off
title SolidWorks 螺纹底孔小径助手 - 卸载程序

>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
if '%errorlevel%' NEQ '0' (
    echo 正在请求管理员权限以注销插件...
    powershell -Command "Start-Process '%~0' -Verb RunAs"
    exit /b
)

cd /d "%~dp0"
set REGASM=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe

echo ========================================================
echo       SolidWorks 螺纹底孔小径助手 - 卸载程序
echo ========================================================
echo.
echo 正在从系统注销 COM 插件组件...
"%REGASM%" /u "%~dp0ThreadTapAssistant.dll"

echo.
echo ========================================================
echo  [完成] 插件已成功从 SolidWorks 中注销！
echo ========================================================
echo.
pause