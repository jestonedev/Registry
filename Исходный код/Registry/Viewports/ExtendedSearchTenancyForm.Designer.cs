namespace Registry.SearchForms
{
    partial class ExtendedSearchTenancyForm
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
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.textBoxRegistrationNum = new System.Windows.Forms.TextBox();
            this.checkBoxContractNumEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxResidenceWarrantNum = new System.Windows.Forms.TextBox();
            this.checkBoxResidenceWarrantNumEnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxKumiOrderNum = new System.Windows.Forms.TextBox();
            this.checkBoxKumiOrderEnable = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTenantSNP = new System.Windows.Forms.TextBox();
            this.checkBoxTenantSNPEnable = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPersonSNP = new System.Windows.Forms.TextBox();
            this.checkBoxPersonSNPEnable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownIDTenancy = new System.Windows.Forms.NumericUpDown();
            this.checkBoxIDTenancyEnable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxRentType = new System.Windows.Forms.ComboBox();
            this.checkBoxRentTypeEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxRegion = new System.Windows.Forms.ComboBox();
            this.checkBoxRegionEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.checkBoxPremisesNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxPremisesNum = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxHouseEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxStreetEnable = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.checkBoxRegDateEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.dateTimePickerRegDate = new System.Windows.Forms.DateTimePicker();
            this.comboBoxRegDateExpr = new System.Windows.Forms.ComboBox();
            this.comboBoxIssueDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerIssueDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxIssueDateEnable = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxBeginDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerBeginDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxBeginDateEnable = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBoxEndDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerEndDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxEndDateEnable = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBoxResidenceWarrDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerResidenceWarrDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxResidenceWarrDateEnable = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxKumiOrderDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerKumiOrderDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxKumiOrderDateEnable = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDTenancy)).BeginInit();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(630, 427);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 41;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(630, 386);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 40;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // textBoxRegistrationNum
            // 
            this.textBoxRegistrationNum.Enabled = false;
            this.textBoxRegistrationNum.Location = new System.Drawing.Point(42, 66);
            this.textBoxRegistrationNum.Name = "textBoxRegistrationNum";
            this.textBoxRegistrationNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxRegistrationNum.TabIndex = 3;
            // 
            // checkBoxContractNumEnable
            // 
            this.checkBoxContractNumEnable.AutoSize = true;
            this.checkBoxContractNumEnable.Location = new System.Drawing.Point(17, 69);
            this.checkBoxContractNumEnable.Name = "checkBoxContractNumEnable";
            this.checkBoxContractNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxContractNumEnable.TabIndex = 2;
            this.checkBoxContractNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxContractNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxContractNumEnable_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 15);
            this.label2.TabIndex = 40;
            this.label2.Text = "Номер договора";
            // 
            // textBoxResidenceWarrantNum
            // 
            this.textBoxResidenceWarrantNum.Enabled = false;
            this.textBoxResidenceWarrantNum.Location = new System.Drawing.Point(42, 107);
            this.textBoxResidenceWarrantNum.Name = "textBoxResidenceWarrantNum";
            this.textBoxResidenceWarrantNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxResidenceWarrantNum.TabIndex = 5;
            // 
            // checkBoxResidenceWarrantNumEnable
            // 
            this.checkBoxResidenceWarrantNumEnable.AutoSize = true;
            this.checkBoxResidenceWarrantNumEnable.Location = new System.Drawing.Point(17, 110);
            this.checkBoxResidenceWarrantNumEnable.Name = "checkBoxResidenceWarrantNumEnable";
            this.checkBoxResidenceWarrantNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxResidenceWarrantNumEnable.TabIndex = 4;
            this.checkBoxResidenceWarrantNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxResidenceWarrantNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxResidenceWarrantNumEnable_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 15);
            this.label1.TabIndex = 43;
            this.label1.Text = "Номер ордера на проживание";
            // 
            // textBoxKumiOrderNum
            // 
            this.textBoxKumiOrderNum.Enabled = false;
            this.textBoxKumiOrderNum.Location = new System.Drawing.Point(42, 148);
            this.textBoxKumiOrderNum.Name = "textBoxKumiOrderNum";
            this.textBoxKumiOrderNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxKumiOrderNum.TabIndex = 7;
            // 
            // checkBoxKumiOrderEnable
            // 
            this.checkBoxKumiOrderEnable.AutoSize = true;
            this.checkBoxKumiOrderEnable.Location = new System.Drawing.Point(17, 151);
            this.checkBoxKumiOrderEnable.Name = "checkBoxKumiOrderEnable";
            this.checkBoxKumiOrderEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxKumiOrderEnable.TabIndex = 6;
            this.checkBoxKumiOrderEnable.UseVisualStyleBackColor = true;
            this.checkBoxKumiOrderEnable.CheckedChanged += new System.EventHandler(this.checkBoxKumiOrderEnable_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 15);
            this.label3.TabIndex = 46;
            this.label3.Text = "Номер распоряжения КУМИ";
            // 
            // textBoxTenantSNP
            // 
            this.textBoxTenantSNP.Enabled = false;
            this.textBoxTenantSNP.Location = new System.Drawing.Point(42, 189);
            this.textBoxTenantSNP.Name = "textBoxTenantSNP";
            this.textBoxTenantSNP.Size = new System.Drawing.Size(437, 21);
            this.textBoxTenantSNP.TabIndex = 9;
            // 
            // checkBoxTenantSNPEnable
            // 
            this.checkBoxTenantSNPEnable.AutoSize = true;
            this.checkBoxTenantSNPEnable.Location = new System.Drawing.Point(17, 192);
            this.checkBoxTenantSNPEnable.Name = "checkBoxTenantSNPEnable";
            this.checkBoxTenantSNPEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTenantSNPEnable.TabIndex = 8;
            this.checkBoxTenantSNPEnable.UseVisualStyleBackColor = true;
            this.checkBoxTenantSNPEnable.CheckedChanged += new System.EventHandler(this.checkBoxTenantSNPEnable_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 15);
            this.label4.TabIndex = 49;
            this.label4.Text = "ФИО нанимателя";
            // 
            // textBoxPersonSNP
            // 
            this.textBoxPersonSNP.Enabled = false;
            this.textBoxPersonSNP.Location = new System.Drawing.Point(42, 230);
            this.textBoxPersonSNP.Name = "textBoxPersonSNP";
            this.textBoxPersonSNP.Size = new System.Drawing.Size(437, 21);
            this.textBoxPersonSNP.TabIndex = 11;
            // 
            // checkBoxPersonSNPEnable
            // 
            this.checkBoxPersonSNPEnable.AutoSize = true;
            this.checkBoxPersonSNPEnable.Location = new System.Drawing.Point(17, 233);
            this.checkBoxPersonSNPEnable.Name = "checkBoxPersonSNPEnable";
            this.checkBoxPersonSNPEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPersonSNPEnable.TabIndex = 10;
            this.checkBoxPersonSNPEnable.UseVisualStyleBackColor = true;
            this.checkBoxPersonSNPEnable.CheckedChanged += new System.EventHandler(this.checkBoxPersonSNPEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 213);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(137, 15);
            this.label5.TabIndex = 52;
            this.label5.Text = "ФИО участника найма";
            // 
            // numericUpDownIDTenancy
            // 
            this.numericUpDownIDTenancy.Enabled = false;
            this.numericUpDownIDTenancy.Location = new System.Drawing.Point(42, 25);
            this.numericUpDownIDTenancy.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownIDTenancy.Name = "numericUpDownIDTenancy";
            this.numericUpDownIDTenancy.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownIDTenancy.TabIndex = 1;
            // 
            // checkBoxIDTenancyEnable
            // 
            this.checkBoxIDTenancyEnable.AutoSize = true;
            this.checkBoxIDTenancyEnable.Location = new System.Drawing.Point(17, 28);
            this.checkBoxIDTenancyEnable.Name = "checkBoxIDTenancyEnable";
            this.checkBoxIDTenancyEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIDTenancyEnable.TabIndex = 0;
            this.checkBoxIDTenancyEnable.UseVisualStyleBackColor = true;
            this.checkBoxIDTenancyEnable.CheckedChanged += new System.EventHandler(this.checkBoxIDTenancyEnable_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(213, 15);
            this.label9.TabIndex = 56;
            this.label9.Text = "Внутренний номер процесса найма";
            // 
            // comboBoxRentType
            // 
            this.comboBoxRentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRentType.Enabled = false;
            this.comboBoxRentType.FormattingEnabled = true;
            this.comboBoxRentType.Location = new System.Drawing.Point(42, 271);
            this.comboBoxRentType.Name = "comboBoxRentType";
            this.comboBoxRentType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRentType.TabIndex = 13;
            // 
            // checkBoxRentTypeEnable
            // 
            this.checkBoxRentTypeEnable.AutoSize = true;
            this.checkBoxRentTypeEnable.Location = new System.Drawing.Point(17, 275);
            this.checkBoxRentTypeEnable.Name = "checkBoxRentTypeEnable";
            this.checkBoxRentTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRentTypeEnable.TabIndex = 12;
            this.checkBoxRentTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxRentTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxRentTypeEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 254);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 15);
            this.label6.TabIndex = 59;
            this.label6.Text = "Вид найма";
            // 
            // comboBoxRegion
            // 
            this.comboBoxRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegion.Enabled = false;
            this.comboBoxRegion.FormattingEnabled = true;
            this.comboBoxRegion.Location = new System.Drawing.Point(42, 314);
            this.comboBoxRegion.Name = "comboBoxRegion";
            this.comboBoxRegion.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRegion.TabIndex = 15;
            // 
            // checkBoxRegionEnable
            // 
            this.checkBoxRegionEnable.AutoSize = true;
            this.checkBoxRegionEnable.Location = new System.Drawing.Point(17, 318);
            this.checkBoxRegionEnable.Name = "checkBoxRegionEnable";
            this.checkBoxRegionEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegionEnable.TabIndex = 14;
            this.checkBoxRegionEnable.UseVisualStyleBackColor = true;
            this.checkBoxRegionEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegionEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 297);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 15);
            this.label8.TabIndex = 71;
            this.label8.Text = "Жилой район";
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Enabled = false;
            this.textBoxHouse.Location = new System.Drawing.Point(42, 400);
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(437, 21);
            this.textBoxHouse.TabIndex = 19;
            // 
            // checkBoxPremisesNumEnable
            // 
            this.checkBoxPremisesNumEnable.AutoSize = true;
            this.checkBoxPremisesNumEnable.Location = new System.Drawing.Point(17, 444);
            this.checkBoxPremisesNumEnable.Name = "checkBoxPremisesNumEnable";
            this.checkBoxPremisesNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPremisesNumEnable.TabIndex = 20;
            this.checkBoxPremisesNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxPremisesNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxPremisesNumEnable_CheckedChanged);
            // 
            // textBoxPremisesNum
            // 
            this.textBoxPremisesNum.Enabled = false;
            this.textBoxPremisesNum.Location = new System.Drawing.Point(42, 441);
            this.textBoxPremisesNum.MaxLength = 4;
            this.textBoxPremisesNum.Name = "textBoxPremisesNum";
            this.textBoxPremisesNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxPremisesNum.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 424);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 15);
            this.label7.TabIndex = 67;
            this.label7.Text = "Номер помещения";
            // 
            // checkBoxHouseEnable
            // 
            this.checkBoxHouseEnable.AutoSize = true;
            this.checkBoxHouseEnable.Location = new System.Drawing.Point(17, 403);
            this.checkBoxHouseEnable.Name = "checkBoxHouseEnable";
            this.checkBoxHouseEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseEnable.TabIndex = 18;
            this.checkBoxHouseEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // checkBoxStreetEnable
            // 
            this.checkBoxStreetEnable.AutoSize = true;
            this.checkBoxStreetEnable.Location = new System.Drawing.Point(17, 361);
            this.checkBoxStreetEnable.Name = "checkBoxStreetEnable";
            this.checkBoxStreetEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetEnable.TabIndex = 16;
            this.checkBoxStreetEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 383);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 15);
            this.label10.TabIndex = 66;
            this.label10.Text = "Номер дома";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 340);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 15);
            this.label11.TabIndex = 65;
            this.label11.Text = "Улица";
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Enabled = false;
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(42, 357);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStreet.TabIndex = 17;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // checkBoxRegDateEnable
            // 
            this.checkBoxRegDateEnable.AutoSize = true;
            this.checkBoxRegDateEnable.Location = new System.Drawing.Point(507, 28);
            this.checkBoxRegDateEnable.Name = "checkBoxRegDateEnable";
            this.checkBoxRegDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegDateEnable.TabIndex = 22;
            this.checkBoxRegDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxRegDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegDateEnable_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(500, 8);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(171, 15);
            this.label12.TabIndex = 74;
            this.label12.Text = "Дата регистрации договора";
            // 
            // dateTimePickerRegDate
            // 
            this.dateTimePickerRegDate.Enabled = false;
            this.dateTimePickerRegDate.Location = new System.Drawing.Point(588, 25);
            this.dateTimePickerRegDate.Name = "dateTimePickerRegDate";
            this.dateTimePickerRegDate.Size = new System.Drawing.Size(159, 21);
            this.dateTimePickerRegDate.TabIndex = 24;
            // 
            // comboBoxRegDateExpr
            // 
            this.comboBoxRegDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegDateExpr.Enabled = false;
            this.comboBoxRegDateExpr.FormattingEnabled = true;
            this.comboBoxRegDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxRegDateExpr.Location = new System.Drawing.Point(532, 25);
            this.comboBoxRegDateExpr.Name = "comboBoxRegDateExpr";
            this.comboBoxRegDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxRegDateExpr.TabIndex = 23;
            // 
            // comboBoxIssueDateExpr
            // 
            this.comboBoxIssueDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIssueDateExpr.Enabled = false;
            this.comboBoxIssueDateExpr.FormattingEnabled = true;
            this.comboBoxIssueDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxIssueDateExpr.Location = new System.Drawing.Point(532, 66);
            this.comboBoxIssueDateExpr.Name = "comboBoxIssueDateExpr";
            this.comboBoxIssueDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxIssueDateExpr.TabIndex = 26;
            // 
            // dateTimePickerIssueDate
            // 
            this.dateTimePickerIssueDate.Enabled = false;
            this.dateTimePickerIssueDate.Location = new System.Drawing.Point(588, 66);
            this.dateTimePickerIssueDate.Name = "dateTimePickerIssueDate";
            this.dateTimePickerIssueDate.Size = new System.Drawing.Size(159, 21);
            this.dateTimePickerIssueDate.TabIndex = 27;
            // 
            // checkBoxIssueDateEnable
            // 
            this.checkBoxIssueDateEnable.AutoSize = true;
            this.checkBoxIssueDateEnable.Location = new System.Drawing.Point(507, 70);
            this.checkBoxIssueDateEnable.Name = "checkBoxIssueDateEnable";
            this.checkBoxIssueDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIssueDateEnable.TabIndex = 25;
            this.checkBoxIssueDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxIssueDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxIssueDateEnable_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(500, 49);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(140, 15);
            this.label13.TabIndex = 78;
            this.label13.Text = "Дата выдачи договора";
            // 
            // comboBoxBeginDateExpr
            // 
            this.comboBoxBeginDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBeginDateExpr.Enabled = false;
            this.comboBoxBeginDateExpr.FormattingEnabled = true;
            this.comboBoxBeginDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxBeginDateExpr.Location = new System.Drawing.Point(532, 107);
            this.comboBoxBeginDateExpr.Name = "comboBoxBeginDateExpr";
            this.comboBoxBeginDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxBeginDateExpr.TabIndex = 29;
            // 
            // dateTimePickerBeginDate
            // 
            this.dateTimePickerBeginDate.Enabled = false;
            this.dateTimePickerBeginDate.Location = new System.Drawing.Point(588, 107);
            this.dateTimePickerBeginDate.Name = "dateTimePickerBeginDate";
            this.dateTimePickerBeginDate.Size = new System.Drawing.Size(159, 21);
            this.dateTimePickerBeginDate.TabIndex = 30;
            // 
            // checkBoxBeginDateEnable
            // 
            this.checkBoxBeginDateEnable.AutoSize = true;
            this.checkBoxBeginDateEnable.Location = new System.Drawing.Point(507, 111);
            this.checkBoxBeginDateEnable.Name = "checkBoxBeginDateEnable";
            this.checkBoxBeginDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBeginDateEnable.TabIndex = 28;
            this.checkBoxBeginDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxBeginDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxBeginDateEnable_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(500, 90);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(196, 15);
            this.label14.TabIndex = 82;
            this.label14.Text = "Дата начала действия договора";
            // 
            // comboBoxEndDateExpr
            // 
            this.comboBoxEndDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEndDateExpr.Enabled = false;
            this.comboBoxEndDateExpr.FormattingEnabled = true;
            this.comboBoxEndDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxEndDateExpr.Location = new System.Drawing.Point(532, 148);
            this.comboBoxEndDateExpr.Name = "comboBoxEndDateExpr";
            this.comboBoxEndDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxEndDateExpr.TabIndex = 32;
            // 
            // dateTimePickerEndDate
            // 
            this.dateTimePickerEndDate.Enabled = false;
            this.dateTimePickerEndDate.Location = new System.Drawing.Point(588, 148);
            this.dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            this.dateTimePickerEndDate.Size = new System.Drawing.Size(159, 21);
            this.dateTimePickerEndDate.TabIndex = 33;
            // 
            // checkBoxEndDateEnable
            // 
            this.checkBoxEndDateEnable.AutoSize = true;
            this.checkBoxEndDateEnable.Location = new System.Drawing.Point(507, 152);
            this.checkBoxEndDateEnable.Name = "checkBoxEndDateEnable";
            this.checkBoxEndDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxEndDateEnable.TabIndex = 31;
            this.checkBoxEndDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxEndDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxEndDateEnable_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(500, 131);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(158, 15);
            this.label15.TabIndex = 86;
            this.label15.Text = "Дата окончания договора";
            // 
            // comboBoxResidenceWarrDateExpr
            // 
            this.comboBoxResidenceWarrDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResidenceWarrDateExpr.Enabled = false;
            this.comboBoxResidenceWarrDateExpr.FormattingEnabled = true;
            this.comboBoxResidenceWarrDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxResidenceWarrDateExpr.Location = new System.Drawing.Point(532, 189);
            this.comboBoxResidenceWarrDateExpr.Name = "comboBoxResidenceWarrDateExpr";
            this.comboBoxResidenceWarrDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxResidenceWarrDateExpr.TabIndex = 35;
            // 
            // dateTimePickerResidenceWarrDate
            // 
            this.dateTimePickerResidenceWarrDate.Enabled = false;
            this.dateTimePickerResidenceWarrDate.Location = new System.Drawing.Point(588, 189);
            this.dateTimePickerResidenceWarrDate.Name = "dateTimePickerResidenceWarrDate";
            this.dateTimePickerResidenceWarrDate.Size = new System.Drawing.Size(159, 21);
            this.dateTimePickerResidenceWarrDate.TabIndex = 36;
            // 
            // checkBoxResidenceWarrDateEnable
            // 
            this.checkBoxResidenceWarrDateEnable.AutoSize = true;
            this.checkBoxResidenceWarrDateEnable.Location = new System.Drawing.Point(507, 193);
            this.checkBoxResidenceWarrDateEnable.Name = "checkBoxResidenceWarrDateEnable";
            this.checkBoxResidenceWarrDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxResidenceWarrDateEnable.TabIndex = 34;
            this.checkBoxResidenceWarrDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxResidenceWarrDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxResidenceWarrDateEnable_CheckedChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(500, 172);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(174, 15);
            this.label16.TabIndex = 90;
            this.label16.Text = "Дата ордера на проживание";
            // 
            // comboBoxKumiOrderDateExpr
            // 
            this.comboBoxKumiOrderDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKumiOrderDateExpr.Enabled = false;
            this.comboBoxKumiOrderDateExpr.FormattingEnabled = true;
            this.comboBoxKumiOrderDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxKumiOrderDateExpr.Location = new System.Drawing.Point(532, 230);
            this.comboBoxKumiOrderDateExpr.Name = "comboBoxKumiOrderDateExpr";
            this.comboBoxKumiOrderDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxKumiOrderDateExpr.TabIndex = 38;
            // 
            // dateTimePickerKumiOrderDate
            // 
            this.dateTimePickerKumiOrderDate.Enabled = false;
            this.dateTimePickerKumiOrderDate.Location = new System.Drawing.Point(588, 230);
            this.dateTimePickerKumiOrderDate.Name = "dateTimePickerKumiOrderDate";
            this.dateTimePickerKumiOrderDate.Size = new System.Drawing.Size(159, 21);
            this.dateTimePickerKumiOrderDate.TabIndex = 39;
            // 
            // checkBoxKumiOrderDateEnable
            // 
            this.checkBoxKumiOrderDateEnable.AutoSize = true;
            this.checkBoxKumiOrderDateEnable.Location = new System.Drawing.Point(507, 234);
            this.checkBoxKumiOrderDateEnable.Name = "checkBoxKumiOrderDateEnable";
            this.checkBoxKumiOrderDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxKumiOrderDateEnable.TabIndex = 37;
            this.checkBoxKumiOrderDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxKumiOrderDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxKumiOrderDateEnable_CheckedChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(500, 213);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(169, 15);
            this.label17.TabIndex = 94;
            this.label17.Text = "Дата постановления КУМИ";
            // 
            // ExtendedSearchTenancyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(758, 470);
            this.Controls.Add(this.comboBoxKumiOrderDateExpr);
            this.Controls.Add(this.dateTimePickerKumiOrderDate);
            this.Controls.Add(this.checkBoxKumiOrderDateEnable);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.comboBoxResidenceWarrDateExpr);
            this.Controls.Add(this.dateTimePickerResidenceWarrDate);
            this.Controls.Add(this.checkBoxResidenceWarrDateEnable);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.comboBoxEndDateExpr);
            this.Controls.Add(this.dateTimePickerEndDate);
            this.Controls.Add(this.checkBoxEndDateEnable);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.comboBoxBeginDateExpr);
            this.Controls.Add(this.dateTimePickerBeginDate);
            this.Controls.Add(this.checkBoxBeginDateEnable);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.comboBoxIssueDateExpr);
            this.Controls.Add(this.dateTimePickerIssueDate);
            this.Controls.Add(this.checkBoxIssueDateEnable);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.comboBoxRegDateExpr);
            this.Controls.Add(this.dateTimePickerRegDate);
            this.Controls.Add(this.checkBoxRegDateEnable);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.comboBoxRegion);
            this.Controls.Add(this.checkBoxRegionEnable);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxHouse);
            this.Controls.Add(this.checkBoxPremisesNumEnable);
            this.Controls.Add(this.textBoxPremisesNum);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxHouseEnable);
            this.Controls.Add(this.checkBoxStreetEnable);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboBoxStreet);
            this.Controls.Add(this.comboBoxRentType);
            this.Controls.Add(this.checkBoxRentTypeEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDownIDTenancy);
            this.Controls.Add(this.checkBoxIDTenancyEnable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxPersonSNP);
            this.Controls.Add(this.checkBoxPersonSNPEnable);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxTenantSNP);
            this.Controls.Add(this.checkBoxTenantSNPEnable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxKumiOrderNum);
            this.Controls.Add(this.checkBoxKumiOrderEnable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxResidenceWarrantNum);
            this.Controls.Add(this.checkBoxResidenceWarrantNumEnable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRegistrationNum);
            this.Controls.Add(this.checkBoxContractNumEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendedSearchTenancyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация процессов найма";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDTenancy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private VIBlend.WinForms.Controls.vButton vButtonSearch;
        private System.Windows.Forms.TextBox textBoxRegistrationNum;
        private System.Windows.Forms.CheckBox checkBoxContractNumEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxResidenceWarrantNum;
        private System.Windows.Forms.CheckBox checkBoxResidenceWarrantNumEnable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxKumiOrderNum;
        private System.Windows.Forms.CheckBox checkBoxKumiOrderEnable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTenantSNP;
        private System.Windows.Forms.CheckBox checkBoxTenantSNPEnable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPersonSNP;
        private System.Windows.Forms.CheckBox checkBoxPersonSNPEnable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownIDTenancy;
        private System.Windows.Forms.CheckBox checkBoxIDTenancyEnable;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxRentType;
        private System.Windows.Forms.CheckBox checkBoxRentTypeEnable;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxRegion;
        private System.Windows.Forms.CheckBox checkBoxRegionEnable;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxHouse;
        private System.Windows.Forms.CheckBox checkBoxPremisesNumEnable;
        private System.Windows.Forms.TextBox textBoxPremisesNum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxHouseEnable;
        private System.Windows.Forms.CheckBox checkBoxStreetEnable;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBoxStreet;
        private System.Windows.Forms.CheckBox checkBoxRegDateEnable;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker dateTimePickerRegDate;
        private System.Windows.Forms.ComboBox comboBoxRegDateExpr;
        private System.Windows.Forms.ComboBox comboBoxIssueDateExpr;
        private System.Windows.Forms.DateTimePicker dateTimePickerIssueDate;
        private System.Windows.Forms.CheckBox checkBoxIssueDateEnable;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboBoxBeginDateExpr;
        private System.Windows.Forms.DateTimePicker dateTimePickerBeginDate;
        private System.Windows.Forms.CheckBox checkBoxBeginDateEnable;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBoxEndDateExpr;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndDate;
        private System.Windows.Forms.CheckBox checkBoxEndDateEnable;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboBoxResidenceWarrDateExpr;
        private System.Windows.Forms.DateTimePicker dateTimePickerResidenceWarrDate;
        private System.Windows.Forms.CheckBox checkBoxResidenceWarrDateEnable;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBoxKumiOrderDateExpr;
        private System.Windows.Forms.DateTimePicker dateTimePickerKumiOrderDate;
        private System.Windows.Forms.CheckBox checkBoxKumiOrderDateEnable;
        private System.Windows.Forms.Label label17;
    }
}