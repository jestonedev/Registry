using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.SearchForms
{
    partial class SimpleSearchClaimsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleSearchClaimsForm));
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.dateTimePickerAtDate = new System.Windows.Forms.DateTimePicker();
            this.label103 = new System.Windows.Forms.Label();
            this.label91 = new System.Windows.Forms.Label();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.checkBoxAtDateChecked = new System.Windows.Forms.CheckBox();
            this.checkBoxStateEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxAccountChecked = new System.Windows.Forms.CheckBox();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.dateTimePickerDateStartStateTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxDateStartStateExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxDateStartStateEnable = new System.Windows.Forms.CheckBox();
            this.dateTimePickerDateStartStateFrom = new System.Windows.Forms.DateTimePicker();
            this.textBoxSRN = new System.Windows.Forms.TextBox();
            this.checkBoxSRNEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxLastState = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCourtOrderNum = new System.Windows.Forms.TextBox();
            this.checkBoxCourtOrderNumEnable = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceOutputDgiTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceOutputDgiFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceOutputDgiExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceOutputDgiChecked = new System.Windows.Forms.CheckBox();
            this.label21 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceOutputTenancyTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceOutputTenancyFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceOutputTenancyExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceOutputTenancyChecked = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceOutputPenaltiesTo = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBalanceOutputPenaltiesFrom = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBalanceOutputPenaltiesExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxBalanceOutputPenaltiesChecked = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDgiTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDgiFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputPenaltiesTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputPenaltiesFrom)).BeginInit();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(214, 470);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 22;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(76, 470);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 21;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Enabled = false;
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(34, 267);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(352, 21);
            this.dateTimePickerAtDate.TabIndex = 6;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(11, 163);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(139, 15);
            this.label103.TabIndex = 53;
            this.label103.Text = "Номер лицевого счета";
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(12, 249);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(130, 15);
            this.label91.TabIndex = 52;
            this.label91.Text = "Дата формирования";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.Enabled = false;
            this.comboBoxState.Location = new System.Drawing.Point(30, 40);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(352, 23);
            this.comboBoxState.TabIndex = 1;
            // 
            // checkBoxAtDateChecked
            // 
            this.checkBoxAtDateChecked.AutoSize = true;
            this.checkBoxAtDateChecked.Location = new System.Drawing.Point(13, 271);
            this.checkBoxAtDateChecked.Name = "checkBoxAtDateChecked";
            this.checkBoxAtDateChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAtDateChecked.TabIndex = 5;
            this.checkBoxAtDateChecked.UseVisualStyleBackColor = true;
            this.checkBoxAtDateChecked.CheckedChanged += new System.EventHandler(this.checkBoxAtDateChecked_CheckedChanged);
            // 
            // checkBoxStateEnable
            // 
            this.checkBoxStateEnable.AutoSize = true;
            this.checkBoxStateEnable.Location = new System.Drawing.Point(9, 44);
            this.checkBoxStateEnable.Name = "checkBoxStateEnable";
            this.checkBoxStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStateEnable.TabIndex = 0;
            this.checkBoxStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxLastStateChecked_CheckedChanged);
            // 
            // checkBoxAccountChecked
            // 
            this.checkBoxAccountChecked.AutoSize = true;
            this.checkBoxAccountChecked.Location = new System.Drawing.Point(13, 185);
            this.checkBoxAccountChecked.Name = "checkBoxAccountChecked";
            this.checkBoxAccountChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAccountChecked.TabIndex = 1;
            this.checkBoxAccountChecked.UseVisualStyleBackColor = true;
            this.checkBoxAccountChecked.CheckedChanged += new System.EventHandler(this.checkBoxAccountChecked_CheckedChanged);
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Enabled = false;
            this.textBoxAccount.Location = new System.Drawing.Point(34, 181);
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(352, 21);
            this.textBoxAccount.TabIndex = 2;
            // 
            // dateTimePickerDateStartStateTo
            // 
            this.dateTimePickerDateStartStateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateStartStateTo.Enabled = false;
            this.dateTimePickerDateStartStateTo.Location = new System.Drawing.Point(241, 91);
            this.dateTimePickerDateStartStateTo.Name = "dateTimePickerDateStartStateTo";
            this.dateTimePickerDateStartStateTo.Size = new System.Drawing.Size(141, 21);
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
            this.comboBoxDateStartStateExpr.Location = new System.Drawing.Point(30, 90);
            this.comboBoxDateStartStateExpr.Name = "comboBoxDateStartStateExpr";
            this.comboBoxDateStartStateExpr.Size = new System.Drawing.Size(51, 23);
            this.comboBoxDateStartStateExpr.TabIndex = 3;
            // 
            // checkBoxDateStartStateEnable
            // 
            this.checkBoxDateStartStateEnable.AutoSize = true;
            this.checkBoxDateStartStateEnable.Location = new System.Drawing.Point(9, 94);
            this.checkBoxDateStartStateEnable.Name = "checkBoxDateStartStateEnable";
            this.checkBoxDateStartStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDateStartStateEnable.TabIndex = 2;
            this.checkBoxDateStartStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxDateStartStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxDateStartStateEnable_CheckedChanged);
            // 
            // dateTimePickerDateStartStateFrom
            // 
            this.dateTimePickerDateStartStateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateStartStateFrom.Enabled = false;
            this.dateTimePickerDateStartStateFrom.Location = new System.Drawing.Point(90, 91);
            this.dateTimePickerDateStartStateFrom.Name = "dateTimePickerDateStartStateFrom";
            this.dateTimePickerDateStartStateFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerDateStartStateFrom.TabIndex = 4;
            // 
            // textBoxSRN
            // 
            this.textBoxSRN.Enabled = false;
            this.textBoxSRN.Location = new System.Drawing.Point(34, 224);
            this.textBoxSRN.Name = "textBoxSRN";
            this.textBoxSRN.Size = new System.Drawing.Size(352, 21);
            this.textBoxSRN.TabIndex = 4;
            // 
            // checkBoxSRNEnable
            // 
            this.checkBoxSRNEnable.AutoSize = true;
            this.checkBoxSRNEnable.Location = new System.Drawing.Point(13, 228);
            this.checkBoxSRNEnable.Name = "checkBoxSRNEnable";
            this.checkBoxSRNEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSRNEnable.TabIndex = 3;
            this.checkBoxSRNEnable.UseVisualStyleBackColor = true;
            this.checkBoxSRNEnable.CheckedChanged += new System.EventHandler(this.checkBoxSRNEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 15);
            this.label6.TabIndex = 201;
            this.label6.Text = "СРН";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxLastState);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxState);
            this.groupBox1.Controls.Add(this.dateTimePickerDateStartStateTo);
            this.groupBox1.Controls.Add(this.comboBoxDateStartStateExpr);
            this.groupBox1.Controls.Add(this.checkBoxStateEnable);
            this.groupBox1.Controls.Add(this.checkBoxDateStartStateEnable);
            this.groupBox1.Controls.Add(this.dateTimePickerDateStartStateFrom);
            this.groupBox1.Location = new System.Drawing.Point(4, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 149);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Состояние";
            // 
            // checkBoxLastState
            // 
            this.checkBoxLastState.AutoSize = true;
            this.checkBoxLastState.Location = new System.Drawing.Point(9, 118);
            this.checkBoxLastState.Name = "checkBoxLastState";
            this.checkBoxLastState.Size = new System.Drawing.Size(292, 19);
            this.checkBoxLastState.TabIndex = 6;
            this.checkBoxLastState.Text = "Фильтровать только по текущему состоянию";
            this.checkBoxLastState.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 15);
            this.label2.TabIndex = 55;
            this.label2.Text = "Вид состояния";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 15);
            this.label3.TabIndex = 193;
            this.label3.Text = "Дата установки";
            // 
            // textBoxCourtOrderNum
            // 
            this.textBoxCourtOrderNum.Enabled = false;
            this.textBoxCourtOrderNum.Location = new System.Drawing.Point(34, 310);
            this.textBoxCourtOrderNum.MaxLength = 255;
            this.textBoxCourtOrderNum.Name = "textBoxCourtOrderNum";
            this.textBoxCourtOrderNum.Size = new System.Drawing.Size(352, 21);
            this.textBoxCourtOrderNum.TabIndex = 8;
            // 
            // checkBoxCourtOrderNumEnable
            // 
            this.checkBoxCourtOrderNumEnable.AutoSize = true;
            this.checkBoxCourtOrderNumEnable.Location = new System.Drawing.Point(13, 314);
            this.checkBoxCourtOrderNumEnable.Name = "checkBoxCourtOrderNumEnable";
            this.checkBoxCourtOrderNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCourtOrderNumEnable.TabIndex = 7;
            this.checkBoxCourtOrderNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxCourtOrderNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxCourtOrderNumEnable_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 292);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(157, 15);
            this.label13.TabIndex = 211;
            this.label13.Text = "Номер судебного приказа";
            // 
            // numericUpDownBalanceOutputDgiTo
            // 
            this.numericUpDownBalanceOutputDgiTo.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputDgiTo.Enabled = false;
            this.numericUpDownBalanceOutputDgiTo.Location = new System.Drawing.Point(245, 395);
            this.numericUpDownBalanceOutputDgiTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputDgiTo.Name = "numericUpDownBalanceOutputDgiTo";
            this.numericUpDownBalanceOutputDgiTo.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownBalanceOutputDgiTo.TabIndex = 16;
            // 
            // numericUpDownBalanceOutputDgiFrom
            // 
            this.numericUpDownBalanceOutputDgiFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputDgiFrom.Enabled = false;
            this.numericUpDownBalanceOutputDgiFrom.Location = new System.Drawing.Point(94, 395);
            this.numericUpDownBalanceOutputDgiFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputDgiFrom.Name = "numericUpDownBalanceOutputDgiFrom";
            this.numericUpDownBalanceOutputDgiFrom.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownBalanceOutputDgiFrom.TabIndex = 15;
            // 
            // comboBoxBalanceOutputDgiExpr
            // 
            this.comboBoxBalanceOutputDgiExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceOutputDgiExpr.Enabled = false;
            this.comboBoxBalanceOutputDgiExpr.FormattingEnabled = true;
            this.comboBoxBalanceOutputDgiExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceOutputDgiExpr.Location = new System.Drawing.Point(33, 394);
            this.comboBoxBalanceOutputDgiExpr.Name = "comboBoxBalanceOutputDgiExpr";
            this.comboBoxBalanceOutputDgiExpr.Size = new System.Drawing.Size(52, 23);
            this.comboBoxBalanceOutputDgiExpr.TabIndex = 14;
            // 
            // checkBoxBalanceOutputDgiChecked
            // 
            this.checkBoxBalanceOutputDgiChecked.AutoSize = true;
            this.checkBoxBalanceOutputDgiChecked.Location = new System.Drawing.Point(12, 399);
            this.checkBoxBalanceOutputDgiChecked.Name = "checkBoxBalanceOutputDgiChecked";
            this.checkBoxBalanceOutputDgiChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceOutputDgiChecked.TabIndex = 13;
            this.checkBoxBalanceOutputDgiChecked.UseVisualStyleBackColor = true;
            this.checkBoxBalanceOutputDgiChecked.CheckedChanged += new System.EventHandler(this.checkBoxBalanceOutputDgiChecked_CheckedChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(9, 377);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(196, 15);
            this.label21.TabIndex = 221;
            this.label21.Text = "Текущее сальдо исходящее  ДГИ";
            // 
            // numericUpDownBalanceOutputTenancyTo
            // 
            this.numericUpDownBalanceOutputTenancyTo.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputTenancyTo.Enabled = false;
            this.numericUpDownBalanceOutputTenancyTo.Location = new System.Drawing.Point(245, 352);
            this.numericUpDownBalanceOutputTenancyTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputTenancyTo.Name = "numericUpDownBalanceOutputTenancyTo";
            this.numericUpDownBalanceOutputTenancyTo.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownBalanceOutputTenancyTo.TabIndex = 12;
            // 
            // numericUpDownBalanceOutputTenancyFrom
            // 
            this.numericUpDownBalanceOutputTenancyFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputTenancyFrom.Enabled = false;
            this.numericUpDownBalanceOutputTenancyFrom.Location = new System.Drawing.Point(94, 352);
            this.numericUpDownBalanceOutputTenancyFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputTenancyFrom.Name = "numericUpDownBalanceOutputTenancyFrom";
            this.numericUpDownBalanceOutputTenancyFrom.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownBalanceOutputTenancyFrom.TabIndex = 11;
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
            this.comboBoxBalanceOutputTenancyExpr.Location = new System.Drawing.Point(33, 351);
            this.comboBoxBalanceOutputTenancyExpr.Name = "comboBoxBalanceOutputTenancyExpr";
            this.comboBoxBalanceOutputTenancyExpr.Size = new System.Drawing.Size(52, 23);
            this.comboBoxBalanceOutputTenancyExpr.TabIndex = 10;
            // 
            // checkBoxBalanceOutputTenancyChecked
            // 
            this.checkBoxBalanceOutputTenancyChecked.AutoSize = true;
            this.checkBoxBalanceOutputTenancyChecked.Location = new System.Drawing.Point(12, 356);
            this.checkBoxBalanceOutputTenancyChecked.Name = "checkBoxBalanceOutputTenancyChecked";
            this.checkBoxBalanceOutputTenancyChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceOutputTenancyChecked.TabIndex = 9;
            this.checkBoxBalanceOutputTenancyChecked.UseVisualStyleBackColor = true;
            this.checkBoxBalanceOutputTenancyChecked.CheckedChanged += new System.EventHandler(this.checkBoxBalanceOutputTenancyChecked_CheckedChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(9, 334);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(198, 15);
            this.label22.TabIndex = 220;
            this.label22.Text = "Текущее сальдо исходящее найм";
            // 
            // numericUpDownBalanceOutputPenaltiesTo
            // 
            this.numericUpDownBalanceOutputPenaltiesTo.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputPenaltiesTo.Enabled = false;
            this.numericUpDownBalanceOutputPenaltiesTo.Location = new System.Drawing.Point(245, 437);
            this.numericUpDownBalanceOutputPenaltiesTo.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputPenaltiesTo.Name = "numericUpDownBalanceOutputPenaltiesTo";
            this.numericUpDownBalanceOutputPenaltiesTo.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownBalanceOutputPenaltiesTo.TabIndex = 20;
            // 
            // numericUpDownBalanceOutputPenaltiesFrom
            // 
            this.numericUpDownBalanceOutputPenaltiesFrom.DecimalPlaces = 2;
            this.numericUpDownBalanceOutputPenaltiesFrom.Enabled = false;
            this.numericUpDownBalanceOutputPenaltiesFrom.Location = new System.Drawing.Point(94, 437);
            this.numericUpDownBalanceOutputPenaltiesFrom.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceOutputPenaltiesFrom.Name = "numericUpDownBalanceOutputPenaltiesFrom";
            this.numericUpDownBalanceOutputPenaltiesFrom.Size = new System.Drawing.Size(141, 21);
            this.numericUpDownBalanceOutputPenaltiesFrom.TabIndex = 19;
            // 
            // comboBoxBalanceOutputPenaltiesExpr
            // 
            this.comboBoxBalanceOutputPenaltiesExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBalanceOutputPenaltiesExpr.Enabled = false;
            this.comboBoxBalanceOutputPenaltiesExpr.FormattingEnabled = true;
            this.comboBoxBalanceOutputPenaltiesExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxBalanceOutputPenaltiesExpr.Location = new System.Drawing.Point(33, 436);
            this.comboBoxBalanceOutputPenaltiesExpr.Name = "comboBoxBalanceOutputPenaltiesExpr";
            this.comboBoxBalanceOutputPenaltiesExpr.Size = new System.Drawing.Size(52, 23);
            this.comboBoxBalanceOutputPenaltiesExpr.TabIndex = 18;
            // 
            // checkBoxBalanceOutputPenaltiesChecked
            // 
            this.checkBoxBalanceOutputPenaltiesChecked.AutoSize = true;
            this.checkBoxBalanceOutputPenaltiesChecked.Location = new System.Drawing.Point(12, 441);
            this.checkBoxBalanceOutputPenaltiesChecked.Name = "checkBoxBalanceOutputPenaltiesChecked";
            this.checkBoxBalanceOutputPenaltiesChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBalanceOutputPenaltiesChecked.TabIndex = 17;
            this.checkBoxBalanceOutputPenaltiesChecked.UseVisualStyleBackColor = true;
            this.checkBoxBalanceOutputPenaltiesChecked.CheckedChanged += new System.EventHandler(this.checkBoxBalanceOutputPenaltiesChecked_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 419);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 15);
            this.label1.TabIndex = 226;
            this.label1.Text = "Текущее сальдо исходящее пени";
            // 
            // SimpleSearchClaimsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(402, 511);
            this.Controls.Add(this.numericUpDownBalanceOutputPenaltiesTo);
            this.Controls.Add(this.numericUpDownBalanceOutputPenaltiesFrom);
            this.Controls.Add(this.comboBoxBalanceOutputPenaltiesExpr);
            this.Controls.Add(this.checkBoxBalanceOutputPenaltiesChecked);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownBalanceOutputDgiTo);
            this.Controls.Add(this.numericUpDownBalanceOutputDgiFrom);
            this.Controls.Add(this.comboBoxBalanceOutputDgiExpr);
            this.Controls.Add(this.checkBoxBalanceOutputDgiChecked);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.numericUpDownBalanceOutputTenancyTo);
            this.Controls.Add(this.numericUpDownBalanceOutputTenancyFrom);
            this.Controls.Add(this.comboBoxBalanceOutputTenancyExpr);
            this.Controls.Add(this.checkBoxBalanceOutputTenancyChecked);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.textBoxCourtOrderNum);
            this.Controls.Add(this.checkBoxCourtOrderNumEnable);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxSRN);
            this.Controls.Add(this.checkBoxSRNEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxAccount);
            this.Controls.Add(this.checkBoxAccountChecked);
            this.Controls.Add(this.checkBoxAtDateChecked);
            this.Controls.Add(this.label103);
            this.Controls.Add(this.label91);
            this.Controls.Add(this.dateTimePickerAtDate);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimpleSearchClaimsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация претензионно-исковых работ";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDgiTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputDgiFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputTenancyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputPenaltiesTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceOutputPenaltiesFrom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vButton vButtonCancel;
        private vButton vButtonSearch;
        private DateTimePicker dateTimePickerAtDate;
        private Label label103;
        private Label label91;
        private ComboBox comboBoxState;
        private CheckBox checkBoxAtDateChecked;
        private CheckBox checkBoxStateEnable;
        private CheckBox checkBoxAccountChecked;
        private TextBox textBoxAccount;
        private DateTimePicker dateTimePickerDateStartStateTo;
        private ComboBox comboBoxDateStartStateExpr;
        private CheckBox checkBoxDateStartStateEnable;
        private DateTimePicker dateTimePickerDateStartStateFrom;
        private TextBox textBoxSRN;
        private CheckBox checkBoxSRNEnable;
        private Label label6;
        private GroupBox groupBox1;
        private CheckBox checkBoxLastState;
        private Label label2;
        private Label label3;
        private TextBox textBoxCourtOrderNum;
        private CheckBox checkBoxCourtOrderNumEnable;
        private Label label13;
        private NumericUpDown numericUpDownBalanceOutputDgiTo;
        private NumericUpDown numericUpDownBalanceOutputDgiFrom;
        private ComboBox comboBoxBalanceOutputDgiExpr;
        private CheckBox checkBoxBalanceOutputDgiChecked;
        private Label label21;
        private NumericUpDown numericUpDownBalanceOutputTenancyTo;
        private NumericUpDown numericUpDownBalanceOutputTenancyFrom;
        private ComboBox comboBoxBalanceOutputTenancyExpr;
        private CheckBox checkBoxBalanceOutputTenancyChecked;
        private Label label22;
        private NumericUpDown numericUpDownBalanceOutputPenaltiesTo;
        private NumericUpDown numericUpDownBalanceOutputPenaltiesFrom;
        private ComboBox comboBoxBalanceOutputPenaltiesExpr;
        private CheckBox checkBoxBalanceOutputPenaltiesChecked;
        private Label label1;
    }
}