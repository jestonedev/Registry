namespace Registry.Reporting
{
    partial class TenancyStatisticFilterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyStatisticFilterForm));
            this.vButtonOk = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.comboBoxRegion = new System.Windows.Forms.ComboBox();
            this.checkBoxRegionEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.checkBoxPremisesNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxPremisesNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxHouseEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxStreetEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.comboBoxRentType = new System.Windows.Forms.ComboBox();
            this.checkBoxRentTypeEnable = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePickerBeginDateTo = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dateTimePickerBeginDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePickerRegistrationTo = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.dateTimePickerRegistrationFrom = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDateTo = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBoxReasonType = new System.Windows.Forms.ComboBox();
            this.checkBoxReasonTypeEnable = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // vButtonOk
            // 
            this.vButtonOk.AllowAnimations = true;
            this.vButtonOk.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOk.Location = new System.Drawing.Point(118, 389);
            this.vButtonOk.Name = "vButtonOk";
            this.vButtonOk.RoundedCornersMask = ((byte)(15));
            this.vButtonOk.Size = new System.Drawing.Size(117, 35);
            this.vButtonOk.TabIndex = 18;
            this.vButtonOk.Text = "Сформировать";
            this.vButtonOk.UseVisualStyleBackColor = false;
            this.vButtonOk.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOk.Click += new System.EventHandler(this.vButtonOk_Click);
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(253, 389);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 19;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // comboBoxRegion
            // 
            this.comboBoxRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegion.Enabled = false;
            this.comboBoxRegion.FormattingEnabled = true;
            this.comboBoxRegion.Location = new System.Drawing.Point(40, 24);
            this.comboBoxRegion.Name = "comboBoxRegion";
            this.comboBoxRegion.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRegion.TabIndex = 1;
            // 
            // checkBoxRegionEnable
            // 
            this.checkBoxRegionEnable.AutoSize = true;
            this.checkBoxRegionEnable.Location = new System.Drawing.Point(15, 28);
            this.checkBoxRegionEnable.Name = "checkBoxRegionEnable";
            this.checkBoxRegionEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegionEnable.TabIndex = 0;
            this.checkBoxRegionEnable.UseVisualStyleBackColor = true;
            this.checkBoxRegionEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegionEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 15);
            this.label8.TabIndex = 58;
            this.label8.Text = "Жилой район";
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Enabled = false;
            this.textBoxHouse.Location = new System.Drawing.Point(40, 110);
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(437, 21);
            this.textBoxHouse.TabIndex = 5;
            // 
            // checkBoxPremisesNumEnable
            // 
            this.checkBoxPremisesNumEnable.AutoSize = true;
            this.checkBoxPremisesNumEnable.Location = new System.Drawing.Point(15, 154);
            this.checkBoxPremisesNumEnable.Name = "checkBoxPremisesNumEnable";
            this.checkBoxPremisesNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPremisesNumEnable.TabIndex = 6;
            this.checkBoxPremisesNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxPremisesNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxPremisesNumEnable_CheckedChanged);
            // 
            // textBoxPremisesNum
            // 
            this.textBoxPremisesNum.Enabled = false;
            this.textBoxPremisesNum.Location = new System.Drawing.Point(40, 151);
            this.textBoxPremisesNum.MaxLength = 4;
            this.textBoxPremisesNum.Name = "textBoxPremisesNum";
            this.textBoxPremisesNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxPremisesNum.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 15);
            this.label3.TabIndex = 54;
            this.label3.Text = "Номер помещения/комнаты";
            // 
            // checkBoxHouseEnable
            // 
            this.checkBoxHouseEnable.AutoSize = true;
            this.checkBoxHouseEnable.Location = new System.Drawing.Point(15, 113);
            this.checkBoxHouseEnable.Name = "checkBoxHouseEnable";
            this.checkBoxHouseEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseEnable.TabIndex = 4;
            this.checkBoxHouseEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // checkBoxStreetEnable
            // 
            this.checkBoxStreetEnable.AutoSize = true;
            this.checkBoxStreetEnable.Location = new System.Drawing.Point(15, 71);
            this.checkBoxStreetEnable.Name = "checkBoxStreetEnable";
            this.checkBoxStreetEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetEnable.TabIndex = 2;
            this.checkBoxStreetEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 53;
            this.label2.Text = "Номер дома";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 52;
            this.label1.Text = "Улица";
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Enabled = false;
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(40, 67);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStreet.TabIndex = 3;
            // 
            // comboBoxRentType
            // 
            this.comboBoxRentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRentType.Enabled = false;
            this.comboBoxRentType.FormattingEnabled = true;
            this.comboBoxRentType.Location = new System.Drawing.Point(40, 192);
            this.comboBoxRentType.Name = "comboBoxRentType";
            this.comboBoxRentType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRentType.TabIndex = 9;
            // 
            // checkBoxRentTypeEnable
            // 
            this.checkBoxRentTypeEnable.AutoSize = true;
            this.checkBoxRentTypeEnable.Location = new System.Drawing.Point(15, 196);
            this.checkBoxRentTypeEnable.Name = "checkBoxRentTypeEnable";
            this.checkBoxRentTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRentTypeEnable.TabIndex = 8;
            this.checkBoxRentTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxRentTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxRentTypeEnable_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 15);
            this.label4.TabIndex = 61;
            this.label4.Text = "Тип найма";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(253, 323);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 15);
            this.label7.TabIndex = 71;
            this.label7.Text = "по";
            // 
            // dateTimePickerBeginDateTo
            // 
            this.dateTimePickerBeginDateTo.Checked = false;
            this.dateTimePickerBeginDateTo.Location = new System.Drawing.Point(278, 319);
            this.dateTimePickerBeginDateTo.Name = "dateTimePickerBeginDateTo";
            this.dateTimePickerBeginDateTo.ShowCheckBox = true;
            this.dateTimePickerBeginDateTo.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerBeginDateTo.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 323);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 15);
            this.label5.TabIndex = 69;
            this.label5.Text = "с";
            // 
            // dateTimePickerBeginDateFrom
            // 
            this.dateTimePickerBeginDateFrom.Checked = false;
            this.dateTimePickerBeginDateFrom.Location = new System.Drawing.Point(38, 319);
            this.dateTimePickerBeginDateFrom.Name = "dateTimePickerBeginDateFrom";
            this.dateTimePickerBeginDateFrom.ShowCheckBox = true;
            this.dateTimePickerBeginDateFrom.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerBeginDateFrom.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 302);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(165, 15);
            this.label6.TabIndex = 67;
            this.label6.Text = "Начало действия договора";
            // 
            // dateTimePickerRegistrationTo
            // 
            this.dateTimePickerRegistrationTo.Checked = false;
            this.dateTimePickerRegistrationTo.Location = new System.Drawing.Point(278, 278);
            this.dateTimePickerRegistrationTo.Name = "dateTimePickerRegistrationTo";
            this.dateTimePickerRegistrationTo.ShowCheckBox = true;
            this.dateTimePickerRegistrationTo.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerRegistrationTo.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(253, 282);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 15);
            this.label9.TabIndex = 65;
            this.label9.Text = "по";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 282);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(13, 15);
            this.label10.TabIndex = 64;
            this.label10.Text = "с";
            // 
            // dateTimePickerRegistrationFrom
            // 
            this.dateTimePickerRegistrationFrom.Checked = false;
            this.dateTimePickerRegistrationFrom.Location = new System.Drawing.Point(38, 278);
            this.dateTimePickerRegistrationFrom.Name = "dateTimePickerRegistrationFrom";
            this.dateTimePickerRegistrationFrom.ShowCheckBox = true;
            this.dateTimePickerRegistrationFrom.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerRegistrationFrom.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 261);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(114, 15);
            this.label11.TabIndex = 62;
            this.label11.Text = "Дата регистрации";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(253, 364);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(21, 15);
            this.label12.TabIndex = 76;
            this.label12.Text = "по";
            // 
            // dateTimePickerEndDateTo
            // 
            this.dateTimePickerEndDateTo.Checked = false;
            this.dateTimePickerEndDateTo.Location = new System.Drawing.Point(278, 360);
            this.dateTimePickerEndDateTo.Name = "dateTimePickerEndDateTo";
            this.dateTimePickerEndDateTo.ShowCheckBox = true;
            this.dateTimePickerEndDateTo.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerEndDateTo.TabIndex = 17;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 364);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(13, 15);
            this.label13.TabIndex = 74;
            this.label13.Text = "с";
            // 
            // dateTimePickerEndDateFrom
            // 
            this.dateTimePickerEndDateFrom.Checked = false;
            this.dateTimePickerEndDateFrom.Location = new System.Drawing.Point(38, 360);
            this.dateTimePickerEndDateFrom.Name = "dateTimePickerEndDateFrom";
            this.dateTimePickerEndDateFrom.ShowCheckBox = true;
            this.dateTimePickerEndDateFrom.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerEndDateFrom.TabIndex = 16;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 343);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(185, 15);
            this.label14.TabIndex = 72;
            this.label14.Text = "Окончание действия договора";
            // 
            // comboBoxReasonType
            // 
            this.comboBoxReasonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReasonType.Enabled = false;
            this.comboBoxReasonType.FormattingEnabled = true;
            this.comboBoxReasonType.Location = new System.Drawing.Point(40, 235);
            this.comboBoxReasonType.Name = "comboBoxReasonType";
            this.comboBoxReasonType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxReasonType.TabIndex = 11;
            // 
            // checkBoxReasonTypeEnable
            // 
            this.checkBoxReasonTypeEnable.AutoSize = true;
            this.checkBoxReasonTypeEnable.Location = new System.Drawing.Point(15, 239);
            this.checkBoxReasonTypeEnable.Name = "checkBoxReasonTypeEnable";
            this.checkBoxReasonTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxReasonTypeEnable.TabIndex = 10;
            this.checkBoxReasonTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxReasonTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxReasonTypeEnable_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 218);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(93, 15);
            this.label15.TabIndex = 79;
            this.label15.Text = "Тип основания";
            // 
            // TenancyStatisticFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(491, 434);
            this.Controls.Add(this.comboBoxReasonType);
            this.Controls.Add(this.checkBoxReasonTypeEnable);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.dateTimePickerEndDateTo);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.dateTimePickerEndDateFrom);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dateTimePickerBeginDateTo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dateTimePickerBeginDateFrom);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePickerRegistrationTo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.dateTimePickerRegistrationFrom);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboBoxRentType);
            this.Controls.Add(this.checkBoxRentTypeEnable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxRegion);
            this.Controls.Add(this.checkBoxRegionEnable);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxHouse);
            this.Controls.Add(this.checkBoxPremisesNumEnable);
            this.Controls.Add(this.textBoxPremisesNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxHouseEnable);
            this.Controls.Add(this.checkBoxStreetEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxStreet);
            this.Controls.Add(this.vButtonOk);
            this.Controls.Add(this.vButtonCancel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TenancyStatisticFilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Критерии отчета статистики найма";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonOk;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.ComboBox comboBoxRegion;
        private System.Windows.Forms.CheckBox checkBoxRegionEnable;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxHouse;
        private System.Windows.Forms.CheckBox checkBoxPremisesNumEnable;
        private System.Windows.Forms.TextBox textBoxPremisesNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxHouseEnable;
        private System.Windows.Forms.CheckBox checkBoxStreetEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxStreet;
        private System.Windows.Forms.ComboBox comboBoxRentType;
        private System.Windows.Forms.CheckBox checkBoxRentTypeEnable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dateTimePickerBeginDateTo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dateTimePickerBeginDateFrom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePickerRegistrationTo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dateTimePickerRegistrationFrom;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndDateTo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndDateFrom;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBoxReasonType;
        private System.Windows.Forms.CheckBox checkBoxReasonTypeEnable;
        private System.Windows.Forms.Label label15;
    }
}