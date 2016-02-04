using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.SearchForms
{
    partial class ExtendedSearchPaymentAccounts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedSearchPaymentAccounts));
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.textBoxCRN = new System.Windows.Forms.TextBox();
            this.checkBoxCRNEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.checkBoxAccountEnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTenantSNP = new System.Windows.Forms.TextBox();
            this.checkBoxTenantSNPEnable = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.checkBoxPremisesNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxPremisesNum = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxHouseEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxStreetEnable = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.checkBoxDateEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.dateTimePickerDate = new System.Windows.Forms.DateTimePicker();
            this.comboBoxDateExpr = new System.Windows.Forms.ComboBox();
            this.textBoxRawAddress = new System.Windows.Forms.TextBox();
            this.checkBoxRawAddressEnable = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxBalanceInputExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceInputEnable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceInputFrom = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceInputTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceInputTenancyTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceInputTenancyFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceInputTenancyExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceInputTenancyEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceInputDGITo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceInputDGIFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceInputDGIExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceInputDGIEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownChargingDGITo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownChargingDGIFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxChargingDGIExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxChargingDGIEnable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownChargingTenancyTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownChargingTenancyFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxChargingTenancyExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxChargingTenancyEnable = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDownChargingTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownChargingFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxChargingExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxChargingEnable = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.numericUpDownTransferBalanceTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTransferBalanceFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxTransferBalanceExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxTransferBalanceEnable = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.numericUpDownRecalcDGITo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRecalcDGIFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxRecalcDGIExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxRecalcDGIEnable = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.numericUpDownRecalcTenancyTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRecalcTenancyFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxRecalcTenancyExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxRecalcTenancyEnable = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceOutputTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceOutputFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceOutputExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceOutputEnable = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.numericUpDownPaymentDGITo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPaymentDGIFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxPaymentDGIExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxPaymentDGIEnable = new System.Windows.Forms.CheckBox();
            this.label19 = new System.Windows.Forms.Label();
            this.numericUpDownPaymentTenancyTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPaymentTenancyFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxPaymentTenancyExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxPaymentTenancyEnable = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceOutputDGITo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceOutputDGIFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceOutputDGIExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceOutputDGIEnable = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceOutputTenancyTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceOutputTenancyFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceOutputTenancyExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceOutputTenancyEnable = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonWithoutUncompletedClaims = new System.Windows.Forms.RadioButton();
            this.radioButtonWithUncompletedClaims = new System.Windows.Forms.RadioButton();
            this.radioButtonWithoutClaims = new System.Windows.Forms.RadioButton();
            this.radioButtonWithClaims = new System.Windows.Forms.RadioButton();
            this.checkBoxByClaimsChecked = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputTenancyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputDGITo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputDGIFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingDGITo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingDGIFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTenancyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferBalanceTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferBalanceFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcDGITo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcDGIFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcTenancyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentDGITo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentDGIFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentTenancyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDGITo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDGIFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyFrom)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(17, 544);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 74;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(17, 503);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 73;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // textBoxCRN
            // 
            this.textBoxCRN.Enabled = false;
            this.textBoxCRN.Location = new System.Drawing.Point(42, 23);
            this.textBoxCRN.Name = "textBoxCRN";
            this.textBoxCRN.Size = new System.Drawing.Size(311, 21);
            this.textBoxCRN.TabIndex = 1;
            this.textBoxCRN.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxCRNEnable
            // 
            this.checkBoxCRNEnable.AutoSize = true;
            this.checkBoxCRNEnable.Location = new System.Drawing.Point(17, 26);
            this.checkBoxCRNEnable.Name = "checkBoxCRNEnable";
            this.checkBoxCRNEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCRNEnable.TabIndex = 0;
            this.checkBoxCRNEnable.UseVisualStyleBackColor = true;
            this.checkBoxCRNEnable.CheckedChanged += new System.EventHandler(this.checkBoxCRNEnable_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 40;
            this.label2.Text = "СРН";
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Enabled = false;
            this.textBoxAccount.Location = new System.Drawing.Point(42, 64);
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(311, 21);
            this.textBoxAccount.TabIndex = 3;
            this.textBoxAccount.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxAccountEnable
            // 
            this.checkBoxAccountEnable.AutoSize = true;
            this.checkBoxAccountEnable.Location = new System.Drawing.Point(17, 67);
            this.checkBoxAccountEnable.Name = "checkBoxAccountEnable";
            this.checkBoxAccountEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAccountEnable.TabIndex = 2;
            this.checkBoxAccountEnable.UseVisualStyleBackColor = true;
            this.checkBoxAccountEnable.CheckedChanged += new System.EventHandler(this.checkBoxAccountEnable_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 43;
            this.label1.Text = "Лицевой счет";
            // 
            // textBoxTenantSNP
            // 
            this.textBoxTenantSNP.Enabled = false;
            this.textBoxTenantSNP.Location = new System.Drawing.Point(42, 107);
            this.textBoxTenantSNP.Name = "textBoxTenantSNP";
            this.textBoxTenantSNP.Size = new System.Drawing.Size(311, 21);
            this.textBoxTenantSNP.TabIndex = 5;
            this.textBoxTenantSNP.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxTenantSNPEnable
            // 
            this.checkBoxTenantSNPEnable.AutoSize = true;
            this.checkBoxTenantSNPEnable.Location = new System.Drawing.Point(17, 110);
            this.checkBoxTenantSNPEnable.Name = "checkBoxTenantSNPEnable";
            this.checkBoxTenantSNPEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTenantSNPEnable.TabIndex = 4;
            this.checkBoxTenantSNPEnable.UseVisualStyleBackColor = true;
            this.checkBoxTenantSNPEnable.CheckedChanged += new System.EventHandler(this.checkBoxTenantSNPEnable_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 15);
            this.label4.TabIndex = 49;
            this.label4.Text = "ФИО нанимателя";
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Enabled = false;
            this.textBoxHouse.Location = new System.Drawing.Point(42, 230);
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(311, 21);
            this.textBoxHouse.TabIndex = 11;
            this.textBoxHouse.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxPremisesNumEnable
            // 
            this.checkBoxPremisesNumEnable.AutoSize = true;
            this.checkBoxPremisesNumEnable.Location = new System.Drawing.Point(17, 272);
            this.checkBoxPremisesNumEnable.Name = "checkBoxPremisesNumEnable";
            this.checkBoxPremisesNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPremisesNumEnable.TabIndex = 12;
            this.checkBoxPremisesNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxPremisesNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxPremisesNumEnable_CheckedChanged);
            // 
            // textBoxPremisesNum
            // 
            this.textBoxPremisesNum.Enabled = false;
            this.textBoxPremisesNum.Location = new System.Drawing.Point(42, 269);
            this.textBoxPremisesNum.MaxLength = 4;
            this.textBoxPremisesNum.Name = "textBoxPremisesNum";
            this.textBoxPremisesNum.Size = new System.Drawing.Size(311, 21);
            this.textBoxPremisesNum.TabIndex = 13;
            this.textBoxPremisesNum.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 252);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(193, 15);
            this.label7.TabIndex = 67;
            this.label7.Text = "Номер помещения (только ЖФ)";
            // 
            // checkBoxHouseEnable
            // 
            this.checkBoxHouseEnable.AutoSize = true;
            this.checkBoxHouseEnable.Location = new System.Drawing.Point(17, 233);
            this.checkBoxHouseEnable.Name = "checkBoxHouseEnable";
            this.checkBoxHouseEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseEnable.TabIndex = 10;
            this.checkBoxHouseEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // checkBoxStreetEnable
            // 
            this.checkBoxStreetEnable.AutoSize = true;
            this.checkBoxStreetEnable.Location = new System.Drawing.Point(17, 191);
            this.checkBoxStreetEnable.Name = "checkBoxStreetEnable";
            this.checkBoxStreetEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetEnable.TabIndex = 8;
            this.checkBoxStreetEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 213);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(156, 15);
            this.label10.TabIndex = 66;
            this.label10.Text = "Номер дома (только ЖФ)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 170);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(120, 15);
            this.label11.TabIndex = 65;
            this.label11.Text = "Улица (только ЖФ)";
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Enabled = false;
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(42, 187);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(311, 23);
            this.comboBoxStreet.TabIndex = 9;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.Enter += new System.EventHandler(this.selectAll_Enter);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // checkBoxDateEnable
            // 
            this.checkBoxDateEnable.AutoSize = true;
            this.checkBoxDateEnable.Location = new System.Drawing.Point(17, 313);
            this.checkBoxDateEnable.Name = "checkBoxDateEnable";
            this.checkBoxDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDateEnable.TabIndex = 14;
            this.checkBoxDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxDateEnable_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 293);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(136, 15);
            this.label12.TabIndex = 74;
            this.label12.Text = "Искать суммы на дату";
            // 
            // dateTimePickerDate
            // 
            this.dateTimePickerDate.Enabled = false;
            this.dateTimePickerDate.Location = new System.Drawing.Point(98, 310);
            this.dateTimePickerDate.Name = "dateTimePickerDate";
            this.dateTimePickerDate.Size = new System.Drawing.Size(255, 21);
            this.dateTimePickerDate.TabIndex = 16;
            // 
            // comboBoxDateExpr
            // 
            this.comboBoxDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDateExpr.Enabled = false;
            this.comboBoxDateExpr.FormattingEnabled = true;
            this.comboBoxDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "="});
            this.comboBoxDateExpr.Location = new System.Drawing.Point(42, 310);
            this.comboBoxDateExpr.Name = "comboBoxDateExpr";
            this.comboBoxDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxDateExpr.TabIndex = 15;
            // 
            // textBoxRawAddress
            // 
            this.textBoxRawAddress.Enabled = false;
            this.textBoxRawAddress.Location = new System.Drawing.Point(42, 148);
            this.textBoxRawAddress.Name = "textBoxRawAddress";
            this.textBoxRawAddress.Size = new System.Drawing.Size(311, 21);
            this.textBoxRawAddress.TabIndex = 7;
            this.textBoxRawAddress.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxRawAddressEnable
            // 
            this.checkBoxRawAddressEnable.AutoSize = true;
            this.checkBoxRawAddressEnable.Location = new System.Drawing.Point(17, 151);
            this.checkBoxRawAddressEnable.Name = "checkBoxRawAddressEnable";
            this.checkBoxRawAddressEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRawAddressEnable.TabIndex = 6;
            this.checkBoxRawAddressEnable.UseVisualStyleBackColor = true;
            this.checkBoxRawAddressEnable.CheckedChanged += new System.EventHandler(this.checkBoxRawAddressEnable_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 15);
            this.label3.TabIndex = 97;
            this.label3.Text = "Адрес по БКС";
            // 
            // comboBoxBalanceInputExpr
            // 
            this.comboBoxBalanceInputExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceInputExpr.Enabled = false;
            this.comboBoxBalanceInputExpr.FormattingEnabled = true;
            this.comboBoxBalanceInputExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceInputExpr.Location = new System.Drawing.Point(407, 22);
            this.comboBoxBalanceInputExpr.Name = "comboBoxBalanceInputExpr";
            this.comboBoxBalanceInputExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxBalanceInputExpr.TabIndex = 20;
            // 
            // checkBoxBalanceInputEnable
            // 
            this.checkBoxBalanceInputEnable.AutoSize = true;
            this.checkBoxBalanceInputEnable.Location = new System.Drawing.Point(382, 26);
            this.checkBoxBalanceInputEnable.Name = "checkBoxBalanceInputEnable";
            this.checkBoxBalanceInputEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceInputEnable.TabIndex = 19;
            this.checkBoxBalanceInputEnable.UseVisualStyleBackColor = true;
            this.checkBoxBalanceInputEnable.CheckedChanged += new System.EventHandler(this.checkBoxBalanceInputEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(379, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 15);
            this.label5.TabIndex = 101;
            this.label5.Text = "Сальдо входящее";
            // 
            // numericUpDownBalanceInputFrom
            // 
            this.numericUpDownBalanceInputFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceInputFrom.Enabled = false;
            this.numericUpDownBalanceInputFrom.Location = new System.Drawing.Point(468, 23);
            this.numericUpDownBalanceInputFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceInputFrom.Name = "numericUpDownBalanceInputFrom";
            this.numericUpDownBalanceInputFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceInputFrom.TabIndex = 21;
            this.numericUpDownBalanceInputFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownBalanceInputTo
            // 
            this.numericUpDownBalanceInputTo.DecimalPlaces = 2;
            this.numericUpDownBalanceInputTo.Enabled = false;
            this.numericUpDownBalanceInputTo.Location = new System.Drawing.Point(626, 23);
            this.numericUpDownBalanceInputTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceInputTo.Name = "numericUpDownBalanceInputTo";
            this.numericUpDownBalanceInputTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceInputTo.TabIndex = 22;
            this.numericUpDownBalanceInputTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownBalanceInputTenancyTo
            // 
            this.numericUpDownBalanceInputTenancyTo.DecimalPlaces = 2;
            this.numericUpDownBalanceInputTenancyTo.Enabled = false;
            this.numericUpDownBalanceInputTenancyTo.Location = new System.Drawing.Point(626, 64);
            this.numericUpDownBalanceInputTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceInputTenancyTo.Name = "numericUpDownBalanceInputTenancyTo";
            this.numericUpDownBalanceInputTenancyTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceInputTenancyTo.TabIndex = 26;
            this.numericUpDownBalanceInputTenancyTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownBalanceInputTenancyFrom
            // 
            this.numericUpDownBalanceInputTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceInputTenancyFrom.Enabled = false;
            this.numericUpDownBalanceInputTenancyFrom.Location = new System.Drawing.Point(468, 64);
            this.numericUpDownBalanceInputTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceInputTenancyFrom.Name = "numericUpDownBalanceInputTenancyFrom";
            this.numericUpDownBalanceInputTenancyFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceInputTenancyFrom.TabIndex = 25;
            this.numericUpDownBalanceInputTenancyFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxBalanceInputTenancyExpr
            // 
            this.comboBoxBalanceInputTenancyExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceInputTenancyExpr.Enabled = false;
            this.comboBoxBalanceInputTenancyExpr.FormattingEnabled = true;
            this.comboBoxBalanceInputTenancyExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceInputTenancyExpr.Location = new System.Drawing.Point(407, 63);
            this.comboBoxBalanceInputTenancyExpr.Name = "comboBoxBalanceInputTenancyExpr";
            this.comboBoxBalanceInputTenancyExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxBalanceInputTenancyExpr.TabIndex = 24;
            // 
            // checkBoxBalanceInputTenancyEnable
            // 
            this.checkBoxBalanceInputTenancyEnable.AutoSize = true;
            this.checkBoxBalanceInputTenancyEnable.Location = new System.Drawing.Point(382, 67);
            this.checkBoxBalanceInputTenancyEnable.Name = "checkBoxBalanceInputTenancyEnable";
            this.checkBoxBalanceInputTenancyEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceInputTenancyEnable.TabIndex = 23;
            this.checkBoxBalanceInputTenancyEnable.UseVisualStyleBackColor = true;
            this.checkBoxBalanceInputTenancyEnable.CheckedChanged += new System.EventHandler(this.checkBoxBalanceInputTenancyEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(379, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 15);
            this.label6.TabIndex = 106;
            this.label6.Text = "Сальдо входящее найм";
            // 
            // numericUpDownBalanceInputDGITo
            // 
            this.numericUpDownBalanceInputDGITo.DecimalPlaces = 2;
            this.numericUpDownBalanceInputDGITo.Enabled = false;
            this.numericUpDownBalanceInputDGITo.Location = new System.Drawing.Point(626, 107);
            this.numericUpDownBalanceInputDGITo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceInputDGITo.Name = "numericUpDownBalanceInputDGITo";
            this.numericUpDownBalanceInputDGITo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceInputDGITo.TabIndex = 28;
            this.numericUpDownBalanceInputDGITo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownBalanceInputDGIFrom
            // 
            this.numericUpDownBalanceInputDGIFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceInputDGIFrom.Enabled = false;
            this.numericUpDownBalanceInputDGIFrom.Location = new System.Drawing.Point(468, 107);
            this.numericUpDownBalanceInputDGIFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceInputDGIFrom.Name = "numericUpDownBalanceInputDGIFrom";
            this.numericUpDownBalanceInputDGIFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceInputDGIFrom.TabIndex = 27;
            this.numericUpDownBalanceInputDGIFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxBalanceInputDGIExpr
            // 
            this.comboBoxBalanceInputDGIExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceInputDGIExpr.Enabled = false;
            this.comboBoxBalanceInputDGIExpr.FormattingEnabled = true;
            this.comboBoxBalanceInputDGIExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceInputDGIExpr.Location = new System.Drawing.Point(407, 106);
            this.comboBoxBalanceInputDGIExpr.Name = "comboBoxBalanceInputDGIExpr";
            this.comboBoxBalanceInputDGIExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxBalanceInputDGIExpr.TabIndex = 26;
            // 
            // checkBoxBalanceInputDGIEnable
            // 
            this.checkBoxBalanceInputDGIEnable.AutoSize = true;
            this.checkBoxBalanceInputDGIEnable.Location = new System.Drawing.Point(382, 110);
            this.checkBoxBalanceInputDGIEnable.Name = "checkBoxBalanceInputDGIEnable";
            this.checkBoxBalanceInputDGIEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceInputDGIEnable.TabIndex = 25;
            this.checkBoxBalanceInputDGIEnable.UseVisualStyleBackColor = true;
            this.checkBoxBalanceInputDGIEnable.CheckedChanged += new System.EventHandler(this.checkBoxBalanceInputDGIEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(379, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(138, 15);
            this.label8.TabIndex = 111;
            this.label8.Text = "Сальдо входящее ДГИ";
            // 
            // numericUpDownChargingDGITo
            // 
            this.numericUpDownChargingDGITo.DecimalPlaces = 2;
            this.numericUpDownChargingDGITo.Enabled = false;
            this.numericUpDownChargingDGITo.Location = new System.Drawing.Point(626, 230);
            this.numericUpDownChargingDGITo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingDGITo.Name = "numericUpDownChargingDGITo";
            this.numericUpDownChargingDGITo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownChargingDGITo.TabIndex = 40;
            this.numericUpDownChargingDGITo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownChargingDGIFrom
            // 
            this.numericUpDownChargingDGIFrom.DecimalPlaces = 2;
            this.numericUpDownChargingDGIFrom.Enabled = false;
            this.numericUpDownChargingDGIFrom.Location = new System.Drawing.Point(468, 230);
            this.numericUpDownChargingDGIFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingDGIFrom.Name = "numericUpDownChargingDGIFrom";
            this.numericUpDownChargingDGIFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownChargingDGIFrom.TabIndex = 39;
            this.numericUpDownChargingDGIFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxChargingDGIExpr
            // 
            this.comboBoxChargingDGIExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChargingDGIExpr.Enabled = false;
            this.comboBoxChargingDGIExpr.FormattingEnabled = true;
            this.comboBoxChargingDGIExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxChargingDGIExpr.Location = new System.Drawing.Point(407, 229);
            this.comboBoxChargingDGIExpr.Name = "comboBoxChargingDGIExpr";
            this.comboBoxChargingDGIExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxChargingDGIExpr.TabIndex = 38;
            // 
            // checkBoxChargingDGIEnable
            // 
            this.checkBoxChargingDGIEnable.AutoSize = true;
            this.checkBoxChargingDGIEnable.Location = new System.Drawing.Point(382, 233);
            this.checkBoxChargingDGIEnable.Name = "checkBoxChargingDGIEnable";
            this.checkBoxChargingDGIEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxChargingDGIEnable.TabIndex = 37;
            this.checkBoxChargingDGIEnable.UseVisualStyleBackColor = true;
            this.checkBoxChargingDGIEnable.CheckedChanged += new System.EventHandler(this.checkBoxChargingDGIEnable_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(379, 213);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(105, 15);
            this.label9.TabIndex = 126;
            this.label9.Text = "Начисление ДГИ";
            // 
            // numericUpDownChargingTenancyTo
            // 
            this.numericUpDownChargingTenancyTo.DecimalPlaces = 2;
            this.numericUpDownChargingTenancyTo.Enabled = false;
            this.numericUpDownChargingTenancyTo.Location = new System.Drawing.Point(626, 189);
            this.numericUpDownChargingTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingTenancyTo.Name = "numericUpDownChargingTenancyTo";
            this.numericUpDownChargingTenancyTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownChargingTenancyTo.TabIndex = 36;
            this.numericUpDownChargingTenancyTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownChargingTenancyFrom
            // 
            this.numericUpDownChargingTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownChargingTenancyFrom.Enabled = false;
            this.numericUpDownChargingTenancyFrom.Location = new System.Drawing.Point(468, 189);
            this.numericUpDownChargingTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingTenancyFrom.Name = "numericUpDownChargingTenancyFrom";
            this.numericUpDownChargingTenancyFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownChargingTenancyFrom.TabIndex = 35;
            this.numericUpDownChargingTenancyFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxChargingTenancyExpr
            // 
            this.comboBoxChargingTenancyExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChargingTenancyExpr.Enabled = false;
            this.comboBoxChargingTenancyExpr.FormattingEnabled = true;
            this.comboBoxChargingTenancyExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxChargingTenancyExpr.Location = new System.Drawing.Point(407, 188);
            this.comboBoxChargingTenancyExpr.Name = "comboBoxChargingTenancyExpr";
            this.comboBoxChargingTenancyExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxChargingTenancyExpr.TabIndex = 34;
            // 
            // checkBoxChargingTenancyEnable
            // 
            this.checkBoxChargingTenancyEnable.AutoSize = true;
            this.checkBoxChargingTenancyEnable.Location = new System.Drawing.Point(382, 192);
            this.checkBoxChargingTenancyEnable.Name = "checkBoxChargingTenancyEnable";
            this.checkBoxChargingTenancyEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxChargingTenancyEnable.TabIndex = 33;
            this.checkBoxChargingTenancyEnable.UseVisualStyleBackColor = true;
            this.checkBoxChargingTenancyEnable.CheckedChanged += new System.EventHandler(this.checkBoxChargingTenancyEnable_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(379, 172);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(110, 15);
            this.label13.TabIndex = 121;
            this.label13.Text = "Начисление найм";
            // 
            // numericUpDownChargingTo
            // 
            this.numericUpDownChargingTo.DecimalPlaces = 2;
            this.numericUpDownChargingTo.Enabled = false;
            this.numericUpDownChargingTo.Location = new System.Drawing.Point(626, 148);
            this.numericUpDownChargingTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingTo.Name = "numericUpDownChargingTo";
            this.numericUpDownChargingTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownChargingTo.TabIndex = 32;
            this.numericUpDownChargingTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownChargingFrom
            // 
            this.numericUpDownChargingFrom.DecimalPlaces = 2;
            this.numericUpDownChargingFrom.Enabled = false;
            this.numericUpDownChargingFrom.Location = new System.Drawing.Point(468, 148);
            this.numericUpDownChargingFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownChargingFrom.Name = "numericUpDownChargingFrom";
            this.numericUpDownChargingFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownChargingFrom.TabIndex = 31;
            this.numericUpDownChargingFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxChargingExpr
            // 
            this.comboBoxChargingExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChargingExpr.Enabled = false;
            this.comboBoxChargingExpr.FormattingEnabled = true;
            this.comboBoxChargingExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxChargingExpr.Location = new System.Drawing.Point(407, 147);
            this.comboBoxChargingExpr.Name = "comboBoxChargingExpr";
            this.comboBoxChargingExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxChargingExpr.TabIndex = 30;
            // 
            // checkBoxChargingEnable
            // 
            this.checkBoxChargingEnable.AutoSize = true;
            this.checkBoxChargingEnable.Location = new System.Drawing.Point(382, 151);
            this.checkBoxChargingEnable.Name = "checkBoxChargingEnable";
            this.checkBoxChargingEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxChargingEnable.TabIndex = 29;
            this.checkBoxChargingEnable.UseVisualStyleBackColor = true;
            this.checkBoxChargingEnable.CheckedChanged += new System.EventHandler(this.checkBoxChargingEnable_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(379, 131);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(113, 15);
            this.label14.TabIndex = 116;
            this.label14.Text = "Начисление итого";
            // 
            // numericUpDownTransferBalanceTo
            // 
            this.numericUpDownTransferBalanceTo.DecimalPlaces = 2;
            this.numericUpDownTransferBalanceTo.Enabled = false;
            this.numericUpDownTransferBalanceTo.Location = new System.Drawing.Point(626, 351);
            this.numericUpDownTransferBalanceTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownTransferBalanceTo.Name = "numericUpDownTransferBalanceTo";
            this.numericUpDownTransferBalanceTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownTransferBalanceTo.TabIndex = 52;
            this.numericUpDownTransferBalanceTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownTransferBalanceFrom
            // 
            this.numericUpDownTransferBalanceFrom.DecimalPlaces = 2;
            this.numericUpDownTransferBalanceFrom.Enabled = false;
            this.numericUpDownTransferBalanceFrom.Location = new System.Drawing.Point(468, 351);
            this.numericUpDownTransferBalanceFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownTransferBalanceFrom.Name = "numericUpDownTransferBalanceFrom";
            this.numericUpDownTransferBalanceFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownTransferBalanceFrom.TabIndex = 51;
            this.numericUpDownTransferBalanceFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxTransferBalanceExpr
            // 
            this.comboBoxTransferBalanceExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTransferBalanceExpr.Enabled = false;
            this.comboBoxTransferBalanceExpr.FormattingEnabled = true;
            this.comboBoxTransferBalanceExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxTransferBalanceExpr.Location = new System.Drawing.Point(407, 350);
            this.comboBoxTransferBalanceExpr.Name = "comboBoxTransferBalanceExpr";
            this.comboBoxTransferBalanceExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxTransferBalanceExpr.TabIndex = 50;
            // 
            // checkBoxTransferBalanceEnable
            // 
            this.checkBoxTransferBalanceEnable.AutoSize = true;
            this.checkBoxTransferBalanceEnable.Location = new System.Drawing.Point(382, 354);
            this.checkBoxTransferBalanceEnable.Name = "checkBoxTransferBalanceEnable";
            this.checkBoxTransferBalanceEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTransferBalanceEnable.TabIndex = 49;
            this.checkBoxTransferBalanceEnable.UseVisualStyleBackColor = true;
            this.checkBoxTransferBalanceEnable.CheckedChanged += new System.EventHandler(this.checkBoxTransferBalanceEnable_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(379, 334);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(101, 15);
            this.label15.TabIndex = 141;
            this.label15.Text = "Перенос сальдо";
            // 
            // numericUpDownRecalcDGITo
            // 
            this.numericUpDownRecalcDGITo.DecimalPlaces = 2;
            this.numericUpDownRecalcDGITo.Enabled = false;
            this.numericUpDownRecalcDGITo.Location = new System.Drawing.Point(626, 310);
            this.numericUpDownRecalcDGITo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRecalcDGITo.Name = "numericUpDownRecalcDGITo";
            this.numericUpDownRecalcDGITo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownRecalcDGITo.TabIndex = 48;
            this.numericUpDownRecalcDGITo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownRecalcDGIFrom
            // 
            this.numericUpDownRecalcDGIFrom.DecimalPlaces = 2;
            this.numericUpDownRecalcDGIFrom.Enabled = false;
            this.numericUpDownRecalcDGIFrom.Location = new System.Drawing.Point(468, 310);
            this.numericUpDownRecalcDGIFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRecalcDGIFrom.Name = "numericUpDownRecalcDGIFrom";
            this.numericUpDownRecalcDGIFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownRecalcDGIFrom.TabIndex = 47;
            this.numericUpDownRecalcDGIFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxRecalcDGIExpr
            // 
            this.comboBoxRecalcDGIExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecalcDGIExpr.Enabled = false;
            this.comboBoxRecalcDGIExpr.FormattingEnabled = true;
            this.comboBoxRecalcDGIExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxRecalcDGIExpr.Location = new System.Drawing.Point(407, 309);
            this.comboBoxRecalcDGIExpr.Name = "comboBoxRecalcDGIExpr";
            this.comboBoxRecalcDGIExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxRecalcDGIExpr.TabIndex = 46;
            // 
            // checkBoxRecalcDGIEnable
            // 
            this.checkBoxRecalcDGIEnable.AutoSize = true;
            this.checkBoxRecalcDGIEnable.Location = new System.Drawing.Point(382, 313);
            this.checkBoxRecalcDGIEnable.Name = "checkBoxRecalcDGIEnable";
            this.checkBoxRecalcDGIEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRecalcDGIEnable.TabIndex = 45;
            this.checkBoxRecalcDGIEnable.UseVisualStyleBackColor = true;
            this.checkBoxRecalcDGIEnable.CheckedChanged += new System.EventHandler(this.checkBoxRecalcDGIEnable_CheckedChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(379, 293);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(105, 15);
            this.label16.TabIndex = 136;
            this.label16.Text = "Перерасчет ДГИ";
            // 
            // numericUpDownRecalcTenancyTo
            // 
            this.numericUpDownRecalcTenancyTo.DecimalPlaces = 2;
            this.numericUpDownRecalcTenancyTo.Enabled = false;
            this.numericUpDownRecalcTenancyTo.Location = new System.Drawing.Point(626, 269);
            this.numericUpDownRecalcTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRecalcTenancyTo.Name = "numericUpDownRecalcTenancyTo";
            this.numericUpDownRecalcTenancyTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownRecalcTenancyTo.TabIndex = 44;
            this.numericUpDownRecalcTenancyTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownRecalcTenancyFrom
            // 
            this.numericUpDownRecalcTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownRecalcTenancyFrom.Enabled = false;
            this.numericUpDownRecalcTenancyFrom.Location = new System.Drawing.Point(468, 269);
            this.numericUpDownRecalcTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownRecalcTenancyFrom.Name = "numericUpDownRecalcTenancyFrom";
            this.numericUpDownRecalcTenancyFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownRecalcTenancyFrom.TabIndex = 43;
            this.numericUpDownRecalcTenancyFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxRecalcTenancyExpr
            // 
            this.comboBoxRecalcTenancyExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecalcTenancyExpr.Enabled = false;
            this.comboBoxRecalcTenancyExpr.FormattingEnabled = true;
            this.comboBoxRecalcTenancyExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxRecalcTenancyExpr.Location = new System.Drawing.Point(407, 268);
            this.comboBoxRecalcTenancyExpr.Name = "comboBoxRecalcTenancyExpr";
            this.comboBoxRecalcTenancyExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxRecalcTenancyExpr.TabIndex = 42;
            // 
            // checkBoxRecalcTenancyEnable
            // 
            this.checkBoxRecalcTenancyEnable.AutoSize = true;
            this.checkBoxRecalcTenancyEnable.Location = new System.Drawing.Point(382, 272);
            this.checkBoxRecalcTenancyEnable.Name = "checkBoxRecalcTenancyEnable";
            this.checkBoxRecalcTenancyEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRecalcTenancyEnable.TabIndex = 41;
            this.checkBoxRecalcTenancyEnable.UseVisualStyleBackColor = true;
            this.checkBoxRecalcTenancyEnable.CheckedChanged += new System.EventHandler(this.checkBoxRecalcTenancyEnable_CheckedChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(379, 252);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(110, 15);
            this.label17.TabIndex = 131;
            this.label17.Text = "Перерасчет найм";
            // 
            // numericUpDownBalanceOutputTo
            // 
            this.numericUpDownBalanceOutputTo.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputTo.Enabled = false;
            this.numericUpDownBalanceOutputTo.Location = new System.Drawing.Point(626, 478);
            this.numericUpDownBalanceOutputTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputTo.Name = "numericUpDownBalanceOutputTo";
            this.numericUpDownBalanceOutputTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceOutputTo.TabIndex = 64;
            this.numericUpDownBalanceOutputTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownBalanceOutputFrom
            // 
            this.numericUpDownBalanceOutputFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputFrom.Enabled = false;
            this.numericUpDownBalanceOutputFrom.Location = new System.Drawing.Point(468, 478);
            this.numericUpDownBalanceOutputFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputFrom.Name = "numericUpDownBalanceOutputFrom";
            this.numericUpDownBalanceOutputFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceOutputFrom.TabIndex = 63;
            this.numericUpDownBalanceOutputFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxBalanceOutputExpr
            // 
            this.comboBoxBalanceOutputExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceOutputExpr.Enabled = false;
            this.comboBoxBalanceOutputExpr.FormattingEnabled = true;
            this.comboBoxBalanceOutputExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceOutputExpr.Location = new System.Drawing.Point(407, 477);
            this.comboBoxBalanceOutputExpr.Name = "comboBoxBalanceOutputExpr";
            this.comboBoxBalanceOutputExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxBalanceOutputExpr.TabIndex = 62;
            // 
            // checkBoxBalanceOutputEnable
            // 
            this.checkBoxBalanceOutputEnable.AutoSize = true;
            this.checkBoxBalanceOutputEnable.Location = new System.Drawing.Point(382, 481);
            this.checkBoxBalanceOutputEnable.Name = "checkBoxBalanceOutputEnable";
            this.checkBoxBalanceOutputEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceOutputEnable.TabIndex = 61;
            this.checkBoxBalanceOutputEnable.UseVisualStyleBackColor = true;
            this.checkBoxBalanceOutputEnable.CheckedChanged += new System.EventHandler(this.checkBoxBalanceOutputEnable_CheckedChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(379, 461);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(116, 15);
            this.label18.TabIndex = 156;
            this.label18.Text = "Сальдо исходящее";
            // 
            // numericUpDownPaymentDGITo
            // 
            this.numericUpDownPaymentDGITo.DecimalPlaces = 2;
            this.numericUpDownPaymentDGITo.Enabled = false;
            this.numericUpDownPaymentDGITo.Location = new System.Drawing.Point(626, 435);
            this.numericUpDownPaymentDGITo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPaymentDGITo.Name = "numericUpDownPaymentDGITo";
            this.numericUpDownPaymentDGITo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownPaymentDGITo.TabIndex = 60;
            this.numericUpDownPaymentDGITo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownPaymentDGIFrom
            // 
            this.numericUpDownPaymentDGIFrom.DecimalPlaces = 2;
            this.numericUpDownPaymentDGIFrom.Enabled = false;
            this.numericUpDownPaymentDGIFrom.Location = new System.Drawing.Point(468, 435);
            this.numericUpDownPaymentDGIFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPaymentDGIFrom.Name = "numericUpDownPaymentDGIFrom";
            this.numericUpDownPaymentDGIFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownPaymentDGIFrom.TabIndex = 59;
            this.numericUpDownPaymentDGIFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxPaymentDGIExpr
            // 
            this.comboBoxPaymentDGIExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPaymentDGIExpr.Enabled = false;
            this.comboBoxPaymentDGIExpr.FormattingEnabled = true;
            this.comboBoxPaymentDGIExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxPaymentDGIExpr.Location = new System.Drawing.Point(407, 434);
            this.comboBoxPaymentDGIExpr.Name = "comboBoxPaymentDGIExpr";
            this.comboBoxPaymentDGIExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxPaymentDGIExpr.TabIndex = 58;
            // 
            // checkBoxPaymentDGIEnable
            // 
            this.checkBoxPaymentDGIEnable.AutoSize = true;
            this.checkBoxPaymentDGIEnable.Location = new System.Drawing.Point(382, 438);
            this.checkBoxPaymentDGIEnable.Name = "checkBoxPaymentDGIEnable";
            this.checkBoxPaymentDGIEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPaymentDGIEnable.TabIndex = 57;
            this.checkBoxPaymentDGIEnable.UseVisualStyleBackColor = true;
            this.checkBoxPaymentDGIEnable.CheckedChanged += new System.EventHandler(this.checkBoxPaymentDGIEnable_CheckedChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(379, 418);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 15);
            this.label19.TabIndex = 151;
            this.label19.Text = "Оплата ДГИ";
            // 
            // numericUpDownPaymentTenancyTo
            // 
            this.numericUpDownPaymentTenancyTo.DecimalPlaces = 2;
            this.numericUpDownPaymentTenancyTo.Enabled = false;
            this.numericUpDownPaymentTenancyTo.Location = new System.Drawing.Point(626, 394);
            this.numericUpDownPaymentTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPaymentTenancyTo.Name = "numericUpDownPaymentTenancyTo";
            this.numericUpDownPaymentTenancyTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownPaymentTenancyTo.TabIndex = 56;
            this.numericUpDownPaymentTenancyTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownPaymentTenancyFrom
            // 
            this.numericUpDownPaymentTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownPaymentTenancyFrom.Enabled = false;
            this.numericUpDownPaymentTenancyFrom.Location = new System.Drawing.Point(468, 394);
            this.numericUpDownPaymentTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownPaymentTenancyFrom.Name = "numericUpDownPaymentTenancyFrom";
            this.numericUpDownPaymentTenancyFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownPaymentTenancyFrom.TabIndex = 55;
            this.numericUpDownPaymentTenancyFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxPaymentTenancyExpr
            // 
            this.comboBoxPaymentTenancyExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPaymentTenancyExpr.Enabled = false;
            this.comboBoxPaymentTenancyExpr.FormattingEnabled = true;
            this.comboBoxPaymentTenancyExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxPaymentTenancyExpr.Location = new System.Drawing.Point(407, 393);
            this.comboBoxPaymentTenancyExpr.Name = "comboBoxPaymentTenancyExpr";
            this.comboBoxPaymentTenancyExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxPaymentTenancyExpr.TabIndex = 54;
            // 
            // checkBoxPaymentTenancyEnable
            // 
            this.checkBoxPaymentTenancyEnable.AutoSize = true;
            this.checkBoxPaymentTenancyEnable.Location = new System.Drawing.Point(382, 397);
            this.checkBoxPaymentTenancyEnable.Name = "checkBoxPaymentTenancyEnable";
            this.checkBoxPaymentTenancyEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPaymentTenancyEnable.TabIndex = 53;
            this.checkBoxPaymentTenancyEnable.UseVisualStyleBackColor = true;
            this.checkBoxPaymentTenancyEnable.CheckedChanged += new System.EventHandler(this.checkBoxPaymentTenancyEnable_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(379, 377);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(84, 15);
            this.label20.TabIndex = 146;
            this.label20.Text = "Оплата найм";
            // 
            // numericUpDownBalanceOutputDGITo
            // 
            this.numericUpDownBalanceOutputDGITo.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputDGITo.Enabled = false;
            this.numericUpDownBalanceOutputDGITo.Location = new System.Drawing.Point(626, 561);
            this.numericUpDownBalanceOutputDGITo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputDGITo.Name = "numericUpDownBalanceOutputDGITo";
            this.numericUpDownBalanceOutputDGITo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceOutputDGITo.TabIndex = 72;
            this.numericUpDownBalanceOutputDGITo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownBalanceOutputDGIFrom
            // 
            this.numericUpDownBalanceOutputDGIFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputDGIFrom.Enabled = false;
            this.numericUpDownBalanceOutputDGIFrom.Location = new System.Drawing.Point(468, 561);
            this.numericUpDownBalanceOutputDGIFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputDGIFrom.Name = "numericUpDownBalanceOutputDGIFrom";
            this.numericUpDownBalanceOutputDGIFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceOutputDGIFrom.TabIndex = 71;
            this.numericUpDownBalanceOutputDGIFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxBalanceOutputDGIExpr
            // 
            this.comboBoxBalanceOutputDGIExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceOutputDGIExpr.Enabled = false;
            this.comboBoxBalanceOutputDGIExpr.FormattingEnabled = true;
            this.comboBoxBalanceOutputDGIExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceOutputDGIExpr.Location = new System.Drawing.Point(407, 560);
            this.comboBoxBalanceOutputDGIExpr.Name = "comboBoxBalanceOutputDGIExpr";
            this.comboBoxBalanceOutputDGIExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxBalanceOutputDGIExpr.TabIndex = 70;
            // 
            // checkBoxBalanceOutputDGIEnable
            // 
            this.checkBoxBalanceOutputDGIEnable.AutoSize = true;
            this.checkBoxBalanceOutputDGIEnable.Location = new System.Drawing.Point(382, 564);
            this.checkBoxBalanceOutputDGIEnable.Name = "checkBoxBalanceOutputDGIEnable";
            this.checkBoxBalanceOutputDGIEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceOutputDGIEnable.TabIndex = 69;
            this.checkBoxBalanceOutputDGIEnable.UseVisualStyleBackColor = true;
            this.checkBoxBalanceOutputDGIEnable.CheckedChanged += new System.EventHandler(this.checkBoxBalanceDGIOutputEnable_CheckedChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(379, 544);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(144, 15);
            this.label21.TabIndex = 166;
            this.label21.Text = "Сальдо исходящее ДГИ";
            // 
            // numericUpDownBalanceOutputTenancyTo
            // 
            this.numericUpDownBalanceOutputTenancyTo.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputTenancyTo.Enabled = false;
            this.numericUpDownBalanceOutputTenancyTo.Location = new System.Drawing.Point(626, 518);
            this.numericUpDownBalanceOutputTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputTenancyTo.Name = "numericUpDownBalanceOutputTenancyTo";
            this.numericUpDownBalanceOutputTenancyTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceOutputTenancyTo.TabIndex = 68;
            this.numericUpDownBalanceOutputTenancyTo.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // numericUpDownBalanceOutputTenancyFrom
            // 
            this.numericUpDownBalanceOutputTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputTenancyFrom.Enabled = false;
            this.numericUpDownBalanceOutputTenancyFrom.Location = new System.Drawing.Point(468, 518);
            this.numericUpDownBalanceOutputTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputTenancyFrom.Name = "numericUpDownBalanceOutputTenancyFrom";
            this.numericUpDownBalanceOutputTenancyFrom.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownBalanceOutputTenancyFrom.TabIndex = 67;
            this.numericUpDownBalanceOutputTenancyFrom.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // comboBoxBalanceOutputTenancyExpr
            // 
            this.comboBoxBalanceOutputTenancyExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceOutputTenancyExpr.Enabled = false;
            this.comboBoxBalanceOutputTenancyExpr.FormattingEnabled = true;
            this.comboBoxBalanceOutputTenancyExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceOutputTenancyExpr.Location = new System.Drawing.Point(407, 517);
            this.comboBoxBalanceOutputTenancyExpr.Name = "comboBoxBalanceOutputTenancyExpr";
            this.comboBoxBalanceOutputTenancyExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxBalanceOutputTenancyExpr.TabIndex = 66;
            // 
            // checkBoxBalanceOutputTenancyEnable
            // 
            this.checkBoxBalanceOutputTenancyEnable.AutoSize = true;
            this.checkBoxBalanceOutputTenancyEnable.Location = new System.Drawing.Point(382, 521);
            this.checkBoxBalanceOutputTenancyEnable.Name = "checkBoxBalanceOutputTenancyEnable";
            this.checkBoxBalanceOutputTenancyEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceOutputTenancyEnable.TabIndex = 65;
            this.checkBoxBalanceOutputTenancyEnable.UseVisualStyleBackColor = true;
            this.checkBoxBalanceOutputTenancyEnable.CheckedChanged += new System.EventHandler(this.checkBoxBalanceTenancyOutputEnable_CheckedChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(379, 501);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(149, 15);
            this.label22.TabIndex = 161;
            this.label22.Text = "Сальдо исходящее найм";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonWithClaims);
            this.groupBox1.Controls.Add(this.radioButtonWithoutClaims);
            this.groupBox1.Controls.Add(this.radioButtonWithUncompletedClaims);
            this.groupBox1.Controls.Add(this.radioButtonWithoutUncompletedClaims);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(8, 341);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 137);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            // 
            // radioButtonWithoutUncompletedClaims
            // 
            this.radioButtonWithoutUncompletedClaims.AutoSize = true;
            this.radioButtonWithoutUncompletedClaims.Location = new System.Drawing.Point(21, 53);
            this.radioButtonWithoutUncompletedClaims.Name = "radioButtonWithoutUncompletedClaims";
            this.radioButtonWithoutUncompletedClaims.Size = new System.Drawing.Size(233, 19);
            this.radioButtonWithoutUncompletedClaims.TabIndex = 1;
            this.radioButtonWithoutUncompletedClaims.Text = "Нет незавершенных исковых работ";
            this.radioButtonWithoutUncompletedClaims.UseVisualStyleBackColor = true;
            // 
            // radioButtonWithUncompletedClaims
            // 
            this.radioButtonWithUncompletedClaims.AutoSize = true;
            this.radioButtonWithUncompletedClaims.Location = new System.Drawing.Point(21, 78);
            this.radioButtonWithUncompletedClaims.Name = "radioButtonWithUncompletedClaims";
            this.radioButtonWithUncompletedClaims.Size = new System.Drawing.Size(197, 19);
            this.radioButtonWithUncompletedClaims.TabIndex = 2;
            this.radioButtonWithUncompletedClaims.Text = "Есть незавершенные работы";
            this.radioButtonWithUncompletedClaims.UseVisualStyleBackColor = true;
            // 
            // radioButtonWithoutClaims
            // 
            this.radioButtonWithoutClaims.AutoSize = true;
            this.radioButtonWithoutClaims.Checked = true;
            this.radioButtonWithoutClaims.Location = new System.Drawing.Point(21, 28);
            this.radioButtonWithoutClaims.Name = "radioButtonWithoutClaims";
            this.radioButtonWithoutClaims.Size = new System.Drawing.Size(137, 19);
            this.radioButtonWithoutClaims.TabIndex = 0;
            this.radioButtonWithoutClaims.TabStop = true;
            this.radioButtonWithoutClaims.Text = "Нет исковых работ";
            this.radioButtonWithoutClaims.UseVisualStyleBackColor = true;
            // 
            // radioButtonWithClaims
            // 
            this.radioButtonWithClaims.AutoSize = true;
            this.radioButtonWithClaims.Location = new System.Drawing.Point(21, 103);
            this.radioButtonWithClaims.Name = "radioButtonWithClaims";
            this.radioButtonWithClaims.Size = new System.Drawing.Size(152, 19);
            this.radioButtonWithClaims.TabIndex = 3;
            this.radioButtonWithClaims.Text = "Есть исковые работы";
            this.radioButtonWithClaims.UseVisualStyleBackColor = true;
            // 
            // checkBoxByClaimsChecked
            // 
            this.checkBoxByClaimsChecked.AutoSize = true;
            this.checkBoxByClaimsChecked.Location = new System.Drawing.Point(17, 340);
            this.checkBoxByClaimsChecked.Name = "checkBoxByClaimsChecked";
            this.checkBoxByClaimsChecked.Size = new System.Drawing.Size(229, 19);
            this.checkBoxByClaimsChecked.TabIndex = 17;
            this.checkBoxByClaimsChecked.Text = "Искать ЛС с учетом исковых работ";
            this.checkBoxByClaimsChecked.UseVisualStyleBackColor = true;
            this.checkBoxByClaimsChecked.CheckedChanged += new System.EventHandler(this.checkBoxByClaimsChecked_CheckedChanged);
            // 
            // ExtendedSearchPaymentAccounts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(786, 591);
            this.Controls.Add(this.checkBoxByClaimsChecked);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numericUpDownBalanceOutputDGITo);
            this.Controls.Add(this.numericUpDownBalanceOutputDGIFrom);
            this.Controls.Add(this.comboBoxBalanceOutputDGIExpr);
            this.Controls.Add(this.checkBoxBalanceOutputDGIEnable);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.numericUpDownBalanceOutputTenancyTo);
            this.Controls.Add(this.numericUpDownBalanceOutputTenancyFrom);
            this.Controls.Add(this.comboBoxBalanceOutputTenancyExpr);
            this.Controls.Add(this.checkBoxBalanceOutputTenancyEnable);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.numericUpDownBalanceOutputTo);
            this.Controls.Add(this.numericUpDownBalanceOutputFrom);
            this.Controls.Add(this.comboBoxBalanceOutputExpr);
            this.Controls.Add(this.checkBoxBalanceOutputEnable);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.numericUpDownPaymentDGITo);
            this.Controls.Add(this.numericUpDownPaymentDGIFrom);
            this.Controls.Add(this.comboBoxPaymentDGIExpr);
            this.Controls.Add(this.checkBoxPaymentDGIEnable);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.numericUpDownPaymentTenancyTo);
            this.Controls.Add(this.numericUpDownPaymentTenancyFrom);
            this.Controls.Add(this.comboBoxPaymentTenancyExpr);
            this.Controls.Add(this.checkBoxPaymentTenancyEnable);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.numericUpDownTransferBalanceTo);
            this.Controls.Add(this.numericUpDownTransferBalanceFrom);
            this.Controls.Add(this.comboBoxTransferBalanceExpr);
            this.Controls.Add(this.checkBoxTransferBalanceEnable);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.numericUpDownRecalcDGITo);
            this.Controls.Add(this.numericUpDownRecalcDGIFrom);
            this.Controls.Add(this.comboBoxRecalcDGIExpr);
            this.Controls.Add(this.checkBoxRecalcDGIEnable);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.numericUpDownRecalcTenancyTo);
            this.Controls.Add(this.numericUpDownRecalcTenancyFrom);
            this.Controls.Add(this.comboBoxRecalcTenancyExpr);
            this.Controls.Add(this.checkBoxRecalcTenancyEnable);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.numericUpDownChargingDGITo);
            this.Controls.Add(this.numericUpDownChargingDGIFrom);
            this.Controls.Add(this.comboBoxChargingDGIExpr);
            this.Controls.Add(this.checkBoxChargingDGIEnable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.numericUpDownChargingTenancyTo);
            this.Controls.Add(this.numericUpDownChargingTenancyFrom);
            this.Controls.Add(this.comboBoxChargingTenancyExpr);
            this.Controls.Add(this.checkBoxChargingTenancyEnable);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.numericUpDownChargingTo);
            this.Controls.Add(this.numericUpDownChargingFrom);
            this.Controls.Add(this.comboBoxChargingExpr);
            this.Controls.Add(this.checkBoxChargingEnable);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.numericUpDownBalanceInputDGITo);
            this.Controls.Add(this.numericUpDownBalanceInputDGIFrom);
            this.Controls.Add(this.comboBoxBalanceInputDGIExpr);
            this.Controls.Add(this.checkBoxBalanceInputDGIEnable);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numericUpDownBalanceInputTenancyTo);
            this.Controls.Add(this.numericUpDownBalanceInputTenancyFrom);
            this.Controls.Add(this.comboBoxBalanceInputTenancyExpr);
            this.Controls.Add(this.checkBoxBalanceInputTenancyEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDownBalanceInputTo);
            this.Controls.Add(this.numericUpDownBalanceInputFrom);
            this.Controls.Add(this.comboBoxBalanceInputExpr);
            this.Controls.Add(this.checkBoxBalanceInputEnable);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxRawAddress);
            this.Controls.Add(this.checkBoxRawAddressEnable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxDateExpr);
            this.Controls.Add(this.dateTimePickerDate);
            this.Controls.Add(this.checkBoxDateEnable);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBoxHouse);
            this.Controls.Add(this.checkBoxPremisesNumEnable);
            this.Controls.Add(this.textBoxPremisesNum);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxHouseEnable);
            this.Controls.Add(this.checkBoxStreetEnable);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboBoxStreet);
            this.Controls.Add(this.textBoxTenantSNP);
            this.Controls.Add(this.checkBoxTenantSNPEnable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxAccount);
            this.Controls.Add(this.checkBoxAccountEnable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxCRN);
            this.Controls.Add(this.checkBoxCRNEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendedSearchPaymentAccounts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация лицевых счетов";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputTenancyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputTenancyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputDGITo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceInputDGIFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingDGITo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingDGIFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTenancyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTenancyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChargingFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferBalanceTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTransferBalanceFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcDGITo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcDGIFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcTenancyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecalcTenancyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentDGITo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentDGIFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentTenancyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPaymentTenancyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDGITo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDGIFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyFrom)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vButton vButtonCancel;
        private vButton vButtonSearch;
        private TextBox textBoxCRN;
        private CheckBox checkBoxCRNEnable;
        private Label label2;
        private TextBox textBoxAccount;
        private CheckBox checkBoxAccountEnable;
        private Label label1;
        private TextBox textBoxTenantSNP;
        private CheckBox checkBoxTenantSNPEnable;
        private Label label4;
        private TextBox textBoxHouse;
        private CheckBox checkBoxPremisesNumEnable;
        private TextBox textBoxPremisesNum;
        private Label label7;
        private CheckBox checkBoxHouseEnable;
        private CheckBox checkBoxStreetEnable;
        private Label label10;
        private Label label11;
        private ComboBox comboBoxStreet;
        private CheckBox checkBoxDateEnable;
        private Label label12;
        private DateTimePicker dateTimePickerDate;
        private ComboBox comboBoxDateExpr;
        private TextBox textBoxRawAddress;
        private CheckBox checkBoxRawAddressEnable;
        private Label label3;
        private ComboBox comboBoxBalanceInputExpr;
        private CheckBox checkBoxBalanceInputEnable;
        private Label label5;
        private NumericUpDown numericUpDownBalanceInputFrom;
        private NumericUpDown numericUpDownBalanceInputTo;
        private NumericUpDown numericUpDownBalanceInputTenancyTo;
        private NumericUpDown numericUpDownBalanceInputTenancyFrom;
        private ComboBox comboBoxBalanceInputTenancyExpr;
        private CheckBox checkBoxBalanceInputTenancyEnable;
        private Label label6;
        private NumericUpDown numericUpDownBalanceInputDGITo;
        private NumericUpDown numericUpDownBalanceInputDGIFrom;
        private ComboBox comboBoxBalanceInputDGIExpr;
        private CheckBox checkBoxBalanceInputDGIEnable;
        private Label label8;
        private NumericUpDown numericUpDownChargingDGITo;
        private NumericUpDown numericUpDownChargingDGIFrom;
        private ComboBox comboBoxChargingDGIExpr;
        private CheckBox checkBoxChargingDGIEnable;
        private Label label9;
        private NumericUpDown numericUpDownChargingTenancyTo;
        private NumericUpDown numericUpDownChargingTenancyFrom;
        private ComboBox comboBoxChargingTenancyExpr;
        private CheckBox checkBoxChargingTenancyEnable;
        private Label label13;
        private NumericUpDown numericUpDownChargingTo;
        private NumericUpDown numericUpDownChargingFrom;
        private ComboBox comboBoxChargingExpr;
        private CheckBox checkBoxChargingEnable;
        private Label label14;
        private NumericUpDown numericUpDownTransferBalanceTo;
        private NumericUpDown numericUpDownTransferBalanceFrom;
        private ComboBox comboBoxTransferBalanceExpr;
        private CheckBox checkBoxTransferBalanceEnable;
        private Label label15;
        private NumericUpDown numericUpDownRecalcDGITo;
        private NumericUpDown numericUpDownRecalcDGIFrom;
        private ComboBox comboBoxRecalcDGIExpr;
        private CheckBox checkBoxRecalcDGIEnable;
        private Label label16;
        private NumericUpDown numericUpDownRecalcTenancyTo;
        private NumericUpDown numericUpDownRecalcTenancyFrom;
        private ComboBox comboBoxRecalcTenancyExpr;
        private CheckBox checkBoxRecalcTenancyEnable;
        private Label label17;
        private NumericUpDown numericUpDownBalanceOutputTo;
        private NumericUpDown numericUpDownBalanceOutputFrom;
        private ComboBox comboBoxBalanceOutputExpr;
        private CheckBox checkBoxBalanceOutputEnable;
        private Label label18;
        private NumericUpDown numericUpDownPaymentDGITo;
        private NumericUpDown numericUpDownPaymentDGIFrom;
        private ComboBox comboBoxPaymentDGIExpr;
        private CheckBox checkBoxPaymentDGIEnable;
        private Label label19;
        private NumericUpDown numericUpDownPaymentTenancyTo;
        private NumericUpDown numericUpDownPaymentTenancyFrom;
        private ComboBox comboBoxPaymentTenancyExpr;
        private CheckBox checkBoxPaymentTenancyEnable;
        private Label label20;
        private NumericUpDown numericUpDownBalanceOutputDGITo;
        private NumericUpDown numericUpDownBalanceOutputDGIFrom;
        private ComboBox comboBoxBalanceOutputDGIExpr;
        private CheckBox checkBoxBalanceOutputDGIEnable;
        private Label label21;
        private NumericUpDown numericUpDownBalanceOutputTenancyTo;
        private NumericUpDown numericUpDownBalanceOutputTenancyFrom;
        private ComboBox comboBoxBalanceOutputTenancyExpr;
        private CheckBox checkBoxBalanceOutputTenancyEnable;
        private Label label22;
        private GroupBox groupBox1;
        private RadioButton radioButtonWithClaims;
        private RadioButton radioButtonWithoutClaims;
        private RadioButton radioButtonWithUncompletedClaims;
        private RadioButton radioButtonWithoutUncompletedClaims;
        private CheckBox checkBoxByClaimsChecked;
    }
}