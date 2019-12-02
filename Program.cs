using System;
using System.Collections;
using System.Configuration.Install;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Application1 {

	static class Program {

		[STAThread]
		static void Main(string[] args) {

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
			AppDomain.CurrentDomain.UnhandledException += _UnhandledException;

			// ловим все не обработанные исключения
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(_UnhandledException2);

			Global.ApplicationFileName = AppDomain.CurrentDomain.FriendlyName;

			if (Environment.UserInteractive) {
				// Приложение имеет пользовательский интерфейс (GUI/Command line)
				if (args.Length == 0) {
					// Нет параметров коммандной строки - запускаем как оконное приложение (GUI)
					try {
						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);
						Application.Run(new Main());
					} catch (CryptographicException Ex) {
						MessageBox.Show(string.Format("Произошло критическое исключение (CryptographicException): {0}\r\n{1}", Ex.Message, Ex.StackTrace));
						LOG.AppendLog(null, (new Error(Ex)).GetMessage(true));
					} catch (Exception Ex) {
						//MessageBox.Show(string.Format("Произошло критическое исключение : {0}\r\n{1}", Ex.Message, Ex.StackTrace));
						LOG.AppendLog(null, (new Error(Ex)).GetMessage(true));
					}
				} else {
					// Есть параметры коммандной строки - запускаем как косольное приложение
					CommnadLine();
				}
			} else {
				// Приложение запущено как служба Windows
				ServiceBase.Run(new WinService());
			}
		}

		public static void CommnadLine() {
			try {
				Global.StartedAs = @"Commant line";
				AllocConsole();

				int Install = 0;
				int Uninstall = 0;
				int CheckInstalled = 0;
				int Stop = 0;
				int Start = 0;
				int Help = 0;
				string ServiceName = @"Application1.Service";
				string DisplayName = @"Application1 Service";
				string Description = @"Application1";
				string ListenerMode = @"None";
				int ConsolePort = 500;
				var parameters = new Parameters(Environment.CommandLine);

				Console.WriteLine($"-------------------------------------");
				Console.WriteLine($"Parameters:");
				Console.WriteLine($"-------------------------------------");
				Console.WriteLine(UTIL.ShowVar(parameters.parameters, ForConsole: false));
				Console.WriteLine($"-------------------------------------");

				if (parameters.Get(@"Install", @"false").ToLower() == @"true") Install = 1;
				if (parameters.Get(@"Uninstall", @"false").ToLower() == @"true") Uninstall = 1;
				if (parameters.Get(@"CheckInstalled", @"false").ToLower() == @"true") CheckInstalled = 1;
				if (parameters.Get(@"Stop", @"false").ToLower() == @"true") Stop = 1;
				if (parameters.Get(@"Start", @"false").ToLower() == @"true") Start = 1;
				if (parameters.Get(@"Help", @"false").ToLower() == @"true") Help = 1;
				if (parameters.ContainsKey(@"ServiceName")) ServiceName = parameters.Get(@"ServiceName");
				if (parameters.ContainsKey(@"DisplayName")) DisplayName = parameters.Get(@"DisplayName");
				if (parameters.ContainsKey(@"Description")) Description = parameters.Get(@"Description");
				if (parameters.ContainsKey(@"ListenerMode")) ListenerMode = parameters.Get(@"ListenerMode");
				if (parameters.ContainsKey(@"ConsolePort")) ConsolePort = parameters.Get(@"ConsolePort").ToInt(-1) ?? -1;
				// Check parameters
				if (Help == 0 && string.IsNullOrWhiteSpace(ServiceName)) {
					Console.WriteLine($"Error. <ServiceName> required parameter for this directive.");
					Pause();
					return;
				}
				if (Install + Uninstall + CheckInstalled + Help + Stop + Start > 1) {
					Console.WriteLine($"Error. Only one directive should be specified.");
					Install = 0;
					Uninstall = 0;
					CheckInstalled = 0;
					Stop = 0;
					Start = 0;
					Help = 1;
				} else if (Install + Uninstall + CheckInstalled + Help + Stop + Start < 1) {
					Console.WriteLine($"Error. There are no acceptable directives.");
					Help = 1;
				}
				// Actions
				if (Install == 1) {
					if (ConsolePort < 1024 || ConsolePort > 65535) {
						Console.WriteLine($"Error. Wrong console port specified. Port should be in range 1024-65535.");
						Pause();
						return;
					}
					Console.WriteLine($"Install");
					//InstallService();
					//StartService();
					Pause();
				} else if (Uninstall == 1) {
					Console.WriteLine($"Uninstall");
					//StopService();
					//UninstallService();
					Pause();
				} else if (CheckInstalled == 1) {
					Console.WriteLine($"CheckInstalled");
					Console.WriteLine(IsInstalled(ServiceName) ? @"Installed" + (IsRunning(ServiceName) ? @" and running." : @" but not running.") : @"Not installed.");
					Pause();
				} else if (Stop == 1) {
					StopService(ServiceName);
					Pause();
				} else if (Start == 1) {
					StartService(ServiceName);
					Pause();
				} else if (Help == 1) {
					Console.WriteLine($"-------------------------------------");
					Console.WriteLine($"Usage:");
					Console.WriteLine($"-------------------------------------");
					Console.WriteLine($"{Global.ApplicationFileName} /Install /ServiceName=<ServiceName> /DisplayName=<DisplayName> /Description=<Description> /ConsolePort=<ConsolePort>");
					Console.WriteLine($"{Global.ApplicationFileName} /UnInstall /ServiceName=<ServiceName>");
					Console.WriteLine($"{Global.ApplicationFileName} /CheckInstalled /ServiceName=<ServiceName>");
					Console.WriteLine($"{Global.ApplicationFileName} /Stop /ServiceName=<ServiceName>");
					Console.WriteLine($"{Global.ApplicationFileName} /Start /ServiceName=<ServiceName>");
					Console.WriteLine($"{Global.ApplicationFileName} /Help");
					Console.WriteLine($"-------------------------------------");
					Pause();
				} else {
					throw new NotImplementedException();
				}
			} catch (Exception Ex) {
				Console.WriteLine($"Error. {Ex.Message}");
				Pause();
			}
		}

		public static void _UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			LOG.AppendLog(null, (new Error(e.ExceptionObject)).GetMessage(true));
			if (Environment.UserInteractive) {
				MessageBox.Show($"Ex: {e.ExceptionObject}");
			}
		}
		static void _UnhandledException2(Object sender, System.Threading.ThreadExceptionEventArgs e) {
			LOG.AppendLog(null, (new Error(e.Exception)).GetMessage(true));
			if (Environment.UserInteractive) {
				MessageBox.Show($"Ex: {e.Exception}");
			}
		}

		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool AllocConsole();


		// Ниже код, обеспечивающий инсталляцию сервиса
		private static bool IsInstalled(string ServiceName) {
			using (ServiceController controller =
				new ServiceController(ServiceName)) {
				try {
					ServiceControllerStatus status = controller.Status;
				} catch {
					return false;
				}
				return true;
			}
		}
		private static bool IsRunning(string ServiceName) {
			using (ServiceController controller =
				new ServiceController(ServiceName)) {
				if (!IsInstalled(ServiceName)) return false;
				return (controller.Status == ServiceControllerStatus.Running);
			}
		}
		private static AssemblyInstaller GetInstaller() {
			AssemblyInstaller installer = new AssemblyInstaller(
				typeof(ProjectInstaller).Assembly, null); //YourServiceType
			installer.UseNewContext = true;
			return installer;
		}

		private static void InstallService(string ServiceName) {
			if (IsInstalled(ServiceName)) return;
			try {
				using (AssemblyInstaller installer = GetInstaller()) {
					IDictionary state = new Hashtable();
					try {
						installer.Install(state);
						installer.Commit(state);
					} catch {
						try {
							installer.Rollback(state);
						} catch { }
						throw;
					}
				}
			} catch {
				throw;
			}
		}
		private static void UninstallService(string ServiceName) {
			if (!IsInstalled(ServiceName)) return;
			try {
				using (AssemblyInstaller installer = GetInstaller()) {
					IDictionary state = new Hashtable();
					try {
						installer.Uninstall(state);
					} catch {
						throw;
					}
				}
			} catch {
				throw;
			}
		}
		private static void StartService(string ServiceName) {
			if (!IsInstalled(ServiceName)) return;
			using (ServiceController controller =
				new ServiceController(ServiceName)) {
				try {
					if (controller.Status != ServiceControllerStatus.Running) {
						controller.Start();
						controller.WaitForStatus(ServiceControllerStatus.Running,
							TimeSpan.FromSeconds(10));
					}
				} catch {
					throw;
				}
			}
		}
		private static void StopService(string ServiceName) {
			if (!IsInstalled(ServiceName)) return;
			using (ServiceController controller =
				new ServiceController(ServiceName)) {
				try {
					if (controller.Status != ServiceControllerStatus.Stopped) {
						controller.Stop();
						controller.WaitForStatus(ServiceControllerStatus.Stopped,
							TimeSpan.FromSeconds(10));
					}
				} catch {
					throw;
				}
			}
		}
		private static void Pause() {
			Console.WriteLine($"Press Enter...");
			Console.ReadLine();
		}

	}

}
