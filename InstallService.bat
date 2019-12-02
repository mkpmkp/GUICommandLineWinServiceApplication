@ECHO OFF

echo Installing Application1 Service
echo -------------------------------
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil /ServiceName="Application1.Service" /DisplayName="Application1 Service" /ConsolePort="500" /Description="Application1" .\Application1.exe
echo -------------------------------
pause
