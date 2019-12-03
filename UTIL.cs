using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Application1 {

	// Класс, содержащий общие утилитарные функции
	public static class UTIL {
		// Получение списка файлов по указанному пути
		public static object GetFiles(string Path, string Mask = @"*.*", SearchOption SearchOptions = SearchOption.AllDirectories) {
			try {
				var Result = new string[0];
				if (!Directory.Exists(Path)) return Result; //new Error("#FIL000001", string.Format(@"Нет указанного каталога '{0}'", Path ?? @"n/a"));
				var Files = Directory.GetFiles(Path, Mask, SearchOptions);
				foreach (string File in Files) {
					FileAttributes FileAttributes = System.IO.File.GetAttributes(File);
					if ((FileAttributes & FileAttributes.Hidden) != FileAttributes.Hidden) {
						Array.Resize(ref Result, Result.Length + 1);
						Result[Result.Length - 1] = File;
					}
				}
				// if (Result.Length < 1) return new Error("#FIL000002", $"В указанном каталоге '{Path ?? @"n/a"}' нет файлов");
				return Result;
			} catch (Exception Ex) {
				return new Error(Ex);
			}
		}
		// Расширение проверяет что указанный объект типа "Ошибка"
		// Usage: TestedObject.isError()
		public static bool IsError(this object TestedObject) => TestedObject != null && TestedObject.GetType() == typeof(Error);
		// Расширение проверяет что указанный объект типа "Успех"
		// Usage: TestedObject.isSuccess()
		public static bool IsSuccess(this object TestedObject) => TestedObject != null && TestedObject.GetType() == typeof(Success);
		public static string PadCenter(this string Str, int Width = 80, char PaddingChar = ' ') {
			if (Str == null) return Str;
			if (Str.Length >= Width) return Str;
			return Str.PadLeft((int)Math.Ceiling(((decimal)Width + (decimal)Str.Length) / 2), PaddingChar).PadRight(Width, PaddingChar);
		}
		public static string Repeat(this char chatToRepeat, int repeat) {
			return new string(chatToRepeat, repeat);
		}
		public static string Repeat(this string stringToRepeat, int repeat) {
			var builder = new StringBuilder(repeat * stringToRepeat.Length);
			for (int i = 0; i < repeat; i++) {
				builder.Append(stringToRepeat);
			}
			return builder.ToString();
		}
		public static int? ToInt(this string Value, int? Replacement = null) {
			if (int.TryParse(Value, out int Result)) {
				return Result;
			}
			return Replacement;
		}
		public static short? ToShort(this string Value, short? Replacement = null) {
			if (short.TryParse(Value, out short Result)) {
				return Result;
			}
			return Replacement;
		}
		public static long? ToLong(this string Value, long? Replacement = null) {
			if (long.TryParse(Value, out long Result)) {
				return Result;
			}
			return Replacement;
		}
		public static DateTime? ToDateTime(this string Value, DateTime? Replacement = null) {
			if (DateTime.TryParse(Value, out DateTime Result)) {
				return Result;
			} else {
				return Replacement;
			}
		}
		public static bool ToBool(this int Value) {
			if (Value == 1) return true;
			return false;
		}
		public static bool ToBool(this string Value) {
			if (new List<string> { @"1", @"true", @"да", @"yes" }.Contains(Value?.ToLower())) return true;
			return false;
		}

		// Универсальная процедура печати/вывода сложных объектов
		public static string ShowVar(object Var, int level = 0, bool ForConsole = true) {
			if (level > 10) return $"{Esc.Red}<Overdeep>{Esc.Reset}";
			var Ret = @"";
			var Prefix = new StringBuilder().Insert(0, "   ", level).ToString();
			var VarType = Var?.GetType();
			try {
				if (Var == null) {
					if (ForConsole) {
						Ret += $"{Esc.BrightCyan}NULL{Esc.Reset}";
					} else {
						Ret += @"NULL";
					}
				} else if (VarType == typeof(bool)) {
					if (ForConsole) {
						var Color = (bool)Var ? Esc.BrightGreen : Esc.BrightRed;
						Ret += $"{Color}{(bool)Var}{Esc.Reset}";
					} else {
						Ret += $"{((bool)Var)}";
					}
				} else if (VarType == typeof(string)) {
					if (ForConsole) {
						Ret += $"\"{Esc.Yellow}{Var}{Esc.Reset}\"";
					} else {
						Ret += $"\"{Var}\"";
					}
				} else if (VarType == typeof(Guid)) {
					var Item = (Guid)Var;
					if (ForConsole) {
						Ret += $"{{{Esc.Yellow}{Var}{Esc.Reset}}}";
					} else {
						Ret += $"\"{Var}\"";
					}
				} else if ((new List<Type>() { typeof(int), typeof(uint), typeof(decimal), typeof(double), typeof(float), typeof(long), typeof(byte), typeof(char), typeof(ulong), typeof(SByte), typeof(Int16), typeof(short), typeof(ushort) }).Contains(VarType)) {
					if (ForConsole) {
						Ret += $"{Esc.BrightBlue}{Var}{Esc.Reset}";
					} else {
						Ret += $"{Var}";
					}
				} else if (VarType.IsEnum) {
					if (ForConsole) {
						Ret += $"{Esc.BrightBlue}{VarType}.{Var} [{(int)Var}]{Esc.Reset}";
					} else {
						Ret += $"{VarType}.{Var} [{(int)Var}]";
					}
				} else if (VarType == typeof(DateTime)) {
					if (ForConsole) {
						Ret += $"{Esc.BrightPurple}{Var:dd\\.MM\\.yyyy HH\\:mm\\:ss}{Esc.Reset}";
					} else {
						Ret += $"{Var:dd\\.MM\\.yyyy HH\\:mm\\:ss}";
					}
				} else if(VarType == typeof(TimeSpan)) {
					if (ForConsole) {
						Ret += $"{Esc.BrightPurple}{Var:d\\.hh\\:mm\\:ss}{Esc.Reset}";
					} else {
						Ret += $"{Var:d\\.hh\\:mm\\:ss}";
					}
				} else if (VarType == typeof(Color)) {
					if (ForConsole) {
						Ret += $"{Var} [{Esc.BackColorTranslate((Color)Var)}  {Esc.Reset}]";
					} else {
						Ret += $"{Var}";
					}
				} else if (VarType == typeof(MatchCollection)) {
					var Items = Var as MatchCollection;
					Ret += $"[<MatchCollection>\r\n";
					foreach (var Item in Items) {
						Ret += $"{Prefix}   {ShowVar(Item, level + 1, ForConsole)}\r\n";
					}
					Ret += $"{Prefix}]\r\n";
				} else if (VarType == typeof(Match)) {
					var Items = Var as Match;
					Ret += $"[<Match>\r\n";
					for (int i = 0; i < Items.Groups.Count; i++) {
						Ret += $"{Prefix}   [{i}] = \"{Items.Groups[i]}\"\r\n";
					}
					Ret += $"{Prefix}]\r\n";
				} else if (VarType.IsArray) {
					var Items = Var as Array;
					Ret += $"{VarType.Name} ({Items.Length}) [\r\n";
					foreach (var Item in Items) {
						Ret += $"{Prefix}   {ShowVar(Item, level + 1, ForConsole)}\r\n";
					}
					Ret += $"{Prefix}]\r\n";
				} else if (VarType.IsGenericType && VarType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
					// ###################################################### Dictionary
					//Type keyType = VarType.GetGenericArguments()[0];
					//Type valueType = VarType.GetGenericArguments()[1];
					var Items = ToDictionary<object, object>(Var);
					Ret += $"<Dictionary>({Items.Count()}) [\r\n";
					foreach (var Item in Items.ToArray()) {
						Ret += $"{Prefix}   [{ShowVar(Item.Key, level + 1, ForConsole)}] = {ShowVar(Item.Value, level + 1, ForConsole)}\r\n";
					}
					Ret += $"{Prefix}]\r\n";
				} else if (VarType == typeof(StringDictionary)) {
					var Items = Var as StringDictionary;
					Ret += $"<DictionaryEntry>({Items.Count}) [\r\n";
					foreach (DictionaryEntry de in Items) {
						Ret += $"{Prefix}   [{ShowVar(de.Key, level + 1, ForConsole)}] = {ShowVar(de.Value, level + 1, ForConsole)}\r\n";
					}
					Ret += $"{Prefix}]\r\n";
				} else if (VarType == typeof(CircleTextBuffer.TextItem)) {
					var Item = Var as CircleTextBuffer.TextItem;
					Ret += $"<CircleTextBuffer.TextItem> [\r\n";
					Ret += $"{Prefix}   Counter: {UTIL.ShowVar(Item.Counter, level + 1, ForConsole)}; Text: {UTIL.ShowVar(Item.Text, level + 1, ForConsole)}\r\n";
					Ret += $"{Prefix}   ForeColor: {UTIL.ShowVar(Item.ForeColor, level + 1, ForConsole)}; BackColor: {UTIL.ShowVar(Item.BackColor, level + 1, ForConsole)}\r\n";
					Ret += $"{Prefix}]\r\n";
				} else if (VarType.IsClass) {
					// ###################################################### CLASS
					var PropertyList = GetAllProperties(VarType);
					Ret += $"<{VarType}> {{\r\n";
					foreach (PropertyInfo p in PropertyList) {
						Ret += $"{Prefix}   {p.Name}: {UTIL.ShowVar(p.GetValue(Var), level + 1, ForConsole)}\r\n";
					}
					Ret += $"{Prefix}}}\r\n";
				} else {
					if (ForConsole) {
						Ret += $"({VarType}) = {Esc.BrightYellow}{Var.ToString()}{Esc.Reset}";
					} else {
						Ret += $"({VarType}) = {Var.ToString()}";
					}
				}
			} catch (Exception Ex) {
				Ret += $"\r\n{Esc.Red}ShowVar Error: {Ex.Message}{Esc.Reset}";
			}
			Ret = Ret.Replace("\r\n\r\n", "\r\n");
			return Ret;
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

		// Шифрование.
		public static string Encrypt(this string plainText, string password, string salt = @"Lebed", string hashAlgorithm = @"SHA1", int passwordIterations = 2, string initialVector = @"OFRna73m*aze01xY", int keySize = 256) {
			if (string.IsNullOrEmpty(plainText)) return "";

			byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
			byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			
			//формируем ключ из пароля, ключевой привязки, хеш -алгоритма с количеством итераций
			var derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
			byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);

			//Предоставляет управляемую реализацию алгоритма симметричного расширенный стандарт шифрования (AES)
			var symmetricKey = new AesManaged {
				Mode = CipherMode.CBC
			};
			byte[] cipherTextBytes = null;

			//Создаем симметричный объект-шифратор с использованием указанного ключа и вектора инициализации (initialVector).
			using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes)) {
				//Шифруем
				using (MemoryStream memStream = new MemoryStream()) {
					using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write)) {
						cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
						cryptoStream.FlushFinalBlock();
						cipherTextBytes = memStream.ToArray();
						memStream.Close();
						cryptoStream.Close();
					}
				}
			}

			symmetricKey.Clear();
			return Convert.ToBase64String(cipherTextBytes);
		}
		// Дешифрование.
		public static string Decrypt(this string cipherText, string password, string salt = @"Lebed", string hashAlgorithm = @"SHA1", int passwordIterations = 2, string initialVector = @"OFRna73m*aze01xY", int keySize = 256) {
			if (string.IsNullOrEmpty(cipherText)) return "";

			try {
				byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
				byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
				byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

				PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
				byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);

				AesManaged symmetricKey = new AesManaged {
					Mode = CipherMode.CBC
				};

				byte[] plainTextBytes = new byte[cipherTextBytes.Length];
				int byteCount = 0;

				using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes)) {
					using (MemoryStream memStream = new MemoryStream(cipherTextBytes)) {
						using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read)) {
							byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
							memStream.Close();
							cryptoStream.Close();
						}
					}
				}

				symmetricKey.Clear();
				return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
			} catch {
				return "";
			}
		}
	}

	// Классы утилит для работы с консолью.
	public static class Esc {

		//
		public class Style {
			private string OutputData;
			public Style(string ForeColor = null, string BackColor = null, bool UnderLine = false) {
				OutputData = $"{ForeColor}{BackColor}{(UnderLine ? Application1.Esc.Underline : null)}";
			}
			public override string ToString() {
				return OutputData;
			}
		}

		/// <summary>
		/// Стиль "Ошибка"
		/// </summary>
		public static Style ER = new Style(Red);
		/// <summary>
		/// Стиль "Подсветка последней комманды в строке приглашения"
		/// </summary>
		public static Style LC = new Style(White, BGPurple);
		/// <summary>
		/// Стиль "Заголовок высшего уровня"
		/// </summary>
		public static Style H0 = new Style(Black, BGGray);
		/// <summary>
		/// Стиль "Заголовок певого уровня"
		/// </summary>
		public static Style H1 = new Style(White, UnderLine: true);
		/// <summary>
		/// Стиль "Заголовок второго уровня"
		/// </summary>
		public static Style H2 = new Style(White);
		/// <summary>
		/// Стиль "Заголовок второго уровня"
		/// </summary>
		public static Style H3 = new Style(Purple);
		/// <summary>
		/// Стиль "Заголовок колонки таблицы"
		/// </summary>
		public static Style TH = new Style(Purple, UnderLine: true);
		/// <summary>
		/// Стиль "Текст ячейки таблицы"
		/// </summary>
		public static Style TD = new Style(White);
		/// <summary>
		/// Стиль "Маркер строки"
		/// </summary>
		public static Style Mark = new Style(Black, BGBrightYellow);

		public static Style pixelWhite = new Style(White, BGWhite);
		public static Style pixelBlue = new Style(Blue, BGBlue);
		public static Style pixelGray = new Style(Gray, BGGray);
		public static Style pixelDarkGray = new Style(DarkGray, BGDarkGray);

		public static Style CommandStyle = new Style(BrightYellow);


		public const byte LF = 0x0A; // \n Переводит принтер на следующую строку печати, оставаясь на той же горизонтальной позиции.
		public const byte CR = 0x0D; // \r Перемещает принтер к левой границе текущей строки.

		public const byte Escape = 0x1B;

		public const byte SE = 0xF0; // Завершает согласование, начатое командой SB.
		public const byte Break = 0xF3; // Нажата кнопка «Break» или «Attention».
		public const byte InterruptProcess = 0xF4; // Приостанавливает, прерывает, аварийно прекращает или завершает процесс.
		public const byte EraseLine = 0xF8; // Стереть последнюю введённую строку, то есть все данные, полученные после последнего перевода строки.
		public const byte SB = 0xFA; // Начало согласования опции, требующего передачи параметров.

		public const byte Will = 0xFB; // обсуждение опции: Отправитель хочет включить эту опцию для себя.
		public const byte Wont = 0xFC; // обсуждение опции: Отправитель хочет выключить эту опцию для себя.
		public const byte Do = 0xFD; // обсуждение опции: Отправитель хочет, чтобы получатель включил эту опцию.
		public const byte Dont = 0xFE; // обсуждение опции: Отправитель хочет, чтобы получатель выключил опцию.
		public const byte IAC = 0xFF; // Байт данных 255.

		public const byte Echo = 0x01;
		public const byte SuppressGoAhead = 0x03;
		public const byte Status = 0x05;
		public const byte RemoteFlowControl = 0x21;
		public const byte LineMode = 0x22;
		public const byte NAWS = 0x1F;
		public const byte NewEnvironmentOption = 0x27;

		public const byte BackSpace = 0x08;

		public const string Reset = "\x1B[0m";
		public const string PerenosOn = "\x1B[?7h";
		public const string PerenosOff = "\x1B[?7l";
		public const string Bold = "\x1B[1m"; // Bright
		public const string Faint = "\x1B[2m";
		public const string Italic = "\x1B[3m";
		public const string Underline = "\x1B[4m";
		public const string Blink = "\x1B[5m";
		public const string RapidBlink = "\x1B[6m";
		public const string ClearScreen = "\x1B[1J\x1B[H"; // "\x1B[2J\x1B[H"
		public const string ClientTitle = "\x1B]0;";

		public const string BGBlack = "\x1B[40m";
		public const string BGRed = "\x1B[41m";
		public const string BGGreen = "\x1B[42m";
		public const string BGYellow = "\x1B[43m";
		public const string BGBlue = "\x1B[44m";
		public const string BGPurple = "\x1B[45m";
		public const string BGCyan = "\x1B[46m";
		public const string BGGray = "\x1B[47m";
		public const string BGDarkGray = "\x1B[100m"; // Светлочёрный!
		public const string BGBrightRed = "\x1B[101m";
		public const string BGBrightGreen = "\x1B[102m";
		public const string BGBrightYellow = "\x1B[103m";
		public const string BGBrightBlue = "\x1B[104m";
		public const string BGBrightPurple = "\x1B[105m";
		public const string BGBrightCyan = "\x1B[106m";
		public const string BGWhite = "\x1B[107m";

		public const string Black = "\x1B[30m";
		public const string Red = "\x1B[31m";
		public const string Green = "\x1B[32m";
		public const string Yellow = "\x1B[33m";
		public const string Blue = "\x1B[34m";
		public const string Purple = "\x1B[35m";
		public const string Cyan = "\x1B[36m";
		public const string Gray = "\x1B[37m";
		public const string DarkGray = "\x1B[90m"; // Светлочёрный!
		public const string BrightRed = "\x1B[91m";
		public const string BrightGreen = "\x1B[92m";
		public const string BrightYellow = "\x1B[93m";
		public const string BrightBlue = "\x1B[94m";
		public const string BrightPurple = "\x1B[95m";
		public const string BrightCyan = "\x1B[96m";
		public const string White = "\x1B[97m";

		//
		private static Dictionary<Color, string> ForeColorTranslateDictionary = new Dictionary<Color, string>() {
			[Color.White] = White,
			[Color.Red] = BrightRed,
			[Color.DarkRed] = Red,
			[Color.Green] = BrightGreen,
			[Color.DarkGreen] = Green,
			[Color.Blue] = BrightBlue,
			[Color.DarkBlue] = Blue,
			[Color.DarkGray] = Gray,
			[Color.Gray] = Gray,
			[Color.Firebrick] = Red,
			[Color.AliceBlue] = BrightBlue,
			[Color.DarkMagenta] = Purple,
			[Color.Magenta] = BrightPurple,
			[Color.ForestGreen] = Green,
			[Color.Fuchsia] = BrightBlue,
			[Color.Yellow] = BrightYellow,
			[Color.YellowGreen] = Yellow,
			[Color.LightYellow] = BrightYellow,
			[Color.Orange] = BrightYellow,
			[Color.Gold] = BrightYellow,
			[Color.Brown] = Yellow,
			[Color.BlueViolet] = BrightPurple,
			[Color.Cyan] = BrightCyan,
			[Color.DarkCyan] = Cyan,
			[Color.LightCyan] = BrightCyan,
			[Color.LightPink] = BrightPurple,
		};
		//
		private static Dictionary<Color, string> BackColorTranslateDictionary = new Dictionary<Color, string>() {
			[Color.White] = BGWhite,
			[Color.Red] = BGBrightRed,
			[Color.DarkRed] = BGRed,
			[Color.Green] = BGBrightGreen,
			[Color.DarkGreen] = BGGreen,
			[Color.Blue] = BGBrightBlue,
			[Color.DarkBlue] = BGBlue,
			[Color.DarkGray] = BGGray,
			[Color.Gray] = BGGray,
			[Color.LightGray] = BGGray,
			[Color.Firebrick] = BGRed,
			[Color.AliceBlue] = BGBrightBlue,
			[Color.DarkMagenta] = BGPurple,
			[Color.Magenta] = BGBrightPurple,
			[Color.ForestGreen] = BGGreen,
			[Color.Fuchsia] = BGBrightBlue,
			[Color.Yellow] = BGBrightYellow,
			[Color.YellowGreen] = BGYellow,
			[Color.LightYellow] = BGBrightYellow,
			[Color.Orange] = BGBrightYellow,
			[Color.Gold] = BGBrightYellow,
			[Color.Brown] = BGYellow,
			[Color.BlueViolet] = BGBrightPurple,
			[Color.Cyan] = BGBrightCyan,
			[Color.DarkCyan] = BGCyan,
			[Color.LightCyan] = BGBrightCyan,
			[Color.LightPink] = BGBrightPurple,
		};
		//
		public static string ForeColorTranslate(Color ForeColor) {
			return ForeColorTranslateDictionary.ContainsKey(ForeColor) ? ForeColorTranslateDictionary[ForeColor] : null;
		}
		//
		public static string BackColorTranslate(Color BackColor) {
			return BackColorTranslateDictionary.ContainsKey(BackColor) ? BackColorTranslateDictionary[BackColor] : null;
		}
	}


	// Транслитерация
	// Transliteration.Front(str);
	public enum TransliterationType {
		Gost,
		ISO
	}
	public static class Transliteration {

		// ГОСТ 16876-71
		private static Dictionary<string, string> gost = new Dictionary<string, string>() {
			["Є"] =  "EH",		["І"] = "I",		["і"] = "i",		["№"] = "#",		["є"] = "eh",
			["А"] = "A",		["Б"] = "B",		["В"] = "V",		["Г"] = "G",		["Д"] = "D",
			["Е"] = "E",		["Ё"] = "JO",		["Ж"] = "ZH",		["З"] = "Z",		["И"] = "I",
			["Й"] = "JJ",		["К"] = "K",		["Л"] = "L",		["М"] = "M",		["Н"] = "N",
			["О"] = "O",		["П"] = "P",		["Р"] = "R",		["С"] = "S",		["Т"] = "T",
			["У"] = "U",		["Ф"] = "F",		["Х"] = "KH",		["Ц"] = "C",		["Ч"] = "CH",
			["Ш"] = "SH",		["Щ"] = "SHH",		["Ъ"] = "'",		["Ы"] = "Y",		["Ь"] = "",
			["Э"] = "EH",		["Ю"] = "YU",		["Я"] = "YA",
			["а"] = "a",		["б"] = "b",		["в"] = "v",		["г"] = "g",		["д"] = "d",
			["е"] = "e",		["ё"] = "jo",		["ж"] = "zh",		["з"] = "z",		["и"] = "i",
			["й"] = "jj",		["к"] = "k",		["л"] = "l",		["м"] = "m",		["н"] = "n",
			["о"] = "o",		["п"] = "p",		["р"] = "r",		["с"] = "s",		["т"] = "t",
			["у"] = "u",		["ф"] = "f",		["х"] = "kh",		["ц"] = "c",		["ч"] = "ch",
			["ш"] = "sh",		["щ"] = "shh",		["ъ"] = "",			["ы"] = "y",		["ь"] = "",
			["э"] = "eh",		["ю"] = "yu",		["я"] = "ya",
			["«"] = "",			["»"] = "",			["—"] = "-",		[" "] = "-",
		};
		// ISO 9-95
		private static Dictionary<string, string> iso = new Dictionary<string, string>() {
			["Є"] = "YE",		["І"] = "I",		["Ѓ"] = "G",		["і"] = "i",		["№"] = "#",
			["є"] = "ye",		["ѓ"] = "g",
			["А"] = "A",		["Б"] = "B",		["В"] = "V",		["Г"] = "G",		["Д"] = "D",
			["Е"] = "E",		["Ё"] = "YO",		["Ж"] = "ZH",		["З"] = "Z",		["И"] = "I",
			["Й"] = "J",		["К"] = "K",		["Л"] = "L",		["М"] = "M",		["Н"] = "N",
			["О"] = "O",		["П"] = "P",		["Р"] = "R",		["С"] = "S",		["Т"] = "T",
			["У"] = "U",		["Ф"] = "F",		["Х"] = "X",		["Ц"] = "C",		["Ч"] = "CH",
			["Ш"] = "SH",		["Щ"] = "SHH",		["Ъ"] = "'",		["Ы"] = "Y",		["Ь"] = "",
			["Э"] = "E",		["Ю"] = "YU",		["Я"] = "YA",
			["а"] = "a",		["б"] = "b",		["в"] = "v",		["г"] = "g",		["д"] = "d",
			["е"] = "e",		["ё"] = "yo",		["ж"] = "zh",		["з"] = "z",		["и"] = "i",
			["й"] = "j",		["к"] = "k",		["л"] = "l",		["м"] = "m",		["н"] = "n",
			["о"] = "o",		["п"] = "p",		["р"] = "r",		["с"] = "s",		["т"] = "t",
			["у"] = "u",		["ф"] = "f",		["х"] = "x",		["ц"] = "c",		["ч"] = "ch",
			["ш"] = "sh",		["щ"] = "shh",		["ъ"] = "",			["ы"] = "y",		["ь"] = "",
			["э"] = "e",		["ю"] = "yu",		["я"] = "ya",
			["«"] = "",			["»"] = "",			["—"] = "-",		[" "] = "-",
		};
		public static string Translit(this string text, TransliterationType type = TransliterationType.ISO) {
			return Front(text, type);
		}
		public static string Front(string text) {
			return Front(text, TransliterationType.ISO);
		}
		public static string Front(string text, TransliterationType type) {
			string output = text;
			//output = Regex.Replace(output, @"\s|\.|\(", " ");
			output = Regex.Replace(output, @"\s+", " ");
			output = Regex.Replace(output, @"[^\s\w\d-—\.\[\]\&\%\$\@\!]", "");
			output = output.Trim();
			Dictionary<string, string> tdict = GetDictionaryByType(type);
			foreach (KeyValuePair<string, string> key in tdict) {
				output = output.Replace(key.Key, key.Value);
			}
			return output;
		}
		public static string Back(string text) {
			return Back(text, TransliterationType.ISO);
		}
		public static string Back(string text, TransliterationType type) {
			string output = text;
			Dictionary<string, string> tdict = GetDictionaryByType(type);
			foreach (KeyValuePair<string, string> key in tdict) {
				output = output.Replace(key.Value, key.Key);
			}
			return output;
		}
		private static Dictionary<string, string> GetDictionaryByType(TransliterationType type) {
			Dictionary<string, string> tdict = iso;
			if (type == TransliterationType.Gost) tdict = gost;
			return tdict;
		}

	}


	/// <summary>
	/// Class for encoding and decoding a string to QuotedPrintable
	/// RFC 1521 http://www.ietf.org/rfc/rfc1521.txt
	/// RFC 2045 http://www.ietf.org/rfc/rfc2045.txt
	/// Date: 2006-03-23
	/// Author: Kevin Spaun
	/// Company: SPAUN Informationstechnik GmbH - http://www.spaun-it.com/
	/// Feedback: kspaun@spaun-it.de
	/// License: This piece of code comes with no guaranties. You can use it for whatever you want for free.
	/// </summary>
	public static class QuotedPrintable {
		private const byte EQUALS = 61;
		private const byte CR = 13;
		private const byte LF = 10;
		private const byte SPACE = 32;
		private const byte TAB = 9;

		/// <summary>
		/// Encodes a string to QuotedPrintable
		/// </summary>
		/// <param name="_ToEncode">String to encode</param>
		/// <returns>QuotedPrintable encoded string</returns>
		public static string QuotedPrintableEncode(this string _ToEncode) {
			StringBuilder Encoded = new StringBuilder();
			string hex = string.Empty;
			byte[] bytes = Encoding.Default.GetBytes(_ToEncode);
			//byte[] bytes = Encoding.UTF8.GetBytes(_ToEncode);
			for (int i = 0; i < bytes.Length; i++) {
				//these characters must be encoded
				if ((bytes[i] < 33 || bytes[i] > 126 || bytes[i] == EQUALS) && bytes[i] != CR && bytes[i] != LF && bytes[i] != SPACE) {
					if (bytes[i].ToString("X").Length < 2) {
						hex = "0" + bytes[i].ToString("X");
						Encoded.Append("=" + hex);
					} else {
						hex = bytes[i].ToString("X");
						Encoded.Append("=" + hex);
					}
				} else {
					//check if index out of range
					if ((i + 1) < bytes.Length) {
						//if TAB is at the end of the line - encode it!
						if ((bytes[i] == TAB && bytes[i + 1] == LF) || (bytes[i] == TAB && bytes[i + 1] == CR)) {
							Encoded.Append("=0" + bytes[i].ToString("X"));
						}
						//if SPACE is at the end of the line - encode it!
						else if ((bytes[i] == SPACE && bytes[i + 1] == LF) || (bytes[i] == SPACE && bytes[i + 1] == CR)) {
							Encoded.Append("=" + bytes[i].ToString("X"));
						} else {
							Encoded.Append(System.Convert.ToChar(bytes[i]));
						}
					} else {
						Encoded.Append(System.Convert.ToChar(bytes[i]));
					}
				}
			}
			return Encoded.ToString();
		}

		/// <summary>
		/// Decodes a QuotedPrintable encoded string
		/// </summary>
		/// <param name="_ToDecode">The encoded string to decode</param>
		/// <returns>Decoded string</returns>
		public static string QuotedPrintableDecode(this string _ToDecode) {
			char[] chars = _ToDecode.ToCharArray();
			byte[] bytes = new byte[chars.Length];
			int bytesCount = 0;
			for (int i = 0; i < chars.Length; i++) {
				// if encoded character found decode it
				if (chars[i] == '=') {
					bytes[bytesCount++] = System.Convert.ToByte(int.Parse(chars[i + 1].ToString() + chars[i + 2].ToString(), System.Globalization.NumberStyles.HexNumber));

					i += 2;
				} else {
					bytes[bytesCount++] = System.Convert.ToByte(chars[i]);
				}
			}
			return System.Text.Encoding.Default.GetString(bytes, 0, bytesCount);
			//return System.Text.Encoding.UTF8.GetString(bytes, 0, bytesCount);
		}
	}

}

