using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class ClaimListViewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanel15;
        TableLayoutPanel tableLayoutPanel16;
        Panel panel8;
        Panel panel9;
        GroupBox groupBox34;
        NumericUpDown numericUpDownAmountOfDebtFine;
        NumericUpDown numericUpDownAmountOfDebtRent;
        NumericUpDown numericUpDownAmountOfFine;
        NumericUpDown numericUpDownAmountOfRent;
        NumericUpDown numericUpDownAmountOfFineRecover;
        NumericUpDown numericUpDownAmountOfRentRecover;
        NumericUpDown numericUpDownProcessID;
        Label label89;
        Label label90;
        Label label91;
        Label label92;
        Label label93;
        Label label94;
        Label label95;
        Label label96;
        Label label97;
        Label label98;
        Label label99;
        Label label102;
        Label label103;
        TextBox textBoxDescription;
        DateTimePicker dateTimePickerStartDeptPeriod;
        DateTimePicker dateTimePickerEndDeptPeriod;
        DateTimePicker dateTimePickerAtDate;
        DateTimePicker dateTimePickerDateOfTransfer;
        DataGridView dataGridViewClaims;
        private DataGridViewTextBoxColumn id_claim;
        private DataGridViewTextBoxColumn date_of_transfer;
        private DataGridViewTextBoxColumn amount_of_debt_rent;
        private DataGridViewTextBoxColumn amount_of_debt_fine;
        private DataGridViewTextBoxColumn at_date;
        private DataGridViewTextBoxColumn description;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimListViewport));
            this.tableLayoutPanel15 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel16 = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.numericUpDownAmountOfDebtRent = new System.Windows.Forms.NumericUpDown();
            this.label102 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfFine = new System.Windows.Forms.NumericUpDown();
            this.label93 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfRent = new System.Windows.Forms.NumericUpDown();
            this.label92 = new System.Windows.Forms.Label();
            this.dateTimePickerAtDate = new System.Windows.Forms.DateTimePicker();
            this.label91 = new System.Windows.Forms.Label();
            this.dateTimePickerDateOfTransfer = new System.Windows.Forms.DateTimePicker();
            this.label90 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfDebtFine = new System.Windows.Forms.NumericUpDown();
            this.label89 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.numericUpDownProcessID = new System.Windows.Forms.NumericUpDown();
            this.label103 = new System.Windows.Forms.Label();
            this.numericUpDownAmountOfFineRecover = new System.Windows.Forms.NumericUpDown();
            this.label95 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.numericUpDownAmountOfRentRecover = new System.Windows.Forms.NumericUpDown();
            this.label99 = new System.Windows.Forms.Label();
            this.label94 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.label97 = new System.Windows.Forms.Label();
            this.dateTimePickerStartDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.label96 = new System.Windows.Forms.Label();
            this.dataGridViewClaims = new System.Windows.Forms.DataGridView();
            this.id_claim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_transfer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_of_debt_rent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_of_debt_fine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.at_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel15.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.tableLayoutPanel16.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtRent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtFine)).BeginInit();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProcessID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFineRecover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRentRecover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel15
            // 
            this.tableLayoutPanel15.ColumnCount = 1;
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Controls.Add(this.groupBox34, 0, 0);
            this.tableLayoutPanel15.Controls.Add(this.dataGridViewClaims, 0, 1);
            this.tableLayoutPanel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel15.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel15.Name = "tableLayoutPanel15";
            this.tableLayoutPanel15.RowCount = 2;
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 196F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Size = new System.Drawing.Size(773, 491);
            this.tableLayoutPanel15.TabIndex = 0;
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.tableLayoutPanel16);
            this.groupBox34.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox34.Location = new System.Drawing.Point(0, 0);
            this.groupBox34.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(773, 196);
            this.groupBox34.TabIndex = 1;
            this.groupBox34.TabStop = false;
            this.groupBox34.Text = "Общие сведения";
            // 
            // tableLayoutPanel16
            // 
            this.tableLayoutPanel16.ColumnCount = 2;
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel16.Controls.Add(this.panel8, 0, 0);
            this.tableLayoutPanel16.Controls.Add(this.panel9, 1, 0);
            this.tableLayoutPanel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel16.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel16.Name = "tableLayoutPanel16";
            this.tableLayoutPanel16.RowCount = 1;
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tableLayoutPanel16.Size = new System.Drawing.Size(767, 176);
            this.tableLayoutPanel16.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.numericUpDownAmountOfDebtRent);
            this.panel8.Controls.Add(this.label102);
            this.panel8.Controls.Add(this.numericUpDownAmountOfFine);
            this.panel8.Controls.Add(this.label93);
            this.panel8.Controls.Add(this.numericUpDownAmountOfRent);
            this.panel8.Controls.Add(this.label92);
            this.panel8.Controls.Add(this.dateTimePickerAtDate);
            this.panel8.Controls.Add(this.label91);
            this.panel8.Controls.Add(this.dateTimePickerDateOfTransfer);
            this.panel8.Controls.Add(this.label90);
            this.panel8.Controls.Add(this.numericUpDownAmountOfDebtFine);
            this.panel8.Controls.Add(this.label89);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(383, 176);
            this.panel8.TabIndex = 0;
            // 
            // numericUpDownAmountOfDebtRent
            // 
            this.numericUpDownAmountOfDebtRent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfDebtRent.DecimalPlaces = 2;
            this.numericUpDownAmountOfDebtRent.Location = new System.Drawing.Point(172, 33);
            this.numericUpDownAmountOfDebtRent.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountOfDebtRent.Name = "numericUpDownAmountOfDebtRent";
            this.numericUpDownAmountOfDebtRent.Size = new System.Drawing.Size(202, 21);
            this.numericUpDownAmountOfDebtRent.TabIndex = 1;
            this.numericUpDownAmountOfDebtRent.ThousandsSeparator = true;
            // 
            // label102
            // 
            this.label102.AutoSize = true;
            this.label102.Location = new System.Drawing.Point(14, 35);
            this.label102.Name = "label102";
            this.label102.Size = new System.Drawing.Size(100, 15);
            this.label102.TabIndex = 37;
            this.label102.Text = "Сумма долга АП";
            // 
            // numericUpDownAmountOfFine
            // 
            this.numericUpDownAmountOfFine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfFine.DecimalPlaces = 2;
            this.numericUpDownAmountOfFine.Location = new System.Drawing.Point(172, 149);
            this.numericUpDownAmountOfFine.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountOfFine.Name = "numericUpDownAmountOfFine";
            this.numericUpDownAmountOfFine.Size = new System.Drawing.Size(202, 21);
            this.numericUpDownAmountOfFine.TabIndex = 5;
            this.numericUpDownAmountOfFine.ThousandsSeparator = true;
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Location = new System.Drawing.Point(14, 151);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(120, 15);
            this.label93.TabIndex = 35;
            this.label93.Text = "Сумма пени по иску";
            // 
            // numericUpDownAmountOfRent
            // 
            this.numericUpDownAmountOfRent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfRent.DecimalPlaces = 2;
            this.numericUpDownAmountOfRent.Location = new System.Drawing.Point(172, 120);
            this.numericUpDownAmountOfRent.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountOfRent.Name = "numericUpDownAmountOfRent";
            this.numericUpDownAmountOfRent.Size = new System.Drawing.Size(202, 21);
            this.numericUpDownAmountOfRent.TabIndex = 4;
            this.numericUpDownAmountOfRent.ThousandsSeparator = true;
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(14, 122);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(108, 15);
            this.label92.TabIndex = 33;
            this.label92.Text = "Сумма АП по иску";
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(172, 91);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.ShowCheckBox = true;
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(202, 21);
            this.dateTimePickerAtDate.TabIndex = 3;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(14, 94);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(52, 15);
            this.label91.TabIndex = 31;
            this.label91.Text = "На дату";
            // 
            // dateTimePickerDateOfTransfer
            // 
            this.dateTimePickerDateOfTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfTransfer.Location = new System.Drawing.Point(172, 4);
            this.dateTimePickerDateOfTransfer.Name = "dateTimePickerDateOfTransfer";
            this.dateTimePickerDateOfTransfer.ShowCheckBox = true;
            this.dateTimePickerDateOfTransfer.Size = new System.Drawing.Size(202, 21);
            this.dateTimePickerDateOfTransfer.TabIndex = 0;
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(14, 7);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(95, 15);
            this.label90.TabIndex = 29;
            this.label90.Text = "Дата передачи";
            // 
            // numericUpDownAmountOfDebtFine
            // 
            this.numericUpDownAmountOfDebtFine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfDebtFine.DecimalPlaces = 2;
            this.numericUpDownAmountOfDebtFine.Location = new System.Drawing.Point(172, 62);
            this.numericUpDownAmountOfDebtFine.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountOfDebtFine.Name = "numericUpDownAmountOfDebtFine";
            this.numericUpDownAmountOfDebtFine.Size = new System.Drawing.Size(202, 21);
            this.numericUpDownAmountOfDebtFine.TabIndex = 2;
            this.numericUpDownAmountOfDebtFine.ThousandsSeparator = true;
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(14, 64);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(112, 15);
            this.label89.TabIndex = 24;
            this.label89.Text = "Сумма долга пени";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.numericUpDownProcessID);
            this.panel9.Controls.Add(this.label103);
            this.panel9.Controls.Add(this.numericUpDownAmountOfFineRecover);
            this.panel9.Controls.Add(this.label95);
            this.panel9.Controls.Add(this.textBoxDescription);
            this.panel9.Controls.Add(this.numericUpDownAmountOfRentRecover);
            this.panel9.Controls.Add(this.label99);
            this.panel9.Controls.Add(this.label94);
            this.panel9.Controls.Add(this.label98);
            this.panel9.Controls.Add(this.dateTimePickerEndDeptPeriod);
            this.panel9.Controls.Add(this.label97);
            this.panel9.Controls.Add(this.dateTimePickerStartDeptPeriod);
            this.panel9.Controls.Add(this.label96);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(383, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(384, 176);
            this.panel9.TabIndex = 1;
            // 
            // numericUpDownProcessID
            // 
            this.numericUpDownProcessID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownProcessID.Location = new System.Drawing.Point(172, 4);
            this.numericUpDownProcessID.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDownProcessID.Name = "numericUpDownProcessID";
            this.numericUpDownProcessID.Size = new System.Drawing.Size(202, 21);
            this.numericUpDownProcessID.TabIndex = 0;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(14, 6);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(96, 15);
            this.label103.TabIndex = 51;
            this.label103.Text = "Процесс найма";
            // 
            // numericUpDownAmountOfFineRecover
            // 
            this.numericUpDownAmountOfFineRecover.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfFineRecover.DecimalPlaces = 2;
            this.numericUpDownAmountOfFineRecover.Location = new System.Drawing.Point(172, 62);
            this.numericUpDownAmountOfFineRecover.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountOfFineRecover.Name = "numericUpDownAmountOfFineRecover";
            this.numericUpDownAmountOfFineRecover.Size = new System.Drawing.Size(202, 21);
            this.numericUpDownAmountOfFineRecover.TabIndex = 8;
            this.numericUpDownAmountOfFineRecover.ThousandsSeparator = true;
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(14, 64);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(152, 15);
            this.label95.TabIndex = 39;
            this.label95.Text = "Сумма пени к взысканию";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(172, 149);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(202, 21);
            this.textBoxDescription.TabIndex = 11;
            // 
            // numericUpDownAmountOfRentRecover
            // 
            this.numericUpDownAmountOfRentRecover.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountOfRentRecover.DecimalPlaces = 2;
            this.numericUpDownAmountOfRentRecover.Location = new System.Drawing.Point(172, 33);
            this.numericUpDownAmountOfRentRecover.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountOfRentRecover.Name = "numericUpDownAmountOfRentRecover";
            this.numericUpDownAmountOfRentRecover.Size = new System.Drawing.Size(202, 21);
            this.numericUpDownAmountOfRentRecover.TabIndex = 7;
            this.numericUpDownAmountOfRentRecover.ThousandsSeparator = true;
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(14, 151);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(80, 15);
            this.label99.TabIndex = 49;
            this.label99.Text = "Примечание";
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(14, 35);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(140, 15);
            this.label94.TabIndex = 37;
            this.label94.Text = "Сумма АП к взысканию";
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(145, 122);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(21, 15);
            this.label98.TabIndex = 48;
            this.label98.Text = "по";
            // 
            // dateTimePickerEndDeptPeriod
            // 
            this.dateTimePickerEndDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDeptPeriod.Location = new System.Drawing.Point(172, 120);
            this.dateTimePickerEndDeptPeriod.Name = "dateTimePickerEndDeptPeriod";
            this.dateTimePickerEndDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerEndDeptPeriod.Size = new System.Drawing.Size(202, 21);
            this.dateTimePickerEndDeptPeriod.TabIndex = 10;
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(151, 93);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(13, 15);
            this.label97.TabIndex = 46;
            this.label97.Text = "с";
            // 
            // dateTimePickerStartDeptPeriod
            // 
            this.dateTimePickerStartDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartDeptPeriod.Location = new System.Drawing.Point(172, 91);
            this.dateTimePickerStartDeptPeriod.Name = "dateTimePickerStartDeptPeriod";
            this.dateTimePickerStartDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerStartDeptPeriod.Size = new System.Drawing.Size(202, 21);
            this.dateTimePickerStartDeptPeriod.TabIndex = 9;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(14, 93);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(131, 15);
            this.label96.TabIndex = 44;
            this.label96.Text = "Период задолжности";
            // 
            // dataGridViewClaims
            // 
            this.dataGridViewClaims.AllowUserToAddRows = false;
            this.dataGridViewClaims.AllowUserToDeleteRows = false;
            this.dataGridViewClaims.AllowUserToResizeRows = false;
            this.dataGridViewClaims.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewClaims.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewClaims.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaims.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_claim,
            this.date_of_transfer,
            this.amount_of_debt_rent,
            this.amount_of_debt_fine,
            this.at_date,
            this.description});
            this.dataGridViewClaims.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaims.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewClaims.Location = new System.Drawing.Point(3, 199);
            this.dataGridViewClaims.MultiSelect = false;
            this.dataGridViewClaims.Name = "dataGridViewClaims";
            this.dataGridViewClaims.ReadOnly = true;
            this.dataGridViewClaims.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewClaims.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaims.ShowCellToolTips = false;
            this.dataGridViewClaims.Size = new System.Drawing.Size(767, 289);
            this.dataGridViewClaims.TabIndex = 0;
            this.dataGridViewClaims.VirtualMode = true;
            this.dataGridViewClaims.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewClaims_CellValueNeeded);
            this.dataGridViewClaims.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewClaims_ColumnHeaderMouseClick);
            this.dataGridViewClaims.SelectionChanged += new System.EventHandler(this.dataGridViewClaims_SelectionChanged);
            // 
            // id_claim
            // 
            this.id_claim.HeaderText = "№";
            this.id_claim.MinimumWidth = 50;
            this.id_claim.Name = "id_claim";
            this.id_claim.ReadOnly = true;
            this.id_claim.Width = 50;
            // 
            // date_of_transfer
            // 
            this.date_of_transfer.HeaderText = "Дата передачи";
            this.date_of_transfer.MinimumWidth = 150;
            this.date_of_transfer.Name = "date_of_transfer";
            this.date_of_transfer.ReadOnly = true;
            this.date_of_transfer.Width = 150;
            // 
            // amount_of_debt_rent
            // 
            dataGridViewCellStyle1.Format = "#0.0# руб.";
            this.amount_of_debt_rent.DefaultCellStyle = dataGridViewCellStyle1;
            this.amount_of_debt_rent.HeaderText = "Сумма долга АП";
            this.amount_of_debt_rent.MinimumWidth = 150;
            this.amount_of_debt_rent.Name = "amount_of_debt_rent";
            this.amount_of_debt_rent.ReadOnly = true;
            this.amount_of_debt_rent.Width = 150;
            // 
            // amount_of_debt_fine
            // 
            dataGridViewCellStyle2.Format = "#0.0# руб.";
            this.amount_of_debt_fine.DefaultCellStyle = dataGridViewCellStyle2;
            this.amount_of_debt_fine.HeaderText = "Сумма долга пени";
            this.amount_of_debt_fine.MinimumWidth = 150;
            this.amount_of_debt_fine.Name = "amount_of_debt_fine";
            this.amount_of_debt_fine.ReadOnly = true;
            this.amount_of_debt_fine.Width = 150;
            // 
            // at_date
            // 
            this.at_date.HeaderText = "На дату";
            this.at_date.MinimumWidth = 150;
            this.at_date.Name = "at_date";
            this.at_date.ReadOnly = true;
            this.at_date.Width = 150;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 200;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            // 
            // ClaimListViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(680, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(779, 497);
            this.Controls.Add(this.tableLayoutPanel15);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исковая работа №{0}";
            this.tableLayoutPanel15.ResumeLayout(false);
            this.groupBox34.ResumeLayout(false);
            this.tableLayoutPanel16.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtRent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfDebtFine)).EndInit();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProcessID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfFineRecover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountOfRentRecover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
