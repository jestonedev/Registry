using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.SearchForms
{
    partial class ExtendedSearchResettleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedSearchResettleForm));
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.textBoxPersonSNP = new System.Windows.Forms.TextBox();
            this.checkBoxPersonSNPEnable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownIDResettle = new System.Windows.Forms.NumericUpDown();
            this.checkBoxIDResettleEnable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxResettleDateEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.dateTimePickerResettleDate = new System.Windows.Forms.DateTimePicker();
            this.comboBoxResettleDateExpr = new System.Windows.Forms.ComboBox();
            this.comboBoxRegionTo = new System.Windows.Forms.ComboBox();
            this.checkBoxRegionToEnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxHouseTo = new System.Windows.Forms.TextBox();
            this.checkBoxPremisesNumToEnable = new System.Windows.Forms.CheckBox();
            this.textBoxPremisesNumTo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxHouseToEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxStreetToEnable = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxStreetTo = new System.Windows.Forms.ComboBox();
            this.comboBoxStreetFrom = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxStreetFromEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxHouseFromEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPremisesNumFrom = new System.Windows.Forms.TextBox();
            this.checkBoxPremisesNumFromEnable = new System.Windows.Forms.CheckBox();
            this.textBoxHouseFrom = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxRegionFromEnable = new System.Windows.Forms.CheckBox();
            this.comboBoxRegionFrom = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDResettle)).BeginInit();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(258, 476);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 24;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(121, 476);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 23;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // textBoxPersonSNP
            // 
            this.textBoxPersonSNP.Enabled = false;
            this.textBoxPersonSNP.Location = new System.Drawing.Point(42, 107);
            this.textBoxPersonSNP.Name = "textBoxPersonSNP";
            this.textBoxPersonSNP.Size = new System.Drawing.Size(437, 21);
            this.textBoxPersonSNP.TabIndex = 6;
            this.textBoxPersonSNP.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxPersonSNPEnable
            // 
            this.checkBoxPersonSNPEnable.AutoSize = true;
            this.checkBoxPersonSNPEnable.Location = new System.Drawing.Point(17, 110);
            this.checkBoxPersonSNPEnable.Name = "checkBoxPersonSNPEnable";
            this.checkBoxPersonSNPEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPersonSNPEnable.TabIndex = 5;
            this.checkBoxPersonSNPEnable.UseVisualStyleBackColor = true;
            this.checkBoxPersonSNPEnable.CheckedChanged += new System.EventHandler(this.checkBoxPersonSNPEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 15);
            this.label5.TabIndex = 52;
            this.label5.Text = "ФИО участника переселения";
            // 
            // numericUpDownIDResettle
            // 
            this.numericUpDownIDResettle.Enabled = false;
            this.numericUpDownIDResettle.Location = new System.Drawing.Point(42, 25);
            this.numericUpDownIDResettle.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownIDResettle.Name = "numericUpDownIDResettle";
            this.numericUpDownIDResettle.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownIDResettle.TabIndex = 1;
            this.numericUpDownIDResettle.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxIDResettleEnable
            // 
            this.checkBoxIDResettleEnable.AutoSize = true;
            this.checkBoxIDResettleEnable.Location = new System.Drawing.Point(17, 28);
            this.checkBoxIDResettleEnable.Name = "checkBoxIDResettleEnable";
            this.checkBoxIDResettleEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIDResettleEnable.TabIndex = 0;
            this.checkBoxIDResettleEnable.UseVisualStyleBackColor = true;
            this.checkBoxIDResettleEnable.CheckedChanged += new System.EventHandler(this.checkBoxIDTenancyEnable_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(252, 15);
            this.label9.TabIndex = 56;
            this.label9.Text = "Внутренний номер процесса переселения";
            // 
            // checkBoxResettleDateEnable
            // 
            this.checkBoxResettleDateEnable.AutoSize = true;
            this.checkBoxResettleDateEnable.Location = new System.Drawing.Point(17, 68);
            this.checkBoxResettleDateEnable.Name = "checkBoxResettleDateEnable";
            this.checkBoxResettleDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxResettleDateEnable.TabIndex = 2;
            this.checkBoxResettleDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxResettleDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegDateEnable_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(201, 15);
            this.label12.TabIndex = 74;
            this.label12.Text = "Дата планируемого переселения";
            // 
            // dateTimePickerResettleDate
            // 
            this.dateTimePickerResettleDate.Enabled = false;
            this.dateTimePickerResettleDate.Location = new System.Drawing.Point(98, 65);
            this.dateTimePickerResettleDate.Name = "dateTimePickerResettleDate";
            this.dateTimePickerResettleDate.Size = new System.Drawing.Size(381, 21);
            this.dateTimePickerResettleDate.TabIndex = 4;
            // 
            // comboBoxResettleDateExpr
            // 
            this.comboBoxResettleDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResettleDateExpr.Enabled = false;
            this.comboBoxResettleDateExpr.FormattingEnabled = true;
            this.comboBoxResettleDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxResettleDateExpr.Location = new System.Drawing.Point(42, 65);
            this.comboBoxResettleDateExpr.Name = "comboBoxResettleDateExpr";
            this.comboBoxResettleDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxResettleDateExpr.TabIndex = 3;
            // 
            // comboBoxRegionTo
            // 
            this.comboBoxRegionTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegionTo.Enabled = false;
            this.comboBoxRegionTo.FormattingEnabled = true;
            this.comboBoxRegionTo.Location = new System.Drawing.Point(42, 316);
            this.comboBoxRegionTo.Name = "comboBoxRegionTo";
            this.comboBoxRegionTo.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRegionTo.TabIndex = 16;
            // 
            // checkBoxRegionToEnable
            // 
            this.checkBoxRegionToEnable.AutoSize = true;
            this.checkBoxRegionToEnable.Location = new System.Drawing.Point(17, 320);
            this.checkBoxRegionToEnable.Name = "checkBoxRegionToEnable";
            this.checkBoxRegionToEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegionToEnable.TabIndex = 15;
            this.checkBoxRegionToEnable.UseVisualStyleBackColor = true;
            this.checkBoxRegionToEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegionToEnable_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 299);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 15);
            this.label1.TabIndex = 86;
            this.label1.Text = "Жилой район (куда переселяются)";
            // 
            // textBoxHouseTo
            // 
            this.textBoxHouseTo.Enabled = false;
            this.textBoxHouseTo.Location = new System.Drawing.Point(42, 402);
            this.textBoxHouseTo.Name = "textBoxHouseTo";
            this.textBoxHouseTo.Size = new System.Drawing.Size(437, 21);
            this.textBoxHouseTo.TabIndex = 20;
            this.textBoxHouseTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxPremisesNumToEnable
            // 
            this.checkBoxPremisesNumToEnable.AutoSize = true;
            this.checkBoxPremisesNumToEnable.Location = new System.Drawing.Point(17, 446);
            this.checkBoxPremisesNumToEnable.Name = "checkBoxPremisesNumToEnable";
            this.checkBoxPremisesNumToEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPremisesNumToEnable.TabIndex = 21;
            this.checkBoxPremisesNumToEnable.UseVisualStyleBackColor = true;
            this.checkBoxPremisesNumToEnable.CheckedChanged += new System.EventHandler(this.checkBoxPremisesNumToEnable_CheckedChanged);
            // 
            // textBoxPremisesNumTo
            // 
            this.textBoxPremisesNumTo.Enabled = false;
            this.textBoxPremisesNumTo.Location = new System.Drawing.Point(42, 443);
            this.textBoxPremisesNumTo.MaxLength = 4;
            this.textBoxPremisesNumTo.Name = "textBoxPremisesNumTo";
            this.textBoxPremisesNumTo.Size = new System.Drawing.Size(437, 21);
            this.textBoxPremisesNumTo.TabIndex = 22;
            this.textBoxPremisesNumTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 426);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(239, 15);
            this.label2.TabIndex = 85;
            this.label2.Text = "Номер помещения (куда переселяются)";
            // 
            // checkBoxHouseToEnable
            // 
            this.checkBoxHouseToEnable.AutoSize = true;
            this.checkBoxHouseToEnable.Location = new System.Drawing.Point(17, 405);
            this.checkBoxHouseToEnable.Name = "checkBoxHouseToEnable";
            this.checkBoxHouseToEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseToEnable.TabIndex = 19;
            this.checkBoxHouseToEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseToEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseToEnable_CheckedChanged);
            // 
            // checkBoxStreetToEnable
            // 
            this.checkBoxStreetToEnable.AutoSize = true;
            this.checkBoxStreetToEnable.Location = new System.Drawing.Point(17, 363);
            this.checkBoxStreetToEnable.Name = "checkBoxStreetToEnable";
            this.checkBoxStreetToEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetToEnable.TabIndex = 17;
            this.checkBoxStreetToEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetToEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetToEnable_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 385);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(202, 15);
            this.label3.TabIndex = 84;
            this.label3.Text = "Номер дома (куда переселяются)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 342);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 15);
            this.label4.TabIndex = 83;
            this.label4.Text = "Улица (куда переселяются)";
            // 
            // comboBoxStreetTo
            // 
            this.comboBoxStreetTo.Enabled = false;
            this.comboBoxStreetTo.FormattingEnabled = true;
            this.comboBoxStreetTo.Location = new System.Drawing.Point(42, 359);
            this.comboBoxStreetTo.Name = "comboBoxStreetTo";
            this.comboBoxStreetTo.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStreetTo.TabIndex = 18;
            this.comboBoxStreetTo.DropDownClosed += new System.EventHandler(this.comboBoxStreetTo_DropDownClosed);
            this.comboBoxStreetTo.Enter += new System.EventHandler(this.selectAll_Enter);
            this.comboBoxStreetTo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreetTo_KeyUp);
            this.comboBoxStreetTo.Leave += new System.EventHandler(this.comboBoxStreetTo_Leave);
            // 
            // comboBoxStreetFrom
            // 
            this.comboBoxStreetFrom.Enabled = false;
            this.comboBoxStreetFrom.FormattingEnabled = true;
            this.comboBoxStreetFrom.Location = new System.Drawing.Point(42, 191);
            this.comboBoxStreetFrom.Name = "comboBoxStreetFrom";
            this.comboBoxStreetFrom.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStreetFrom.TabIndex = 10;
            this.comboBoxStreetFrom.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreetFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            this.comboBoxStreetFrom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreetFrom.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 174);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(180, 15);
            this.label11.TabIndex = 65;
            this.label11.Text = "Улица (откуда переселяются)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 217);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(216, 15);
            this.label10.TabIndex = 66;
            this.label10.Text = "Номер дома (откуда переселяются)";
            // 
            // checkBoxStreetFromEnable
            // 
            this.checkBoxStreetFromEnable.AutoSize = true;
            this.checkBoxStreetFromEnable.Location = new System.Drawing.Point(17, 195);
            this.checkBoxStreetFromEnable.Name = "checkBoxStreetFromEnable";
            this.checkBoxStreetFromEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetFromEnable.TabIndex = 9;
            this.checkBoxStreetFromEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetFromEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // checkBoxHouseFromEnable
            // 
            this.checkBoxHouseFromEnable.AutoSize = true;
            this.checkBoxHouseFromEnable.Location = new System.Drawing.Point(17, 237);
            this.checkBoxHouseFromEnable.Name = "checkBoxHouseFromEnable";
            this.checkBoxHouseFromEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseFromEnable.TabIndex = 11;
            this.checkBoxHouseFromEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseFromEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(253, 15);
            this.label7.TabIndex = 67;
            this.label7.Text = "Номер помещения (откуда переселяются)";
            // 
            // textBoxPremisesNumFrom
            // 
            this.textBoxPremisesNumFrom.Enabled = false;
            this.textBoxPremisesNumFrom.Location = new System.Drawing.Point(42, 275);
            this.textBoxPremisesNumFrom.MaxLength = 4;
            this.textBoxPremisesNumFrom.Name = "textBoxPremisesNumFrom";
            this.textBoxPremisesNumFrom.Size = new System.Drawing.Size(437, 21);
            this.textBoxPremisesNumFrom.TabIndex = 14;
            this.textBoxPremisesNumFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxPremisesNumFromEnable
            // 
            this.checkBoxPremisesNumFromEnable.AutoSize = true;
            this.checkBoxPremisesNumFromEnable.Location = new System.Drawing.Point(17, 278);
            this.checkBoxPremisesNumFromEnable.Name = "checkBoxPremisesNumFromEnable";
            this.checkBoxPremisesNumFromEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPremisesNumFromEnable.TabIndex = 13;
            this.checkBoxPremisesNumFromEnable.UseVisualStyleBackColor = true;
            this.checkBoxPremisesNumFromEnable.CheckedChanged += new System.EventHandler(this.checkBoxPremisesNumEnable_CheckedChanged);
            // 
            // textBoxHouseFrom
            // 
            this.textBoxHouseFrom.Enabled = false;
            this.textBoxHouseFrom.Location = new System.Drawing.Point(42, 234);
            this.textBoxHouseFrom.Name = "textBoxHouseFrom";
            this.textBoxHouseFrom.Size = new System.Drawing.Size(437, 21);
            this.textBoxHouseFrom.TabIndex = 12;
            this.textBoxHouseFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(221, 15);
            this.label8.TabIndex = 71;
            this.label8.Text = "Жилой район (откуда переселяются)";
            // 
            // checkBoxRegionFromEnable
            // 
            this.checkBoxRegionFromEnable.AutoSize = true;
            this.checkBoxRegionFromEnable.Location = new System.Drawing.Point(17, 152);
            this.checkBoxRegionFromEnable.Name = "checkBoxRegionFromEnable";
            this.checkBoxRegionFromEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegionFromEnable.TabIndex = 7;
            this.checkBoxRegionFromEnable.UseVisualStyleBackColor = true;
            this.checkBoxRegionFromEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegionEnable_CheckedChanged);
            // 
            // comboBoxRegionFrom
            // 
            this.comboBoxRegionFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegionFrom.Enabled = false;
            this.comboBoxRegionFrom.FormattingEnabled = true;
            this.comboBoxRegionFrom.Location = new System.Drawing.Point(42, 148);
            this.comboBoxRegionFrom.Name = "comboBoxRegionFrom";
            this.comboBoxRegionFrom.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRegionFrom.TabIndex = 8;
            // 
            // ExtendedSearchResettleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(493, 520);
            this.Controls.Add(this.comboBoxRegionTo);
            this.Controls.Add(this.checkBoxRegionToEnable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxHouseTo);
            this.Controls.Add(this.checkBoxPremisesNumToEnable);
            this.Controls.Add(this.textBoxPremisesNumTo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxHouseToEnable);
            this.Controls.Add(this.checkBoxStreetToEnable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxStreetTo);
            this.Controls.Add(this.comboBoxResettleDateExpr);
            this.Controls.Add(this.dateTimePickerResettleDate);
            this.Controls.Add(this.checkBoxResettleDateEnable);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.comboBoxRegionFrom);
            this.Controls.Add(this.checkBoxRegionFromEnable);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxHouseFrom);
            this.Controls.Add(this.checkBoxPremisesNumFromEnable);
            this.Controls.Add(this.textBoxPremisesNumFrom);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxHouseFromEnable);
            this.Controls.Add(this.checkBoxStreetFromEnable);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboBoxStreetFrom);
            this.Controls.Add(this.numericUpDownIDResettle);
            this.Controls.Add(this.checkBoxIDResettleEnable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxPersonSNP);
            this.Controls.Add(this.checkBoxPersonSNPEnable);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendedSearchResettleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация процессов переселения";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDResettle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vButton vButtonCancel;
        private vButton vButtonSearch;
        private TextBox textBoxPersonSNP;
        private CheckBox checkBoxPersonSNPEnable;
        private Label label5;
        private NumericUpDown numericUpDownIDResettle;
        private CheckBox checkBoxIDResettleEnable;
        private Label label9;
        private CheckBox checkBoxResettleDateEnable;
        private Label label12;
        private DateTimePicker dateTimePickerResettleDate;
        private ComboBox comboBoxResettleDateExpr;
        private ComboBox comboBoxRegionTo;
        private CheckBox checkBoxRegionToEnable;
        private Label label1;
        private TextBox textBoxHouseTo;
        private CheckBox checkBoxPremisesNumToEnable;
        private TextBox textBoxPremisesNumTo;
        private Label label2;
        private CheckBox checkBoxHouseToEnable;
        private CheckBox checkBoxStreetToEnable;
        private Label label3;
        private Label label4;
        private ComboBox comboBoxStreetTo;
        private ComboBox comboBoxStreetFrom;
        private Label label11;
        private Label label10;
        private CheckBox checkBoxStreetFromEnable;
        private CheckBox checkBoxHouseFromEnable;
        private Label label7;
        private TextBox textBoxPremisesNumFrom;
        private CheckBox checkBoxPremisesNumFromEnable;
        private TextBox textBoxHouseFrom;
        private Label label8;
        private CheckBox checkBoxRegionFromEnable;
        private ComboBox comboBoxRegionFrom;
    }
}