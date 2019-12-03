# GUICommandLineWinServiceApplication
Заготовка приложения для проекта C# которое может запускаться как оконное (GUI) приложение, служба Windows и как Command Line приложение. Содержит Telnet консоль для контроля и управления.

C# проект демонстрирует приложение которое может работать в 3 режимах.
1. Служба Windows.
2. Оконное GUI приложение.
3. Command line приложение (в примере используется для Install/Uninstall Windows service #пока не доделано).

Приложение. запущенное как GUI или Service, запускает Telnet server, к которому можно подключиться Telnet-клиентом (#PuTTY).

Приложение может быть установлено как сервис при помощи утилиты InstallUtil:

`C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil /ServiceName="Application1.Service" /DisplayName="Application1 Service" /Description="Application1" .\Application1.exe`

Приложение может быть удалено как сервис при помощи утилиты InstallUtil:

`C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil /u /ServiceName="Application1.Service" /DisplayName="Application1 Service" .\Application1.exe`

В проекте реализованы:

* Работа с конфигурацией.
* Разбор строки параметров.
* Универсальная процедура отображения в консоле сложных объектов (классы, справочники, массивы... и их комбинации).
* Справочная система (man).

**Примеры:**

    > state
    <Dictionary>(17) [
       ["ApplicationFileName"] = "Application1.exe"
       ["ApplicationName"] = "Application1"
       ["Режим работы клиента"] = "GUI"
       ["ApplicationStartTime"] = 03.12.2019 08:23:43
       ["Current system time"] = 03.12.2019 08:24:00
       ["Current system user"] = "<UserName>"
       ["LogLevel"] = 0
       ["ConsoleHost"] = "<HostName>"
       ["ConsolePort"] = 500
       ["ConfigProfile"] = "Default"
       ["DB_Name"] = "DataBaseName"
       ["DB_User"] = "DB_User"
       ["DB_Password"] = "123456789"
       ["SenderID"] = "Application1"
       ["BuildDate"] = 03.12.2019 08:23:42
       ["CurrentConfigProfile"] = "Default"
       ["CurrentConfigParameters"] = <Dictionary>(7) [
          ["SenderID"] = "Application1"
          ["TransportCertificate"] = "9E94DD0C3C0A1ABE37CA2DFB2A408F30A417A4DA"
          ["DBHost"] = "sql.server.local"
          ["DBName"] = "DataBaseName"
          ["DBUserName"] = "DB_User"
          ["DBUserPassword"] = "vCRYH7xXuh5opv/RTcTjWw=="
          ["ConsolePort"] = "500"
       ]
    ]


    > man set
    SET
    Usage
      set <parameter> <volume>
    Примеры
      set loglevel <0-9> - установить уровень логирования 0-9.


    > test parameters /Install /Description=1234 /id=12,12,3,6 /Reload /First:12
    <Dictionary>(5) [
       ["install"] = "True"
       ["description"] = "1234"
       ["id"] = "12,12,3,6"
       ["reload"] = "True"
       ["first"] = "12"
    ]



Приложение будет дорабатываться, комментироваьтся.

**Контакты:**
E-mail: mkp[@]inbox.ru  (Желательно в теме письма указывать "Application1")
Skype: kirill.mastushkin
