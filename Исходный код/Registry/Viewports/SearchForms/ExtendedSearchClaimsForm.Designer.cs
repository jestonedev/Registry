using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.SearchForms
{
    partial class ExtendedSearchClaimsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedSearchClaimsForm));
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.dateTimePickerAtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label103 = new System.Windows.Forms.Label();
            this.label91 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.checkBoxAtDateChecked = new System.Windows.Forms.CheckBox();
            this.checkBoxStateEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxAccountEnable = new System.Windows.Forms.CheckBox();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.comboBoxAtDateExpr = new System.Windows.Forms.ComboBox();
            this.numericUpDownAmmountDGITo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownAmmountDGIFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxAmmountDGIExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxAmmountDGIChecked = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.numericUpDownAmmountTenancyTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownAmmountTenancyFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxAmmountTenancyExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxAmmountTenancyChecked = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.dateTimePickerAtDateTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStartDeptPeriodTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxStartDeptPeriodExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxStartDeptPeriodChecked = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerStartDeptPeriodFrom = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEndDeptPeriodTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxEndDeptPeriodExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxEndDeptPeriodChecked = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDeptPeriodFrom = new System.Windows.Forms.DateTimePicker();
            this.numericUpDownClaimId = new System.Windows.Forms.NumericUpDown();
            this.checkBoxClaimIdChecked = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerDateStartStateTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxDateStartStateExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxDateStartStateEnable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dateTimePickerDateStartStateFrom = new System.Windows.Forms.DateTimePicker();
            this.textBoxSRN = new System.Windows.Forms.TextBox();
            this.checkBoxSRNEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxCourtOrderNum = new System.Windows.Forms.TextBox();
            this.checkBoxCourtOrderNumEnable = new System.Windows.Forms.CheckBox();
            this.dateTimePickerObtainingCourtOrderDateTo = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxObtainingCourtOrderDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerObtainingCourtOrderDateFrom = new System.Windows.Forms.DateTimePicker();
            this.checkBoxObtainingCourtOrderDateEnable = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.dateTimePickerCourtOrderDateTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxCourtOrderDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerCourtOrderDateFrom = new System.Windows.Forms.DateTimePicker();
            this.checkBoxCourtOrderDateEnable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dateTimePickerClaimDirectionDateTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxClaimDirectionDateExpr = new System.Windows.Forms.ComboBox();
            this.dateTimePickerClaimDirectionDateFrom = new System.Windows.Forms.DateTimePicker();
            this.checkBoxClaimDirectionDateEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxLastState = new System.Windows.Forms.CheckBox();
            this.textBoxAcceptedByLegalDepartmentWho = new System.Windows.Forms.TextBox();
            this.checkBoxAcceptedByLegalDepartmentWhoEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxTransferedToLegalDepartmentWho = new System.Windows.Forms.TextBox();
            this.checkBoxTransferedToLegalDepartmentWhoEnable = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxBksRequester = new System.Windows.Forms.TextBox();
            this.checkBoxBksRequesterEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountDGITo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountDGIFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountTenancyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClaimId)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(683, 413);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 34;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(545, 413);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 33;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // dateTimePickerAtDateFrom
            // 
            this.dateTimePickerAtDateFrom.Enabled = false;
            this.dateTimePickerAtDateFrom.Location = new System.Drawing.Point(500, 147);
            this.dateTimePickerAtDateFrom.Name = "dateTimePickerAtDateFrom";
            this.dateTimePickerAtDateFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerAtDateFrom.TabIndex = 15;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(8, 45);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(139, 15);
            this.label103.TabIndex = 53;
            this.label103.Text = "Номер лицевого счета";
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(415, 129);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(130, 15);
            this.label91.TabIndex = 52;
            this.label91.Text = "Дата формирования";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 15);
            this.label1.TabIndex = 55;
            this.label1.Text = "Вид состояния";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.Enabled = false;
            this.comboBoxState.Location = new System.Drawing.Point(29, 40);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(339, 23);
            this.comboBoxState.TabIndex = 1;
            this.comboBoxState.SelectedValueChanged += new System.EventHandler(this.comboBoxState_SelectedValueChanged);
            // 
            // checkBoxAtDateChecked
            // 
            this.checkBoxAtDateChecked.AutoSize = true;
            this.checkBoxAtDateChecked.Location = new System.Drawing.Point(418, 151);
            this.checkBoxAtDateChecked.Name = "checkBoxAtDateChecked";
            this.checkBoxAtDateChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAtDateChecked.TabIndex = 13;
            this.checkBoxAtDateChecked.UseVisualStyleBackColor = true;
            this.checkBoxAtDateChecked.CheckedChanged += new System.EventHandler(this.checkBoxAtDateChecked_CheckedChanged);
            // 
            // checkBoxStateEnable
            // 
            this.checkBoxStateEnable.AutoSize = true;
            this.checkBoxStateEnable.Location = new System.Drawing.Point(8, 44);
            this.checkBoxStateEnable.Name = "checkBoxStateEnable";
            this.checkBoxStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStateEnable.TabIndex = 0;
            this.checkBoxStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxLastStateChecked_CheckedChanged);
            // 
            // checkBoxAccountEnable
            // 
            this.checkBoxAccountEnable.AutoSize = true;
            this.checkBoxAccountEnable.Location = new System.Drawing.Point(11, 67);
            this.checkBoxAccountEnable.Name = "checkBoxAccountEnable";
            this.checkBoxAccountEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAccountEnable.TabIndex = 2;
            this.checkBoxAccountEnable.UseVisualStyleBackColor = true;
            this.checkBoxAccountEnable.CheckedChanged += new System.EventHandler(this.checkBoxAccountChecked_CheckedChanged);
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Enabled = false;
            this.textBoxAccount.Location = new System.Drawing.Point(32, 63);
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(360, 21);
            this.textBoxAccount.TabIndex = 3;
            // 
            // comboBoxAtDateExpr
            // 
            this.comboBoxAtDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAtDateExpr.Enabled = false;
            this.comboBoxAtDateExpr.FormattingEnabled = true;
            this.comboBoxAtDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxAtDateExpr.Location = new System.Drawing.Point(440, 146);
            this.comboBoxAtDateExpr.Name = "comboBoxAtDateExpr";
            this.comboBoxAtDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxAtDateExpr.TabIndex = 14;
            // 
            // numericUpDownAmmountDGITo
            // 
            this.numericUpDownAmmountDGITo.DecimalPlaces = 2;
            this.numericUpDownAmmountDGITo.Enabled = false;
            this.numericUpDownAmmountDGITo.Location = new System.Drawing.Point(651, 319);
            this.numericUpDownAmmountDGITo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountDGITo.Name = "numericUpDownAmmountDGITo";
            this.numericUpDownAmmountDGITo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownAmmountDGITo.TabIndex = 32;
            // 
            // numericUpDownAmmountDGIFrom
            // 
            this.numericUpDownAmmountDGIFrom.DecimalPlaces = 2;
            this.numericUpDownAmmountDGIFrom.Enabled = false;
            this.numericUpDownAmmountDGIFrom.Location = new System.Drawing.Point(500, 319);
            this.numericUpDownAmmountDGIFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountDGIFrom.Name = "numericUpDownAmmountDGIFrom";
            this.numericUpDownAmmountDGIFrom.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownAmmountDGIFrom.TabIndex = 31;
            // 
            // comboBoxAmmountDGIExpr
            // 
            this.comboBoxAmmountDGIExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAmmountDGIExpr.Enabled = false;
            this.comboBoxAmmountDGIExpr.FormattingEnabled = true;
            this.comboBoxAmmountDGIExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxAmmountDGIExpr.Location = new System.Drawing.Point(439, 318);
            this.comboBoxAmmountDGIExpr.Name = "comboBoxAmmountDGIExpr";
            this.comboBoxAmmountDGIExpr.Size = new System.Drawing.Size(49, 23);
            this.comboBoxAmmountDGIExpr.TabIndex = 30;
            // 
            // checkBoxAmmountDGIChecked
            // 
            this.checkBoxAmmountDGIChecked.AutoSize = true;
            this.checkBoxAmmountDGIChecked.Location = new System.Drawing.Point(418, 323);
            this.checkBoxAmmountDGIChecked.Name = "checkBoxAmmountDGIChecked";
            this.checkBoxAmmountDGIChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAmmountDGIChecked.TabIndex = 29;
            this.checkBoxAmmountDGIChecked.UseVisualStyleBackColor = true;
            this.checkBoxAmmountDGIChecked.CheckedChanged += new System.EventHandler(this.checkBoxAmmountDGIChecked_CheckedChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(415, 301);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(149, 15);
            this.label21.TabIndex = 176;
            this.label21.Text = "Сумма к взысканию ДГИ";
            // 
            // numericUpDownAmmountTenancyTo
            // 
            this.numericUpDownAmmountTenancyTo.DecimalPlaces = 2;
            this.numericUpDownAmmountTenancyTo.Enabled = false;
            this.numericUpDownAmmountTenancyTo.Location = new System.Drawing.Point(651, 276);
            this.numericUpDownAmmountTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountTenancyTo.Name = "numericUpDownAmmountTenancyTo";
            this.numericUpDownAmmountTenancyTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownAmmountTenancyTo.TabIndex = 28;
            // 
            // numericUpDownAmmountTenancyFrom
            // 
            this.numericUpDownAmmountTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownAmmountTenancyFrom.Enabled = false;
            this.numericUpDownAmmountTenancyFrom.Location = new System.Drawing.Point(500, 276);
            this.numericUpDownAmmountTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountTenancyFrom.Name = "numericUpDownAmmountTenancyFrom";
            this.numericUpDownAmmountTenancyFrom.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownAmmountTenancyFrom.TabIndex = 27;
            // 
            // comboBoxAmmountTenancyExpr
            // 
            this.comboBoxAmmountTenancyExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAmmountTenancyExpr.Enabled = false;
            this.comboBoxAmmountTenancyExpr.FormattingEnabled = true;
            this.comboBoxAmmountTenancyExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxAmmountTenancyExpr.Location = new System.Drawing.Point(439, 275);
            this.comboBoxAmmountTenancyExpr.Name = "comboBoxAmmountTenancyExpr";
            this.comboBoxAmmountTenancyExpr.Size = new System.Drawing.Size(49, 23);
            this.comboBoxAmmountTenancyExpr.TabIndex = 26;
            // 
            // checkBoxAmmountTenancyChecked
            // 
            this.checkBoxAmmountTenancyChecked.AutoSize = true;
            this.checkBoxAmmountTenancyChecked.Location = new System.Drawing.Point(418, 280);
            this.checkBoxAmmountTenancyChecked.Name = "checkBoxAmmountTenancyChecked";
            this.checkBoxAmmountTenancyChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAmmountTenancyChecked.TabIndex = 25;
            this.checkBoxAmmountTenancyChecked.UseVisualStyleBackColor = true;
            this.checkBoxAmmountTenancyChecked.CheckedChanged += new System.EventHandler(this.checkBoxAmmountTenancyChecked_CheckedChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(415, 258);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(154, 15);
            this.label22.TabIndex = 175;
            this.label22.Text = "Сумма к взысканию найм";
            // 
            // dateTimePickerAtDateTo
            // 
            this.dateTimePickerAtDateTo.Enabled = false;
            this.dateTimePickerAtDateTo.Location = new System.Drawing.Point(651, 147);
            this.dateTimePickerAtDateTo.Name = "dateTimePickerAtDateTo";
            this.dateTimePickerAtDateTo.Size = new System.Drawing.Size(148, 21);
            this.dateTimePickerAtDateTo.TabIndex = 16;
            // 
            // dateTimePickerStartDeptPeriodTo
            // 
            this.dateTimePickerStartDeptPeriodTo.Enabled = false;
            this.dateTimePickerStartDeptPeriodTo.Location = new System.Drawing.Point(651, 190);
            this.dateTimePickerStartDeptPeriodTo.Name = "dateTimePickerStartDeptPeriodTo";
            this.dateTimePickerStartDeptPeriodTo.Size = new System.Drawing.Size(148, 21);
            this.dateTimePickerStartDeptPeriodTo.TabIndex = 20;
            // 
            // comboBoxStartDeptPeriodExpr
            // 
            this.comboBoxStartDeptPeriodExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStartDeptPeriodExpr.Enabled = false;
            this.comboBoxStartDeptPeriodExpr.FormattingEnabled = true;
            this.comboBoxStartDeptPeriodExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxStartDeptPeriodExpr.Location = new System.Drawing.Point(440, 189);
            this.comboBoxStartDeptPeriodExpr.Name = "comboBoxStartDeptPeriodExpr";
            this.comboBoxStartDeptPeriodExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxStartDeptPeriodExpr.TabIndex = 18;
            // 
            // checkBoxStartDeptPeriodChecked
            // 
            this.checkBoxStartDeptPeriodChecked.AutoSize = true;
            this.checkBoxStartDeptPeriodChecked.Location = new System.Drawing.Point(418, 194);
            this.checkBoxStartDeptPeriodChecked.Name = "checkBoxStartDeptPeriodChecked";
            this.checkBoxStartDeptPeriodChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStartDeptPeriodChecked.TabIndex = 17;
            this.checkBoxStartDeptPeriodChecked.UseVisualStyleBackColor = true;
            this.checkBoxStartDeptPeriodChecked.CheckedChanged += new System.EventHandler(this.checkBoxStartDeptPeriodChecked_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(415, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(275, 15);
            this.label2.TabIndex = 180;
            this.label2.Text = "Предъявленный период (дата начала период)";
            // 
            // dateTimePickerStartDeptPeriodFrom
            // 
            this.dateTimePickerStartDeptPeriodFrom.Enabled = false;
            this.dateTimePickerStartDeptPeriodFrom.Location = new System.Drawing.Point(500, 190);
            this.dateTimePickerStartDeptPeriodFrom.Name = "dateTimePickerStartDeptPeriodFrom";
            this.dateTimePickerStartDeptPeriodFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerStartDeptPeriodFrom.TabIndex = 19;
            // 
            // dateTimePickerEndDeptPeriodTo
            // 
            this.dateTimePickerEndDeptPeriodTo.Enabled = false;
            this.dateTimePickerEndDeptPeriodTo.Location = new System.Drawing.Point(651, 233);
            this.dateTimePickerEndDeptPeriodTo.Name = "dateTimePickerEndDeptPeriodTo";
            this.dateTimePickerEndDeptPeriodTo.Size = new System.Drawing.Size(148, 21);
            this.dateTimePickerEndDeptPeriodTo.TabIndex = 24;
            // 
            // comboBoxEndDeptPeriodExpr
            // 
            this.comboBoxEndDeptPeriodExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEndDeptPeriodExpr.Enabled = false;
            this.comboBoxEndDeptPeriodExpr.FormattingEnabled = true;
            this.comboBoxEndDeptPeriodExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxEndDeptPeriodExpr.Location = new System.Drawing.Point(440, 232);
            this.comboBoxEndDeptPeriodExpr.Name = "comboBoxEndDeptPeriodExpr";
            this.comboBoxEndDeptPeriodExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxEndDeptPeriodExpr.TabIndex = 22;
            // 
            // checkBoxEndDeptPeriodChecked
            // 
            this.checkBoxEndDeptPeriodChecked.AutoSize = true;
            this.checkBoxEndDeptPeriodChecked.Location = new System.Drawing.Point(418, 237);
            this.checkBoxEndDeptPeriodChecked.Name = "checkBoxEndDeptPeriodChecked";
            this.checkBoxEndDeptPeriodChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxEndDeptPeriodChecked.TabIndex = 21;
            this.checkBoxEndDeptPeriodChecked.UseVisualStyleBackColor = true;
            this.checkBoxEndDeptPeriodChecked.CheckedChanged += new System.EventHandler(this.checkBoxEndDeptPeriodChecked_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(415, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(295, 15);
            this.label3.TabIndex = 185;
            this.label3.Text = "Предъявленный период (дата окончания период)";
            // 
            // dateTimePickerEndDeptPeriodFrom
            // 
            this.dateTimePickerEndDeptPeriodFrom.Enabled = false;
            this.dateTimePickerEndDeptPeriodFrom.Location = new System.Drawing.Point(500, 233);
            this.dateTimePickerEndDeptPeriodFrom.Name = "dateTimePickerEndDeptPeriodFrom";
            this.dateTimePickerEndDeptPeriodFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerEndDeptPeriodFrom.TabIndex = 23;
            // 
            // numericUpDownClaimId
            // 
            this.numericUpDownClaimId.Enabled = false;
            this.numericUpDownClaimId.Location = new System.Drawing.Point(33, 21);
            this.numericUpDownClaimId.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownClaimId.Name = "numericUpDownClaimId";
            this.numericUpDownClaimId.Size = new System.Drawing.Size(360, 21);
            this.numericUpDownClaimId.TabIndex = 1;
            // 
            // checkBoxClaimIdChecked
            // 
            this.checkBoxClaimIdChecked.AutoSize = true;
            this.checkBoxClaimIdChecked.Location = new System.Drawing.Point(12, 25);
            this.checkBoxClaimIdChecked.Name = "checkBoxClaimIdChecked";
            this.checkBoxClaimIdChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxClaimIdChecked.TabIndex = 0;
            this.checkBoxClaimIdChecked.UseVisualStyleBackColor = true;
            this.checkBoxClaimIdChecked.CheckedChanged += new System.EventHandler(this.checkBoxClaimIdChecked_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 15);
            this.label4.TabIndex = 188;
            this.label4.Text = "Внутренний номер";
            // 
            // dateTimePickerDateStartStateTo
            // 
            this.dateTimePickerDateStartStateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateStartStateTo.Enabled = false;
            this.dateTimePickerDateStartStateTo.Location = new System.Drawing.Point(233, 85);
            this.dateTimePickerDateStartStateTo.Name = "dateTimePickerDateStartStateTo";
            this.dateTimePickerDateStartStateTo.Size = new System.Drawing.Size(131, 21);
            this.dateTimePickerDateStartStateTo.TabIndex = 5;
            // 
            // comboBoxDateStartStateExpr
            // 
            this.comboBoxDateStartStateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDateStartStateExpr.Enabled = false;
            this.comboBoxDateStartStateExpr.FormattingEnabled = true;
            this.comboBoxDateStartStateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxDateStartStateExpr.Location = new System.Drawing.Point(30, 84);
            this.comboBoxDateStartStateExpr.Name = "comboBoxDateStartStateExpr";
            this.comboBoxDateStartStateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxDateStartStateExpr.TabIndex = 3;
            // 
            // checkBoxDateStartStateEnable
            // 
            this.checkBoxDateStartStateEnable.AutoSize = true;
            this.checkBoxDateStartStateEnable.Location = new System.Drawing.Point(8, 88);
            this.checkBoxDateStartStateEnable.Name = "checkBoxDateStartStateEnable";
            this.checkBoxDateStartStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDateStartStateEnable.TabIndex = 2;
            this.checkBoxDateStartStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxDateStartStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxDateStartStateEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 15);
            this.label5.TabIndex = 193;
            this.label5.Text = "Дата установки";
            // 
            // dateTimePickerDateStartStateFrom
            // 
            this.dateTimePickerDateStartStateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateStartStateFrom.Enabled = false;
            this.dateTimePickerDateStartStateFrom.Location = new System.Drawing.Point(90, 85);
            this.dateTimePickerDateStartStateFrom.Name = "dateTimePickerDateStartStateFrom";
            this.dateTimePickerDateStartStateFrom.Size = new System.Drawing.Size(134, 21);
            this.dateTimePickerDateStartStateFrom.TabIndex = 4;
            // 
            // textBoxSRN
            // 
            this.textBoxSRN.Enabled = false;
            this.textBoxSRN.Location = new System.Drawing.Point(33, 106);
            this.textBoxSRN.Name = "textBoxSRN";
            this.textBoxSRN.Size = new System.Drawing.Size(360, 21);
            this.textBoxSRN.TabIndex = 5;
            // 
            // checkBoxSRNEnable
            // 
            this.checkBoxSRNEnable.AutoSize = true;
            this.checkBoxSRNEnable.Location = new System.Drawing.Point(12, 110);
            this.checkBoxSRNEnable.Name = "checkBoxSRNEnable";
            this.checkBoxSRNEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSRNEnable.TabIndex = 4;
            this.checkBoxSRNEnable.UseVisualStyleBackColor = true;
            this.checkBoxSRNEnable.CheckedChanged += new System.EventHandler(this.checkBoxSRNChecked_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 15);
            this.label6.TabIndex = 196;
            this.label6.Text = "СРН";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.textBoxCourtOrderNum);
            this.groupBox1.Controls.Add(this.checkBoxCourtOrderNumEnable);
            this.groupBox1.Controls.Add(this.dateTimePickerObtainingCourtOrderDateTo);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.comboBoxObtainingCourtOrderDateExpr);
            this.groupBox1.Controls.Add(this.dateTimePickerObtainingCourtOrderDateFrom);
            this.groupBox1.Controls.Add(this.checkBoxObtainingCourtOrderDateEnable);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.dateTimePickerCourtOrderDateTo);
            this.groupBox1.Controls.Add(this.comboBoxCourtOrderDateExpr);
            this.groupBox1.Controls.Add(this.dateTimePickerCourtOrderDateFrom);
            this.groupBox1.Controls.Add(this.checkBoxCourtOrderDateEnable);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.dateTimePickerClaimDirectionDateTo);
            this.groupBox1.Controls.Add(this.comboBoxClaimDirectionDateExpr);
            this.groupBox1.Controls.Add(this.dateTimePickerClaimDirectionDateFrom);
            this.groupBox1.Controls.Add(this.checkBoxClaimDirectionDateEnable);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.checkBoxLastState);
            this.groupBox1.Controls.Add(this.dateTimePickerDateStartStateTo);
            this.groupBox1.Controls.Add(this.comboBoxState);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxStateEnable);
            this.groupBox1.Controls.Add(this.comboBoxDateStartStateExpr);
            this.groupBox1.Controls.Add(this.dateTimePickerDateStartStateFrom);
            this.groupBox1.Controls.Add(this.checkBoxDateStartStateEnable);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 133);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 313);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Состояние";
            // 
            // textBoxCourtOrderNum
            // 
            this.textBoxCourtOrderNum.Enabled = false;
            this.textBoxCourtOrderNum.Location = new System.Drawing.Point(30, 254);
            this.textBoxCourtOrderNum.MaxLength = 255;
            this.textBoxCourtOrderNum.Name = "textBoxCourtOrderNum";
            this.textBoxCourtOrderNum.Size = new System.Drawing.Size(334, 21);
            this.textBoxCourtOrderNum.TabIndex = 19;
            // 
            // checkBoxCourtOrderNumEnable
            // 
            this.checkBoxCourtOrderNumEnable.AutoSize = true;
            this.checkBoxCourtOrderNumEnable.Location = new System.Drawing.Point(9, 258);
            this.checkBoxCourtOrderNumEnable.Name = "checkBoxCourtOrderNumEnable";
            this.checkBoxCourtOrderNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCourtOrderNumEnable.TabIndex = 18;
            this.checkBoxCourtOrderNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxCourtOrderNumEnable.CheckStateChanged += new System.EventHandler(this.checkBoxCourtOrderNumEnable_CheckStateChanged);
            // 
            // dateTimePickerObtainingCourtOrderDateTo
            // 
            this.dateTimePickerObtainingCourtOrderDateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerObtainingCourtOrderDateTo.Enabled = false;
            this.dateTimePickerObtainingCourtOrderDateTo.Location = new System.Drawing.Point(233, 212);
            this.dateTimePickerObtainingCourtOrderDateTo.Name = "dateTimePickerObtainingCourtOrderDateTo";
            this.dateTimePickerObtainingCourtOrderDateTo.Size = new System.Drawing.Size(131, 21);
            this.dateTimePickerObtainingCourtOrderDateTo.TabIndex = 17;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 236);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(157, 15);
            this.label13.TabIndex = 208;
            this.label13.Text = "Номер судебного приказа";
            // 
            // comboBoxObtainingCourtOrderDateExpr
            // 
            this.comboBoxObtainingCourtOrderDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxObtainingCourtOrderDateExpr.Enabled = false;
            this.comboBoxObtainingCourtOrderDateExpr.FormattingEnabled = true;
            this.comboBoxObtainingCourtOrderDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxObtainingCourtOrderDateExpr.Location = new System.Drawing.Point(30, 211);
            this.comboBoxObtainingCourtOrderDateExpr.Name = "comboBoxObtainingCourtOrderDateExpr";
            this.comboBoxObtainingCourtOrderDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxObtainingCourtOrderDateExpr.TabIndex = 15;
            // 
            // dateTimePickerObtainingCourtOrderDateFrom
            // 
            this.dateTimePickerObtainingCourtOrderDateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerObtainingCourtOrderDateFrom.Enabled = false;
            this.dateTimePickerObtainingCourtOrderDateFrom.Location = new System.Drawing.Point(90, 212);
            this.dateTimePickerObtainingCourtOrderDateFrom.Name = "dateTimePickerObtainingCourtOrderDateFrom";
            this.dateTimePickerObtainingCourtOrderDateFrom.Size = new System.Drawing.Size(134, 21);
            this.dateTimePickerObtainingCourtOrderDateFrom.TabIndex = 16;
            // 
            // checkBoxObtainingCourtOrderDateEnable
            // 
            this.checkBoxObtainingCourtOrderDateEnable.AutoSize = true;
            this.checkBoxObtainingCourtOrderDateEnable.Location = new System.Drawing.Point(8, 215);
            this.checkBoxObtainingCourtOrderDateEnable.Name = "checkBoxObtainingCourtOrderDateEnable";
            this.checkBoxObtainingCourtOrderDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxObtainingCourtOrderDateEnable.TabIndex = 14;
            this.checkBoxObtainingCourtOrderDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxObtainingCourtOrderDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxObtainingCourtOrderDateEnable_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 194);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 15);
            this.label10.TabIndex = 208;
            this.label10.Text = "Дата получения с/п";
            // 
            // dateTimePickerCourtOrderDateTo
            // 
            this.dateTimePickerCourtOrderDateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerCourtOrderDateTo.Enabled = false;
            this.dateTimePickerCourtOrderDateTo.Location = new System.Drawing.Point(233, 170);
            this.dateTimePickerCourtOrderDateTo.Name = "dateTimePickerCourtOrderDateTo";
            this.dateTimePickerCourtOrderDateTo.Size = new System.Drawing.Size(131, 21);
            this.dateTimePickerCourtOrderDateTo.TabIndex = 13;
            // 
            // comboBoxCourtOrderDateExpr
            // 
            this.comboBoxCourtOrderDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCourtOrderDateExpr.Enabled = false;
            this.comboBoxCourtOrderDateExpr.FormattingEnabled = true;
            this.comboBoxCourtOrderDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxCourtOrderDateExpr.Location = new System.Drawing.Point(30, 169);
            this.comboBoxCourtOrderDateExpr.Name = "comboBoxCourtOrderDateExpr";
            this.comboBoxCourtOrderDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxCourtOrderDateExpr.TabIndex = 11;
            // 
            // dateTimePickerCourtOrderDateFrom
            // 
            this.dateTimePickerCourtOrderDateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerCourtOrderDateFrom.Enabled = false;
            this.dateTimePickerCourtOrderDateFrom.Location = new System.Drawing.Point(90, 170);
            this.dateTimePickerCourtOrderDateFrom.Name = "dateTimePickerCourtOrderDateFrom";
            this.dateTimePickerCourtOrderDateFrom.Size = new System.Drawing.Size(134, 21);
            this.dateTimePickerCourtOrderDateFrom.TabIndex = 12;
            // 
            // checkBoxCourtOrderDateEnable
            // 
            this.checkBoxCourtOrderDateEnable.AutoSize = true;
            this.checkBoxCourtOrderDateEnable.Location = new System.Drawing.Point(8, 173);
            this.checkBoxCourtOrderDateEnable.Name = "checkBoxCourtOrderDateEnable";
            this.checkBoxCourtOrderDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCourtOrderDateEnable.TabIndex = 10;
            this.checkBoxCourtOrderDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxCourtOrderDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxCourtOrderDateEnable_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 152);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(123, 15);
            this.label9.TabIndex = 203;
            this.label9.Text = "Дата вынесения с/п";
            // 
            // dateTimePickerClaimDirectionDateTo
            // 
            this.dateTimePickerClaimDirectionDateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerClaimDirectionDateTo.Enabled = false;
            this.dateTimePickerClaimDirectionDateTo.Location = new System.Drawing.Point(233, 128);
            this.dateTimePickerClaimDirectionDateTo.Name = "dateTimePickerClaimDirectionDateTo";
            this.dateTimePickerClaimDirectionDateTo.Size = new System.Drawing.Size(131, 21);
            this.dateTimePickerClaimDirectionDateTo.TabIndex = 9;
            // 
            // comboBoxClaimDirectionDateExpr
            // 
            this.comboBoxClaimDirectionDateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClaimDirectionDateExpr.Enabled = false;
            this.comboBoxClaimDirectionDateExpr.FormattingEnabled = true;
            this.comboBoxClaimDirectionDateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxClaimDirectionDateExpr.Location = new System.Drawing.Point(30, 127);
            this.comboBoxClaimDirectionDateExpr.Name = "comboBoxClaimDirectionDateExpr";
            this.comboBoxClaimDirectionDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxClaimDirectionDateExpr.TabIndex = 7;
            // 
            // dateTimePickerClaimDirectionDateFrom
            // 
            this.dateTimePickerClaimDirectionDateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerClaimDirectionDateFrom.Enabled = false;
            this.dateTimePickerClaimDirectionDateFrom.Location = new System.Drawing.Point(90, 128);
            this.dateTimePickerClaimDirectionDateFrom.Name = "dateTimePickerClaimDirectionDateFrom";
            this.dateTimePickerClaimDirectionDateFrom.Size = new System.Drawing.Size(134, 21);
            this.dateTimePickerClaimDirectionDateFrom.TabIndex = 8;
            // 
            // checkBoxClaimDirectionDateEnable
            // 
            this.checkBoxClaimDirectionDateEnable.AutoSize = true;
            this.checkBoxClaimDirectionDateEnable.Location = new System.Drawing.Point(8, 131);
            this.checkBoxClaimDirectionDateEnable.Name = "checkBoxClaimDirectionDateEnable";
            this.checkBoxClaimDirectionDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxClaimDirectionDateEnable.TabIndex = 6;
            this.checkBoxClaimDirectionDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxClaimDirectionDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxClaimDirectionDateEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 15);
            this.label8.TabIndex = 198;
            this.label8.Text = "Дата направления";
            // 
            // checkBoxLastState
            // 
            this.checkBoxLastState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxLastState.AutoSize = true;
            this.checkBoxLastState.Location = new System.Drawing.Point(9, 283);
            this.checkBoxLastState.Name = "checkBoxLastState";
            this.checkBoxLastState.Size = new System.Drawing.Size(292, 19);
            this.checkBoxLastState.TabIndex = 20;
            this.checkBoxLastState.Text = "Фильтровать только по текущему состоянию";
            this.checkBoxLastState.UseVisualStyleBackColor = true;
            // 
            // textBoxAcceptedByLegalDepartmentWho
            // 
            this.textBoxAcceptedByLegalDepartmentWho.Enabled = false;
            this.textBoxAcceptedByLegalDepartmentWho.Location = new System.Drawing.Point(439, 102);
            this.textBoxAcceptedByLegalDepartmentWho.MaxLength = 255;
            this.textBoxAcceptedByLegalDepartmentWho.Name = "textBoxAcceptedByLegalDepartmentWho";
            this.textBoxAcceptedByLegalDepartmentWho.Size = new System.Drawing.Size(360, 21);
            this.textBoxAcceptedByLegalDepartmentWho.TabIndex = 12;
            // 
            // checkBoxAcceptedByLegalDepartmentWhoEnable
            // 
            this.checkBoxAcceptedByLegalDepartmentWhoEnable.AutoSize = true;
            this.checkBoxAcceptedByLegalDepartmentWhoEnable.Location = new System.Drawing.Point(418, 106);
            this.checkBoxAcceptedByLegalDepartmentWhoEnable.Name = "checkBoxAcceptedByLegalDepartmentWhoEnable";
            this.checkBoxAcceptedByLegalDepartmentWhoEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAcceptedByLegalDepartmentWhoEnable.TabIndex = 11;
            this.checkBoxAcceptedByLegalDepartmentWhoEnable.UseVisualStyleBackColor = true;
            this.checkBoxAcceptedByLegalDepartmentWhoEnable.CheckedChanged += new System.EventHandler(this.checkBoxAcceptedByLegalDepartmentWhoEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(415, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 15);
            this.label7.TabIndex = 199;
            this.label7.Text = "Кто принял работу в юр. отдел";
            // 
            // textBoxTransferedToLegalDepartmentWho
            // 
            this.textBoxTransferedToLegalDepartmentWho.Enabled = false;
            this.textBoxTransferedToLegalDepartmentWho.Location = new System.Drawing.Point(439, 62);
            this.textBoxTransferedToLegalDepartmentWho.MaxLength = 255;
            this.textBoxTransferedToLegalDepartmentWho.Name = "textBoxTransferedToLegalDepartmentWho";
            this.textBoxTransferedToLegalDepartmentWho.Size = new System.Drawing.Size(360, 21);
            this.textBoxTransferedToLegalDepartmentWho.TabIndex = 10;
            // 
            // checkBoxTransferedToLegalDepartmentWhoEnable
            // 
            this.checkBoxTransferedToLegalDepartmentWhoEnable.AutoSize = true;
            this.checkBoxTransferedToLegalDepartmentWhoEnable.Location = new System.Drawing.Point(418, 66);
            this.checkBoxTransferedToLegalDepartmentWhoEnable.Name = "checkBoxTransferedToLegalDepartmentWhoEnable";
            this.checkBoxTransferedToLegalDepartmentWhoEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTransferedToLegalDepartmentWhoEnable.TabIndex = 9;
            this.checkBoxTransferedToLegalDepartmentWhoEnable.UseVisualStyleBackColor = true;
            this.checkBoxTransferedToLegalDepartmentWhoEnable.CheckedChanged += new System.EventHandler(this.checkBoxTransferedToLegalDepartmentWhoEnable_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(415, 44);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(194, 15);
            this.label11.TabIndex = 202;
            this.label11.Text = "Кто передал работу в юр. отдел";
            // 
            // textBoxBksRequester
            // 
            this.textBoxBksRequester.Enabled = false;
            this.textBoxBksRequester.Location = new System.Drawing.Point(439, 21);
            this.textBoxBksRequester.MaxLength = 255;
            this.textBoxBksRequester.Name = "textBoxBksRequester";
            this.textBoxBksRequester.Size = new System.Drawing.Size(360, 21);
            this.textBoxBksRequester.TabIndex = 8;
            // 
            // checkBoxBksRequesterEnable
            // 
            this.checkBoxBksRequesterEnable.AutoSize = true;
            this.checkBoxBksRequesterEnable.Location = new System.Drawing.Point(418, 25);
            this.checkBoxBksRequesterEnable.Name = "checkBoxBksRequesterEnable";
            this.checkBoxBksRequesterEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBksRequesterEnable.TabIndex = 7;
            this.checkBoxBksRequesterEnable.UseVisualStyleBackColor = true;
            this.checkBoxBksRequesterEnable.CheckedChanged += new System.EventHandler(this.checkBoxBksRequesterEnable_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(415, 3);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(153, 15);
            this.label12.TabIndex = 205;
            this.label12.Text = "Кто сделал запрос в БКС";
            // 
            // ExtendedSearchClaimsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(811, 457);
            this.Controls.Add(this.textBoxBksRequester);
            this.Controls.Add(this.checkBoxBksRequesterEnable);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBoxTransferedToLegalDepartmentWho);
            this.Controls.Add(this.checkBoxTransferedToLegalDepartmentWhoEnable);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBoxAcceptedByLegalDepartmentWho);
            this.Controls.Add(this.checkBoxAcceptedByLegalDepartmentWhoEnable);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxSRN);
            this.Controls.Add(this.checkBoxSRNEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDownClaimId);
            this.Controls.Add(this.checkBoxClaimIdChecked);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dateTimePickerEndDeptPeriodTo);
            this.Controls.Add(this.comboBoxEndDeptPeriodExpr);
            this.Controls.Add(this.checkBoxEndDeptPeriodChecked);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateTimePickerEndDeptPeriodFrom);
            this.Controls.Add(this.dateTimePickerStartDeptPeriodTo);
            this.Controls.Add(this.comboBoxStartDeptPeriodExpr);
            this.Controls.Add(this.checkBoxStartDeptPeriodChecked);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePickerStartDeptPeriodFrom);
            this.Controls.Add(this.dateTimePickerAtDateTo);
            this.Controls.Add(this.numericUpDownAmmountDGITo);
            this.Controls.Add(this.numericUpDownAmmountDGIFrom);
            this.Controls.Add(this.comboBoxAmmountDGIExpr);
            this.Controls.Add(this.checkBoxAmmountDGIChecked);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.numericUpDownAmmountTenancyTo);
            this.Controls.Add(this.numericUpDownAmmountTenancyFrom);
            this.Controls.Add(this.comboBoxAmmountTenancyExpr);
            this.Controls.Add(this.checkBoxAmmountTenancyChecked);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.comboBoxAtDateExpr);
            this.Controls.Add(this.textBoxAccount);
            this.Controls.Add(this.checkBoxAccountEnable);
            this.Controls.Add(this.checkBoxAtDateChecked);
            this.Controls.Add(this.label103);
            this.Controls.Add(this.label91);
            this.Controls.Add(this.dateTimePickerAtDateFrom);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendedSearchClaimsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация претензионно-исковых работ";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountDGITo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountDGIFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountTenancyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountTenancyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClaimId)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vButton vButtonCancel;
        private vButton vButtonSearch;
        private DateTimePicker dateTimePickerAtDateFrom;
        private Label label103;
        private Label label91;
        private Label label1;
        private ComboBox comboBoxState;
        private CheckBox checkBoxAtDateChecked;
        private CheckBox checkBoxStateEnable;
        private CheckBox checkBoxAccountEnable;
        private TextBox textBoxAccount;
        private ComboBox comboBoxAtDateExpr;
        private NumericUpDown numericUpDownAmmountDGITo;
        private NumericUpDown numericUpDownAmmountDGIFrom;
        private ComboBox comboBoxAmmountDGIExpr;
        private CheckBox checkBoxAmmountDGIChecked;
        private Label label21;
        private NumericUpDown numericUpDownAmmountTenancyTo;
        private NumericUpDown numericUpDownAmmountTenancyFrom;
        private ComboBox comboBoxAmmountTenancyExpr;
        private CheckBox checkBoxAmmountTenancyChecked;
        private Label label22;
        private DateTimePicker dateTimePickerAtDateTo;
        private DateTimePicker dateTimePickerStartDeptPeriodTo;
        private ComboBox comboBoxStartDeptPeriodExpr;
        private CheckBox checkBoxStartDeptPeriodChecked;
        private Label label2;
        private DateTimePicker dateTimePickerStartDeptPeriodFrom;
        private DateTimePicker dateTimePickerEndDeptPeriodTo;
        private ComboBox comboBoxEndDeptPeriodExpr;
        private CheckBox checkBoxEndDeptPeriodChecked;
        private Label label3;
        private DateTimePicker dateTimePickerEndDeptPeriodFrom;
        private NumericUpDown numericUpDownClaimId;
        private CheckBox checkBoxClaimIdChecked;
        private Label label4;
        private DateTimePicker dateTimePickerDateStartStateTo;
        private ComboBox comboBoxDateStartStateExpr;
        private CheckBox checkBoxDateStartStateEnable;
        private Label label5;
        private DateTimePicker dateTimePickerDateStartStateFrom;
        private TextBox textBoxSRN;
        private CheckBox checkBoxSRNEnable;
        private Label label6;
        private GroupBox groupBox1;
        private CheckBox checkBoxLastState;
        private DateTimePicker dateTimePickerObtainingCourtOrderDateTo;
        private ComboBox comboBoxObtainingCourtOrderDateExpr;
        private DateTimePicker dateTimePickerObtainingCourtOrderDateFrom;
        private CheckBox checkBoxObtainingCourtOrderDateEnable;
        private Label label10;
        private DateTimePicker dateTimePickerCourtOrderDateTo;
        private ComboBox comboBoxCourtOrderDateExpr;
        private DateTimePicker dateTimePickerCourtOrderDateFrom;
        private CheckBox checkBoxCourtOrderDateEnable;
        private Label label9;
        private DateTimePicker dateTimePickerClaimDirectionDateTo;
        private ComboBox comboBoxClaimDirectionDateExpr;
        private DateTimePicker dateTimePickerClaimDirectionDateFrom;
        private CheckBox checkBoxClaimDirectionDateEnable;
        private Label label8;
        private TextBox textBoxAcceptedByLegalDepartmentWho;
        private CheckBox checkBoxAcceptedByLegalDepartmentWhoEnable;
        private Label label7;
        private TextBox textBoxTransferedToLegalDepartmentWho;
        private CheckBox checkBoxTransferedToLegalDepartmentWhoEnable;
        private Label label11;
        private TextBox textBoxBksRequester;
        private CheckBox checkBoxBksRequesterEnable;
        private Label label12;
        private TextBox textBoxCourtOrderNum;
        private CheckBox checkBoxCourtOrderNumEnable;
        private Label label13;
    }
}