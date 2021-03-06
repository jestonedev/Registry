﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomControls;
using VIBlend.Utilities;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport
{
    internal partial class TenancyAgreementsViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel12;
        private GroupBox groupBox29;
        private GroupBox groupBox30;
        private TabControl tabControl1;
        private Panel panel7;
        private DataGridView dataGridView;
        private DataGridView dataGridViewTenancyPersons;
        private TextBox textBoxAgreementContent;
        private TextBox textBoxAgreementWarrant;
        private TextBox textBoxExcludePoint;
        private TextBox textBoxIncludePoint;
        private TextBox textBoxIncludeSNP;
        private TextBox textBoxExplainPoint;
        private TextBox textBoxExplainContent;
        private TextBox textBoxTerminateAgreement;
        private TabPage tabPageExclude;
        private TabPage tabPageInclude;
        private TabPage tabPageExplain;
        private TabPage tabPageTerminate;
        private Label label71;
        private Label label72;
        private Label label73;
        private Label label74;
        private Label label75;
        private Label label76;
        private Label label77;
        private Label label78;
        private Label label79;
        private Label label80;
        private ComboBox comboBoxExecutor;
        private ComboBox comboBoxIncludeKinship;
        private vButton vButtonSelectWarrant;
        private vButton vButtonExcludePaste;
        private vButton vButtonIncludePaste;
        private vButton vButtonExplainPaste;
        private vButton vButtonTerminatePaste;
        private DateTimePicker dateTimePickerAgreementDate;
        private DateTimePicker dateTimePickerIncludeDateOfBirth;
        private DataGridViewTextBoxColumn id_agreement;
        private DataGridViewDateTimeColumn agreement_date;
        private DataGridViewTextBoxColumn agreement_content;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DateTimePicker dateTimePickerTerminateDate;
        private Label label1;
        private TextBox textBoxGeneralIncludePoint;
        private TextBox textBoxGeneralExcludePoint;
        private Label label2;
        private Label label3;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyAgreementsViewport));
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.vButtonPaymentInsert = new VIBlend.WinForms.Controls.vButton();
            this.comboBoxExecutor = new System.Windows.Forms.ComboBox();
            this.label73 = new System.Windows.Forms.Label();
            this.vButtonSelectWarrant = new VIBlend.WinForms.Controls.vButton();
            this.textBoxAgreementWarrant = new System.Windows.Forms.TextBox();
            this.label72 = new System.Windows.Forms.Label();
            this.dateTimePickerAgreementDate = new System.Windows.Forms.DateTimePicker();
            this.label71 = new System.Windows.Forms.Label();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.textBoxAgreementContent = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageExclude = new System.Windows.Forms.TabPage();
            this.textBoxGeneralExcludePoint = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridViewTenancyPersons = new System.Windows.Forms.DataGridView();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vButtonExcludePaste = new VIBlend.WinForms.Controls.vButton();
            this.textBoxExcludePoint = new System.Windows.Forms.TextBox();
            this.label74 = new System.Windows.Forms.Label();
            this.tabPageInclude = new System.Windows.Forms.TabPage();
            this.textBoxGeneralIncludePoint = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerIncludeDateOfBirth = new System.Windows.Forms.DateTimePicker();
            this.comboBoxIncludeKinship = new System.Windows.Forms.ComboBox();
            this.label76 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.textBoxIncludeSNP = new System.Windows.Forms.TextBox();
            this.textBoxIncludePoint = new System.Windows.Forms.TextBox();
            this.label78 = new System.Windows.Forms.Label();
            this.vButtonIncludePaste = new VIBlend.WinForms.Controls.vButton();
            this.label75 = new System.Windows.Forms.Label();
            this.tabPageExplain = new System.Windows.Forms.TabPage();
            this.textBoxExplainGeneralPoint = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxExplainContent = new System.Windows.Forms.TextBox();
            this.textBoxExplainPoint = new System.Windows.Forms.TextBox();
            this.vButtonExplainPaste = new VIBlend.WinForms.Controls.vButton();
            this.label79 = new System.Windows.Forms.Label();
            this.tabPageTerminate = new System.Windows.Forms.TabPage();
            this.dateTimePickerTerminateDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.vButtonTerminatePaste = new VIBlend.WinForms.Controls.vButton();
            this.textBoxTerminateAgreement = new System.Windows.Forms.TextBox();
            this.label80 = new System.Windows.Forms.Label();
            this.tabPageProlong = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxCommercialProlongReasonType = new System.Windows.Forms.ComboBox();
            this.checkBoxCommercialProlongUntilDismissal = new System.Windows.Forms.CheckBox();
            this.textBoxCommercialProlongGeneralPoint = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dateTimePickerCommercialProlongTo = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.dateTimePickerCommercialProlongFrom = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePickerCommercialProlongRequest = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.vButtonProlong = new VIBlend.WinForms.Controls.vButton();
            this.tabPageProlongSpecial = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxSpecialProlongGeneralPoint = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxSpecialProlongPoint = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxSpecialProlongUntilDismissal = new System.Windows.Forms.CheckBox();
            this.dateTimePickerSpecialProlongTo = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.dateTimePickerSpecialProlongFrom = new System.Windows.Forms.DateTimePicker();
            this.label14 = new System.Windows.Forms.Label();
            this.vButtonProlongSpecial = new VIBlend.WinForms.Controls.vButton();
            this.tabPageChangeTenancy = new System.Windows.Forms.TabPage();
            this.textBoxChangeTenantChangeFIO = new System.Windows.Forms.TextBox();
            this.dataGridViewChangeTenant = new System.Windows.Forms.DataGridView();
            this.surnameChangeTenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameChangeTenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymicChangeTenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateofbirthChangeTenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxExcludeTenant = new System.Windows.Forms.CheckBox();
            this.vButtonChangeTenancy = new VIBlend.WinForms.Controls.vButton();
            this.comboboxTenantChangeKinship = new System.Windows.Forms.ComboBox();
            this.label100 = new System.Windows.Forms.Label();
            this.newTenancy = new System.Windows.Forms.Label();
            this.curTenancy = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_agreement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agreement_date = new CustomControls.DataGridViewDateTimeColumn();
            this.agreement_content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel12.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox29.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageExclude.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).BeginInit();
            this.tabPageInclude.SuspendLayout();
            this.tabPageExplain.SuspendLayout();
            this.tabPageTerminate.SuspendLayout();
            this.tabPageProlong.SuspendLayout();
            this.tabPageProlongSpecial.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageChangeTenancy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeTenant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel12.ColumnCount = 2;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Controls.Add(this.panel7, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.groupBox30, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.dataGridView, 0, 2);
            this.tableLayoutPanel12.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 3;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(981, 712);
            this.tableLayoutPanel12.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.groupBox29);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(3, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(484, 134);
            this.panel7.TabIndex = 0;
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.label4);
            this.groupBox29.Controls.Add(this.vButtonPaymentInsert);
            this.groupBox29.Controls.Add(this.comboBoxExecutor);
            this.groupBox29.Controls.Add(this.label73);
            this.groupBox29.Controls.Add(this.vButtonSelectWarrant);
            this.groupBox29.Controls.Add(this.textBoxAgreementWarrant);
            this.groupBox29.Controls.Add(this.label72);
            this.groupBox29.Controls.Add(this.dateTimePickerAgreementDate);
            this.groupBox29.Controls.Add(this.label71);
            this.groupBox29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox29.Location = new System.Drawing.Point(0, 0);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Size = new System.Drawing.Size(484, 134);
            this.groupBox29.TabIndex = 0;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Общие сведения";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(327, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 15);
            this.label4.TabIndex = 40;
            this.label4.Text = "Изменение оплаты";
            // 
            // vButtonPaymentInsert
            // 
            this.vButtonPaymentInsert.AllowAnimations = true;
            this.vButtonPaymentInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonPaymentInsert.BackColor = System.Drawing.Color.Transparent;
            this.vButtonPaymentInsert.Location = new System.Drawing.Point(452, 108);
            this.vButtonPaymentInsert.Name = "vButtonPaymentInsert";
            this.vButtonPaymentInsert.RoundedCornersMask = ((byte)(15));
            this.vButtonPaymentInsert.Size = new System.Drawing.Size(27, 20);
            this.vButtonPaymentInsert.TabIndex = 39;
            this.vButtonPaymentInsert.Text = "→";
            this.vButtonPaymentInsert.UseVisualStyleBackColor = false;
            this.vButtonPaymentInsert.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonPaymentInsert.Click += new System.EventHandler(this.vButtonPaymentInsert_Click);
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(164, 77);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(315, 23);
            this.comboBoxExecutor.TabIndex = 3;
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(17, 80);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(85, 15);
            this.label73.TabIndex = 38;
            this.label73.Text = "Исполнитель";
            // 
            // vButtonSelectWarrant
            // 
            this.vButtonSelectWarrant.AllowAnimations = true;
            this.vButtonSelectWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonSelectWarrant.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSelectWarrant.Location = new System.Drawing.Point(452, 48);
            this.vButtonSelectWarrant.Name = "vButtonSelectWarrant";
            this.vButtonSelectWarrant.RoundedCornersMask = ((byte)(15));
            this.vButtonSelectWarrant.Size = new System.Drawing.Size(27, 20);
            this.vButtonSelectWarrant.TabIndex = 2;
            this.vButtonSelectWarrant.Text = "...";
            this.vButtonSelectWarrant.UseVisualStyleBackColor = false;
            this.vButtonSelectWarrant.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSelectWarrant.Click += new System.EventHandler(this.vButtonSelectWarrant_Click);
            // 
            // textBoxAgreementWarrant
            // 
            this.textBoxAgreementWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAgreementWarrant.Location = new System.Drawing.Point(164, 48);
            this.textBoxAgreementWarrant.Name = "textBoxAgreementWarrant";
            this.textBoxAgreementWarrant.ReadOnly = true;
            this.textBoxAgreementWarrant.Size = new System.Drawing.Size(282, 21);
            this.textBoxAgreementWarrant.TabIndex = 1;
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(17, 51);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(109, 15);
            this.label72.TabIndex = 35;
            this.label72.Text = "По доверенности";
            // 
            // dateTimePickerAgreementDate
            // 
            this.dateTimePickerAgreementDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAgreementDate.Location = new System.Drawing.Point(164, 19);
            this.dateTimePickerAgreementDate.Name = "dateTimePickerAgreementDate";
            this.dateTimePickerAgreementDate.Size = new System.Drawing.Size(315, 21);
            this.dateTimePickerAgreementDate.TabIndex = 0;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Location = new System.Drawing.Point(17, 23);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(109, 15);
            this.label71.TabIndex = 33;
            this.label71.Text = "Дата соглашения";
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.textBoxAgreementContent);
            this.groupBox30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox30.Location = new System.Drawing.Point(493, 3);
            this.groupBox30.Name = "groupBox30";
            this.tableLayoutPanel12.SetRowSpan(this.groupBox30, 2);
            this.groupBox30.Size = new System.Drawing.Size(485, 354);
            this.groupBox30.TabIndex = 1;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Содержание";
            // 
            // textBoxAgreementContent
            // 
            this.textBoxAgreementContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAgreementContent.Location = new System.Drawing.Point(3, 17);
            this.textBoxAgreementContent.MaxLength = 4000;
            this.textBoxAgreementContent.Multiline = true;
            this.textBoxAgreementContent.Name = "textBoxAgreementContent";
            this.textBoxAgreementContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxAgreementContent.Size = new System.Drawing.Size(479, 334);
            this.textBoxAgreementContent.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageExclude);
            this.tabControl1.Controls.Add(this.tabPageInclude);
            this.tabControl1.Controls.Add(this.tabPageExplain);
            this.tabControl1.Controls.Add(this.tabPageTerminate);
            this.tabControl1.Controls.Add(this.tabPageProlong);
            this.tabControl1.Controls.Add(this.tabPageProlongSpecial);
            this.tabControl1.Controls.Add(this.tabPageChangeTenancy);
            this.tabControl1.Location = new System.Drawing.Point(0, 140);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(490, 220);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageExclude
            // 
            this.tabPageExclude.BackColor = System.Drawing.Color.White;
            this.tabPageExclude.Controls.Add(this.textBoxGeneralExcludePoint);
            this.tabPageExclude.Controls.Add(this.label3);
            this.tabPageExclude.Controls.Add(this.dataGridViewTenancyPersons);
            this.tabPageExclude.Controls.Add(this.vButtonExcludePaste);
            this.tabPageExclude.Controls.Add(this.textBoxExcludePoint);
            this.tabPageExclude.Controls.Add(this.label74);
            this.tabPageExclude.Location = new System.Drawing.Point(4, 24);
            this.tabPageExclude.Name = "tabPageExclude";
            this.tabPageExclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExclude.Size = new System.Drawing.Size(482, 192);
            this.tabPageExclude.TabIndex = 0;
            this.tabPageExclude.Text = "Исключить";
            // 
            // textBoxGeneralExcludePoint
            // 
            this.textBoxGeneralExcludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxGeneralExcludePoint.Location = new System.Drawing.Point(164, 6);
            this.textBoxGeneralExcludePoint.Name = "textBoxGeneralExcludePoint";
            this.textBoxGeneralExcludePoint.Size = new System.Drawing.Size(280, 21);
            this.textBoxGeneralExcludePoint.TabIndex = 0;
            this.textBoxGeneralExcludePoint.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 15);
            this.label3.TabIndex = 50;
            this.label3.Text = "Пункт";
            // 
            // dataGridViewTenancyPersons
            // 
            this.dataGridViewTenancyPersons.AllowUserToAddRows = false;
            this.dataGridViewTenancyPersons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTenancyPersons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTenancyPersons.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyPersons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTenancyPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.surname,
            this.name,
            this.patronymic,
            this.date_of_birth});
            this.dataGridViewTenancyPersons.Location = new System.Drawing.Point(3, 61);
            this.dataGridViewTenancyPersons.MultiSelect = false;
            this.dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            this.dataGridViewTenancyPersons.ReadOnly = true;
            this.dataGridViewTenancyPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyPersons.Size = new System.Drawing.Size(476, 117);
            this.dataGridViewTenancyPersons.TabIndex = 3;
            // 
            // surname
            // 
            this.surname.HeaderText = "Фамилия";
            this.surname.MinimumWidth = 100;
            this.surname.Name = "surname";
            this.surname.ReadOnly = true;
            // 
            // name
            // 
            this.name.HeaderText = "Имя";
            this.name.MinimumWidth = 100;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // patronymic
            // 
            this.patronymic.HeaderText = "Отчество";
            this.patronymic.MinimumWidth = 100;
            this.patronymic.Name = "patronymic";
            this.patronymic.ReadOnly = true;
            // 
            // date_of_birth
            // 
            this.date_of_birth.HeaderText = "Дата рождения";
            this.date_of_birth.MinimumWidth = 140;
            this.date_of_birth.Name = "date_of_birth";
            this.date_of_birth.ReadOnly = true;
            // 
            // vButtonExcludePaste
            // 
            this.vButtonExcludePaste.AllowAnimations = true;
            this.vButtonExcludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExcludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExcludePaste.Location = new System.Drawing.Point(450, 6);
            this.vButtonExcludePaste.Name = "vButtonExcludePaste";
            this.vButtonExcludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExcludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExcludePaste.TabIndex = 2;
            this.vButtonExcludePaste.Text = "→";
            this.vButtonExcludePaste.UseVisualStyleBackColor = false;
            this.vButtonExcludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonExcludePaste.Click += new System.EventHandler(this.vButtonExcludePaste_Click);
            // 
            // textBoxExcludePoint
            // 
            this.textBoxExcludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludePoint.Location = new System.Drawing.Point(164, 34);
            this.textBoxExcludePoint.Name = "textBoxExcludePoint";
            this.textBoxExcludePoint.Size = new System.Drawing.Size(280, 21);
            this.textBoxExcludePoint.TabIndex = 1;
            this.textBoxExcludePoint.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(13, 37);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(62, 15);
            this.label74.TabIndex = 37;
            this.label74.Text = "Подпункт";
            // 
            // tabPageInclude
            // 
            this.tabPageInclude.BackColor = System.Drawing.Color.White;
            this.tabPageInclude.Controls.Add(this.textBoxGeneralIncludePoint);
            this.tabPageInclude.Controls.Add(this.label2);
            this.tabPageInclude.Controls.Add(this.dateTimePickerIncludeDateOfBirth);
            this.tabPageInclude.Controls.Add(this.comboBoxIncludeKinship);
            this.tabPageInclude.Controls.Add(this.label76);
            this.tabPageInclude.Controls.Add(this.label77);
            this.tabPageInclude.Controls.Add(this.textBoxIncludeSNP);
            this.tabPageInclude.Controls.Add(this.textBoxIncludePoint);
            this.tabPageInclude.Controls.Add(this.label78);
            this.tabPageInclude.Controls.Add(this.vButtonIncludePaste);
            this.tabPageInclude.Controls.Add(this.label75);
            this.tabPageInclude.Location = new System.Drawing.Point(4, 22);
            this.tabPageInclude.Name = "tabPageInclude";
            this.tabPageInclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInclude.Size = new System.Drawing.Size(482, 194);
            this.tabPageInclude.TabIndex = 1;
            this.tabPageInclude.Text = "Включить";
            // 
            // textBoxGeneralIncludePoint
            // 
            this.textBoxGeneralIncludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxGeneralIncludePoint.Location = new System.Drawing.Point(164, 6);
            this.textBoxGeneralIncludePoint.Name = "textBoxGeneralIncludePoint";
            this.textBoxGeneralIncludePoint.Size = new System.Drawing.Size(280, 21);
            this.textBoxGeneralIncludePoint.TabIndex = 0;
            this.textBoxGeneralIncludePoint.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 15);
            this.label2.TabIndex = 48;
            this.label2.Text = "Пункт";
            // 
            // dateTimePickerIncludeDateOfBirth
            // 
            this.dateTimePickerIncludeDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeDateOfBirth.Location = new System.Drawing.Point(164, 90);
            this.dateTimePickerIncludeDateOfBirth.Name = "dateTimePickerIncludeDateOfBirth";
            this.dateTimePickerIncludeDateOfBirth.Size = new System.Drawing.Size(280, 21);
            this.dateTimePickerIncludeDateOfBirth.TabIndex = 3;
            // 
            // comboBoxIncludeKinship
            // 
            this.comboBoxIncludeKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIncludeKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIncludeKinship.FormattingEnabled = true;
            this.comboBoxIncludeKinship.Location = new System.Drawing.Point(164, 118);
            this.comboBoxIncludeKinship.Name = "comboBoxIncludeKinship";
            this.comboBoxIncludeKinship.Size = new System.Drawing.Size(280, 23);
            this.comboBoxIncludeKinship.TabIndex = 4;
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(13, 122);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(110, 15);
            this.label76.TabIndex = 46;
            this.label76.Text = "Отношение/связь";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(13, 93);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(98, 15);
            this.label77.TabIndex = 45;
            this.label77.Text = "Дата рождения";
            // 
            // textBoxIncludeSNP
            // 
            this.textBoxIncludeSNP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludeSNP.Location = new System.Drawing.Point(164, 62);
            this.textBoxIncludeSNP.Name = "textBoxIncludeSNP";
            this.textBoxIncludeSNP.Size = new System.Drawing.Size(280, 21);
            this.textBoxIncludeSNP.TabIndex = 2;
            this.textBoxIncludeSNP.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // textBoxIncludePoint
            // 
            this.textBoxIncludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludePoint.Location = new System.Drawing.Point(164, 34);
            this.textBoxIncludePoint.Name = "textBoxIncludePoint";
            this.textBoxIncludePoint.Size = new System.Drawing.Size(280, 21);
            this.textBoxIncludePoint.TabIndex = 1;
            this.textBoxIncludePoint.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(13, 65);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(36, 15);
            this.label78.TabIndex = 43;
            this.label78.Text = "ФИО";
            // 
            // vButtonIncludePaste
            // 
            this.vButtonIncludePaste.AllowAnimations = true;
            this.vButtonIncludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonIncludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonIncludePaste.Location = new System.Drawing.Point(450, 6);
            this.vButtonIncludePaste.Name = "vButtonIncludePaste";
            this.vButtonIncludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonIncludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonIncludePaste.TabIndex = 4;
            this.vButtonIncludePaste.Text = "→";
            this.vButtonIncludePaste.UseVisualStyleBackColor = false;
            this.vButtonIncludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonIncludePaste.Click += new System.EventHandler(this.vButtonIncludePaste_Click);
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(13, 37);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(62, 15);
            this.label75.TabIndex = 40;
            this.label75.Text = "Подпункт";
            // 
            // tabPageExplain
            // 
            this.tabPageExplain.BackColor = System.Drawing.Color.White;
            this.tabPageExplain.Controls.Add(this.textBoxExplainGeneralPoint);
            this.tabPageExplain.Controls.Add(this.label5);
            this.tabPageExplain.Controls.Add(this.textBoxExplainContent);
            this.tabPageExplain.Controls.Add(this.textBoxExplainPoint);
            this.tabPageExplain.Controls.Add(this.vButtonExplainPaste);
            this.tabPageExplain.Controls.Add(this.label79);
            this.tabPageExplain.Location = new System.Drawing.Point(4, 24);
            this.tabPageExplain.Name = "tabPageExplain";
            this.tabPageExplain.Size = new System.Drawing.Size(482, 192);
            this.tabPageExplain.TabIndex = 2;
            this.tabPageExplain.Text = "Изложить";
            // 
            // textBoxExplainGeneralPoint
            // 
            this.textBoxExplainGeneralPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainGeneralPoint.Location = new System.Drawing.Point(164, 6);
            this.textBoxExplainGeneralPoint.Name = "textBoxExplainGeneralPoint";
            this.textBoxExplainGeneralPoint.Size = new System.Drawing.Size(280, 21);
            this.textBoxExplainGeneralPoint.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 15);
            this.label5.TabIndex = 50;
            this.label5.Text = "Пункт";
            // 
            // textBoxExplainContent
            // 
            this.textBoxExplainContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainContent.Location = new System.Drawing.Point(15, 62);
            this.textBoxExplainContent.Multiline = true;
            this.textBoxExplainContent.Name = "textBoxExplainContent";
            this.textBoxExplainContent.Size = new System.Drawing.Size(429, 81);
            this.textBoxExplainContent.TabIndex = 2;
            this.textBoxExplainContent.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // textBoxExplainPoint
            // 
            this.textBoxExplainPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainPoint.Location = new System.Drawing.Point(164, 34);
            this.textBoxExplainPoint.Name = "textBoxExplainPoint";
            this.textBoxExplainPoint.Size = new System.Drawing.Size(280, 21);
            this.textBoxExplainPoint.TabIndex = 1;
            this.textBoxExplainPoint.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // vButtonExplainPaste
            // 
            this.vButtonExplainPaste.AllowAnimations = true;
            this.vButtonExplainPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExplainPaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExplainPaste.Location = new System.Drawing.Point(450, 6);
            this.vButtonExplainPaste.Name = "vButtonExplainPaste";
            this.vButtonExplainPaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExplainPaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExplainPaste.TabIndex = 3;
            this.vButtonExplainPaste.Text = "→";
            this.vButtonExplainPaste.UseVisualStyleBackColor = false;
            this.vButtonExplainPaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonExplainPaste.Click += new System.EventHandler(this.vButtonExplainPaste_Click);
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(13, 37);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(62, 15);
            this.label79.TabIndex = 40;
            this.label79.Text = "Подпункт";
            // 
            // tabPageTerminate
            // 
            this.tabPageTerminate.BackColor = System.Drawing.Color.White;
            this.tabPageTerminate.Controls.Add(this.dateTimePickerTerminateDate);
            this.tabPageTerminate.Controls.Add(this.label1);
            this.tabPageTerminate.Controls.Add(this.vButtonTerminatePaste);
            this.tabPageTerminate.Controls.Add(this.textBoxTerminateAgreement);
            this.tabPageTerminate.Controls.Add(this.label80);
            this.tabPageTerminate.Location = new System.Drawing.Point(4, 22);
            this.tabPageTerminate.Name = "tabPageTerminate";
            this.tabPageTerminate.Size = new System.Drawing.Size(482, 194);
            this.tabPageTerminate.TabIndex = 3;
            this.tabPageTerminate.Text = "Расторгнуть";
            // 
            // dateTimePickerTerminateDate
            // 
            this.dateTimePickerTerminateDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerTerminateDate.Location = new System.Drawing.Point(164, 34);
            this.dateTimePickerTerminateDate.Name = "dateTimePickerTerminateDate";
            this.dateTimePickerTerminateDate.Size = new System.Drawing.Size(280, 21);
            this.dateTimePickerTerminateDate.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 15);
            this.label1.TabIndex = 44;
            this.label1.Text = "Дата расторжения";
            // 
            // vButtonTerminatePaste
            // 
            this.vButtonTerminatePaste.AllowAnimations = true;
            this.vButtonTerminatePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonTerminatePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonTerminatePaste.Location = new System.Drawing.Point(450, 6);
            this.vButtonTerminatePaste.Name = "vButtonTerminatePaste";
            this.vButtonTerminatePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonTerminatePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonTerminatePaste.TabIndex = 1;
            this.vButtonTerminatePaste.Text = "→";
            this.vButtonTerminatePaste.UseVisualStyleBackColor = false;
            this.vButtonTerminatePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonTerminatePaste.Click += new System.EventHandler(this.vButtonTerminatePaste_Click);
            // 
            // textBoxTerminateAgreement
            // 
            this.textBoxTerminateAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTerminateAgreement.Location = new System.Drawing.Point(164, 6);
            this.textBoxTerminateAgreement.Name = "textBoxTerminateAgreement";
            this.textBoxTerminateAgreement.Size = new System.Drawing.Size(280, 21);
            this.textBoxTerminateAgreement.TabIndex = 0;
            this.textBoxTerminateAgreement.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(12, 10);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(110, 15);
            this.label80.TabIndex = 43;
            this.label80.Text = "По какой причине";
            // 
            // tabPageProlong
            // 
            this.tabPageProlong.Controls.Add(this.label12);
            this.tabPageProlong.Controls.Add(this.comboBoxCommercialProlongReasonType);
            this.tabPageProlong.Controls.Add(this.checkBoxCommercialProlongUntilDismissal);
            this.tabPageProlong.Controls.Add(this.textBoxCommercialProlongGeneralPoint);
            this.tabPageProlong.Controls.Add(this.label11);
            this.tabPageProlong.Controls.Add(this.dateTimePickerCommercialProlongTo);
            this.tabPageProlong.Controls.Add(this.label8);
            this.tabPageProlong.Controls.Add(this.dateTimePickerCommercialProlongFrom);
            this.tabPageProlong.Controls.Add(this.label7);
            this.tabPageProlong.Controls.Add(this.dateTimePickerCommercialProlongRequest);
            this.tabPageProlong.Controls.Add(this.label6);
            this.tabPageProlong.Controls.Add(this.vButtonProlong);
            this.tabPageProlong.Location = new System.Drawing.Point(4, 22);
            this.tabPageProlong.Name = "tabPageProlong";
            this.tabPageProlong.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProlong.Size = new System.Drawing.Size(482, 194);
            this.tabPageProlong.TabIndex = 5;
            this.tabPageProlong.Text = "Срок по ком. найму";
            this.tabPageProlong.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(130, 15);
            this.label12.TabIndex = 58;
            this.label12.Text = "Документ-основание";
            // 
            // comboBoxCommercialProlongReasonType
            // 
            this.comboBoxCommercialProlongReasonType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCommercialProlongReasonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCommercialProlongReasonType.FormattingEnabled = true;
            this.comboBoxCommercialProlongReasonType.Location = new System.Drawing.Point(189, 6);
            this.comboBoxCommercialProlongReasonType.Name = "comboBoxCommercialProlongReasonType";
            this.comboBoxCommercialProlongReasonType.Size = new System.Drawing.Size(254, 23);
            this.comboBoxCommercialProlongReasonType.TabIndex = 57;
            // 
            // checkBoxCommercialProlongUntilDismissal
            // 
            this.checkBoxCommercialProlongUntilDismissal.AutoSize = true;
            this.checkBoxCommercialProlongUntilDismissal.Location = new System.Drawing.Point(189, 117);
            this.checkBoxCommercialProlongUntilDismissal.Name = "checkBoxCommercialProlongUntilDismissal";
            this.checkBoxCommercialProlongUntilDismissal.Size = new System.Drawing.Size(213, 19);
            this.checkBoxCommercialProlongUntilDismissal.TabIndex = 3;
            this.checkBoxCommercialProlongUntilDismissal.Text = "На период трудовых отношений";
            this.checkBoxCommercialProlongUntilDismissal.UseVisualStyleBackColor = true;
            this.checkBoxCommercialProlongUntilDismissal.CheckedChanged += new System.EventHandler(this.checkBoxCommercialProlongUntilDismissal_CheckedChanged);
            // 
            // textBoxCommercialProlongGeneralPoint
            // 
            this.textBoxCommercialProlongGeneralPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCommercialProlongGeneralPoint.Location = new System.Drawing.Point(188, 141);
            this.textBoxCommercialProlongGeneralPoint.Name = "textBoxCommercialProlongGeneralPoint";
            this.textBoxCommercialProlongGeneralPoint.Size = new System.Drawing.Size(255, 21);
            this.textBoxCommercialProlongGeneralPoint.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 144);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(106, 15);
            this.label11.TabIndex = 56;
            this.label11.Text = "Пункт исключить";
            // 
            // dateTimePickerCommercialProlongTo
            // 
            this.dateTimePickerCommercialProlongTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerCommercialProlongTo.Location = new System.Drawing.Point(188, 90);
            this.dateTimePickerCommercialProlongTo.Name = "dateTimePickerCommercialProlongTo";
            this.dateTimePickerCommercialProlongTo.ShowCheckBox = true;
            this.dateTimePickerCommercialProlongTo.Size = new System.Drawing.Size(255, 21);
            this.dateTimePickerCommercialProlongTo.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(151, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 15);
            this.label8.TabIndex = 50;
            this.label8.Text = "по";
            // 
            // dateTimePickerCommercialProlongFrom
            // 
            this.dateTimePickerCommercialProlongFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerCommercialProlongFrom.Location = new System.Drawing.Point(188, 62);
            this.dateTimePickerCommercialProlongFrom.Name = "dateTimePickerCommercialProlongFrom";
            this.dateTimePickerCommercialProlongFrom.ShowCheckBox = true;
            this.dateTimePickerCommercialProlongFrom.Size = new System.Drawing.Size(255, 21);
            this.dateTimePickerCommercialProlongFrom.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 15);
            this.label7.TabIndex = 49;
            this.label7.Text = "Продлить срок договора с";
            // 
            // dateTimePickerCommercialProlongRequest
            // 
            this.dateTimePickerCommercialProlongRequest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerCommercialProlongRequest.Location = new System.Drawing.Point(188, 35);
            this.dateTimePickerCommercialProlongRequest.Name = "dateTimePickerCommercialProlongRequest";
            this.dateTimePickerCommercialProlongRequest.Size = new System.Drawing.Size(255, 21);
            this.dateTimePickerCommercialProlongRequest.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(168, 15);
            this.label6.TabIndex = 47;
            this.label6.Text = "Дата документа-основания";
            // 
            // vButtonProlong
            // 
            this.vButtonProlong.AllowAnimations = true;
            this.vButtonProlong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonProlong.BackColor = System.Drawing.Color.Transparent;
            this.vButtonProlong.Location = new System.Drawing.Point(450, 6);
            this.vButtonProlong.Name = "vButtonProlong";
            this.vButtonProlong.RoundedCornersMask = ((byte)(15));
            this.vButtonProlong.Size = new System.Drawing.Size(27, 20);
            this.vButtonProlong.TabIndex = 4;
            this.vButtonProlong.Text = "→";
            this.vButtonProlong.UseVisualStyleBackColor = false;
            this.vButtonProlong.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonProlong.Click += new System.EventHandler(this.vButtonProlongCommercial_Click);
            // 
            // tabPageProlongSpecial
            // 
            this.tabPageProlongSpecial.Controls.Add(this.groupBox1);
            this.tabPageProlongSpecial.Controls.Add(this.checkBoxSpecialProlongUntilDismissal);
            this.tabPageProlongSpecial.Controls.Add(this.dateTimePickerSpecialProlongTo);
            this.tabPageProlongSpecial.Controls.Add(this.label13);
            this.tabPageProlongSpecial.Controls.Add(this.dateTimePickerSpecialProlongFrom);
            this.tabPageProlongSpecial.Controls.Add(this.label14);
            this.tabPageProlongSpecial.Controls.Add(this.vButtonProlongSpecial);
            this.tabPageProlongSpecial.Location = new System.Drawing.Point(4, 22);
            this.tabPageProlongSpecial.Name = "tabPageProlongSpecial";
            this.tabPageProlongSpecial.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProlongSpecial.Size = new System.Drawing.Size(482, 194);
            this.tabPageProlongSpecial.TabIndex = 6;
            this.tabPageProlongSpecial.Text = "Срок по спец. найму";
            this.tabPageProlongSpecial.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBoxSpecialProlongGeneralPoint);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBoxSpecialProlongPoint);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(6, 83);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 77);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Изложить в новой редакции";
            // 
            // textBoxSpecialProlongGeneralPoint
            // 
            this.textBoxSpecialProlongGeneralPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSpecialProlongGeneralPoint.Location = new System.Drawing.Point(182, 20);
            this.textBoxSpecialProlongGeneralPoint.Name = "textBoxSpecialProlongGeneralPoint";
            this.textBoxSpecialProlongGeneralPoint.Size = new System.Drawing.Size(255, 21);
            this.textBoxSpecialProlongGeneralPoint.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 15);
            this.label9.TabIndex = 58;
            this.label9.Text = "Пункт";
            // 
            // textBoxSpecialProlongPoint
            // 
            this.textBoxSpecialProlongPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSpecialProlongPoint.Location = new System.Drawing.Point(182, 48);
            this.textBoxSpecialProlongPoint.Name = "textBoxSpecialProlongPoint";
            this.textBoxSpecialProlongPoint.Size = new System.Drawing.Size(255, 21);
            this.textBoxSpecialProlongPoint.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 15);
            this.label10.TabIndex = 57;
            this.label10.Text = "Подпункт";
            // 
            // checkBoxSpecialProlongUntilDismissal
            // 
            this.checkBoxSpecialProlongUntilDismissal.AutoSize = true;
            this.checkBoxSpecialProlongUntilDismissal.Location = new System.Drawing.Point(189, 60);
            this.checkBoxSpecialProlongUntilDismissal.Name = "checkBoxSpecialProlongUntilDismissal";
            this.checkBoxSpecialProlongUntilDismissal.Size = new System.Drawing.Size(213, 19);
            this.checkBoxSpecialProlongUntilDismissal.TabIndex = 2;
            this.checkBoxSpecialProlongUntilDismissal.Text = "На период трудовых отношений";
            this.checkBoxSpecialProlongUntilDismissal.UseVisualStyleBackColor = true;
            this.checkBoxSpecialProlongUntilDismissal.CheckedChanged += new System.EventHandler(this.checkBoxSpecialProlongUntilDismissal_CheckedChanged);
            // 
            // dateTimePickerSpecialProlongTo
            // 
            this.dateTimePickerSpecialProlongTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerSpecialProlongTo.Location = new System.Drawing.Point(188, 33);
            this.dateTimePickerSpecialProlongTo.Name = "dateTimePickerSpecialProlongTo";
            this.dateTimePickerSpecialProlongTo.ShowCheckBox = true;
            this.dateTimePickerSpecialProlongTo.Size = new System.Drawing.Size(255, 21);
            this.dateTimePickerSpecialProlongTo.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(151, 36);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(21, 15);
            this.label13.TabIndex = 62;
            this.label13.Text = "по";
            // 
            // dateTimePickerSpecialProlongFrom
            // 
            this.dateTimePickerSpecialProlongFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerSpecialProlongFrom.Location = new System.Drawing.Point(188, 6);
            this.dateTimePickerSpecialProlongFrom.Name = "dateTimePickerSpecialProlongFrom";
            this.dateTimePickerSpecialProlongFrom.ShowCheckBox = true;
            this.dateTimePickerSpecialProlongFrom.Size = new System.Drawing.Size(255, 21);
            this.dateTimePickerSpecialProlongFrom.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(160, 15);
            this.label14.TabIndex = 61;
            this.label14.Text = "Продлить срок договора с";
            // 
            // vButtonProlongSpecial
            // 
            this.vButtonProlongSpecial.AllowAnimations = true;
            this.vButtonProlongSpecial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonProlongSpecial.BackColor = System.Drawing.Color.Transparent;
            this.vButtonProlongSpecial.Location = new System.Drawing.Point(450, 6);
            this.vButtonProlongSpecial.Name = "vButtonProlongSpecial";
            this.vButtonProlongSpecial.RoundedCornersMask = ((byte)(15));
            this.vButtonProlongSpecial.Size = new System.Drawing.Size(27, 20);
            this.vButtonProlongSpecial.TabIndex = 4;
            this.vButtonProlongSpecial.Text = "→";
            this.vButtonProlongSpecial.UseVisualStyleBackColor = false;
            this.vButtonProlongSpecial.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonProlongSpecial.Click += new System.EventHandler(this.vButtonProlongSpecial_Click);
            // 
            // tabPageChangeTenancy
            // 
            this.tabPageChangeTenancy.BackColor = System.Drawing.Color.White;
            this.tabPageChangeTenancy.Controls.Add(this.textBoxChangeTenantChangeFIO);
            this.tabPageChangeTenancy.Controls.Add(this.dataGridViewChangeTenant);
            this.tabPageChangeTenancy.Controls.Add(this.checkBoxExcludeTenant);
            this.tabPageChangeTenancy.Controls.Add(this.vButtonChangeTenancy);
            this.tabPageChangeTenancy.Controls.Add(this.comboboxTenantChangeKinship);
            this.tabPageChangeTenancy.Controls.Add(this.label100);
            this.tabPageChangeTenancy.Controls.Add(this.newTenancy);
            this.tabPageChangeTenancy.Controls.Add(this.curTenancy);
            this.tabPageChangeTenancy.Location = new System.Drawing.Point(4, 22);
            this.tabPageChangeTenancy.Name = "tabPageChangeTenancy";
            this.tabPageChangeTenancy.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageChangeTenancy.Size = new System.Drawing.Size(482, 194);
            this.tabPageChangeTenancy.TabIndex = 4;
            this.tabPageChangeTenancy.Text = "Сменить нанимателя";
            // 
            // textBoxChangeTenantChangeFIO
            // 
            this.textBoxChangeTenantChangeFIO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxChangeTenantChangeFIO.Enabled = false;
            this.textBoxChangeTenantChangeFIO.Location = new System.Drawing.Point(188, 6);
            this.textBoxChangeTenantChangeFIO.Name = "textBoxChangeTenantChangeFIO";
            this.textBoxChangeTenantChangeFIO.Size = new System.Drawing.Size(255, 21);
            this.textBoxChangeTenantChangeFIO.TabIndex = 0;
            // 
            // dataGridViewChangeTenant
            // 
            this.dataGridViewChangeTenant.AllowUserToAddRows = false;
            this.dataGridViewChangeTenant.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewChangeTenant.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewChangeTenant.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewChangeTenant.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewChangeTenant.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewChangeTenant.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewChangeTenant.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.surnameChangeTenant,
            this.nameChangeTenant,
            this.patronymicChangeTenant,
            this.dateofbirthChangeTenant});
            this.dataGridViewChangeTenant.Location = new System.Drawing.Point(3, 96);
            this.dataGridViewChangeTenant.MultiSelect = false;
            this.dataGridViewChangeTenant.Name = "dataGridViewChangeTenant";
            this.dataGridViewChangeTenant.ReadOnly = true;
            this.dataGridViewChangeTenant.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewChangeTenant.Size = new System.Drawing.Size(476, 89);
            this.dataGridViewChangeTenant.TabIndex = 3;
            // 
            // surnameChangeTenant
            // 
            this.surnameChangeTenant.HeaderText = "Фамилия";
            this.surnameChangeTenant.MinimumWidth = 100;
            this.surnameChangeTenant.Name = "surnameChangeTenant";
            this.surnameChangeTenant.ReadOnly = true;
            // 
            // nameChangeTenant
            // 
            this.nameChangeTenant.HeaderText = "Имя";
            this.nameChangeTenant.MinimumWidth = 100;
            this.nameChangeTenant.Name = "nameChangeTenant";
            this.nameChangeTenant.ReadOnly = true;
            // 
            // patronymicChangeTenant
            // 
            this.patronymicChangeTenant.HeaderText = "Отчество";
            this.patronymicChangeTenant.MinimumWidth = 100;
            this.patronymicChangeTenant.Name = "patronymicChangeTenant";
            this.patronymicChangeTenant.ReadOnly = true;
            // 
            // dateofbirthChangeTenant
            // 
            this.dateofbirthChangeTenant.HeaderText = "Дата рождения";
            this.dateofbirthChangeTenant.MinimumWidth = 140;
            this.dateofbirthChangeTenant.Name = "dateofbirthChangeTenant";
            this.dateofbirthChangeTenant.ReadOnly = true;
            // 
            // checkBoxExcludeTenant
            // 
            this.checkBoxExcludeTenant.AutoSize = true;
            this.checkBoxExcludeTenant.Location = new System.Drawing.Point(189, 61);
            this.checkBoxExcludeTenant.Name = "checkBoxExcludeTenant";
            this.checkBoxExcludeTenant.Size = new System.Drawing.Size(221, 19);
            this.checkBoxExcludeTenant.TabIndex = 2;
            this.checkBoxExcludeTenant.Text = "Исключить нанимателя из найма";
            this.checkBoxExcludeTenant.UseVisualStyleBackColor = true;
            this.checkBoxExcludeTenant.CheckStateChanged += new System.EventHandler(this.checkBoxExcludeTenant_CheckStateChanged);
            // 
            // vButtonChangeTenancy
            // 
            this.vButtonChangeTenancy.AllowAnimations = true;
            this.vButtonChangeTenancy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonChangeTenancy.BackColor = System.Drawing.Color.Transparent;
            this.vButtonChangeTenancy.Location = new System.Drawing.Point(450, 6);
            this.vButtonChangeTenancy.Name = "vButtonChangeTenancy";
            this.vButtonChangeTenancy.RoundedCornersMask = ((byte)(15));
            this.vButtonChangeTenancy.Size = new System.Drawing.Size(27, 20);
            this.vButtonChangeTenancy.TabIndex = 4;
            this.vButtonChangeTenancy.Text = "→";
            this.vButtonChangeTenancy.UseVisualStyleBackColor = false;
            this.vButtonChangeTenancy.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonChangeTenancy.Click += new System.EventHandler(this.vButtonChangeTenancy_Click);
            // 
            // comboboxTenantChangeKinship
            // 
            this.comboboxTenantChangeKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboboxTenantChangeKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxTenantChangeKinship.FormattingEnabled = true;
            this.comboboxTenantChangeKinship.Location = new System.Drawing.Point(189, 33);
            this.comboboxTenantChangeKinship.Name = "comboboxTenantChangeKinship";
            this.comboboxTenantChangeKinship.Size = new System.Drawing.Size(254, 23);
            this.comboboxTenantChangeKinship.TabIndex = 1;
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Location = new System.Drawing.Point(11, 38);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(159, 15);
            this.label100.TabIndex = 4;
            this.label100.Text = "Новая родственная связь";
            // 
            // newTenancy
            // 
            this.newTenancy.AutoSize = true;
            this.newTenancy.Location = new System.Drawing.Point(11, 78);
            this.newTenancy.Name = "newTenancy";
            this.newTenancy.Size = new System.Drawing.Size(121, 15);
            this.newTenancy.TabIndex = 2;
            this.newTenancy.Text = "Новый наниматель";
            // 
            // curTenancy
            // 
            this.curTenancy.AutoSize = true;
            this.curTenancy.Location = new System.Drawing.Point(11, 9);
            this.curTenancy.Name = "curTenancy";
            this.curTenancy.Size = new System.Drawing.Size(130, 15);
            this.curTenancy.TabIndex = 0;
            this.curTenancy.Text = "Текущий наниматель";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_agreement,
            this.agreement_date,
            this.agreement_content});
            this.tableLayoutPanel12.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Location = new System.Drawing.Point(3, 363);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(975, 346);
            this.dataGridView.TabIndex = 2;
            // 
            // id_agreement
            // 
            this.id_agreement.HeaderText = "Номер соглашения";
            this.id_agreement.MinimumWidth = 100;
            this.id_agreement.Name = "id_agreement";
            this.id_agreement.ReadOnly = true;
            this.id_agreement.Visible = false;
            // 
            // agreement_date
            // 
            this.agreement_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.agreement_date.HeaderText = "Дата соглашения";
            this.agreement_date.MinimumWidth = 150;
            this.agreement_date.Name = "agreement_date";
            this.agreement_date.ReadOnly = true;
            this.agreement_date.Width = 150;
            // 
            // agreement_content
            // 
            this.agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.agreement_content.FillWeight = 500F;
            this.agreement_content.HeaderText = "Содержание";
            this.agreement_content.MinimumWidth = 100;
            this.agreement_content.Name = "agreement_content";
            this.agreement_content.ReadOnly = true;
            // 
            // TenancyAgreementsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(660, 360);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(987, 718);
            this.Controls.Add(this.tableLayoutPanel12);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyAgreementsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Соглашения найма №{0}";
            this.tableLayoutPanel12.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            this.groupBox30.ResumeLayout(false);
            this.groupBox30.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageExclude.ResumeLayout(false);
            this.tabPageExclude.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).EndInit();
            this.tabPageInclude.ResumeLayout(false);
            this.tabPageInclude.PerformLayout();
            this.tabPageExplain.ResumeLayout(false);
            this.tabPageExplain.PerformLayout();
            this.tabPageTerminate.ResumeLayout(false);
            this.tabPageTerminate.PerformLayout();
            this.tabPageProlong.ResumeLayout(false);
            this.tabPageProlong.PerformLayout();
            this.tabPageProlongSpecial.ResumeLayout(false);
            this.tabPageProlongSpecial.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageChangeTenancy.ResumeLayout(false);
            this.tabPageChangeTenancy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeTenant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private vButton vButtonPaymentInsert;
        private IContainer components;
        private Label label4;
        private TextBox textBoxExplainGeneralPoint;
        private Label label5;
        private TabPage tabPageChangeTenancy;
        private ComboBox comboboxTenantChangeKinship;
        private Label label100;
        private Label newTenancy;
        private Label curTenancy;
        private vButton vButtonChangeTenancy;
        private DataGridView dataGridViewChangeTenant;
        private CheckBox checkBoxExcludeTenant;
        private DataGridViewTextBoxColumn surnameChangeTenant;
        private DataGridViewTextBoxColumn nameChangeTenant;
        private DataGridViewTextBoxColumn patronymicChangeTenant;
        private DataGridViewTextBoxColumn dateofbirthChangeTenant;
        private TextBox textBoxChangeTenantChangeFIO;
        private TabPage tabPageProlong;
        private DateTimePicker dateTimePickerCommercialProlongTo;
        private Label label8;
        private DateTimePicker dateTimePickerCommercialProlongFrom;
        private Label label7;
        private DateTimePicker dateTimePickerCommercialProlongRequest;
        private Label label6;
        private vButton vButtonProlong;
        private TabPage tabPageProlongSpecial;
        private vButton vButtonProlongSpecial;
        private TextBox textBoxCommercialProlongGeneralPoint;
        private Label label11;
        private CheckBox checkBoxCommercialProlongUntilDismissal;
        private CheckBox checkBoxSpecialProlongUntilDismissal;
        private DateTimePicker dateTimePickerSpecialProlongTo;
        private Label label13;
        private DateTimePicker dateTimePickerSpecialProlongFrom;
        private Label label14;
        private GroupBox groupBox1;
        private TextBox textBoxSpecialProlongGeneralPoint;
        private Label label9;
        private TextBox textBoxSpecialProlongPoint;
        private Label label10;
        private Label label12;
        private ComboBox comboBoxCommercialProlongReasonType;
    }
}
