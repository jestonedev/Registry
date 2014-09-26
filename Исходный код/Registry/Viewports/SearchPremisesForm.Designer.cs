namespace Registry.Viewport
{
    partial class SearchPremisesForm
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
            this.checkBoxHouseEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxStreetEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.checkBoxPremisesNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxPremisesNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxFloorEnable = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownFloor = new System.Windows.Forms.NumericUpDown();
            this.checkBoxCadastralNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxFundType = new System.Windows.Forms.ComboBox();
            this.checkBoxFundTypeEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxAcceptedByExchangeEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxAcceptedByExchange = new System.Windows.Forms.CheckBox();
            this.checkBoxForOrpahnsEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxForOrpahns = new System.Windows.Forms.CheckBox();
            this.checkBoxAcceptedByOtherEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxAcceptedByOther = new System.Windows.Forms.CheckBox();
            this.checkBoxAcceptedByDonationEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxAcceptedByDonation = new System.Windows.Forms.CheckBox();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).BeginInit();
            this.SuspendLayout();
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton2.Location = new System.Drawing.Point(220, 337);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(100, 30);
            this.vButton2.TabIndex = 21;
            this.vButton2.Text = "Отмена";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.Location = new System.Drawing.Point(102, 337);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(100, 30);
            this.vButton1.TabIndex = 20;
            this.vButton1.Text = "Поиск";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButton1.Click += new System.EventHandler(this.vButton1_Click);
            // 
            // checkBoxHouseEnable
            // 
            this.checkBoxHouseEnable.AutoSize = true;
            this.checkBoxHouseEnable.Location = new System.Drawing.Point(15, 68);
            this.checkBoxHouseEnable.Name = "checkBoxHouseEnable";
            this.checkBoxHouseEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseEnable.TabIndex = 2;
            this.checkBoxHouseEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // checkBoxStreetEnable
            // 
            this.checkBoxStreetEnable.AutoSize = true;
            this.checkBoxStreetEnable.Location = new System.Drawing.Point(15, 28);
            this.checkBoxStreetEnable.Name = "checkBoxStreetEnable";
            this.checkBoxStreetEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetEnable.TabIndex = 0;
            this.checkBoxStreetEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Номер дома";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Улица";
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Enabled = false;
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(36, 25);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(375, 21);
            this.comboBoxStreet.TabIndex = 1;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // checkBoxPremisesNumEnable
            // 
            this.checkBoxPremisesNumEnable.AutoSize = true;
            this.checkBoxPremisesNumEnable.Location = new System.Drawing.Point(15, 109);
            this.checkBoxPremisesNumEnable.Name = "checkBoxPremisesNumEnable";
            this.checkBoxPremisesNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPremisesNumEnable.TabIndex = 4;
            this.checkBoxPremisesNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxPremisesNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxPremisesNumEnable_CheckedChanged);
            // 
            // textBoxPremisesNum
            // 
            this.textBoxPremisesNum.Enabled = false;
            this.textBoxPremisesNum.Location = new System.Drawing.Point(36, 106);
            this.textBoxPremisesNum.MaxLength = 4;
            this.textBoxPremisesNum.Name = "textBoxPremisesNum";
            this.textBoxPremisesNum.Size = new System.Drawing.Size(375, 20);
            this.textBoxPremisesNum.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Номер помещения";
            // 
            // checkBoxFloorEnable
            // 
            this.checkBoxFloorEnable.AutoSize = true;
            this.checkBoxFloorEnable.Location = new System.Drawing.Point(15, 148);
            this.checkBoxFloorEnable.Name = "checkBoxFloorEnable";
            this.checkBoxFloorEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFloorEnable.TabIndex = 6;
            this.checkBoxFloorEnable.UseVisualStyleBackColor = true;
            this.checkBoxFloorEnable.CheckedChanged += new System.EventHandler(this.checkBoxFloorEnable_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Этаж";
            // 
            // numericUpDownFloor
            // 
            this.numericUpDownFloor.Enabled = false;
            this.numericUpDownFloor.Location = new System.Drawing.Point(36, 145);
            this.numericUpDownFloor.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownFloor.Name = "numericUpDownFloor";
            this.numericUpDownFloor.Size = new System.Drawing.Size(375, 20);
            this.numericUpDownFloor.TabIndex = 7;
            // 
            // checkBoxCadastralNumEnable
            // 
            this.checkBoxCadastralNumEnable.AutoSize = true;
            this.checkBoxCadastralNumEnable.Location = new System.Drawing.Point(15, 187);
            this.checkBoxCadastralNumEnable.Name = "checkBoxCadastralNumEnable";
            this.checkBoxCadastralNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCadastralNumEnable.TabIndex = 8;
            this.checkBoxCadastralNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxCadastralNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxCadastralNumEnable_CheckedChanged);
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Enabled = false;
            this.textBoxCadastralNum.Location = new System.Drawing.Point(36, 184);
            this.textBoxCadastralNum.MaxLength = 15;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(375, 20);
            this.textBoxCadastralNum.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Кадастровый номер";
            // 
            // comboBoxFundType
            // 
            this.comboBoxFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFundType.Enabled = false;
            this.comboBoxFundType.FormattingEnabled = true;
            this.comboBoxFundType.Location = new System.Drawing.Point(36, 224);
            this.comboBoxFundType.Name = "comboBoxFundType";
            this.comboBoxFundType.Size = new System.Drawing.Size(375, 21);
            this.comboBoxFundType.TabIndex = 11;
            // 
            // checkBoxFundTypeEnable
            // 
            this.checkBoxFundTypeEnable.AutoSize = true;
            this.checkBoxFundTypeEnable.Location = new System.Drawing.Point(15, 227);
            this.checkBoxFundTypeEnable.Name = "checkBoxFundTypeEnable";
            this.checkBoxFundTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFundTypeEnable.TabIndex = 10;
            this.checkBoxFundTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxFundTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxFundTypeEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 208);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Вид найма";
            // 
            // checkBoxAcceptedByExchangeEnable
            // 
            this.checkBoxAcceptedByExchangeEnable.AutoSize = true;
            this.checkBoxAcceptedByExchangeEnable.Location = new System.Drawing.Point(15, 274);
            this.checkBoxAcceptedByExchangeEnable.Name = "checkBoxAcceptedByExchangeEnable";
            this.checkBoxAcceptedByExchangeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAcceptedByExchangeEnable.TabIndex = 14;
            this.checkBoxAcceptedByExchangeEnable.UseVisualStyleBackColor = true;
            this.checkBoxAcceptedByExchangeEnable.CheckedChanged += new System.EventHandler(this.checkBoxAcceptedByExchangeEnable_CheckedChanged);
            // 
            // checkBoxAcceptedByExchange
            // 
            this.checkBoxAcceptedByExchange.AutoSize = true;
            this.checkBoxAcceptedByExchange.Enabled = false;
            this.checkBoxAcceptedByExchange.Location = new System.Drawing.Point(36, 273);
            this.checkBoxAcceptedByExchange.Name = "checkBoxAcceptedByExchange";
            this.checkBoxAcceptedByExchange.Size = new System.Drawing.Size(284, 17);
            this.checkBoxAcceptedByExchange.TabIndex = 15;
            this.checkBoxAcceptedByExchange.Text = "Принято в муниципальную собственность по мене";
            this.checkBoxAcceptedByExchange.UseVisualStyleBackColor = true;
            // 
            // checkBoxForOrpahnsEnable
            // 
            this.checkBoxForOrpahnsEnable.AutoSize = true;
            this.checkBoxForOrpahnsEnable.Location = new System.Drawing.Point(15, 253);
            this.checkBoxForOrpahnsEnable.Name = "checkBoxForOrpahnsEnable";
            this.checkBoxForOrpahnsEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxForOrpahnsEnable.TabIndex = 12;
            this.checkBoxForOrpahnsEnable.UseVisualStyleBackColor = true;
            this.checkBoxForOrpahnsEnable.CheckedChanged += new System.EventHandler(this.checkBoxForOrpahnsEnable_CheckedChanged);
            // 
            // checkBoxForOrpahns
            // 
            this.checkBoxForOrpahns.AutoSize = true;
            this.checkBoxForOrpahns.Enabled = false;
            this.checkBoxForOrpahns.Location = new System.Drawing.Point(36, 252);
            this.checkBoxForOrpahns.Name = "checkBoxForOrpahns";
            this.checkBoxForOrpahns.Size = new System.Drawing.Size(173, 17);
            this.checkBoxForOrpahns.TabIndex = 13;
            this.checkBoxForOrpahns.Text = "Приобретено детям-сиротам";
            this.checkBoxForOrpahns.UseVisualStyleBackColor = true;
            // 
            // checkBoxAcceptedByOtherEnable
            // 
            this.checkBoxAcceptedByOtherEnable.AutoSize = true;
            this.checkBoxAcceptedByOtherEnable.Location = new System.Drawing.Point(15, 315);
            this.checkBoxAcceptedByOtherEnable.Name = "checkBoxAcceptedByOtherEnable";
            this.checkBoxAcceptedByOtherEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAcceptedByOtherEnable.TabIndex = 18;
            this.checkBoxAcceptedByOtherEnable.UseVisualStyleBackColor = true;
            this.checkBoxAcceptedByOtherEnable.CheckedChanged += new System.EventHandler(this.checkBoxAcceptedByOtherEnbale_CheckedChanged);
            // 
            // checkBoxAcceptedByOther
            // 
            this.checkBoxAcceptedByOther.AutoSize = true;
            this.checkBoxAcceptedByOther.Enabled = false;
            this.checkBoxAcceptedByOther.Location = new System.Drawing.Point(36, 314);
            this.checkBoxAcceptedByOther.Name = "checkBoxAcceptedByOther";
            this.checkBoxAcceptedByOther.Size = new System.Drawing.Size(370, 17);
            this.checkBoxAcceptedByOther.TabIndex = 19;
            this.checkBoxAcceptedByOther.Text = "Прочее основание для включения в муниципальную собственность";
            this.checkBoxAcceptedByOther.UseVisualStyleBackColor = true;
            // 
            // checkBoxAcceptedByDonationEnable
            // 
            this.checkBoxAcceptedByDonationEnable.AutoSize = true;
            this.checkBoxAcceptedByDonationEnable.Location = new System.Drawing.Point(15, 294);
            this.checkBoxAcceptedByDonationEnable.Name = "checkBoxAcceptedByDonationEnable";
            this.checkBoxAcceptedByDonationEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAcceptedByDonationEnable.TabIndex = 16;
            this.checkBoxAcceptedByDonationEnable.UseVisualStyleBackColor = true;
            this.checkBoxAcceptedByDonationEnable.CheckedChanged += new System.EventHandler(this.checkBoxAcceptedByDonationEnable_CheckedChanged);
            // 
            // checkBoxAcceptedByDonation
            // 
            this.checkBoxAcceptedByDonation.AutoSize = true;
            this.checkBoxAcceptedByDonation.Enabled = false;
            this.checkBoxAcceptedByDonation.Location = new System.Drawing.Point(36, 293);
            this.checkBoxAcceptedByDonation.Name = "checkBoxAcceptedByDonation";
            this.checkBoxAcceptedByDonation.Size = new System.Drawing.Size(302, 17);
            this.checkBoxAcceptedByDonation.TabIndex = 17;
            this.checkBoxAcceptedByDonation.Text = "Принято в муниципальную собственность по дарению";
            this.checkBoxAcceptedByDonation.UseVisualStyleBackColor = true;
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Enabled = false;
            this.textBoxHouse.Location = new System.Drawing.Point(36, 65);
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(375, 20);
            this.textBoxHouse.TabIndex = 38;
            // 
            // SearchPremisesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(423, 375);
            this.Controls.Add(this.textBoxHouse);
            this.Controls.Add(this.checkBoxAcceptedByOtherEnable);
            this.Controls.Add(this.checkBoxAcceptedByOther);
            this.Controls.Add(this.checkBoxAcceptedByDonationEnable);
            this.Controls.Add(this.checkBoxAcceptedByDonation);
            this.Controls.Add(this.checkBoxAcceptedByExchangeEnable);
            this.Controls.Add(this.checkBoxAcceptedByExchange);
            this.Controls.Add(this.checkBoxForOrpahnsEnable);
            this.Controls.Add(this.checkBoxForOrpahns);
            this.Controls.Add(this.comboBoxFundType);
            this.Controls.Add(this.checkBoxFundTypeEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBoxCadastralNumEnable);
            this.Controls.Add(this.textBoxCadastralNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBoxFloorEnable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownFloor);
            this.Controls.Add(this.checkBoxPremisesNumEnable);
            this.Controls.Add(this.textBoxPremisesNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxHouseEnable);
            this.Controls.Add(this.checkBoxStreetEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxStreet);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SearchPremisesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация квартир";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButton2;
        private VIBlend.WinForms.Controls.vButton vButton1;
        private System.Windows.Forms.CheckBox checkBoxHouseEnable;
        private System.Windows.Forms.CheckBox checkBoxStreetEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxStreet;
        private System.Windows.Forms.CheckBox checkBoxPremisesNumEnable;
        private System.Windows.Forms.TextBox textBoxPremisesNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxFloorEnable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownFloor;
        private System.Windows.Forms.CheckBox checkBoxCadastralNumEnable;
        private System.Windows.Forms.TextBox textBoxCadastralNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxFundType;
        private System.Windows.Forms.CheckBox checkBoxFundTypeEnable;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxAcceptedByExchangeEnable;
        private System.Windows.Forms.CheckBox checkBoxAcceptedByExchange;
        private System.Windows.Forms.CheckBox checkBoxForOrpahnsEnable;
        private System.Windows.Forms.CheckBox checkBoxForOrpahns;
        private System.Windows.Forms.CheckBox checkBoxAcceptedByOtherEnable;
        private System.Windows.Forms.CheckBox checkBoxAcceptedByOther;
        private System.Windows.Forms.CheckBox checkBoxAcceptedByDonationEnable;
        private System.Windows.Forms.CheckBox checkBoxAcceptedByDonation;
        private System.Windows.Forms.TextBox textBoxHouse;
    }
}