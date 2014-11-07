namespace Registry
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.numericUpDownMaxDBConnectionCount = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxLDAPUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLDAPPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxActivityManagerPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxActivityManagerOutputCodepage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxActivityManagerConfigsPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxDBConnectionCount)).BeginInit();
            this.SuspendLayout();
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.Location = new System.Drawing.Point(87, 293);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(100, 30);
            this.vButton2.TabIndex = 4;
            this.vButton2.Text = "Сохранить";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButton2.Click += new System.EventHandler(this.vButton2_Click);
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton1.Location = new System.Drawing.Point(203, 293);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(100, 30);
            this.vButton1.TabIndex = 5;
            this.vButton1.Text = "Отменить";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // numericUpDownMaxDBConnectionCount
            // 
            this.numericUpDownMaxDBConnectionCount.Location = new System.Drawing.Point(10, 264);
            this.numericUpDownMaxDBConnectionCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxDBConnectionCount.Name = "numericUpDownMaxDBConnectionCount";
            this.numericUpDownMaxDBConnectionCount.Size = new System.Drawing.Size(370, 20);
            this.numericUpDownMaxDBConnectionCount.TabIndex = 48;
            this.numericUpDownMaxDBConnectionCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 248);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(296, 13);
            this.label9.TabIndex = 49;
            this.label9.Text = "Количество одновременных подключений к базе данных";
            // 
            // textBoxConnectionString
            // 
            this.textBoxConnectionString.Location = new System.Drawing.Point(10, 24);
            this.textBoxConnectionString.MaxLength = 16;
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            this.textBoxConnectionString.Size = new System.Drawing.Size(370, 20);
            this.textBoxConnectionString.TabIndex = 52;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(106, 13);
            this.label10.TabIndex = 51;
            this.label10.Text = "Строка соединения";
            // 
            // textBoxLDAPUserName
            // 
            this.textBoxLDAPUserName.Location = new System.Drawing.Point(10, 63);
            this.textBoxLDAPUserName.MaxLength = 16;
            this.textBoxLDAPUserName.Name = "textBoxLDAPUserName";
            this.textBoxLDAPUserName.Size = new System.Drawing.Size(370, 20);
            this.textBoxLDAPUserName.TabIndex = 54;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 53;
            this.label1.Text = "Пользователь LDAP";
            // 
            // textBoxLDAPPassword
            // 
            this.textBoxLDAPPassword.Location = new System.Drawing.Point(10, 102);
            this.textBoxLDAPPassword.MaxLength = 16;
            this.textBoxLDAPPassword.Name = "textBoxLDAPPassword";
            this.textBoxLDAPPassword.PasswordChar = '•';
            this.textBoxLDAPPassword.Size = new System.Drawing.Size(370, 20);
            this.textBoxLDAPPassword.TabIndex = 56;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "Пароль LDAP";
            // 
            // textBoxActivityManagerPath
            // 
            this.textBoxActivityManagerPath.Location = new System.Drawing.Point(10, 143);
            this.textBoxActivityManagerPath.MaxLength = 16;
            this.textBoxActivityManagerPath.Name = "textBoxActivityManagerPath";
            this.textBoxActivityManagerPath.Size = new System.Drawing.Size(370, 20);
            this.textBoxActivityManagerPath.TabIndex = 58;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 57;
            this.label3.Text = "Путь до менеджера отчетов";
            // 
            // textBoxActivityManagerOutputCodepage
            // 
            this.textBoxActivityManagerOutputCodepage.Location = new System.Drawing.Point(10, 183);
            this.textBoxActivityManagerOutputCodepage.MaxLength = 16;
            this.textBoxActivityManagerOutputCodepage.Name = "textBoxActivityManagerOutputCodepage";
            this.textBoxActivityManagerOutputCodepage.Size = new System.Drawing.Size(370, 20);
            this.textBoxActivityManagerOutputCodepage.TabIndex = 60;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(203, 13);
            this.label4.TabIndex = 59;
            this.label4.Text = "Кодовая страница менеджера отчетов";
            // 
            // textBoxActivityManagerConfigsPath
            // 
            this.textBoxActivityManagerConfigsPath.Location = new System.Drawing.Point(10, 224);
            this.textBoxActivityManagerConfigsPath.MaxLength = 16;
            this.textBoxActivityManagerConfigsPath.Name = "textBoxActivityManagerConfigsPath";
            this.textBoxActivityManagerConfigsPath.Size = new System.Drawing.Size(370, 20);
            this.textBoxActivityManagerConfigsPath.TabIndex = 62;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(229, 13);
            this.label5.TabIndex = 61;
            this.label5.Text = "Путь до конфигурационных файлов отчетов";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(391, 333);
            this.Controls.Add(this.textBoxActivityManagerConfigsPath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxActivityManagerOutputCodepage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxActivityManagerPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxLDAPPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxLDAPUserName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxConnectionString);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.numericUpDownMaxDBConnectionCount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxDBConnectionCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButton2;
        private VIBlend.WinForms.Controls.vButton vButton1;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxDBConnectionCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxLDAPUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLDAPPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxActivityManagerPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxActivityManagerOutputCodepage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxActivityManagerConfigsPath;
        private System.Windows.Forms.Label label5;
    }
}