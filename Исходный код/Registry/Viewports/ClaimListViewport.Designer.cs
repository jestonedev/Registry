﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomControls;

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
        FixedNumericUpDown numericUpDownAmountDGI;
        FixedNumericUpDown numericUpDownAmountTenancy;
        Label label91;
        Label label94;
        Label label95;
        Label label96;
        Label label97;
        Label label98;
        Label label99;
        Label label103;
        TextBox textBoxDescription;
        DateTimePicker dateTimePickerStartDeptPeriod;
        DateTimePicker dateTimePickerEndDeptPeriod;
        DateTimePicker dateTimePickerAtDate;
        DataGridView dataGridViewClaims;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimListViewport));
            this.tableLayoutPanel15 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel16 = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.buttonShowAttachments = new System.Windows.Forms.Button();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.label103 = new System.Windows.Forms.Label();
            this.dateTimePickerAtDate = new System.Windows.Forms.DateTimePicker();
            this.label91 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label96 = new System.Windows.Forms.Label();
            this.label99 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.dateTimePickerStartDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.label97 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.numericUpDownAmountPkk = new CustomControls.FixedNumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownAmountPadun = new CustomControls.FixedNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownAmountPenalties = new CustomControls.FixedNumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownAmountTotal = new CustomControls.FixedNumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownAmountDGI = new CustomControls.FixedNumericUpDown();
            this.label95 = new System.Windows.Forms.Label();
            this.numericUpDownAmountTenancy = new CustomControls.FixedNumericUpDown();
            this.label94 = new System.Windows.Forms.Label();
            this.dataGridViewClaims = new System.Windows.Forms.DataGridView();
            this.id_claim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.raw_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.at_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_start_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.state_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.start_dept_period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_dept_period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_padun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_pkk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_penalties = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel15.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.tableLayoutPanel16.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountPkk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountPadun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountPenalties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountDGI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountTenancy)).BeginInit();
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
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Size = new System.Drawing.Size(1002, 485);
            this.tableLayoutPanel15.TabIndex = 0;
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.tableLayoutPanel16);
            this.groupBox34.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox34.Location = new System.Drawing.Point(0, 0);
            this.groupBox34.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(1002, 200);
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
            this.tableLayoutPanel16.Size = new System.Drawing.Size(996, 180);
            this.tableLayoutPanel16.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.buttonShowAttachments);
            this.panel8.Controls.Add(this.textBoxAccount);
            this.panel8.Controls.Add(this.label103);
            this.panel8.Controls.Add(this.dateTimePickerAtDate);
            this.panel8.Controls.Add(this.label91);
            this.panel8.Controls.Add(this.dateTimePickerEndDeptPeriod);
            this.panel8.Controls.Add(this.textBoxDescription);
            this.panel8.Controls.Add(this.label96);
            this.panel8.Controls.Add(this.label99);
            this.panel8.Controls.Add(this.label98);
            this.panel8.Controls.Add(this.dateTimePickerStartDeptPeriod);
            this.panel8.Controls.Add(this.label97);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(498, 180);
            this.panel8.TabIndex = 0;
            // 
            // buttonShowAttachments
            // 
            this.buttonShowAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowAttachments.Location = new System.Drawing.Point(12, 148);
            this.buttonShowAttachments.Name = "buttonShowAttachments";
            this.buttonShowAttachments.Size = new System.Drawing.Size(475, 25);
            this.buttonShowAttachments.TabIndex = 53;
            this.buttonShowAttachments.Text = "Прикрепленные файлы";
            this.buttonShowAttachments.UseVisualStyleBackColor = true;
            this.buttonShowAttachments.Click += new System.EventHandler(this.buttonShowAttachments_Click);
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAccount.Location = new System.Drawing.Point(170, 6);
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(317, 21);
            this.textBoxAccount.TabIndex = 52;
            this.textBoxAccount.Leave += new System.EventHandler(this.textBoxAccount_Leave);
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(9, 7);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(139, 15);
            this.label103.TabIndex = 51;
            this.label103.Text = "Номер лицевого счета";
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(170, 35);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.ShowCheckBox = true;
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(317, 21);
            this.dateTimePickerAtDate.TabIndex = 1;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(9, 37);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(130, 15);
            this.label91.TabIndex = 31;
            this.label91.Text = "Дата формирования";
            // 
            // dateTimePickerEndDeptPeriod
            // 
            this.dateTimePickerEndDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDeptPeriod.Location = new System.Drawing.Point(170, 92);
            this.dateTimePickerEndDeptPeriod.Name = "dateTimePickerEndDeptPeriod";
            this.dateTimePickerEndDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerEndDeptPeriod.Size = new System.Drawing.Size(317, 21);
            this.dateTimePickerEndDeptPeriod.TabIndex = 3;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(170, 121);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(317, 21);
            this.textBoxDescription.TabIndex = 4;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(9, 65);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(124, 15);
            this.label96.TabIndex = 44;
            this.label96.Text = "Предъявлен период";
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(9, 123);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(80, 15);
            this.label99.TabIndex = 49;
            this.label99.Text = "Примечание";
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(143, 94);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(21, 15);
            this.label98.TabIndex = 48;
            this.label98.Text = "по";
            // 
            // dateTimePickerStartDeptPeriod
            // 
            this.dateTimePickerStartDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartDeptPeriod.Location = new System.Drawing.Point(170, 63);
            this.dateTimePickerStartDeptPeriod.Name = "dateTimePickerStartDeptPeriod";
            this.dateTimePickerStartDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerStartDeptPeriod.Size = new System.Drawing.Size(317, 21);
            this.dateTimePickerStartDeptPeriod.TabIndex = 2;
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(149, 65);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(13, 15);
            this.label97.TabIndex = 46;
            this.label97.Text = "с";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.numericUpDownAmountPkk);
            this.panel9.Controls.Add(this.label4);
            this.panel9.Controls.Add(this.numericUpDownAmountPadun);
            this.panel9.Controls.Add(this.label3);
            this.panel9.Controls.Add(this.numericUpDownAmountPenalties);
            this.panel9.Controls.Add(this.label2);
            this.panel9.Controls.Add(this.numericUpDownAmountTotal);
            this.panel9.Controls.Add(this.label1);
            this.panel9.Controls.Add(this.numericUpDownAmountDGI);
            this.panel9.Controls.Add(this.label95);
            this.panel9.Controls.Add(this.numericUpDownAmountTenancy);
            this.panel9.Controls.Add(this.label94);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(498, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(498, 180);
            this.panel9.TabIndex = 1;
            // 
            // numericUpDownAmountPkk
            // 
            this.numericUpDownAmountPkk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountPkk.DecimalPlaces = 2;
            this.numericUpDownAmountPkk.Location = new System.Drawing.Point(172, 92);
            this.numericUpDownAmountPkk.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountPkk.Name = "numericUpDownAmountPkk";
            this.numericUpDownAmountPkk.Size = new System.Drawing.Size(316, 21);
            this.numericUpDownAmountPkk.TabIndex = 3;
            this.numericUpDownAmountPkk.ThousandsSeparator = true;
            this.numericUpDownAmountPkk.ValueChanged += new System.EventHandler(this.numericUpDownAmountPkk_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 15);
            this.label4.TabIndex = 57;
            this.label4.Text = "Сумма к взысканию ПКК";
            // 
            // numericUpDownAmountPadun
            // 
            this.numericUpDownAmountPadun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountPadun.DecimalPlaces = 2;
            this.numericUpDownAmountPadun.Location = new System.Drawing.Point(172, 63);
            this.numericUpDownAmountPadun.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountPadun.Name = "numericUpDownAmountPadun";
            this.numericUpDownAmountPadun.Size = new System.Drawing.Size(316, 21);
            this.numericUpDownAmountPadun.TabIndex = 2;
            this.numericUpDownAmountPadun.ThousandsSeparator = true;
            this.numericUpDownAmountPadun.ValueChanged += new System.EventHandler(this.numericUpDownAmountPadun_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 15);
            this.label3.TabIndex = 55;
            this.label3.Text = "Сумма к взысканию Падун";
            // 
            // numericUpDownAmountPenalties
            // 
            this.numericUpDownAmountPenalties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountPenalties.DecimalPlaces = 2;
            this.numericUpDownAmountPenalties.Location = new System.Drawing.Point(172, 121);
            this.numericUpDownAmountPenalties.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountPenalties.Name = "numericUpDownAmountPenalties";
            this.numericUpDownAmountPenalties.Size = new System.Drawing.Size(316, 21);
            this.numericUpDownAmountPenalties.TabIndex = 4;
            this.numericUpDownAmountPenalties.ThousandsSeparator = true;
            this.numericUpDownAmountPenalties.ValueChanged += new System.EventHandler(this.numericUpDownAmountPenalties_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 15);
            this.label2.TabIndex = 53;
            this.label2.Text = "Сумма к взысканию пени";
            // 
            // numericUpDownAmountTotal
            // 
            this.numericUpDownAmountTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountTotal.BackColor = System.Drawing.Color.White;
            this.numericUpDownAmountTotal.DecimalPlaces = 2;
            this.numericUpDownAmountTotal.Location = new System.Drawing.Point(172, 149);
            this.numericUpDownAmountTotal.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountTotal.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numericUpDownAmountTotal.Name = "numericUpDownAmountTotal";
            this.numericUpDownAmountTotal.ReadOnly = true;
            this.numericUpDownAmountTotal.Size = new System.Drawing.Size(316, 21);
            this.numericUpDownAmountTotal.TabIndex = 5;
            this.numericUpDownAmountTotal.ThousandsSeparator = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 15);
            this.label1.TabIndex = 51;
            this.label1.Text = "Сумма итого";
            // 
            // numericUpDownAmountDGI
            // 
            this.numericUpDownAmountDGI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountDGI.DecimalPlaces = 2;
            this.numericUpDownAmountDGI.Location = new System.Drawing.Point(172, 35);
            this.numericUpDownAmountDGI.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountDGI.Name = "numericUpDownAmountDGI";
            this.numericUpDownAmountDGI.Size = new System.Drawing.Size(316, 21);
            this.numericUpDownAmountDGI.TabIndex = 1;
            this.numericUpDownAmountDGI.ThousandsSeparator = true;
            this.numericUpDownAmountDGI.ValueChanged += new System.EventHandler(this.numericUpDownAmountDGI_ValueChanged);
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(10, 37);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(149, 15);
            this.label95.TabIndex = 39;
            this.label95.Text = "Сумма к взысканию ДГИ";
            // 
            // numericUpDownAmountTenancy
            // 
            this.numericUpDownAmountTenancy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountTenancy.DecimalPlaces = 2;
            this.numericUpDownAmountTenancy.Location = new System.Drawing.Point(172, 6);
            this.numericUpDownAmountTenancy.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountTenancy.Name = "numericUpDownAmountTenancy";
            this.numericUpDownAmountTenancy.Size = new System.Drawing.Size(316, 21);
            this.numericUpDownAmountTenancy.TabIndex = 0;
            this.numericUpDownAmountTenancy.ThousandsSeparator = true;
            this.numericUpDownAmountTenancy.ValueChanged += new System.EventHandler(this.numericUpDownAmountTenancy_ValueChanged);
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(10, 8);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(154, 15);
            this.label94.TabIndex = 0;
            this.label94.Text = "Сумма к взысканию найм";
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
            this.account,
            this.raw_address,
            this.tenant,
            this.at_date,
            this.date_start_state,
            this.state_type,
            this.start_dept_period,
            this.end_dept_period,
            this.amount_tenancy,
            this.amount_dgi,
            this.amount_padun,
            this.amount_pkk,
            this.amount_penalties});
            this.dataGridViewClaims.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaims.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewClaims.Location = new System.Drawing.Point(3, 203);
            this.dataGridViewClaims.Name = "dataGridViewClaims";
            this.dataGridViewClaims.ReadOnly = true;
            this.dataGridViewClaims.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewClaims.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaims.ShowCellToolTips = false;
            this.dataGridViewClaims.Size = new System.Drawing.Size(996, 279);
            this.dataGridViewClaims.TabIndex = 0;
            this.dataGridViewClaims.VirtualMode = true;
            this.dataGridViewClaims.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewClaims_CellDoubleClick);
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
            // account
            // 
            this.account.HeaderText = "Лицевой счет";
            this.account.MinimumWidth = 110;
            this.account.Name = "account";
            this.account.ReadOnly = true;
            this.account.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.account.Width = 110;
            // 
            // raw_address
            // 
            this.raw_address.FillWeight = 200F;
            this.raw_address.HeaderText = "Адрес";
            this.raw_address.Name = "raw_address";
            this.raw_address.ReadOnly = true;
            this.raw_address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.raw_address.Width = 200;
            // 
            // tenant
            // 
            this.tenant.FillWeight = 200F;
            this.tenant.HeaderText = "Наниматель";
            this.tenant.Name = "tenant";
            this.tenant.ReadOnly = true;
            this.tenant.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.tenant.Width = 200;
            // 
            // at_date
            // 
            this.at_date.HeaderText = "Дата формирования";
            this.at_date.MinimumWidth = 110;
            this.at_date.Name = "at_date";
            this.at_date.ReadOnly = true;
            this.at_date.Width = 110;
            // 
            // date_start_state
            // 
            this.date_start_state.HeaderText = "Состояние установлено";
            this.date_start_state.MinimumWidth = 110;
            this.date_start_state.Name = "date_start_state";
            this.date_start_state.ReadOnly = true;
            this.date_start_state.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.date_start_state.Width = 110;
            // 
            // state_type
            // 
            this.state_type.HeaderText = "Текущее состояние";
            this.state_type.MinimumWidth = 150;
            this.state_type.Name = "state_type";
            this.state_type.ReadOnly = true;
            this.state_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.state_type.Width = 150;
            // 
            // start_dept_period
            // 
            this.start_dept_period.HeaderText = "Период с";
            this.start_dept_period.MinimumWidth = 110;
            this.start_dept_period.Name = "start_dept_period";
            this.start_dept_period.ReadOnly = true;
            this.start_dept_period.Width = 110;
            // 
            // end_dept_period
            // 
            this.end_dept_period.HeaderText = "Период по";
            this.end_dept_period.MinimumWidth = 110;
            this.end_dept_period.Name = "end_dept_period";
            this.end_dept_period.ReadOnly = true;
            this.end_dept_period.Width = 110;
            // 
            // amount_tenancy
            // 
            dataGridViewCellStyle1.Format = "#0.0# руб.";
            this.amount_tenancy.DefaultCellStyle = dataGridViewCellStyle1;
            this.amount_tenancy.HeaderText = "Сумма долга найм";
            this.amount_tenancy.MinimumWidth = 110;
            this.amount_tenancy.Name = "amount_tenancy";
            this.amount_tenancy.ReadOnly = true;
            this.amount_tenancy.Width = 110;
            // 
            // amount_dgi
            // 
            dataGridViewCellStyle2.Format = "#0.0# руб.";
            this.amount_dgi.DefaultCellStyle = dataGridViewCellStyle2;
            this.amount_dgi.HeaderText = "Сумма долга ДГИ";
            this.amount_dgi.MinimumWidth = 110;
            this.amount_dgi.Name = "amount_dgi";
            this.amount_dgi.ReadOnly = true;
            this.amount_dgi.Width = 110;
            // 
            // amount_padun
            // 
            this.amount_padun.HeaderText = "Сумма долга Падун";
            this.amount_padun.MinimumWidth = 110;
            this.amount_padun.Name = "amount_padun";
            this.amount_padun.ReadOnly = true;
            this.amount_padun.Width = 110;
            // 
            // amount_pkk
            // 
            this.amount_pkk.HeaderText = "Сумма долга ПКК";
            this.amount_pkk.MinimumWidth = 110;
            this.amount_pkk.Name = "amount_pkk";
            this.amount_pkk.ReadOnly = true;
            this.amount_pkk.Width = 110;
            // 
            // amount_penalties
            // 
            dataGridViewCellStyle3.Format = "#0.0# руб.";
            this.amount_penalties.DefaultCellStyle = dataGridViewCellStyle3;
            this.amount_penalties.HeaderText = "Сумма долга пени";
            this.amount_penalties.MinimumWidth = 110;
            this.amount_penalties.Name = "amount_penalties";
            this.amount_penalties.ReadOnly = true;
            this.amount_penalties.Width = 110;
            // 
            // ClaimListViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(680, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 491);
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
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountPkk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountPadun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountPenalties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountDGI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountTenancy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).EndInit();
            this.ResumeLayout(false);

        }
        private TextBox textBoxAccount;
        private Button buttonShowAttachments;
        private FixedNumericUpDown numericUpDownAmountTotal;
        private Label label1;
        private FixedNumericUpDown numericUpDownAmountPenalties;
        private Label label2;
        private FixedNumericUpDown numericUpDownAmountPkk;
        private Label label4;
        private FixedNumericUpDown numericUpDownAmountPadun;
        private Label label3;
        private DataGridViewTextBoxColumn id_claim;
        private DataGridViewTextBoxColumn account;
        private DataGridViewTextBoxColumn raw_address;
        private DataGridViewTextBoxColumn tenant;
        private DataGridViewTextBoxColumn at_date;
        private DataGridViewTextBoxColumn date_start_state;
        private DataGridViewTextBoxColumn state_type;
        private DataGridViewTextBoxColumn start_dept_period;
        private DataGridViewTextBoxColumn end_dept_period;
        private DataGridViewTextBoxColumn amount_tenancy;
        private DataGridViewTextBoxColumn amount_dgi;
        private DataGridViewTextBoxColumn amount_padun;
        private DataGridViewTextBoxColumn amount_pkk;
        private DataGridViewTextBoxColumn amount_penalties;
    }
}
