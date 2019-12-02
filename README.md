# GUICommandLineWinServiceApplication
Заготовка приложения для проекта C# которое может запускаться как оконное (GUI) приложение, служба Windows и как Command Line приложение. Содержит Telnet консоль для контроля и управления.

C# проект демонстрирует приложение которое может работать в 3 режимах.
1. Служба Windows.
2. Оконное GUI приложение.
3. Command line приложение (в примере используется для Install/Uninstall Windows service #пока не доделано).

Приложение. запущенное как GUI или Service, запускает Telnet server, к которому можно подключиться Telnet-клиентом (PuTTY).

Приложение может быть установлено как сервис при помощи утилиты InstallUtil:
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil /ServiceName="Application1.Service" /DisplayName="Application1 Service" /ConsolePort="500" /Description="Application1" .\Application1.exe

и удалено:
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil /u /ServiceName="Application1.Service" /DisplayName="Application1 Service" .\Application1.exe

Приложение будет дорабатываться.
