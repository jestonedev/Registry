using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VIBlend.Utilities;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport
{
    internal partial class TenancyViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel10;
        private GroupBox groupBoxTenancyContract;
        private GroupBox groupBoxProtocol;
        private GroupBox groupBox21;
        private GroupBox groupBox24;
        private GroupBox groupBox25;
        private Panel panel5;
        private Panel panel6;
        private DataGridView dataGridViewTenancyAddress;
        private DataGridView dataGridViewTenancyReasons;
        private DataGridViewTextBoxColumn reason_prepared;
        private DataGridViewTextBoxColumn reason_number;
        private DataGridViewTextBoxColumn reason_date;
        private DataGridView dataGridViewTenancyAgreements;
        private DataGridViewTextBoxColumn agreement_date;
        private DataGridViewTextBoxColumn agreement_content;
        private CheckBox checkBoxContractEnable;
        private CheckBox checkBoxProtocolEnable;
        private Label label42;
        private Label label45;
        private Label label47;
        private Label label48;
        private Label label49;
        private Label label50;
        private Label label51;
        private Label label52;
        private Label label82;
        private TextBox textBoxProtocolNumber;
        private TextBox textBoxRegistrationNumber;
        private TextBox textBoxSelectedWarrant = new TextBox();
        private DateTimePicker dateTimePickerProtocolDate;
        private DateTimePicker dateTimePickerRegistrationDate;
        private DateTimePicker dateTimePickerIssueDate;
        private DateTimePicker dateTimePickerBeginDate;
        private DateTimePicker dateTimePickerEndDate;
        private vButton vButtonWarrant = new vButton();
        private GroupBox groupBox22;
        private ComboBox comboBoxExecutor;
        private Label label41;
        private ComboBox comboBoxRentType;
        private Label label46;
        private DataGridViewTextBoxColumn address;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn rent_area;
        private GroupBox groupBox31;
        private TextBox textBoxDescription;
        private GroupBox groupBox1;
        private DataGridView dataGridViewTenancyPersons;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DataGridViewComboBoxColumn id_kinship;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyViewport));
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox31 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.comboBoxExecutor = new System.Windows.Forms.ComboBox();
            this.label41 = new System.Windows.Forms.Label();
            this.comboBoxRentType = new System.Windows.Forms.ComboBox();
            this.label46 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyPersons = new System.Windows.Forms.DataGridView();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_kinship = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBoxTenancyContract = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.checkBoxUntilDismissal = new System.Windows.Forms.CheckBox();
            this.label52 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDate = new System.Windows.Forms.DateTimePicker();
            this.label50 = new System.Windows.Forms.Label();
            this.dateTimePickerBeginDate = new System.Windows.Forms.DateTimePicker();
            this.label49 = new System.Windows.Forms.Label();
            this.dateTimePickerIssueDate = new System.Windows.Forms.DateTimePicker();
            this.panel5 = new System.Windows.Forms.Panel();
            this.vButtonWarrant = new VIBlend.WinForms.Controls.vButton();
            this.textBoxSelectedWarrant = new System.Windows.Forms.TextBox();
            this.label82 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.dateTimePickerRegistrationDate = new System.Windows.Forms.DateTimePicker();
            this.textBoxRegistrationNumber = new System.Windows.Forms.TextBox();
            this.label47 = new System.Windows.Forms.Label();
            this.checkBoxContractEnable = new System.Windows.Forms.CheckBox();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyAgreements = new System.Windows.Forms.DataGridView();
            this.agreement_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agreement_content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyReasons = new System.Windows.Forms.DataGridView();
            this.reason_prepared = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reason_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reason_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxProtocol = new System.Windows.Forms.GroupBox();
            this.label45 = new System.Windows.Forms.Label();
            this.dateTimePickerProtocolDate = new System.Windows.Forms.DateTimePicker();
            this.label42 = new System.Windows.Forms.Label();
            this.textBoxProtocolNumber = new System.Windows.Forms.TextBox();
            this.checkBoxProtocolEnable = new System.Windows.Forms.CheckBox();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.dataGridViewTenancyAddress = new System.Windows.Forms.DataGridView();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel9.SuspendLayout();
            this.groupBox31.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).BeginInit();
            this.groupBoxTenancyContract.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox25.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAgreements)).BeginInit();
            this.groupBox24.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyReasons)).BeginInit();
            this.groupBoxProtocol.SuspendLayout();
            this.groupBox21.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Controls.Add(this.groupBox31, 0, 3);
            this.tableLayoutPanel9.Controls.Add(this.groupBox22, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.groupBox1, 1, 3);
            this.tableLayoutPanel9.Controls.Add(this.groupBoxTenancyContract, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBox25, 1, 2);
            this.tableLayoutPanel9.Controls.Add(this.groupBox24, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.groupBoxProtocol, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.groupBox21, 0, 4);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 5;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(1002, 724);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.textBoxDescription);
            this.groupBox31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox31.Location = new System.Drawing.Point(3, 437);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Size = new System.Drawing.Size(495, 79);
            this.groupBox31.TabIndex = 6;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(489, 59);
            this.textBoxDescription.TabIndex = 0;
            this.textBoxDescription.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.comboBoxExecutor);
            this.groupBox22.Controls.Add(this.label41);
            this.groupBox22.Controls.Add(this.comboBoxRentType);
            this.groupBox22.Controls.Add(this.label46);
            this.groupBox22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox22.Location = new System.Drawing.Point(3, 3);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(495, 79);
            this.groupBox22.TabIndex = 0;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Общие сведения";
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(168, 48);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(318, 23);
            this.comboBoxExecutor.TabIndex = 1;
            // 
            // label41
            // 
            this.label41.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(12, 51);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(141, 15);
            this.label41.TabIndex = 1;
            this.label41.Text = "Составитель договора";
            // 
            // comboBoxRentType
            // 
            this.comboBoxRentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRentType.FormattingEnabled = true;
            this.comboBoxRentType.Location = new System.Drawing.Point(168, 19);
            this.comboBoxRentType.Name = "comboBoxRentType";
            this.comboBoxRentType.Size = new System.Drawing.Size(318, 23);
            this.comboBoxRentType.TabIndex = 0;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(12, 22);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(108, 15);
            this.label46.TabIndex = 16;
            this.label46.Text = "Тип найма жилья";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridViewTenancyPersons);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(504, 437);
            this.groupBox1.Name = "groupBox1";
            this.tableLayoutPanel9.SetRowSpan(this.groupBox1, 2);
            this.groupBox1.Size = new System.Drawing.Size(495, 284);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Участники найма";
            // 
            // dataGridViewTenancyPersons
            // 
            this.dataGridViewTenancyPersons.AllowUserToAddRows = false;
            this.dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyPersons.AllowUserToResizeRows = false;
            this.dataGridViewTenancyPersons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTenancyPersons.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.surname,
            this.name,
            this.patronymic,
            this.date_of_birth,
            this.id_kinship});
            this.dataGridViewTenancyPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyPersons.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyPersons.MultiSelect = false;
            this.dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            this.dataGridViewTenancyPersons.ReadOnly = true;
            this.dataGridViewTenancyPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyPersons.Size = new System.Drawing.Size(489, 264);
            this.dataGridViewTenancyPersons.TabIndex = 0;
            this.dataGridViewTenancyPersons.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyPersons_CellDoubleClick);
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
            this.date_of_birth.MinimumWidth = 130;
            this.date_of_birth.Name = "date_of_birth";
            this.date_of_birth.ReadOnly = true;
            // 
            // id_kinship
            // 
            this.id_kinship.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_kinship.HeaderText = "Отношение/связь";
            this.id_kinship.MinimumWidth = 120;
            this.id_kinship.Name = "id_kinship";
            this.id_kinship.ReadOnly = true;
            // 
            // groupBoxTenancyContract
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.groupBoxTenancyContract, 2);
            this.groupBoxTenancyContract.Controls.Add(this.tableLayoutPanel10);
            this.groupBoxTenancyContract.Controls.Add(this.checkBoxContractEnable);
            this.groupBoxTenancyContract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTenancyContract.Location = new System.Drawing.Point(3, 88);
            this.groupBoxTenancyContract.Name = "groupBoxTenancyContract";
            this.groupBoxTenancyContract.Size = new System.Drawing.Size(996, 139);
            this.groupBoxTenancyContract.TabIndex = 2;
            this.groupBoxTenancyContract.TabStop = false;
            this.groupBoxTenancyContract.Text = "      Договор найма";
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(990, 119);
            this.tableLayoutPanel10.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.checkBoxUntilDismissal);
            this.panel6.Controls.Add(this.label52);
            this.panel6.Controls.Add(this.label51);
            this.panel6.Controls.Add(this.dateTimePickerEndDate);
            this.panel6.Controls.Add(this.label50);
            this.panel6.Controls.Add(this.dateTimePickerBeginDate);
            this.panel6.Controls.Add(this.label49);
            this.panel6.Controls.Add(this.dateTimePickerIssueDate);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(495, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(495, 119);
            this.panel6.TabIndex = 1;
            // 
            // checkBoxUntilDismissal
            // 
            this.checkBoxUntilDismissal.AutoSize = true;
            this.checkBoxUntilDismissal.Location = new System.Drawing.Point(179, 92);
            this.checkBoxUntilDismissal.Name = "checkBoxUntilDismissal";
            this.checkBoxUntilDismissal.Size = new System.Drawing.Size(311, 19);
            this.checkBoxUntilDismissal.TabIndex = 29;
            this.checkBoxUntilDismissal.Text = "На период трудовых отношений / пролонгирован";
            this.checkBoxUntilDismissal.UseVisualStyleBackColor = true;
            this.checkBoxUntilDismissal.CheckedChanged += new System.EventHandler(this.checkBoxUntilDismissal_CheckedChanged);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(150, 65);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(21, 15);
            this.label52.TabIndex = 28;
            this.label52.Text = "по";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(158, 36);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(13, 15);
            this.label51.TabIndex = 27;
            this.label51.Text = "с";
            // 
            // dateTimePickerEndDate
            // 
            this.dateTimePickerEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDate.Location = new System.Drawing.Point(174, 62);
            this.dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            this.dateTimePickerEndDate.ShowCheckBox = true;
            this.dateTimePickerEndDate.Size = new System.Drawing.Size(313, 21);
            this.dateTimePickerEndDate.TabIndex = 2;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(15, 36);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(93, 15);
            this.label50.TabIndex = 25;
            this.label50.Text = "Срок действия";
            // 
            // dateTimePickerBeginDate
            // 
            this.dateTimePickerBeginDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerBeginDate.Location = new System.Drawing.Point(174, 33);
            this.dateTimePickerBeginDate.Name = "dateTimePickerBeginDate";
            this.dateTimePickerBeginDate.ShowCheckBox = true;
            this.dateTimePickerBeginDate.Size = new System.Drawing.Size(313, 21);
            this.dateTimePickerBeginDate.TabIndex = 1;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(15, 7);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(83, 15);
            this.label49.TabIndex = 23;
            this.label49.Text = "Дата выдачи";
            // 
            // dateTimePickerIssueDate
            // 
            this.dateTimePickerIssueDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIssueDate.Location = new System.Drawing.Point(174, 4);
            this.dateTimePickerIssueDate.Name = "dateTimePickerIssueDate";
            this.dateTimePickerIssueDate.ShowCheckBox = true;
            this.dateTimePickerIssueDate.Size = new System.Drawing.Size(313, 21);
            this.dateTimePickerIssueDate.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.vButtonWarrant);
            this.panel5.Controls.Add(this.textBoxSelectedWarrant);
            this.panel5.Controls.Add(this.label82);
            this.panel5.Controls.Add(this.label48);
            this.panel5.Controls.Add(this.dateTimePickerRegistrationDate);
            this.panel5.Controls.Add(this.textBoxRegistrationNumber);
            this.panel5.Controls.Add(this.label47);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(495, 119);
            this.panel5.TabIndex = 0;
            // 
            // vButtonWarrant
            // 
            this.vButtonWarrant.AllowAnimations = true;
            this.vButtonWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonWarrant.BackColor = System.Drawing.Color.Transparent;
            this.vButtonWarrant.Location = new System.Drawing.Point(459, 62);
            this.vButtonWarrant.Name = "vButtonWarrant";
            this.vButtonWarrant.RoundedCornersMask = ((byte)(15));
            this.vButtonWarrant.Size = new System.Drawing.Size(27, 20);
            this.vButtonWarrant.TabIndex = 24;
            this.vButtonWarrant.Text = "...";
            this.vButtonWarrant.UseVisualStyleBackColor = false;
            this.vButtonWarrant.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonWarrant.Click += new System.EventHandler(this.vButtonWarrant_Click);
            // 
            // textBoxSelectedWarrant
            // 
            this.textBoxSelectedWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSelectedWarrant.Location = new System.Drawing.Point(172, 62);
            this.textBoxSelectedWarrant.Name = "textBoxSelectedWarrant";
            this.textBoxSelectedWarrant.ReadOnly = true;
            this.textBoxSelectedWarrant.Size = new System.Drawing.Size(281, 21);
            this.textBoxSelectedWarrant.TabIndex = 2;
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Location = new System.Drawing.Point(14, 65);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(92, 15);
            this.label82.TabIndex = 23;
            this.label82.Text = "Доверенность";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(14, 36);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(114, 15);
            this.label48.TabIndex = 21;
            this.label48.Text = "Дата регистрации";
            // 
            // dateTimePickerRegistrationDate
            // 
            this.dateTimePickerRegistrationDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRegistrationDate.Location = new System.Drawing.Point(172, 33);
            this.dateTimePickerRegistrationDate.Name = "dateTimePickerRegistrationDate";
            this.dateTimePickerRegistrationDate.Size = new System.Drawing.Size(314, 21);
            this.dateTimePickerRegistrationDate.TabIndex = 1;
            // 
            // textBoxRegistrationNumber
            // 
            this.textBoxRegistrationNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationNumber.Location = new System.Drawing.Point(172, 4);
            this.textBoxRegistrationNumber.MaxLength = 255;
            this.textBoxRegistrationNumber.Name = "textBoxRegistrationNumber";
            this.textBoxRegistrationNumber.Size = new System.Drawing.Size(314, 21);
            this.textBoxRegistrationNumber.TabIndex = 0;
            this.textBoxRegistrationNumber.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(14, 7);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(152, 15);
            this.label47.TabIndex = 18;
            this.label47.Text = "Регистрационный номер";
            // 
            // checkBoxContractEnable
            // 
            this.checkBoxContractEnable.AutoSize = true;
            this.checkBoxContractEnable.Checked = true;
            this.checkBoxContractEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxContractEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxContractEnable.Name = "checkBoxContractEnable";
            this.checkBoxContractEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxContractEnable.TabIndex = 0;
            this.checkBoxContractEnable.UseVisualStyleBackColor = true;
            this.checkBoxContractEnable.CheckedChanged += new System.EventHandler(this.checkBoxProcessEnable_CheckedChanged);
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.dataGridViewTenancyAgreements);
            this.groupBox25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox25.Location = new System.Drawing.Point(504, 233);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(495, 198);
            this.groupBox25.TabIndex = 4;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Соглашения найма";
            // 
            // dataGridViewTenancyAgreements
            // 
            this.dataGridViewTenancyAgreements.AllowUserToAddRows = false;
            this.dataGridViewTenancyAgreements.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyAgreements.AllowUserToResizeRows = false;
            this.dataGridViewTenancyAgreements.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyAgreements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyAgreements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.agreement_date,
            this.agreement_content});
            this.dataGridViewTenancyAgreements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyAgreements.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyAgreements.MultiSelect = false;
            this.dataGridViewTenancyAgreements.Name = "dataGridViewTenancyAgreements";
            this.dataGridViewTenancyAgreements.ReadOnly = true;
            this.dataGridViewTenancyAgreements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyAgreements.Size = new System.Drawing.Size(489, 178);
            this.dataGridViewTenancyAgreements.TabIndex = 0;
            this.dataGridViewTenancyAgreements.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyAgreements_CellDoubleClick);
            // 
            // agreement_date
            // 
            this.agreement_date.HeaderText = "Дата";
            this.agreement_date.Name = "agreement_date";
            this.agreement_date.ReadOnly = true;
            // 
            // agreement_content
            // 
            this.agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.agreement_content.HeaderText = "Содержание";
            this.agreement_content.Name = "agreement_content";
            this.agreement_content.ReadOnly = true;
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.dataGridViewTenancyReasons);
            this.groupBox24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox24.Location = new System.Drawing.Point(3, 233);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(495, 198);
            this.groupBox24.TabIndex = 3;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Основания найма";
            // 
            // dataGridViewTenancyReasons
            // 
            this.dataGridViewTenancyReasons.AllowUserToAddRows = false;
            this.dataGridViewTenancyReasons.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyReasons.AllowUserToResizeRows = false;
            this.dataGridViewTenancyReasons.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyReasons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyReasons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.reason_prepared,
            this.reason_number,
            this.reason_date});
            this.dataGridViewTenancyReasons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyReasons.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyReasons.MultiSelect = false;
            this.dataGridViewTenancyReasons.Name = "dataGridViewTenancyReasons";
            this.dataGridViewTenancyReasons.ReadOnly = true;
            this.dataGridViewTenancyReasons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyReasons.Size = new System.Drawing.Size(489, 178);
            this.dataGridViewTenancyReasons.TabIndex = 0;
            this.dataGridViewTenancyReasons.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyReasons_CellDoubleClick);
            // 
            // reason_prepared
            // 
            this.reason_prepared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.reason_prepared.HeaderText = "Основание";
            this.reason_prepared.Name = "reason_prepared";
            this.reason_prepared.ReadOnly = true;
            // 
            // reason_number
            // 
            this.reason_number.HeaderText = "№";
            this.reason_number.Name = "reason_number";
            this.reason_number.ReadOnly = true;
            // 
            // reason_date
            // 
            this.reason_date.HeaderText = "Дата";
            this.reason_date.Name = "reason_date";
            this.reason_date.ReadOnly = true;
            // 
            // groupBoxProtocol
            // 
            this.groupBoxProtocol.Controls.Add(this.label45);
            this.groupBoxProtocol.Controls.Add(this.dateTimePickerProtocolDate);
            this.groupBoxProtocol.Controls.Add(this.label42);
            this.groupBoxProtocol.Controls.Add(this.textBoxProtocolNumber);
            this.groupBoxProtocol.Controls.Add(this.checkBoxProtocolEnable);
            this.groupBoxProtocol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProtocol.Location = new System.Drawing.Point(504, 3);
            this.groupBoxProtocol.Name = "groupBoxProtocol";
            this.groupBoxProtocol.Size = new System.Drawing.Size(495, 79);
            this.groupBoxProtocol.TabIndex = 1;
            this.groupBoxProtocol.TabStop = false;
            this.groupBoxProtocol.Text = "      Протокол жилищной комиссии";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(12, 52);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(102, 15);
            this.label45.TabIndex = 18;
            this.label45.Text = "Дата протокола";
            // 
            // dateTimePickerProtocolDate
            // 
            this.dateTimePickerProtocolDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerProtocolDate.Location = new System.Drawing.Point(171, 48);
            this.dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            this.dateTimePickerProtocolDate.Size = new System.Drawing.Size(313, 21);
            this.dateTimePickerProtocolDate.TabIndex = 2;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(12, 22);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(111, 15);
            this.label42.TabIndex = 12;
            this.label42.Text = "Номер протокола";
            // 
            // textBoxProtocolNumber
            // 
            this.textBoxProtocolNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProtocolNumber.Location = new System.Drawing.Point(171, 19);
            this.textBoxProtocolNumber.MaxLength = 50;
            this.textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            this.textBoxProtocolNumber.Size = new System.Drawing.Size(313, 21);
            this.textBoxProtocolNumber.TabIndex = 1;
            this.textBoxProtocolNumber.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxProtocolEnable
            // 
            this.checkBoxProtocolEnable.AutoSize = true;
            this.checkBoxProtocolEnable.Checked = true;
            this.checkBoxProtocolEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProtocolEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxProtocolEnable.Name = "checkBoxProtocolEnable";
            this.checkBoxProtocolEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxProtocolEnable.TabIndex = 0;
            this.checkBoxProtocolEnable.UseVisualStyleBackColor = true;
            this.checkBoxProtocolEnable.CheckedChanged += new System.EventHandler(this.checkBoxProtocolEnable_CheckedChanged);
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.dataGridViewTenancyAddress);
            this.groupBox21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox21.Location = new System.Drawing.Point(3, 522);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(495, 199);
            this.groupBox21.TabIndex = 7;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Нанимаемое жилье";
            // 
            // dataGridViewTenancyAddress
            // 
            this.dataGridViewTenancyAddress.AllowUserToAddRows = false;
            this.dataGridViewTenancyAddress.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyAddress.AllowUserToResizeRows = false;
            this.dataGridViewTenancyAddress.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTenancyAddress.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewTenancyAddress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTenancyAddress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.address,
            this.total_area,
            this.living_area,
            this.rent_area});
            this.dataGridViewTenancyAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyAddress.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewTenancyAddress.MultiSelect = false;
            this.dataGridViewTenancyAddress.Name = "dataGridViewTenancyAddress";
            this.dataGridViewTenancyAddress.ReadOnly = true;
            this.dataGridViewTenancyAddress.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyAddress.Size = new System.Drawing.Size(489, 179);
            this.dataGridViewTenancyAddress.TabIndex = 0;
            this.dataGridViewTenancyAddress.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTenancyAddress_CellDoubleClick);
            // 
            // address
            // 
            this.address.HeaderText = "Адрес";
            this.address.MinimumWidth = 400;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            // 
            // total_area
            // 
            dataGridViewCellStyle1.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle1;
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 150;
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            // 
            // living_area
            // 
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.living_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.MinimumWidth = 150;
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            // 
            // rent_area
            // 
            dataGridViewCellStyle3.Format = "#0.0## м²";
            this.rent_area.DefaultCellStyle = dataGridViewCellStyle3;
            this.rent_area.HeaderText = "Площадь койко-места";
            this.rent_area.MinimumWidth = 200;
            this.rent_area.Name = "rent_area";
            this.rent_area.ReadOnly = true;
            // 
            // TenancyViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(720, 480);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.tableLayoutPanel9);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процесс найма №{0}";
            this.tableLayoutPanel9.ResumeLayout(false);
            this.groupBox31.ResumeLayout(false);
            this.groupBox31.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).EndInit();
            this.groupBoxTenancyContract.ResumeLayout(false);
            this.groupBoxTenancyContract.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAgreements)).EndInit();
            this.groupBox24.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyReasons)).EndInit();
            this.groupBoxProtocol.ResumeLayout(false);
            this.groupBoxProtocol.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyAddress)).EndInit();
            this.ResumeLayout(false);

        }

        private CheckBox checkBoxUntilDismissal;
    }
}
