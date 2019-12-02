using System;
using System.Reflection;
using System.ServiceProcess;

namespace Application1 {
	partial class WinService : ServiceBase {

		public static string SessionID;

		public WinService() {
			InitializeComponent();

			Global.StartedAs = @"Service";
			SessionID = Guid.NewGuid().ToString();
		}

		protected override void OnStart(string[] args) {
			Global.LocalPaths();
			// Дата сборки в заголовке окна
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			Global.BuildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
			
			LOG.AsyncFileWrite(Global.LocalLogPath, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} SessionID=[{SessionID}]. Запуск службы. Сборка от {Global.BuildDate:dd.MM.yyyy HH:mm:ss}");
			try {
				Global.Configuration = new ConfigurationClass();
			} catch (Exception Ex) {
				LOG.AsyncFileWrite(Global.LocalLogPath, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} SessionID=[{SessionID}]. Ошибка конфигурации: {Ex.Message}");
				this.Stop();
				return;
			}
			Global.ApplyConfiguration();
			try {
				Global.StartServices();
			} catch(Exception Ex) {
				LOG.AsyncFileWrite(Global.LocalLogPath, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} SessionID=[{SessionID}]. Ошибка запуска основных сервисов: {Ex.Message}\r\n{Ex.StackTrace}\r\n");
				this.Stop();
				return;
			}
			LOG.AsyncFileWrite(Global.LocalLogPath, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} SessionID=[{SessionID}]. Служба запущена.");
		}

		protected override void OnStop() {
			ConsoleServer.Stop();
			LOG.AsyncFileWrite(Global.LocalLogPath, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} SessionID=[{SessionID}]. Служба остановлена.");
		}
	}
}
