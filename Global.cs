using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace Application1
{

	// Класс для хранения глобальных переменных приложения
	// Есть мнение что такая организация хранения данных - зло
	public static class Global {

		public static string ApplicationFileName = @"Application1.exe";
		public static string ApplicationName = Application.ProductName.ToString();

		// Дата сборки
		public static DateTime BuildDate;

		// Как запущено приложение Service/GUI
		public static string StartedAs = null;

		// Конфигурация приложения
		public static ConfigurationClass Configuration;

		public static int LogLevel = 0;

		public static DateTime ApplicationStartTime = DateTime.Now;

		public static string HomePath;
		public static string LocalCommandLogPath;
		public static string LocalLogPath;

		// Параметры из конфагурации. Устанавливаются функцией ApplyConfiguration().
		public static string SenderId;
		public static string DBName;
		public static string DBUser;

		private static readonly object syncRoot = new object();

		// Указатели на сервисные потоки
		public static Thread Console;
		public static int ConsolePort = 500;
		public static string ConsolePassword = @"123456789";

		// Метод для получения сертификата по заданному отпечатку
		public static X509Certificate2 GetCertificateByThumbprint(string certificateThumbprint) {
			var certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
			certificateStore.Open(OpenFlags.ReadOnly);
			var certificateCollection = certificateStore.Certificates.Find((X509FindType)Enum.Parse(typeof(X509FindType), "FindByThumbprint"), certificateThumbprint, false);
			return certificateCollection.Count != 0 ? certificateCollection[0] : null;
		}

		// Применение текущей конфигурации
		public static object ApplyConfiguration() {
			try {
				SenderId = (string)Configuration.Current.SenderID ?? Guid.Empty.ToString();
				DBName = Configuration.Current.DBName;
				DBUser = Configuration.Current.DBUserName;
				if (Configuration.Current.Has(@"ConsolePort")) ConsolePort = ((string)Configuration.Current.ConsolePort)?.ToInt() ?? 500;
				if (Configuration.Current.Has(@"ConsolePassword")) ConsolePassword = Configuration.Current.ConsolePassword;
				return true;
			} catch (Exception Ex) {
				if (Environment.UserInteractive) {
					MessageBox.Show($"Ошибка применения конфигурации: {Ex.Message}");
				}
				return new Error(@"#INT000004", $"Ex: {Ex.Message}", Ex.StackTrace);
			}
		}

		// Запуск всех параллельных служб
		public static void StartServices() {
			// Стартуем консоль
			Console = new Thread(ConsoleServer.Start);
			Console.Start();
		}

		// Проверяем и создаём необходимые локальные пути и имена логфайлов
		public static void LocalPaths() {
			HomePath = AppDomain.CurrentDomain.BaseDirectory;
			LocalCommandLogPath = Path.Combine(HomePath, @"CommandLog.txt");
			LocalLogPath = Path.Combine(HomePath, @"Log.txt");
		}

	}
}
