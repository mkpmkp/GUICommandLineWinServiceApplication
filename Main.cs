using System;
using System.Reflection;
using System.Windows.Forms;

namespace Application1 {

	public partial class Main : Form {

		// Конструктор
		public Main() {
			InitializeComponent();

			Global.StartedAs = @"GUI";
			// Дата сборки в заголовке окна
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			Global.BuildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
			this.Text += $" сборка от {Global.BuildDate:dd.MM.yyyy HH:mm:ss}";

			// Конфигурация
			try {
				Global.Configuration = new ConfigurationClass();
			} catch (Exception Ex) {
				MessageBox.Show($"Ошибка конфигурации: {Ex.Message}");
			}
			Global.ApplyConfiguration();

			// Локальные пути
			Global.LocalPaths();

			// Сервисы
			Global.StartServices();
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e) {
			ConsoleServer.Stop();
			Application.Exit();
		}

		private void fMainBtnClose_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void fMainBtnConfiguration_Click(object sender, EventArgs e) {
			var ConForm = new ConfigurationForm();
			ConForm.ShowDialog();
		}
	}
}
