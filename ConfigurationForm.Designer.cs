namespace Application1 {
	partial class ConfigurationForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.labelDBHoist = new System.Windows.Forms.Label();
			this.textBoxDBHost = new System.Windows.Forms.TextBox();
			this.textBoxDBUser = new System.Windows.Forms.TextBox();
			this.textBoxDBPassword = new System.Windows.Forms.TextBox();
			this.comboBoxProfileName = new System.Windows.Forms.ComboBox();
			this.labelProfile = new System.Windows.Forms.Label();
			this.labelDBUser = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonSave = new System.Windows.Forms.Button();
			this.groupProfileParameters = new System.Windows.Forms.GroupBox();
			this.labelCommentOneStar = new System.Windows.Forms.Label();
			this.textBoxConsolePort = new System.Windows.Forms.TextBox();
			this.labelConsolePort = new System.Windows.Forms.Label();
			this.textBoxDBName = new System.Windows.Forms.TextBox();
			this.buttonSelectTransportCertificate = new System.Windows.Forms.Button();
			this.textBoxTransportCertificate = new System.Windows.Forms.TextBox();
			this.labelTransportCertificare = new System.Windows.Forms.Label();
			this.textBoxSenderID = new System.Windows.Forms.TextBox();
			this.labelSenderID = new System.Windows.Forms.Label();
			this.labelDBName = new System.Windows.Forms.Label();
			this.labelDBPassword = new System.Windows.Forms.Label();
			this.textBoxNewProfileName = new System.Windows.Forms.TextBox();
			this.buttonNewProfileCreate = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.labelCreate = new System.Windows.Forms.Label();
			this.buttonLoadFromCurrent = new System.Windows.Forms.Button();
			this.groupProfileParameters.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelDBHoist
			// 
			this.labelDBHoist.AutoSize = true;
			this.labelDBHoist.Location = new System.Drawing.Point(162, 81);
			this.labelDBHoist.Name = "labelDBHoist";
			this.labelDBHoist.Size = new System.Drawing.Size(50, 13);
			this.labelDBHoist.TabIndex = 0;
			this.labelDBHoist.Text = "Хост БД";
			// 
			// textBoxDBHost
			// 
			this.textBoxDBHost.Location = new System.Drawing.Point(219, 78);
			this.textBoxDBHost.Name = "textBoxDBHost";
			this.textBoxDBHost.Size = new System.Drawing.Size(207, 20);
			this.textBoxDBHost.TabIndex = 1;
			// 
			// textBoxDBUser
			// 
			this.textBoxDBUser.Location = new System.Drawing.Point(219, 131);
			this.textBoxDBUser.Name = "textBoxDBUser";
			this.textBoxDBUser.Size = new System.Drawing.Size(123, 20);
			this.textBoxDBUser.TabIndex = 2;
			// 
			// textBoxDBPassword
			// 
			this.textBoxDBPassword.Location = new System.Drawing.Point(219, 157);
			this.textBoxDBPassword.Name = "textBoxDBPassword";
			this.textBoxDBPassword.PasswordChar = '*';
			this.textBoxDBPassword.Size = new System.Drawing.Size(123, 20);
			this.textBoxDBPassword.TabIndex = 3;
			// 
			// comboBoxProfileName
			// 
			this.comboBoxProfileName.FormattingEnabled = true;
			this.comboBoxProfileName.Location = new System.Drawing.Point(71, 12);
			this.comboBoxProfileName.Name = "comboBoxProfileName";
			this.comboBoxProfileName.Size = new System.Drawing.Size(125, 21);
			this.comboBoxProfileName.TabIndex = 6;
			this.comboBoxProfileName.SelectedIndexChanged += new System.EventHandler(this.ComboBoxProfileName_SelectedIndexChanged);
			// 
			// labelProfile
			// 
			this.labelProfile.AutoSize = true;
			this.labelProfile.BackColor = System.Drawing.SystemColors.Control;
			this.labelProfile.Location = new System.Drawing.Point(12, 15);
			this.labelProfile.Name = "labelProfile";
			this.labelProfile.Size = new System.Drawing.Size(53, 13);
			this.labelProfile.TabIndex = 7;
			this.labelProfile.Text = "Профиль";
			// 
			// labelDBUser
			// 
			this.labelDBUser.AutoSize = true;
			this.labelDBUser.Location = new System.Drawing.Point(114, 134);
			this.labelDBUser.Name = "labelDBUser";
			this.labelDBUser.Size = new System.Drawing.Size(99, 13);
			this.labelDBUser.TabIndex = 8;
			this.labelDBUser.Text = "Пользователь БД";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCancel.Location = new System.Drawing.Point(11, 352);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Закрыть";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			// 
			// buttonApply
			// 
			this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonApply.Location = new System.Drawing.Point(92, 352);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(75, 23);
			this.buttonApply.TabIndex = 10;
			this.buttonApply.Text = "Применить";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.ButtonApply_Click);
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSave.Location = new System.Drawing.Point(173, 352);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(75, 23);
			this.buttonSave.TabIndex = 11;
			this.buttonSave.Text = "Сохранить";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
			// 
			// groupProfileParameters
			// 
			this.groupProfileParameters.Controls.Add(this.labelCommentOneStar);
			this.groupProfileParameters.Controls.Add(this.textBoxConsolePort);
			this.groupProfileParameters.Controls.Add(this.labelConsolePort);
			this.groupProfileParameters.Controls.Add(this.textBoxDBName);
			this.groupProfileParameters.Controls.Add(this.buttonSelectTransportCertificate);
			this.groupProfileParameters.Controls.Add(this.textBoxTransportCertificate);
			this.groupProfileParameters.Controls.Add(this.labelTransportCertificare);
			this.groupProfileParameters.Controls.Add(this.textBoxSenderID);
			this.groupProfileParameters.Controls.Add(this.labelSenderID);
			this.groupProfileParameters.Controls.Add(this.labelDBName);
			this.groupProfileParameters.Controls.Add(this.labelDBPassword);
			this.groupProfileParameters.Controls.Add(this.labelDBUser);
			this.groupProfileParameters.Controls.Add(this.textBoxDBPassword);
			this.groupProfileParameters.Controls.Add(this.textBoxDBUser);
			this.groupProfileParameters.Controls.Add(this.textBoxDBHost);
			this.groupProfileParameters.Controls.Add(this.labelDBHoist);
			this.groupProfileParameters.Location = new System.Drawing.Point(12, 53);
			this.groupProfileParameters.Name = "groupProfileParameters";
			this.groupProfileParameters.Size = new System.Drawing.Size(603, 284);
			this.groupProfileParameters.TabIndex = 13;
			this.groupProfileParameters.TabStop = false;
			this.groupProfileParameters.Text = "Параметры профиля";
			// 
			// labelCommentOneStar
			// 
			this.labelCommentOneStar.AutoSize = true;
			this.labelCommentOneStar.Location = new System.Drawing.Point(6, 251);
			this.labelCommentOneStar.Name = "labelCommentOneStar";
			this.labelCommentOneStar.Size = new System.Drawing.Size(220, 13);
			this.labelCommentOneStar.TabIndex = 44;
			this.labelCommentOneStar.Text = "* Необходимо перезапустить приложение";
			// 
			// textBoxConsolePort
			// 
			this.textBoxConsolePort.Location = new System.Drawing.Point(219, 209);
			this.textBoxConsolePort.Name = "textBoxConsolePort";
			this.textBoxConsolePort.Size = new System.Drawing.Size(62, 20);
			this.textBoxConsolePort.TabIndex = 41;
			// 
			// labelConsolePort
			// 
			this.labelConsolePort.AutoSize = true;
			this.labelConsolePort.Location = new System.Drawing.Point(129, 212);
			this.labelConsolePort.Name = "labelConsolePort";
			this.labelConsolePort.Size = new System.Drawing.Size(84, 13);
			this.labelConsolePort.TabIndex = 40;
			this.labelConsolePort.Text = "Порт консоли *";
			// 
			// textBoxDBName
			// 
			this.textBoxDBName.Location = new System.Drawing.Point(219, 105);
			this.textBoxDBName.Name = "textBoxDBName";
			this.textBoxDBName.Size = new System.Drawing.Size(207, 20);
			this.textBoxDBName.TabIndex = 35;
			// 
			// buttonSelectTransportCertificate
			// 
			this.buttonSelectTransportCertificate.Location = new System.Drawing.Point(542, 181);
			this.buttonSelectTransportCertificate.Name = "buttonSelectTransportCertificate";
			this.buttonSelectTransportCertificate.Size = new System.Drawing.Size(43, 23);
			this.buttonSelectTransportCertificate.TabIndex = 33;
			this.buttonSelectTransportCertificate.Text = "...";
			this.buttonSelectTransportCertificate.UseVisualStyleBackColor = true;
			this.buttonSelectTransportCertificate.Click += new System.EventHandler(this.ButtonSelectTransportCertificate_Click);
			// 
			// textBoxTransportCertificate
			// 
			this.textBoxTransportCertificate.Location = new System.Drawing.Point(219, 183);
			this.textBoxTransportCertificate.Name = "textBoxTransportCertificate";
			this.textBoxTransportCertificate.ReadOnly = true;
			this.textBoxTransportCertificate.Size = new System.Drawing.Size(317, 20);
			this.textBoxTransportCertificate.TabIndex = 32;
			// 
			// labelTransportCertificare
			// 
			this.labelTransportCertificare.AutoSize = true;
			this.labelTransportCertificare.Location = new System.Drawing.Point(69, 186);
			this.labelTransportCertificare.Name = "labelTransportCertificare";
			this.labelTransportCertificare.Size = new System.Drawing.Size(144, 13);
			this.labelTransportCertificare.TabIndex = 31;
			this.labelTransportCertificare.Text = "Транспортный сертификат";
			// 
			// textBoxSenderID
			// 
			this.textBoxSenderID.Location = new System.Drawing.Point(219, 31);
			this.textBoxSenderID.Name = "textBoxSenderID";
			this.textBoxSenderID.Size = new System.Drawing.Size(170, 20);
			this.textBoxSenderID.TabIndex = 25;
			// 
			// labelSenderID
			// 
			this.labelSenderID.AutoSize = true;
			this.labelSenderID.Location = new System.Drawing.Point(20, 34);
			this.labelSenderID.Name = "labelSenderID";
			this.labelSenderID.Size = new System.Drawing.Size(192, 13);
			this.labelSenderID.TabIndex = 24;
			this.labelSenderID.Text = "Идентификатор поставщика данных";
			// 
			// labelDBName
			// 
			this.labelDBName.AutoSize = true;
			this.labelDBName.Location = new System.Drawing.Point(164, 107);
			this.labelDBName.Name = "labelDBName";
			this.labelDBName.Size = new System.Drawing.Size(48, 13);
			this.labelDBName.TabIndex = 23;
			this.labelDBName.Text = "Имя БД";
			// 
			// labelDBPassword
			// 
			this.labelDBPassword.AutoSize = true;
			this.labelDBPassword.Location = new System.Drawing.Point(75, 160);
			this.labelDBPassword.Name = "labelDBPassword";
			this.labelDBPassword.Size = new System.Drawing.Size(138, 13);
			this.labelDBPassword.TabIndex = 13;
			this.labelDBPassword.Text = "Пароль пользователя БД";
			// 
			// textBoxNewProfileName
			// 
			this.textBoxNewProfileName.Location = new System.Drawing.Point(490, 11);
			this.textBoxNewProfileName.Name = "textBoxNewProfileName";
			this.textBoxNewProfileName.Size = new System.Drawing.Size(87, 20);
			this.textBoxNewProfileName.TabIndex = 14;
			// 
			// buttonNewProfileCreate
			// 
			this.buttonNewProfileCreate.Location = new System.Drawing.Point(585, 9);
			this.buttonNewProfileCreate.Name = "buttonNewProfileCreate";
			this.buttonNewProfileCreate.Size = new System.Drawing.Size(29, 23);
			this.buttonNewProfileCreate.TabIndex = 15;
			this.buttonNewProfileCreate.Text = "+";
			this.buttonNewProfileCreate.UseVisualStyleBackColor = true;
			this.buttonNewProfileCreate.Click += new System.EventHandler(this.ButtonNewProfileCreate_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDelete.Location = new System.Drawing.Point(539, 352);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(75, 23);
			this.buttonDelete.TabIndex = 16;
			this.buttonDelete.Text = "Удалить";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
			// 
			// labelCreate
			// 
			this.labelCreate.AutoSize = true;
			this.labelCreate.Location = new System.Drawing.Point(388, 14);
			this.labelCreate.Name = "labelCreate";
			this.labelCreate.Size = new System.Drawing.Size(96, 13);
			this.labelCreate.TabIndex = 17;
			this.labelCreate.Text = "Создать профиль";
			// 
			// buttonLoadFromCurrent
			// 
			this.buttonLoadFromCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonLoadFromCurrent.Location = new System.Drawing.Point(273, 352);
			this.buttonLoadFromCurrent.Name = "buttonLoadFromCurrent";
			this.buttonLoadFromCurrent.Size = new System.Drawing.Size(238, 23);
			this.buttonLoadFromCurrent.TabIndex = 18;
			this.buttonLoadFromCurrent.Text = "Загрузить из текущего профиля";
			this.buttonLoadFromCurrent.UseVisualStyleBackColor = true;
			this.buttonLoadFromCurrent.Click += new System.EventHandler(this.ButtonLoadFromCurrent_Click);
			// 
			// ConfigurationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 384);
			this.ControlBox = false;
			this.Controls.Add(this.buttonLoadFromCurrent);
			this.Controls.Add(this.labelCreate);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonNewProfileCreate);
			this.Controls.Add(this.textBoxNewProfileName);
			this.Controls.Add(this.groupProfileParameters);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelProfile);
			this.Controls.Add(this.comboBoxProfileName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigurationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Конфигурация";
			this.groupProfileParameters.ResumeLayout(false);
			this.groupProfileParameters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelDBHoist;
		private System.Windows.Forms.TextBox textBoxDBHost;
		private System.Windows.Forms.TextBox textBoxDBUser;
		private System.Windows.Forms.TextBox textBoxDBPassword;
		private System.Windows.Forms.ComboBox comboBoxProfileName;
		private System.Windows.Forms.Label labelProfile;
		private System.Windows.Forms.Label labelDBUser;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.GroupBox groupProfileParameters;
		private System.Windows.Forms.Label labelDBPassword;
		private System.Windows.Forms.Label labelDBName;
		private System.Windows.Forms.Label labelSenderID;
		private System.Windows.Forms.TextBox textBoxSenderID;
		private System.Windows.Forms.TextBox textBoxNewProfileName;
		private System.Windows.Forms.Button buttonNewProfileCreate;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Label labelCreate;
		private System.Windows.Forms.Button buttonLoadFromCurrent;
		private System.Windows.Forms.TextBox textBoxTransportCertificate;
		private System.Windows.Forms.Label labelTransportCertificare;
		private System.Windows.Forms.Button buttonSelectTransportCertificate;
		private System.Windows.Forms.TextBox textBoxDBName;
		private System.Windows.Forms.TextBox textBoxConsolePort;
		private System.Windows.Forms.Label labelConsolePort;
		private System.Windows.Forms.Label labelCommentOneStar;
	}
}