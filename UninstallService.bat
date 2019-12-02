@ECHO OFF

echo Uninstalling Application1 Service
echo ---------------------------------
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil /u /ServiceName="Application1.Service" /DisplayName="Application1 Service" .\Application1.exe
echo ---------------------------------
pause