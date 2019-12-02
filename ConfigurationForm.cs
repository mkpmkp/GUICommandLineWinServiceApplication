using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace Application1 {

	public partial class ConfigurationForm : Form {

		public ConfigurationForm() {
			InitializeComponent();

			// Принудительно переключаем вторичный профиль = текущему
			Global.Configuration.SecondaryProfile(Global.Configuration.CurrentProfile());

			buttonLoadFromCurrent.Enabled = false;
			buttonDelete.Enabled = false;

			comboBoxProfileName.Items.AddRange(Global.Configuration.GetProfiles());
			comboBoxProfileName.SelectedItem = Global.Configuration.SecondaryProfile();

			// Загружаем данные в поля
			LoadParametersFromProfile();
			// Текст кнопки "Загрузить из..."
			buttonLoadFromCurrent.Text = $"Загрузить из '{Global.Configuration.CurrentProfile()}'";
		}

		private void LoadParametersFromProfile(dynamic Profile = null) {
			if (Profile == null) Profile = Global.Configuration.Secondary;
			try {
				textBoxSenderID.Text = Profile.SenderID;
				textBoxTransportCertificate.Text = Profile.TransportCertificate;
				textBoxDBHost.Text = Profile.DBHost;
				textBoxDBName.Text = Profile.DBName;
				textBoxDBUser.Text = Profile.DBUserName;
				textBoxDBPassword.Text = ((string)Profile.DBUserPassword).Decrypt(@"ENbvcxz54");
				textBoxConsolePort.Text = (((string)Profile.ConsolePort).ToInt() ?? 500).ToString();
			} catch (Exception Ex) {
				MessageBox.Show($"Ошибка: {Ex.Message}");
			}
		}

		private void ApplyParametersToSecondaryProfile() {
			try {
				Global.Configuration.Secondary.SenderID = textBoxSenderID.Text;
				Global.Configuration.Secondary.TransportCertificate = textBoxTransportCertificate.Text;
				Global.Configuration.Secondary.DBHost = textBoxDBHost.Text;
				Global.Configuration.Secondary.DBName = textBoxDBName.Text;
				Global.Configuration.Secondary.DBUserName = textBoxDBUser.Text;
				Global.Configuration.Secondary.DBUserPassword = textBoxDBPassword.Text.Encrypt(@"ENbvcxz54");
				Global.Configuration.Secondary.ConsolePort = textBoxConsolePort.Text;
				Global.ApplyConfiguration();
			} catch (Exception Ex) {
				MessageBox.Show($"Ошибка: {Ex.Message}");
			}
		}

		private void ButtonCancel_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void ButtonDelete_Click(object sender, EventArgs e) {
			try {
				if (System.Windows.Forms.MessageBox.Show($"Вы действительно хотите удалить профиль '{Global.Configuration.SecondaryProfile()}'?",
				"Удалени профиля", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
					var DeletedSecondaryProfile = Global.Configuration.SecondaryProfile();
					Global.Configuration.SecondaryProfile(Global.Configuration.CurrentProfile());
					Global.Configuration.DeleteProfile(DeletedSecondaryProfile);
					comboBoxProfileName.Items.Clear();
					comboBoxProfileName.Items.AddRange(Global.Configuration.GetProfiles());
					comboBoxProfileName.SelectedItem = Global.Configuration.SecondaryProfile();
					LoadParametersFromProfile();
					MessageBox.Show($"Конфигурация '{DeletedSecondaryProfile}' удалена");
				}
			} catch (Exception Ex) {
				MessageBox.Show($"Ошибка: {Ex.Message}");
			}
		}

		private void ButtonSave_Click(object sender, EventArgs e) {
			try {
				if (MessageBox.Show(@"Сохранить текущую конфигурацию?", "Сохранение профиля", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
					ApplyParametersToSecondaryProfile();
					Global.Configuration.Save();
					MessageBox.Show("Конфигурация сохранена");
				}
			} catch (Exception Ex) {
				MessageBox.Show($"Ошибка: {Ex.Message}");
			}
		}

		private void ButtonApply_Click(object sender, EventArgs e) {
			ApplyParametersToSecondaryProfile();
		}

		private void ComboBoxProfileName_SelectedIndexChanged(object sender, EventArgs e) {
			// Переключаем вторичный профиль
			try {
				Global.Configuration.SecondaryProfile(comboBoxProfileName.SelectedItem.ToString());
			} catch(Exception Ex) {
				MessageBox.Show($"Ошибка: {Ex.Message}");
			}
			// Загружаем данные в поля
			LoadParametersFromProfile();
			if (Global.Configuration.CurrentProfile() == Global.Configuration.SecondaryProfile()) {
				buttonLoadFromCurrent.Enabled = false;
			} else {
				buttonLoadFromCurrent.Enabled = true;
			}
			if (Global.Configuration.CurrentProfile() == Global.Configuration.SecondaryProfile() || Global.Configuration.Secondary.Protected) {
				buttonDelete.Enabled = false;
			} else {
				buttonDelete.Enabled = true;
			}
			textBoxSenderID.Focus();
		}

		private void ButtonNewProfileCreate_Click(object sender, EventArgs e) {
			if (!string.IsNullOrWhiteSpace(textBoxNewProfileName.Text)) {
				try {
					Global.Configuration.CreateProfile(textBoxNewProfileName.Text.Trim());
					Global.Configuration.SecondaryProfile(textBoxNewProfileName.Text.Trim());
					comboBoxProfileName.Items.Clear();
					comboBoxProfileName.Items.AddRange(Global.Configuration.GetProfiles());
					comboBoxProfileName.SelectedItem = Global.Configuration.SecondaryProfile();
					textBoxNewProfileName.Text = null;
				} catch (Exception Ex) {
					MessageBox.Show($"Ошибка: {Ex.Message}");
				}
			}
		}

		private void ButtonSelectTransportCertificate_Click(object sender, EventArgs e) {
			//Открываем личное хранилище сертификатов
			X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
			//Только на чтение
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
			//Выбираем все сертификаты в хранилище
			X509Certificate2Collection fcollection = store.Certificates;
			//Открываем диалог выбора сертификата
			X509Certificate2Collection fc = X509Certificate2UI.SelectFromCollection(fcollection.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, true), "Сертификат ЭЦП для ГИС ЖКХ", "Сертификат ЭЦП для ГИС ЖКХ", X509SelectionFlag.SingleSelection);
			// Если в диалоге выбран сертификат.
			if (fc.Count > 0) {
				X509Certificate2 cert = fc[0];
				textBoxTransportCertificate.Text = cert.Thumbprint;
			}
		}

		private void ButtonLoadFromCurrent_Click(object sender, EventArgs e) {
			try {
				if (System.Windows.Forms.MessageBox.Show($"Вы действительно хотите загрузить данные из профиля '{Global.Configuration.CurrentProfile()}'?",
				"Загрузка данных из текущего профиля", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
					LoadParametersFromProfile(Global.Configuration.Current);
				}
			} catch (Exception Ex) {
				MessageBox.Show($"Ошибка: {Ex.Message}");
			}
		}
	}
}
