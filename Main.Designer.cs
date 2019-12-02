namespace Application1
{
    public partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
			this.sqlInsertCommand2 = new System.Data.SqlClient.SqlCommand();
			this.fMainBtnClose = new System.Windows.Forms.Button();
			this.fMainBtnConfiguration = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// fMainBtnClose
			// 
			this.fMainBtnClose.Location = new System.Drawing.Point(12, 12);
			this.fMainBtnClose.Name = "fMainBtnClose";
			this.fMainBtnClose.Size = new System.Drawing.Size(115, 45);
			this.fMainBtnClose.TabIndex = 1;
			this.fMainBtnClose.Text = "Закрыть";
			this.fMainBtnClose.UseVisualStyleBackColor = true;
			this.fMainBtnClose.Click += new System.EventHandler(this.fMainBtnClose_Click);
			// 
			// fMainBtnConfiguration
			// 
			this.fMainBtnConfiguration.Location = new System.Drawing.Point(133, 12);
			this.fMainBtnConfiguration.Name = "fMainBtnConfiguration";
			this.fMainBtnConfiguration.Size = new System.Drawing.Size(125, 44);
			this.fMainBtnConfiguration.TabIndex = 3;
			this.fMainBtnConfiguration.Text = "Конфигурация";
			this.fMainBtnConfiguration.UseVisualStyleBackColor = true;
			this.fMainBtnConfiguration.Click += new System.EventHandler(this.fMainBtnConfiguration_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(718, 405);
			this.Controls.Add(this.fMainBtnConfiguration);
			this.Controls.Add(this.fMainBtnClose);
			this.IsMdiContainer = true;
			this.MinimumSize = new System.Drawing.Size(600, 300);
			this.Name = "Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Application1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.ResumeLayout(false);

        }

        #endregion
        public System.Data.SqlClient.SqlCommand sqlInsertCommand2;
		private System.Windows.Forms.Button fMainBtnClose;
		private System.Windows.Forms.Button fMainBtnConfiguration;
	}
}

