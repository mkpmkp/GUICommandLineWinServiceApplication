using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Application1 {

	public class CircleTextBuffer {

		public class TextItem {
			public long Counter = 0;
			public string Text;
			public Color ForeColor;
			public Color BackColor;
		}

		// Запирающая переменная
		public static readonly object syncRoot2 = new object();
		// Длина буфера
		const int BufferLength = 1200;
		private TextItem[] Items = new TextItem[BufferLength];
		private long Counter = 0;
		private long CycleCounter = 0;
		public long CurrentCounter { get{ return Counter; } }
		int WritePointer = 0;
		public void Add(string Text, Color ForeColor = default(Color), Color BackColor = default(Color)) {
			lock (syncRoot2) {
				Counter++;
				Items[WritePointer] = new TextItem {
					Counter = Counter,
					Text = Text,
					ForeColor = ForeColor,
					BackColor = BackColor
				};
				WritePointer++;
				if (WritePointer >= BufferLength) {
					WritePointer = 0;
					CycleCounter++;
				}
			}
		}
		public TextItem[] Get(long LastCounter = 0) {
			var sortedItems = from entry in Items where (entry != null && entry.Counter > LastCounter) orderby entry.Counter ascending select entry;
			return sortedItems.ToArray();
		}
		public void Clear() {
			lock (syncRoot2) {
				Items = new TextItem[BufferLength];
				Counter = 0;
				CycleCounter = 0;
				WritePointer = 0;
			}
		}
		public Dictionary<string, object> Test(string verbose = null) {
			var ret = new Dictionary<string, object>() {
				[@"BufferLength"] = BufferLength,
				[@"Counter"] = Counter,
				[@"CycleCounter"] = CycleCounter,
				[@"WritePointer"] = WritePointer,
			};
			if (verbose == @"items") {
				ret.Add(@"Items", Items);
			}
			if (verbose == @"get") {
				ret.Add(@"Get", Get());
			}
			return ret;
		}
	}

	// Класс отвечает за логирование всего
	static public class LOG {

		// Запирающая переменная для AsyncFileWrite
		public static readonly object syncRoot = new object();

		// Циклический буфер для вывода лога в консоль
		public static CircleTextBuffer ConsoleTextBuffer = new CircleTextBuffer();
		public static void ClearCircleTextBuffer() {
			ConsoleTextBuffer = new CircleTextBuffer();
		}

		// Асинхронная запись в файл
		public static void AsyncFileWrite(string pathFile, string txt) {
			lock (syncRoot) {
				try {
					var writer = File.AppendText(pathFile);
					using (writer) {
						writer.WriteLine(txt);
					}
				} catch { };
			}
		}

		// Помещает строку в RichTextBox (одновременно пишет в лог-файл)
		delegate void AppendLogCall(RichTextBox rb, string txt, Color ForeColor = default(Color), Color BackColor = default(Color));
		public static void AppendLog(RichTextBox rb, string txt, Color ForeColor = default(Color), Color BackColor = default(Color)) {
			if (rb != null) {
				if (rb.InvokeRequired) {
					var d = new AppendLogCall(AppendLog);
					rb.Invoke(d, rb, txt, ForeColor, BackColor);
				} else {
					var Message = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {txt}";
					// Пишем в файл
					LOG.AsyncFileWrite(Global.LocalLogPath, Message);
					// Пишем в циклический буфер
					LOG.ConsoleTextBuffer.Add(Message, ForeColor, BackColor);
					try {
						rb.SelectionColor = ForeColor;
						rb.AppendText(Message);
						rb.AppendText(Environment.NewLine);
						rb.SelectionStart = rb.TextLength;
						rb.ScrollToCaret();
					} catch { }
				}
			} else {
				// Пишем в файл
				var Message = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {txt}";
				LOG.AsyncFileWrite(Global.LocalLogPath, Message);
				// Пишем в циклический буфер
				LOG.ConsoleTextBuffer.Add(Message, ForeColor, BackColor);
			}
		}

		// Обновление свойства Text у Label из другого потока приложения
		public delegate void UpdateTextPropertyCall(object L, string Txt, Color Clr = default(Color));
		public static void UpdateTextProperty(object O, string Txt, Color Clr = default(Color)) {
			if (O != null) {
				if (O.GetType() == typeof(Label)) {
					var L = O as Label;
					if (L.InvokeRequired) {
						var c = new UpdateTextPropertyCall(UpdateTextProperty);
						L.Invoke(c, L, Txt, Clr);
					} else {
						L.Text = Txt;
						L.ForeColor = Clr;
					}
				} else if(O.GetType() == typeof(ToolStripStatusLabel)) {
					var L = O as ToolStripStatusLabel;
					var Lp = L.GetCurrentParent();
					if (Lp.InvokeRequired) {
						var c = new UpdateTextPropertyCall(UpdateTextProperty);
						Lp.Invoke(c, L, Txt, Clr);
					} else {
						L.Text = Txt;
						L.ForeColor = Clr;
					}
				}
			}
		}

	}
}
