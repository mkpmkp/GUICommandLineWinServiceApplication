using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Application1
{

	//
	public class ConsoleServer {
		public static bool ConsoleServerFlag = true;
		private static TcpListener Listener;
		public static int ConsolePort;

		public static Dictionary<int, Connection> Connections = new Dictionary<int, Connection>() { };
		private static int ConnectionsCounter = 0;

		public static void Run(int Port) {
			ConsolePort = Port;
			Listener = new TcpListener(IPAddress.Any, Port);
			Listener.Start();
			try {
				while (ConsoleServerFlag) {
					ThreadPool.QueueUserWorkItem(new WaitCallback(ConsoleClientThread), Listener.AcceptTcpClient());
				}
				Listener.Stop();
			} catch { }
		}
		static void ConsoleClientThread(Object StateInfo) {
			var NewID = ++ConnectionsCounter;
			var conn = new Connection((TcpClient)StateInfo, NewID);
			Connections[NewID] = conn;
			conn.Work();
			CloseConnection(NewID);
			conn = null;
		}
		public static void CloseConnection(int ConnID, Connection Conn = null) {
			if (Connections.ContainsKey(ConnID) && Connections[ConnID] != null) {
				Connections[ConnID].Stream?.Close();
				Connections[ConnID].tcpClient?.Close();
				Connections[ConnID] = null;
				Connections.Remove(ConnID);
				try {
					Conn?.Put($"Connection {UTIL.ShowVar(ConnID)} closed.");
				} catch { }
			}
		}
		public static void Stop() {
			ConsoleServerFlag = false;
			Listener.Stop();
		}
		public static void Start() {
			// Заботимся чтоб хватило активных Thread-ов
			//Process.SetThreads(Entity.MaximumManagersAmount, Entity.MaximumWorkersAmount, null, null);

			ConsolePort = Global.ConsolePort;
			try {
				Run(Port: ConsolePort);
			} catch (Exception Ex) {
				var ErrorItem = new Error(Ex);
				if (Environment.UserInteractive) {
					MessageBox.Show($"Ошибка при запуске консоли: {ErrorItem.GetMessage()}", @"Console", buttons: MessageBoxButtons.OK);
				} else {
					LOG.AsyncFileWrite(Global.LocalLogPath, $"{DateTime.Now} SessionID=[{WinService.SessionID}]. Ошибка при запуске консоли: {ErrorItem.GetMessage()}");
				}
			}
		}
	}

	//
	public class Connection {
		public int ConnectionID;
		public string ConnectionIP = null;
		public bool ConnectionLogged = false;
		public TcpClient tcpClient;
		public NetworkStream Stream;
		public int WindowWidth = 80;
		public int WindowHeight = 24;
		public string Prompt = @"> ";

		// Конструктор
		public Connection(TcpClient NewClient, int NewID) {
			ConnectionID = NewID;
			tcpClient = NewClient;
			Stream = tcpClient.GetStream();
			ConnectionIP = tcpClient.Client.RemoteEndPoint.ToString();
		}

		// Output data
		public void Put(byte[] Buffer) {
			try {
				Stream?.Write(Buffer, 0, Buffer.Length);
				Stream?.Flush();
			} catch { }
		}
		public void Put(byte Byte) {
			Put(new byte[] { Byte });
		}
		public void Put(string str, bool NewLine = true) {
			byte[] Buffer;
			Buffer = Encoding.UTF8.GetBytes(str + (NewLine ? Environment.NewLine : @""));
			Put(Buffer);
		}
		public void PutVar(object Var, int level = 0) {
			if (level > 20) {
				Put($"{Esc.Red}<Overdeep>{Esc.Reset}", false);
				return;
			}
			var Prefix = new StringBuilder().Insert(0, "   ", level).ToString();
			var VarType = Var?.GetType();
			try {
				if (Var == null) {
					Put($"{Esc.BrightCyan}NULL{Esc.Reset}", false);
				} else if (VarType.ToString().Contains(@"System.Xml.")) {
					Put($"{Esc.Red}<{VarType}>{Esc.Reset}", false);
				} else if (VarType == typeof(bool)) {
					var Color = (bool)Var ? Esc.BrightGreen : Esc.BrightRed;
					Put($"{Color}{(bool)Var}{Esc.Reset}", false);
				} else if (VarType == typeof(string)) {
					Put($"\"{Esc.Yellow}{Var}{Esc.Reset}\"", false);
				} else if (VarType == typeof(Guid)) {
					Put($"{{{Esc.Yellow}{Var}{Esc.Reset}}}", false);
				} else if ((new List<Type>() { typeof(int), typeof(uint), typeof(decimal), typeof(float), typeof(double), typeof(long), typeof(ulong), typeof(byte), typeof(char), typeof(SByte), typeof(Int16), typeof(short), typeof(ushort) }).Contains(VarType)) {
					Put($"{Esc.BrightBlue}{Var}{Esc.Reset}", false);
				} else if (VarType.IsEnum) {
					Put($"{Esc.BrightBlue}{VarType}.{Var} [{(int)Var}]{Esc.Reset}", false);
				} else if (VarType == typeof(DateTime)) {
					Put($"{Esc.BrightPurple}{Var:dd\\.MM\\.yyyy HH\\:mm\\:ss}{Esc.Reset}", false);
				} else if(VarType == typeof(TimeSpan)) {
					Put($"{Esc.BrightPurple}{Var:d\\.hh\\:mm\\:ss}{Esc.Reset}", false);
				} else if (VarType == typeof(Color)) {
					Put($"{Var} [{Esc.BackColorTranslate((Color)Var)}  {Esc.Reset}]", false);
				} else if (VarType == typeof(MatchCollection)) {
					var Items = Var as MatchCollection;
					Put($"[<MatchCollection>\r\n", false);
					foreach (var Item in Items) {
						Put($"{Prefix}   ", false); PutVar(Item, level + 1); Put("\r\n", false);
					}
					Put($"{Prefix}]", false); // \r\n
				} else if (VarType == typeof(Match)) {
					var Items = Var as Match;
					Put($"[<Match>\r\n", false);
					for (int i = 0; i < Items.Groups.Count; i++) {
						Put($"{Prefix}   [{i}] = \"{Items.Groups[i]}\"\r\n", false);
					}
					Put($"{Prefix}]\r\n", false); //\r\n
				} else if (VarType.IsArray) {
					var Items = Var as Array;
					Put($"{VarType.Name} ({Items.Length}) [\r\n", false);
					foreach (var Item in Items) {
						Put($"{Prefix}   ", false); PutVar(Item, level + 1); Put("\r\n", false);
					}
					Put($"{Prefix}]", false); //\r\n
				} else if (VarType.IsGenericType && VarType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
					var Items = ToDictionary<object, object>(Var);
					Put($"<Dictionary>({Items.Count()}) [\r\n", false);
					foreach (var Item in Items.ToArray()) {
						Put($"{Prefix}   [", false); PutVar(Item.Key, level + 1); Put("] = ", false); PutVar(Item.Value, level + 1); Put("\r\n", false);
					}
					Put($"{Prefix}]", false); //\r\n
				} else if (VarType == typeof(StringDictionary)) {
					var Items = Var as StringDictionary;
					Put($"<DictionaryEntry>({Items.Count}) [\r\n", false);
					foreach (DictionaryEntry de in Items) {
						Put($"{Prefix}   [", false); PutVar(de.Key, level + 1); Put("] = ", false); PutVar(de.Value, level + 1); Put("\r\n", false);
					}
					Put($"{Prefix}]", false); //\r\n
				} else if (VarType == typeof(CircleTextBuffer.TextItem)) {
					var Item = Var as CircleTextBuffer.TextItem;
					Put($"<CircleTextBuffer.TextItem> [\r\n", false);
					Put($"{Prefix}   Counter: ", false); PutVar(Item.Counter, level + 1); Put("; Text: ", false); PutVar(Item.Text, level + 1); Put("\r\n", false);
					Put($"{Prefix}   ForeColor: ", false); PutVar(Item.ForeColor, level + 1); Put("; BackColor: ", false); PutVar(Item.BackColor, level + 1); Put("\r\n", false);
					Put($"{Prefix}]"); //\r\n
				} else if (VarType.IsClass) {
					var PropertyList = GetAllProperties(VarType);
					Put($"<{VarType}> {{\r\n", false);
					foreach (PropertyInfo p in PropertyList) {
						Put($"{Prefix}   {p.Name}: ", false); PutVar(p.GetValue(Var), level + 1); Put("\r\n", false);
					}
					Put($"{Prefix}}}", false); //\r\n
				} else {
					Put($"({VarType}) = {Esc.BrightYellow}{Var.ToString()}{Esc.Reset}");
				}
			} catch (Exception Ex) {
				Put($"\r\n{Esc.Red}PutVar Error: {Ex.Message}{Esc.Reset}. [{VarType}]");
			}
			if (level == 0) {
				Put("\r\n\r\n");
			}
		}
		static IEnumerable<PropertyInfo> GetAllProperties(Type T) {
			IEnumerable<PropertyInfo> PropertyList = T.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			if (T.GetTypeInfo().BaseType != null) {
				PropertyList = PropertyList.Concat(GetAllProperties(T.GetTypeInfo().BaseType));
			}
			return PropertyList;
		}
		static public IDictionary<T, V> ToDictionary<T, V>(Object objAttached) {
			var dicCurrent = new Dictionary<T, V>();
			foreach (DictionaryEntry dicData in (objAttached as IDictionary)) {
				dicCurrent.Add((T)dicData.Key, (V)dicData.Value);
			}
			return dicCurrent;
		}
		// Input data
		public string ReadBuffer() {
			byte[] myReadBuffer = new byte[1024];
			var myCompleteMessage = new StringBuilder();
			int numberOfBytesRead = 0;
			if (Stream.CanRead) {
				if (Stream.DataAvailable) {
					do {
						numberOfBytesRead = Stream.Read(myReadBuffer, 0, myReadBuffer.Length);
						myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
					} while (Stream.DataAvailable);
				}
			}
			return myCompleteMessage.ToString();
		}
		public string ReadLine() {
			Put(Prompt, NewLine: false);
			Stream.Flush();
			byte[] Buffer = new byte[1024];
			int Count;
			string Request = "";
			if (Stream.CanRead) {
				while ((Count = Stream.Read(Buffer, 0, Buffer.Length)) > 0) {
					Request += Encoding.UTF8.GetString(Buffer, 0, Count);
					if (Request.IndexOf("\r") >= 0 || Request.IndexOf("\n") >= 0 || Request.Length > 4096) {
						break;
					}
				}
			}
			return Request;
		}
		private List<string> UnloggedCommands = new List<string> {
			@"", @"0", @"1", @"2", @"3", @"3", @"4", @"5", @"6",
			@"help", @"man", @"last", @"tail", @"ping", @"old", @"exit", @"return",
			@"connections", @"clr", @"clear", @"cancel", @"cancelstep", @"close", @"quit"
		};

		// Точка входа
		public void Work() {
			try {
				// ECHO ON
				Put(new byte[] { Esc.IAC, Esc.Will, Esc.Echo });

				// Client title
				Put($"{Esc.ClientTitle}{Environment.MachineName}:{ConsoleServer.ConsolePort}\a", NewLine: false);

				// For fun only
				//LoadingAnimation();

				byte[] Buffer = new byte[1024];
				string Request = "";
				// Вычитываем мусор
				Stream.Read(Buffer, 0, Buffer.Length);
				// Ждем пока пароль не совпадет
				while (true) {
					if (!(tcpClient?.Connected ?? false)) {
						return;
					}

					try {
						//int Count;
						Prompt = $"Password: ";
						Request = ReadLine();
					} catch {
						tcpClient?.Close();
						return;
					}
					string password = Request.Trim();
					if (password == Global.ConsolePassword) {
						byte[] buffer = Encoding.UTF8.GetBytes(Esc.ClearScreen);
						Stream.Write(buffer, 0, buffer.Length);
						Stream.Flush();
						break;
					}
					Put("\a", NewLine: true);
				}
				// ECHO OFF
				Put(new byte[] { Esc.IAC, Esc.Wont, Esc.Echo });

				Put($"{Esc.ClearScreen}");
				Put(GetHelloMessage());

				ConnectionLogged = true;

				// Обрабатываем комманды
				while (true) {
					if (!(tcpClient?.Connected ?? false)) {
						return;
					}

					try {
						Prompt = $"[{Environment.MachineName}:{ConsoleServer.ConsolePort}]> ";
						Request = ReadLine();
					} catch {
						tcpClient.Close();
						return;
					}

					string protocommand = Request.Trim();

					// Выделяю 3 куска - 2 лексемы и блок данных "cccccccc sssssss dddddddd"
					// Первая лексема - комманда. Вторая подкомманда. Данные нужные в контексте комманды + подкомманды
					Regex regex = new Regex(@"(\w+)(\s+(\w+)(\s+(.+))?)?");
					Match match = regex.Match(protocommand);
					string command = match.Groups[1].Value;
					string subcommand = match.Groups[3].Value;
					string data = match.Groups[5].Value;

					try {
						if (!string.IsNullOrWhiteSpace(command) && !UnloggedCommands.Contains(command.ToLower())) {
							LOG.AsyncFileWrite(Global.LocalCommandLogPath, $"{command} {subcommand} {data}");
						}
					} catch { }

					if (command.Length > 0) {
						switch (command.ToLower()) {
							case @"man":
							case @"help":
								Put(Man(subcommand, data, @"work"));
								break;
							case @"state":
								Put(GetState());
								break;
							case @"tail":
								Tail(subcommand, data);
								break;
							case @"exit":
							case @"return":
								return;
							case @"close":
							case @"quit":
								ConsoleServer.CloseConnection(ConnectionID);
								break;
							case @"date":
								Put($"{Esc.Purple}Current date: {DateTime.Now:dd.MM.yyyy}{Esc.Reset}");
								break;
							case @"time":
								Put($"{Esc.Purple}Current time: {DateTime.Now:HH:mm:ss}{Esc.Reset}");
								break;
							case @"clr":
							case @"clear":
								Put($"{Esc.Reset}{ Esc.ClearScreen}", false);
								Stream.Flush();
								break;
							case @"message":
								PutMessage(subcommand, data);
								break;
							case @"hello":
								Put(GetHelloMessage());
								break;
							case @"last":
							case @"old":
								try {
									if (subcommand == @"grep" && !string.IsNullOrWhiteSpace(data)) {
										foreach (string line in File.ReadLines(Global.LocalCommandLogPath)) {
											if (line.ToLower().Contains(data.ToLower())) {
												Put(line);
											}
										}
									} else {
										foreach (string line in File.ReadLines(Global.LocalCommandLogPath)) {
											Put(line);
										}
									}
								} catch (Exception Ex) {
									Put($"{Esc.ER}Ex: {Ex.Message}{Esc.Reset}");
								}
								break;
							case @"profile":
							case @"currentprofile":
								Put($"Current profile: {Esc.Green}{Global.Configuration.CurrentProfile()}{Esc.Reset}:");
								Put(UTIL.ShowVar(Global.Configuration.Current.GetAllParameters()));
								break;
							case @"set":
								subcommand = subcommand.ToLower();
								switch (subcommand) {
									case @"":
									case @"help":
										Put(Man(@"set"));
										break;
									case @"loglevel":
										int LogLevel = 0;
										if (string.IsNullOrWhiteSpace(data) || data == "?") {
											Put($"{Esc.H2}Usage:{Esc.Reset} set loglevel <0-9>\r\n{Esc.H2}Current LogLevel:{Esc.Reset} {UTIL.ShowVar(Global.LogLevel)}\r\n");
										} else if (int.TryParse(data, out LogLevel)) {
											if (LogLevel >= 0 && LogLevel <= 9) {
												Global.LogLevel = LogLevel;
												Put($"{Esc.Green}LogLevel set to {LogLevel}{Esc.Reset}");
												LOG.AppendLog(null, $"LogLevel set to {LogLevel}", Color.Magenta);
											} else {
												Put($"{Esc.ER}LogLevel must be in diapasone 0-9{Esc.Reset}");
											}
										} else {
											Put($"{Esc.ER}Unknown parameter '{data}'{Esc.Reset}");
										}
										break;
									case @"profile":
									case @"currentprofile":
										try {
											Global.Configuration.CurrentProfile(data.Trim());
											Put($"Profile set to {Esc.Purple}'{Global.Configuration.CurrentProfile()}'{Esc.Reset}");
											Put(UTIL.ShowVar(Global.Configuration.Current.GetAllParameters()));
											Global.ApplyConfiguration();
											LOG.AppendLog(null, $"Profile set to '{Global.Configuration.CurrentProfile()}'", Color.Magenta);
										} catch (Exception Ex) {
											Put($"{Esc.ER}Error: {Ex.Message}{Esc.Reset}");
										}
										break;
									default:
										Put($"{Esc.ER}Unknown parameter '{subcommand}'{Esc.Reset}");
										break;
								}
								break;
							case @"connections":
								subcommand = subcommand.ToLower();
								switch (subcommand) {
									case @"close":
										if (data.Trim().ToLower() == @"all") {
											foreach (var Item in ConsoleServer.Connections.ToArray()) {
												if (Item.Key != ConnectionID) {
													ConsoleServer.CloseConnection(Item.Key, this);
												}
											}
										} else if (int.TryParse(data, out int ConnID)) {
											ConsoleServer.CloseConnection(ConnID, this);
										}
										break;
									case @"":
									default:
										Put($"Current connection marked by {Esc.Mark}[!]{Esc.Reset}");
										Put(@"".PadLeft(60, '-'));
										foreach (var Item in ConsoleServer.Connections.ToArray()) {
											var Current = Item.Key == ConnectionID ? $" {Esc.Mark}[!]{Esc.Reset}" : @"";
											Put($"{UTIL.ShowVar(Item.Key)} = {UTIL.ShowVar(Item.Value?.ConnectionIP)}. Connected: {UTIL.ShowVar(Item.Value?.tcpClient?.Connected ?? false)}. Logged: {UTIL.ShowVar(Item.Value?.ConnectionLogged ?? false)}.{Current}");
										}
										Put(@"".PadLeft(60, '-'));
										break;
								}
								break;
							default:
								Put($"{Esc.ER}Unknown command '{command}'{Esc.Reset}");
								break;
						}
					}
				}
			} catch (Exception Ex) {
				var ErrorItem = new Error(Ex);
				Stream?.Close();
				tcpClient?.Close();
				return;
			}
		}

		// Tail
		public void Tail(string Subcommand = null, string Data = "") {

			Put($"{Esc.Mark}Tail started{Esc.Reset}");

			if (Subcommand == @"clearbuffer" || Subcommand == @"clear" || Subcommand == @"reset") {
				LOG.ConsoleTextBuffer.Clear();
				return;
			}
			if (Subcommand == @"clearbuffer2" || Subcommand == @"clear2" || Subcommand == @"reset2") {
				LOG.ClearCircleTextBuffer();
				return;
			}
			if (Subcommand == @"test") {
				Put(UTIL.ShowVar(LOG.ConsoleTextBuffer.Test(Data)));
				return;
			}
			long LastCounter = -1;
			if (Subcommand == @"continue") {
				LastCounter = LOG.ConsoleTextBuffer.CurrentCounter;
			}
			byte[] Buffer = new byte[1024];
			try {
				while (true) {
					if (!(tcpClient?.Connected ?? false)) {
						return;
					}

					//LOG.AppendLog(null, $"Tail: {LastCounter}");

					var list = LOG.ConsoleTextBuffer.Get(LastCounter);
					if (list != null) {
						var RegExp = new Regex(Data);
						foreach (var i in list) {
							var str = i.Text;
							var mark = @"";
							switch (Subcommand) {
								case @"grep":
									if (!str.Contains(Data)) str = null;
									break;
								case @"mark":
									if (str.Contains(Data)) mark = $"{Esc.Mark}[!]{Esc.Reset} ";
									break;
								case @"reg":
								case @"regex":
								case @"regexp":
									if (!RegExp.IsMatch(str)) str = null;
									break;
								default:
									break;
							}
							if (str != null) {
								Put($"{mark}{Esc.ForeColorTranslate(i.ForeColor)}{Esc.BackColorTranslate(i.BackColor)}{i.Text}{Esc.Reset}");
							}
							LastCounter = i.Counter;
						}
					}
					if (Stream.CanRead) {
						if (Stream.DataAvailable) {
							ReadBuffer();
							Stream.Flush();
							return;
						}
					} else {
						Put($"{Esc.ER}### CAN'T READ DATA{Esc.Reset}");
						return;
					}
					Thread.Sleep(500);
					if (LastCounter > LOG.ConsoleTextBuffer.CurrentCounter) {
						LastCounter = -1;
					}
				}
			} catch { }
		}

		// Message
		public void PutMessage(string subcommand, string data) {
			var IP = ((IPEndPoint)tcpClient?.Client.RemoteEndPoint).Address.ToString();
			var Header = $" [ Message from {IP} ] ".PadCenter(60, '\\');
			var Footer = $"".PadCenter(60, '/');
			switch (subcommand) {
				case "text":
					if (string.IsNullOrWhiteSpace(data)) {
						Put($"{Esc.ER}Empty message text{Esc.Reset}\r\n");
					} else {
						LOG.ConsoleTextBuffer.Add($"{Header}\r\n\r\n{data}\r\n\r\n{Footer}", Color.LightPink);
						Put($"{Esc.Green}Message text was sent{Esc.Reset}");
					}
					break;
				case "logo":
					LOG.ConsoleTextBuffer.Add($"{Header}\r\n\r\n{Man(@"logo")}\r\n\r\n{Footer}");
					Put($"{Esc.Green}Message logo was sent{Esc.Reset}");
					break;
				default:
					Put($"{Esc.Red}Error!{Esc.Reset}");
					Put(Man(@"message"));
					break;
			}
		}

		//
		public static string GetHelloMessage() {
			return
				$"{Esc.ClearScreen}" +
				Man(@"logo") +
				$"\r\n{Esc.H0}Добро пожаловать в консоль системы Application1{Esc.Reset}\r\n";
		}

		//
		public static string GetState() {
			return UTIL.ShowVar(new Dictionary<string, object>() {
				["Режим работы клиента"] = Global.StartedAs,
				["ApplicationStartTime"] = Global.ApplicationStartTime,
				["Current system time"] = DateTime.Now,
				["Current system user"] = Environment.UserName,
				["LogLevel"] = Global.LogLevel,
				["ConsoleHost"] = Environment.MachineName,
				["ConsolePort"] = ConsoleServer.ConsolePort,
				["ConfigProfile"] = Global.Configuration.CurrentProfile(),
				["DB_Name"] = Global.DBName,
				["DB_User"] = Global.DBUser,
				["SenderID"] = Global.SenderId,
				["BuildDate"] = Global.BuildDate,
			});
		}

		// Manual (Help)
		public static string Man(string subcommand, string data = null, string Mode = @"work") {
			var context = subcommand?.ToLower();
			var ManFileName = string.IsNullOrEmpty(context) ? Mode : context;
			var CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
			var ManFilePath = Path.Combine(new[] { CurrentPath, @"man", $"{ManFileName}.man" });
			if (File.Exists(ManFilePath)) {
				try {
					var ManContent = File.ReadAllText(ManFilePath);
					if (data != @"raw") {
						// Если не заявлен "сырой" вывод заменяем макроподстановки разметкой и данными
						// ESCAPE-последовательности
						ManContent = ManContent.Replace(@"{Esc.Reset}", $"{Esc.Reset}");
						ManContent = ManContent.Replace(@"{Esc.ER}", $"{Esc.ER}");
						ManContent = ManContent.Replace(@"{Esc.LC}", $"{Esc.LC}");
						ManContent = ManContent.Replace(@"{Esc.RapidBlink}", $"{Esc.RapidBlink}");
						ManContent = ManContent.Replace(@"{Esc.H0}", $"{Esc.H0}");
						ManContent = ManContent.Replace(@"{Esc.H1}", $"{Esc.H1}");
						ManContent = ManContent.Replace(@"{Esc.H2}", $"{Esc.H2}");
						ManContent = ManContent.Replace(@"{Esc.H3}", $"{Esc.H3}");
						ManContent = ManContent.Replace(@"{Esc.Mark}", $"{Esc.Mark}");
						ManContent = ManContent.Replace(@"{Esc.TD}", $"{Esc.TD}");
						ManContent = ManContent.Replace(@"{Esc.TH}", $"{Esc.TH}");
						ManContent = ManContent.Replace(@"{Esc.Bold}", $"{Esc.Bold}");
						ManContent = ManContent.Replace(@"{Esc.Faint}", $"{Esc.Faint}");
						ManContent = ManContent.Replace(@"{Esc.Italic}", $"{Esc.Italic}");
						ManContent = ManContent.Replace(@"{Esc.Underline}", $"{Esc.Underline}");
						ManContent = ManContent.Replace(@"{Esc.Blink}", $"{Esc.Blink}");
						ManContent = ManContent.Replace(@"{Esc.RapidBlink}", $"{Esc.RapidBlink}");
						ManContent = ManContent.Replace(@"{Esc.BGBlack}", $"{Esc.BGBlack}");
						ManContent = ManContent.Replace(@"{Esc.BGRed}", $"{Esc.BGRed}");
						ManContent = ManContent.Replace(@"{Esc.BGGreen}", $"{Esc.BGGreen}");
						ManContent = ManContent.Replace(@"{Esc.BGYellow}", $"{Esc.BGYellow}");
						ManContent = ManContent.Replace(@"{Esc.BGBlue}", $"{Esc.BGBlue}");
						ManContent = ManContent.Replace(@"{Esc.BGPurple}", $"{Esc.BGPurple}");
						ManContent = ManContent.Replace(@"{Esc.BGCyan}", $"{Esc.BGCyan}");
						ManContent = ManContent.Replace(@"{Esc.BGGray}", $"{Esc.BGGray}");
						ManContent = ManContent.Replace(@"{Esc.BGDarkGray}", $"{Esc.BGDarkGray}");
						ManContent = ManContent.Replace(@"{Esc.BGBrightRed}", $"{Esc.BGBrightRed}");
						ManContent = ManContent.Replace(@"{Esc.BGBrightGreen}", $"{Esc.BGBrightGreen}");
						ManContent = ManContent.Replace(@"{Esc.BGBrightYellow}", $"{Esc.BGBrightYellow}");
						ManContent = ManContent.Replace(@"{Esc.BGBrightBlue}", $"{Esc.BGBrightBlue}");
						ManContent = ManContent.Replace(@"{Esc.BGBrightPurple}", $"{Esc.BGBrightPurple}");
						ManContent = ManContent.Replace(@"{Esc.BGBrightCyan}", $"{Esc.BGBrightCyan}");
						ManContent = ManContent.Replace(@"{Esc.BGWhite}", $"{Esc.BGWhite}");
						ManContent = ManContent.Replace(@"{Esc.Black}", $"{Esc.Black}");
						ManContent = ManContent.Replace(@"{Esc.Red}", $"{Esc.Red}");
						ManContent = ManContent.Replace(@"{Esc.Green}", $"{Esc.Green}");
						ManContent = ManContent.Replace(@"{Esc.Yellow}", $"{Esc.Yellow}");
						ManContent = ManContent.Replace(@"{Esc.Blue}", $"{Esc.Blue}");
						ManContent = ManContent.Replace(@"{Esc.Purple}", $"{Esc.Purple}");
						ManContent = ManContent.Replace(@"{Esc.Cyan}", $"{Esc.Cyan}");
						ManContent = ManContent.Replace(@"{Esc.Gray}", $"{Esc.Gray}");
						ManContent = ManContent.Replace(@"{Esc.DarkGray}", $"{Esc.DarkGray}");
						ManContent = ManContent.Replace(@"{Esc.BrightRed}", $"{Esc.BrightRed}");
						ManContent = ManContent.Replace(@"{Esc.BrightGreen}", $"{Esc.BrightGreen}");
						ManContent = ManContent.Replace(@"{Esc.BrightYellow}", $"{Esc.BrightYellow}");
						ManContent = ManContent.Replace(@"{Esc.BrightBlue}", $"{Esc.BrightBlue}");
						ManContent = ManContent.Replace(@"{Esc.BrightPurple}", $"{Esc.BrightPurple}");
						ManContent = ManContent.Replace(@"{Esc.BrightCyan}", $"{Esc.BrightCyan}");
						ManContent = ManContent.Replace(@"{Esc.White}", $"{Esc.White}");
						// Данные
						ManContent = ManContent.Replace(@"{Data.State}", GetState());
						ManContent = ManContent.Replace(@"{Data.ManTopics}", GetManTopics());
						ManContent = ManContent.Replace(@"{Data.CurrentPeriod}", $"{DateTime.Now:yyyyMM}");
						ManContent = ManContent.Replace(@"{Data.PreviousPeriod}", $"{DateTime.Now.AddMonths(-1):yyyyMM}");
					}
					return ManContent.TrimEnd(new char[] { ' ', (char)10, (char)13, (char)9 }) + "\r\n";
				} catch (Exception Ex) {
					return $"Ex: {Ex.Message}";
				}
			} else {
				return $"{Esc.ER}Нет мануала для ключевого слова '{subcommand}' (Нет файла '{ManFilePath}'){Esc.Reset}\r\nДоступные топики: {GetManTopics()}";
			}
		}

		//
		public static string GetManTopics()
		{
			var CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
			var ManFilePath = Path.Combine(new[] { CurrentPath, @"man" });
			var FilesData = UTIL.GetFiles(ManFilePath, @"*.man");
			if (FilesData.IsError()) {
				var ErrorItem = FilesData as Error;
				return $"Er: {Esc.ER}{ErrorItem.GetMessage()}{Esc.Reset}";
			}
			var FilesDataArray = FilesData as string[];
			for (var i = 0; i < FilesDataArray.Length; i++) {
				FilesDataArray[i] = $"{Esc.BrightCyan}{Path.GetFileNameWithoutExtension(FilesDataArray[i])}{Esc.Reset}";
			}
			return string.Join(@" ", FilesDataArray);
		}

	}

}
