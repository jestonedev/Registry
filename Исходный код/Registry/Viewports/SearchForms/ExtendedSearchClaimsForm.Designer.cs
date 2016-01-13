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
            this.comboBoxLastState = new System.Windows.Forms.ComboBox();
            this.checkBoxAtDateChecked = new System.Windows.Forms.CheckBox();
            this.checkBoxLastStateChecked = new System.Windows.Forms.CheckBox();
            this.checkBoxAccountChecked = new System.Windows.Forms.CheckBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountDGITo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountDGIFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmmountTenancyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClaimId)).BeginInit();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(214, 403);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 31;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(76, 403);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 30;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // dateTimePickerAtDateFrom
            // 
            this.dateTimePickerAtDateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDateFrom.Enabled = false;
            this.dateTimePickerAtDateFrom.Location = new System.Drawing.Point(94, 195);
            this.dateTimePickerAtDateFrom.Name = "dateTimePickerAtDateFrom";
            this.dateTimePickerAtDateFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerAtDateFrom.TabIndex = 12;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(9, 134);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(139, 15);
            this.label103.TabIndex = 53;
            this.label103.Text = "Номер лицевого счета";
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(9, 177);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(130, 15);
            this.label91.TabIndex = 52;
            this.label91.Text = "Дата формирования";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 15);
            this.label1.TabIndex = 55;
            this.label1.Text = "Текущее состояние";
            // 
            // comboBoxLastState
            // 
            this.comboBoxLastState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLastState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLastState.Enabled = false;
            this.comboBoxLastState.Location = new System.Drawing.Point(33, 63);
            this.comboBoxLastState.Name = "comboBoxLastState";
            this.comboBoxLastState.Size = new System.Drawing.Size(360, 23);
            this.comboBoxLastState.TabIndex = 3;
            // 
            // checkBoxAtDateChecked
            // 
            this.checkBoxAtDateChecked.AutoSize = true;
            this.checkBoxAtDateChecked.Location = new System.Drawing.Point(12, 199);
            this.checkBoxAtDateChecked.Name = "checkBoxAtDateChecked";
            this.checkBoxAtDateChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAtDateChecked.TabIndex = 10;
            this.checkBoxAtDateChecked.UseVisualStyleBackColor = true;
            this.checkBoxAtDateChecked.CheckedChanged += new System.EventHandler(this.checkBoxAtDateChecked_CheckedChanged);
            // 
            // checkBoxLastStateChecked
            // 
            this.checkBoxLastStateChecked.AutoSize = true;
            this.checkBoxLastStateChecked.Location = new System.Drawing.Point(12, 67);
            this.checkBoxLastStateChecked.Name = "checkBoxLastStateChecked";
            this.checkBoxLastStateChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxLastStateChecked.TabIndex = 2;
            this.checkBoxLastStateChecked.UseVisualStyleBackColor = true;
            this.checkBoxLastStateChecked.CheckedChanged += new System.EventHandler(this.checkBoxLastStateChecked_CheckedChanged);
            // 
            // checkBoxAccountChecked
            // 
            this.checkBoxAccountChecked.AutoSize = true;
            this.checkBoxAccountChecked.Location = new System.Drawing.Point(12, 156);
            this.checkBoxAccountChecked.Name = "checkBoxAccountChecked";
            this.checkBoxAccountChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAccountChecked.TabIndex = 8;
            this.checkBoxAccountChecked.UseVisualStyleBackColor = true;
            this.checkBoxAccountChecked.CheckedChanged += new System.EventHandler(this.checkBoxAccountChecked_CheckedChanged);
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Enabled = false;
            this.textBoxAccount.Location = new System.Drawing.Point(33, 152);
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(360, 21);
            this.textBoxAccount.TabIndex = 9;
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
            this.comboBoxAtDateExpr.Location = new System.Drawing.Point(34, 194);
            this.comboBoxAtDateExpr.Name = "comboBoxAtDateExpr";
            this.comboBoxAtDateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxAtDateExpr.TabIndex = 11;
            // 
            // numericUpDownAmmountDGITo
            // 
            this.numericUpDownAmmountDGITo.DecimalPlaces = 2;
            this.numericUpDownAmmountDGITo.Enabled = false;
            this.numericUpDownAmmountDGITo.Location = new System.Drawing.Point(245, 367);
            this.numericUpDownAmmountDGITo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountDGITo.Name = "numericUpDownAmmountDGITo";
            this.numericUpDownAmmountDGITo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownAmmountDGITo.TabIndex = 29;
            // 
            // numericUpDownAmmountDGIFrom
            // 
            this.numericUpDownAmmountDGIFrom.DecimalPlaces = 2;
            this.numericUpDownAmmountDGIFrom.Enabled = false;
            this.numericUpDownAmmountDGIFrom.Location = new System.Drawing.Point(94, 367);
            this.numericUpDownAmmountDGIFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountDGIFrom.Name = "numericUpDownAmmountDGIFrom";
            this.numericUpDownAmmountDGIFrom.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownAmmountDGIFrom.TabIndex = 28;
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
            this.comboBoxAmmountDGIExpr.Location = new System.Drawing.Point(33, 366);
            this.comboBoxAmmountDGIExpr.Name = "comboBoxAmmountDGIExpr";
            this.comboBoxAmmountDGIExpr.Size = new System.Drawing.Size(49, 23);
            this.comboBoxAmmountDGIExpr.TabIndex = 27;
            // 
            // checkBoxAmmountDGIChecked
            // 
            this.checkBoxAmmountDGIChecked.AutoSize = true;
            this.checkBoxAmmountDGIChecked.Location = new System.Drawing.Point(12, 371);
            this.checkBoxAmmountDGIChecked.Name = "checkBoxAmmountDGIChecked";
            this.checkBoxAmmountDGIChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAmmountDGIChecked.TabIndex = 26;
            this.checkBoxAmmountDGIChecked.UseVisualStyleBackColor = true;
            this.checkBoxAmmountDGIChecked.CheckedChanged += new System.EventHandler(this.checkBoxAmmountDGIChecked_CheckedChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(9, 349);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(149, 15);
            this.label21.TabIndex = 176;
            this.label21.Text = "Сумма к взысканию ДГИ";
            // 
            // numericUpDownAmmountTenancyTo
            // 
            this.numericUpDownAmmountTenancyTo.DecimalPlaces = 2;
            this.numericUpDownAmmountTenancyTo.Enabled = false;
            this.numericUpDownAmmountTenancyTo.Location = new System.Drawing.Point(245, 324);
            this.numericUpDownAmmountTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountTenancyTo.Name = "numericUpDownAmmountTenancyTo";
            this.numericUpDownAmmountTenancyTo.Size = new System.Drawing.Size(148, 21);
            this.numericUpDownAmmountTenancyTo.TabIndex = 25;
            // 
            // numericUpDownAmmountTenancyFrom
            // 
            this.numericUpDownAmmountTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownAmmountTenancyFrom.Enabled = false;
            this.numericUpDownAmmountTenancyFrom.Location = new System.Drawing.Point(94, 324);
            this.numericUpDownAmmountTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmmountTenancyFrom.Name = "numericUpDownAmmountTenancyFrom";
            this.numericUpDownAmmountTenancyFrom.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownAmmountTenancyFrom.TabIndex = 24;
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
            this.comboBoxAmmountTenancyExpr.Location = new System.Drawing.Point(33, 323);
            this.comboBoxAmmountTenancyExpr.Name = "comboBoxAmmountTenancyExpr";
            this.comboBoxAmmountTenancyExpr.Size = new System.Drawing.Size(49, 23);
            this.comboBoxAmmountTenancyExpr.TabIndex = 23;
            // 
            // checkBoxAmmountTenancyChecked
            // 
            this.checkBoxAmmountTenancyChecked.AutoSize = true;
            this.checkBoxAmmountTenancyChecked.Location = new System.Drawing.Point(12, 328);
            this.checkBoxAmmountTenancyChecked.Name = "checkBoxAmmountTenancyChecked";
            this.checkBoxAmmountTenancyChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAmmountTenancyChecked.TabIndex = 22;
            this.checkBoxAmmountTenancyChecked.UseVisualStyleBackColor = true;
            this.checkBoxAmmountTenancyChecked.CheckedChanged += new System.EventHandler(this.checkBoxAmmountTenancyChecked_CheckedChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(9, 306);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(154, 15);
            this.label22.TabIndex = 175;
            this.label22.Text = "Сумма к взысканию найм";
            // 
            // dateTimePickerAtDateTo
            // 
            this.dateTimePickerAtDateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDateTo.Enabled = false;
            this.dateTimePickerAtDateTo.Location = new System.Drawing.Point(245, 195);
            this.dateTimePickerAtDateTo.Name = "dateTimePickerAtDateTo";
            this.dateTimePickerAtDateTo.Size = new System.Drawing.Size(148, 21);
            this.dateTimePickerAtDateTo.TabIndex = 13;
            // 
            // dateTimePickerStartDeptPeriodTo
            // 
            this.dateTimePickerStartDeptPeriodTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartDeptPeriodTo.Enabled = false;
            this.dateTimePickerStartDeptPeriodTo.Location = new System.Drawing.Point(245, 238);
            this.dateTimePickerStartDeptPeriodTo.Name = "dateTimePickerStartDeptPeriodTo";
            this.dateTimePickerStartDeptPeriodTo.Size = new System.Drawing.Size(148, 21);
            this.dateTimePickerStartDeptPeriodTo.TabIndex = 17;
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
            this.comboBoxStartDeptPeriodExpr.Location = new System.Drawing.Point(34, 237);
            this.comboBoxStartDeptPeriodExpr.Name = "comboBoxStartDeptPeriodExpr";
            this.comboBoxStartDeptPeriodExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxStartDeptPeriodExpr.TabIndex = 15;
            // 
            // checkBoxStartDeptPeriodChecked
            // 
            this.checkBoxStartDeptPeriodChecked.AutoSize = true;
            this.checkBoxStartDeptPeriodChecked.Location = new System.Drawing.Point(12, 242);
            this.checkBoxStartDeptPeriodChecked.Name = "checkBoxStartDeptPeriodChecked";
            this.checkBoxStartDeptPeriodChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStartDeptPeriodChecked.TabIndex = 14;
            this.checkBoxStartDeptPeriodChecked.UseVisualStyleBackColor = true;
            this.checkBoxStartDeptPeriodChecked.CheckedChanged += new System.EventHandler(this.checkBoxStartDeptPeriodChecked_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 220);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(275, 15);
            this.label2.TabIndex = 180;
            this.label2.Text = "Предъявленный период (дата начала период)";
            // 
            // dateTimePickerStartDeptPeriodFrom
            // 
            this.dateTimePickerStartDeptPeriodFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartDeptPeriodFrom.Enabled = false;
            this.dateTimePickerStartDeptPeriodFrom.Location = new System.Drawing.Point(94, 238);
            this.dateTimePickerStartDeptPeriodFrom.Name = "dateTimePickerStartDeptPeriodFrom";
            this.dateTimePickerStartDeptPeriodFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerStartDeptPeriodFrom.TabIndex = 16;
            // 
            // dateTimePickerEndDeptPeriodTo
            // 
            this.dateTimePickerEndDeptPeriodTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDeptPeriodTo.Enabled = false;
            this.dateTimePickerEndDeptPeriodTo.Location = new System.Drawing.Point(245, 281);
            this.dateTimePickerEndDeptPeriodTo.Name = "dateTimePickerEndDeptPeriodTo";
            this.dateTimePickerEndDeptPeriodTo.Size = new System.Drawing.Size(148, 21);
            this.dateTimePickerEndDeptPeriodTo.TabIndex = 21;
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
            this.comboBoxEndDeptPeriodExpr.Location = new System.Drawing.Point(34, 280);
            this.comboBoxEndDeptPeriodExpr.Name = "comboBoxEndDeptPeriodExpr";
            this.comboBoxEndDeptPeriodExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxEndDeptPeriodExpr.TabIndex = 19;
            // 
            // checkBoxEndDeptPeriodChecked
            // 
            this.checkBoxEndDeptPeriodChecked.AutoSize = true;
            this.checkBoxEndDeptPeriodChecked.Location = new System.Drawing.Point(12, 285);
            this.checkBoxEndDeptPeriodChecked.Name = "checkBoxEndDeptPeriodChecked";
            this.checkBoxEndDeptPeriodChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxEndDeptPeriodChecked.TabIndex = 18;
            this.checkBoxEndDeptPeriodChecked.UseVisualStyleBackColor = true;
            this.checkBoxEndDeptPeriodChecked.CheckedChanged += new System.EventHandler(this.checkBoxEndDeptPeriodChecked_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(295, 15);
            this.label3.TabIndex = 185;
            this.label3.Text = "Предъявленный период (дата окончания период)";
            // 
            // dateTimePickerEndDeptPeriodFrom
            // 
            this.dateTimePickerEndDeptPeriodFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDeptPeriodFrom.Enabled = false;
            this.dateTimePickerEndDeptPeriodFrom.Location = new System.Drawing.Point(94, 281);
            this.dateTimePickerEndDeptPeriodFrom.Name = "dateTimePickerEndDeptPeriodFrom";
            this.dateTimePickerEndDeptPeriodFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerEndDeptPeriodFrom.TabIndex = 20;
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
            this.dateTimePickerDateStartStateTo.Location = new System.Drawing.Point(245, 108);
            this.dateTimePickerDateStartStateTo.Name = "dateTimePickerDateStartStateTo";
            this.dateTimePickerDateStartStateTo.Size = new System.Drawing.Size(148, 21);
            this.dateTimePickerDateStartStateTo.TabIndex = 7;
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
            this.comboBoxDateStartStateExpr.Location = new System.Drawing.Point(34, 107);
            this.comboBoxDateStartStateExpr.Name = "comboBoxDateStartStateExpr";
            this.comboBoxDateStartStateExpr.Size = new System.Drawing.Size(48, 23);
            this.comboBoxDateStartStateExpr.TabIndex = 5;
            // 
            // checkBoxDateStartStateEnable
            // 
            this.checkBoxDateStartStateEnable.AutoSize = true;
            this.checkBoxDateStartStateEnable.Location = new System.Drawing.Point(12, 112);
            this.checkBoxDateStartStateEnable.Name = "checkBoxDateStartStateEnable";
            this.checkBoxDateStartStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDateStartStateEnable.TabIndex = 4;
            this.checkBoxDateStartStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxDateStartStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxDateStartStateEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(163, 15);
            this.label5.TabIndex = 193;
            this.label5.Text = "Дата установки состояиня";
            // 
            // dateTimePickerDateStartStateFrom
            // 
            this.dateTimePickerDateStartStateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateStartStateFrom.Enabled = false;
            this.dateTimePickerDateStartStateFrom.Location = new System.Drawing.Point(94, 108);
            this.dateTimePickerDateStartStateFrom.Name = "dateTimePickerDateStartStateFrom";
            this.dateTimePickerDateStartStateFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerDateStartStateFrom.TabIndex = 6;
            // 
            // ExtendedSearchClaimsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(405, 443);
            this.Controls.Add(this.dateTimePickerDateStartStateTo);
            this.Controls.Add(this.comboBoxDateStartStateExpr);
            this.Controls.Add(this.checkBoxDateStartStateEnable);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dateTimePickerDateStartStateFrom);
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
            this.Controls.Add(this.checkBoxAccountChecked);
            this.Controls.Add(this.checkBoxLastStateChecked);
            this.Controls.Add(this.checkBoxAtDateChecked);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxLastState);
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
        private ComboBox comboBoxLastState;
        private CheckBox checkBoxAtDateChecked;
        private CheckBox checkBoxLastStateChecked;
        private CheckBox checkBoxAccountChecked;
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
    }
}