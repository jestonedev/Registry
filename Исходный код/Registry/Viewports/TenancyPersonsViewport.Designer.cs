using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class TenancyPersonsViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel11;
        private GroupBox groupBox23;
        private GroupBox groupBox26;
        private GroupBox groupBox27;
        private GroupBox groupBox28;
        private DataGridView dataGridViewTenancyPersons;
        private Label label53;
        private Label label54;
        private Label label55;
        private Label label56;
        private Label label57;
        private Label label58;
        private Label label59;
        private Label label60;
        private Label label61;
        private Label label62;
        private Label label63;
        private Label label64;
        private Label label65;
        private Label label66;
        private Label label67;
        private Label label68;
        private Label label69;
        private Label label70;
        private Label label81;
        private TextBox textBoxSurname;
        private TextBox textBoxName;
        private TextBox textBoxPatronymic;
        private TextBox textBoxDocumentSeria;
        private TextBox textBoxDocumentNumber;
        private TextBox textBoxRegistrationHouse;
        private TextBox textBoxRegistrationFlat;
        private TextBox textBoxRegistrationRoom;
        private TextBox textBoxResidenceRoom;
        private TextBox textBoxResidenceFlat;
        private TextBox textBoxResidenceHouse;
        private TextBox textBoxPersonalAccount;
        private ComboBox comboBoxKinship;
        private ComboBox comboBoxDocumentType;
        private ComboBox comboBoxIssuedBy;
        private ComboBox comboBoxRegistrationStreet;
        private ComboBox comboBoxResidenceStreet;
        private DateTimePicker dateTimePickerDateOfBirth;
        private DateTimePicker dateTimePickerDateOfDocumentIssue;
        private DateTimePicker dateTimePickerExcludeDate;
        private Label label2;
        private DateTimePicker dateTimePickerIncludeDate;
        private Label label1;
        #endregion Components

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyPersonsViewport));
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.buttonImportFromMSP = new System.Windows.Forms.Button();
            this.textBoxPersonalAccount = new System.Windows.Forms.TextBox();
            this.label81 = new System.Windows.Forms.Label();
            this.dateTimePickerDateOfBirth = new System.Windows.Forms.DateTimePicker();
            this.comboBoxKinship = new System.Windows.Forms.ComboBox();
            this.label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.textBoxPatronymic = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label54 = new System.Windows.Forms.Label();
            this.textBoxSurname = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.groupBox27 = new System.Windows.Forms.GroupBox();
            this.label66 = new System.Windows.Forms.Label();
            this.textBoxRegistrationRoom = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.textBoxRegistrationFlat = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.comboBoxRegistrationStreet = new System.Windows.Forms.ComboBox();
            this.textBoxRegistrationHouse = new System.Windows.Forms.TextBox();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.dateTimePickerExcludeDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerIncludeDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxIssuedBy = new System.Windows.Forms.ComboBox();
            this.label62 = new System.Windows.Forms.Label();
            this.dateTimePickerDateOfDocumentIssue = new System.Windows.Forms.DateTimePicker();
            this.label61 = new System.Windows.Forms.Label();
            this.textBoxDocumentNumber = new System.Windows.Forms.TextBox();
            this.label60 = new System.Windows.Forms.Label();
            this.textBoxDocumentSeria = new System.Windows.Forms.TextBox();
            this.label59 = new System.Windows.Forms.Label();
            this.comboBoxDocumentType = new System.Windows.Forms.ComboBox();
            this.label58 = new System.Windows.Forms.Label();
            this.groupBox28 = new System.Windows.Forms.GroupBox();
            this.label67 = new System.Windows.Forms.Label();
            this.textBoxResidenceRoom = new System.Windows.Forms.TextBox();
            this.label68 = new System.Windows.Forms.Label();
            this.textBoxResidenceFlat = new System.Windows.Forms.TextBox();
            this.label69 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.comboBoxResidenceStreet = new System.Windows.Forms.ComboBox();
            this.textBoxResidenceHouse = new System.Windows.Forms.TextBox();
            this.dataGridViewTenancyPersons = new System.Windows.Forms.DataGridView();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_kinship = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.registration_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel11.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.groupBox28.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.Controls.Add(this.groupBox23, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox27, 0, 1);
            this.tableLayoutPanel11.Controls.Add(this.groupBox26, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox28, 1, 1);
            this.tableLayoutPanel11.Controls.Add(this.dataGridViewTenancyPersons, 0, 2);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 3;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(1002, 724);
            this.tableLayoutPanel11.TabIndex = 0;
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.buttonImportFromMSP);
            this.groupBox23.Controls.Add(this.textBoxPersonalAccount);
            this.groupBox23.Controls.Add(this.label81);
            this.groupBox23.Controls.Add(this.dateTimePickerDateOfBirth);
            this.groupBox23.Controls.Add(this.comboBoxKinship);
            this.groupBox23.Controls.Add(this.label57);
            this.groupBox23.Controls.Add(this.label56);
            this.groupBox23.Controls.Add(this.textBoxPatronymic);
            this.groupBox23.Controls.Add(this.label55);
            this.groupBox23.Controls.Add(this.textBoxName);
            this.groupBox23.Controls.Add(this.label54);
            this.groupBox23.Controls.Add(this.textBoxSurname);
            this.groupBox23.Controls.Add(this.label53);
            this.groupBox23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox23.Location = new System.Drawing.Point(3, 3);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Size = new System.Drawing.Size(495, 214);
            this.groupBox23.TabIndex = 1;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Личные данные";
            // 
            // buttonImportFromMSP
            // 
            this.buttonImportFromMSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImportFromMSP.Location = new System.Drawing.Point(164, 183);
            this.buttonImportFromMSP.Name = "buttonImportFromMSP";
            this.buttonImportFromMSP.Size = new System.Drawing.Size(325, 24);
            this.buttonImportFromMSP.TabIndex = 6;
            this.buttonImportFromMSP.Text = "Импортировать из МСП";
            this.buttonImportFromMSP.UseVisualStyleBackColor = true;
            this.buttonImportFromMSP.Click += new System.EventHandler(this.buttonImportFromMSP_Click);
            // 
            // textBoxPersonalAccount
            // 
            this.textBoxPersonalAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPersonalAccount.Location = new System.Drawing.Point(164, 157);
            this.textBoxPersonalAccount.MaxLength = 255;
            this.textBoxPersonalAccount.Name = "textBoxPersonalAccount";
            this.textBoxPersonalAccount.Size = new System.Drawing.Size(325, 21);
            this.textBoxPersonalAccount.TabIndex = 5;
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(17, 160);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(109, 15);
            this.label81.TabIndex = 30;
            this.label81.Text = "Номер телефона";
            // 
            // dateTimePickerDateOfBirth
            // 
            this.dateTimePickerDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfBirth.Location = new System.Drawing.Point(164, 100);
            this.dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            this.dateTimePickerDateOfBirth.ShowCheckBox = true;
            this.dateTimePickerDateOfBirth.Size = new System.Drawing.Size(325, 21);
            this.dateTimePickerDateOfBirth.TabIndex = 3;
            // 
            // comboBoxKinship
            // 
            this.comboBoxKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxKinship.DropDownHeight = 405;
            this.comboBoxKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKinship.FormattingEnabled = true;
            this.comboBoxKinship.IntegralHeight = false;
            this.comboBoxKinship.Location = new System.Drawing.Point(164, 127);
            this.comboBoxKinship.Name = "comboBoxKinship";
            this.comboBoxKinship.Size = new System.Drawing.Size(325, 23);
            this.comboBoxKinship.TabIndex = 4;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(17, 131);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(110, 15);
            this.label57.TabIndex = 28;
            this.label57.Text = "Отношение/связь";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(17, 103);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(98, 15);
            this.label56.TabIndex = 26;
            this.label56.Text = "Дата рождения";
            // 
            // textBoxPatronymic
            // 
            this.textBoxPatronymic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPatronymic.Location = new System.Drawing.Point(164, 73);
            this.textBoxPatronymic.MaxLength = 255;
            this.textBoxPatronymic.Name = "textBoxPatronymic";
            this.textBoxPatronymic.Size = new System.Drawing.Size(325, 21);
            this.textBoxPatronymic.TabIndex = 2;
            this.textBoxPatronymic.Leave += new System.EventHandler(this.textBoxSNP_Leave);
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(17, 76);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(63, 15);
            this.label55.TabIndex = 24;
            this.label55.Text = "Отчество";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(164, 46);
            this.textBoxName.MaxLength = 50;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(325, 21);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Leave += new System.EventHandler(this.textBoxSNP_Leave);
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(17, 49);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(32, 15);
            this.label54.TabIndex = 22;
            this.label54.Text = "Имя";
            // 
            // textBoxSurname
            // 
            this.textBoxSurname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSurname.Location = new System.Drawing.Point(164, 19);
            this.textBoxSurname.MaxLength = 50;
            this.textBoxSurname.Name = "textBoxSurname";
            this.textBoxSurname.Size = new System.Drawing.Size(325, 21);
            this.textBoxSurname.TabIndex = 0;
            this.textBoxSurname.Leave += new System.EventHandler(this.textBoxSNP_Leave);
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(17, 22);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(62, 15);
            this.label53.TabIndex = 20;
            this.label53.Text = "Фамилия";
            // 
            // groupBox27
            // 
            this.groupBox27.Controls.Add(this.label66);
            this.groupBox27.Controls.Add(this.textBoxRegistrationRoom);
            this.groupBox27.Controls.Add(this.label65);
            this.groupBox27.Controls.Add(this.textBoxRegistrationFlat);
            this.groupBox27.Controls.Add(this.label63);
            this.groupBox27.Controls.Add(this.label64);
            this.groupBox27.Controls.Add(this.comboBoxRegistrationStreet);
            this.groupBox27.Controls.Add(this.textBoxRegistrationHouse);
            this.groupBox27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox27.Location = new System.Drawing.Point(3, 223);
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.Size = new System.Drawing.Size(495, 134);
            this.groupBox27.TabIndex = 3;
            this.groupBox27.TabStop = false;
            this.groupBox27.Text = "Адрес регистрации";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(17, 109);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(101, 15);
            this.label66.TabIndex = 18;
            this.label66.Text = "Номер комнаты";
            // 
            // textBoxRegistrationRoom
            // 
            this.textBoxRegistrationRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationRoom.Location = new System.Drawing.Point(164, 106);
            this.textBoxRegistrationRoom.MaxLength = 15;
            this.textBoxRegistrationRoom.Name = "textBoxRegistrationRoom";
            this.textBoxRegistrationRoom.Size = new System.Drawing.Size(325, 21);
            this.textBoxRegistrationRoom.TabIndex = 3;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(17, 81);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(106, 15);
            this.label65.TabIndex = 16;
            this.label65.Text = "Номер квартиры";
            // 
            // textBoxRegistrationFlat
            // 
            this.textBoxRegistrationFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxRegistrationFlat.MaxLength = 15;
            this.textBoxRegistrationFlat.Name = "textBoxRegistrationFlat";
            this.textBoxRegistrationFlat.Size = new System.Drawing.Size(325, 21);
            this.textBoxRegistrationFlat.TabIndex = 2;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(17, 24);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(43, 15);
            this.label63.TabIndex = 12;
            this.label63.Text = "Улица";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(17, 53);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(79, 15);
            this.label64.TabIndex = 13;
            this.label64.Text = "Номер дома";
            // 
            // comboBoxRegistrationStreet
            // 
            this.comboBoxRegistrationStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRegistrationStreet.FormattingEnabled = true;
            this.comboBoxRegistrationStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxRegistrationStreet.Name = "comboBoxRegistrationStreet";
            this.comboBoxRegistrationStreet.Size = new System.Drawing.Size(325, 23);
            this.comboBoxRegistrationStreet.TabIndex = 0;
            this.comboBoxRegistrationStreet.DropDownClosed += new System.EventHandler(this.comboBoxRegistrationStreet_DropDownClosed);
            this.comboBoxRegistrationStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxRegistrationStreet_KeyUp);
            this.comboBoxRegistrationStreet.Leave += new System.EventHandler(this.comboBoxRegistrationStreet_Leave);
            // 
            // textBoxRegistrationHouse
            // 
            this.textBoxRegistrationHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationHouse.Location = new System.Drawing.Point(164, 50);
            this.textBoxRegistrationHouse.MaxLength = 10;
            this.textBoxRegistrationHouse.Name = "textBoxRegistrationHouse";
            this.textBoxRegistrationHouse.Size = new System.Drawing.Size(325, 21);
            this.textBoxRegistrationHouse.TabIndex = 1;
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.dateTimePickerExcludeDate);
            this.groupBox26.Controls.Add(this.label2);
            this.groupBox26.Controls.Add(this.dateTimePickerIncludeDate);
            this.groupBox26.Controls.Add(this.label1);
            this.groupBox26.Controls.Add(this.comboBoxIssuedBy);
            this.groupBox26.Controls.Add(this.label62);
            this.groupBox26.Controls.Add(this.dateTimePickerDateOfDocumentIssue);
            this.groupBox26.Controls.Add(this.label61);
            this.groupBox26.Controls.Add(this.textBoxDocumentNumber);
            this.groupBox26.Controls.Add(this.label60);
            this.groupBox26.Controls.Add(this.textBoxDocumentSeria);
            this.groupBox26.Controls.Add(this.label59);
            this.groupBox26.Controls.Add(this.comboBoxDocumentType);
            this.groupBox26.Controls.Add(this.label58);
            this.groupBox26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox26.Location = new System.Drawing.Point(504, 3);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(495, 214);
            this.groupBox26.TabIndex = 2;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Документ, удостоверяющий личность";
            // 
            // dateTimePickerExcludeDate
            // 
            this.dateTimePickerExcludeDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerExcludeDate.Location = new System.Drawing.Point(164, 184);
            this.dateTimePickerExcludeDate.Name = "dateTimePickerExcludeDate";
            this.dateTimePickerExcludeDate.ShowCheckBox = true;
            this.dateTimePickerExcludeDate.Size = new System.Drawing.Size(325, 21);
            this.dateTimePickerExcludeDate.TabIndex = 41;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 15);
            this.label2.TabIndex = 42;
            this.label2.Text = "Дата исключения";
            // 
            // dateTimePickerIncludeDate
            // 
            this.dateTimePickerIncludeDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeDate.Location = new System.Drawing.Point(164, 157);
            this.dateTimePickerIncludeDate.Name = "dateTimePickerIncludeDate";
            this.dateTimePickerIncludeDate.ShowCheckBox = true;
            this.dateTimePickerIncludeDate.Size = new System.Drawing.Size(325, 21);
            this.dateTimePickerIncludeDate.TabIndex = 39;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 40;
            this.label1.Text = "Дата включения";
            // 
            // comboBoxIssuedBy
            // 
            this.comboBoxIssuedBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIssuedBy.FormattingEnabled = true;
            this.comboBoxIssuedBy.Location = new System.Drawing.Point(164, 128);
            this.comboBoxIssuedBy.Name = "comboBoxIssuedBy";
            this.comboBoxIssuedBy.Size = new System.Drawing.Size(325, 23);
            this.comboBoxIssuedBy.TabIndex = 4;
            this.comboBoxIssuedBy.DropDownClosed += new System.EventHandler(this.comboBoxIssuedBy_DropDownClosed);
            this.comboBoxIssuedBy.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxIssuedBy_KeyUp);
            this.comboBoxIssuedBy.Leave += new System.EventHandler(this.comboBoxIssuedBy_Leave);
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(17, 132);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(71, 15);
            this.label62.TabIndex = 38;
            this.label62.Text = "Кем выдан";
            // 
            // dateTimePickerDateOfDocumentIssue
            // 
            this.dateTimePickerDateOfDocumentIssue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfDocumentIssue.Location = new System.Drawing.Point(164, 101);
            this.dateTimePickerDateOfDocumentIssue.Name = "dateTimePickerDateOfDocumentIssue";
            this.dateTimePickerDateOfDocumentIssue.ShowCheckBox = true;
            this.dateTimePickerDateOfDocumentIssue.Size = new System.Drawing.Size(325, 21);
            this.dateTimePickerDateOfDocumentIssue.TabIndex = 3;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(17, 104);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(83, 15);
            this.label61.TabIndex = 36;
            this.label61.Text = "Дата выдачи";
            // 
            // textBoxDocumentNumber
            // 
            this.textBoxDocumentNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentNumber.Location = new System.Drawing.Point(164, 74);
            this.textBoxDocumentNumber.MaxLength = 8;
            this.textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            this.textBoxDocumentNumber.Size = new System.Drawing.Size(325, 21);
            this.textBoxDocumentNumber.TabIndex = 2;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(17, 77);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(46, 15);
            this.label60.TabIndex = 34;
            this.label60.Text = "Номер";
            // 
            // textBoxDocumentSeria
            // 
            this.textBoxDocumentSeria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentSeria.Location = new System.Drawing.Point(164, 47);
            this.textBoxDocumentSeria.MaxLength = 8;
            this.textBoxDocumentSeria.Name = "textBoxDocumentSeria";
            this.textBoxDocumentSeria.Size = new System.Drawing.Size(325, 21);
            this.textBoxDocumentSeria.TabIndex = 1;
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(17, 50);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(43, 15);
            this.label59.TabIndex = 32;
            this.label59.Text = "Серия";
            // 
            // comboBoxDocumentType
            // 
            this.comboBoxDocumentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDocumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDocumentType.FormattingEnabled = true;
            this.comboBoxDocumentType.Location = new System.Drawing.Point(164, 18);
            this.comboBoxDocumentType.Name = "comboBoxDocumentType";
            this.comboBoxDocumentType.Size = new System.Drawing.Size(325, 23);
            this.comboBoxDocumentType.TabIndex = 0;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(17, 22);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(94, 15);
            this.label58.TabIndex = 30;
            this.label58.Text = "Вид документа";
            // 
            // groupBox28
            // 
            this.groupBox28.Controls.Add(this.label67);
            this.groupBox28.Controls.Add(this.textBoxResidenceRoom);
            this.groupBox28.Controls.Add(this.label68);
            this.groupBox28.Controls.Add(this.textBoxResidenceFlat);
            this.groupBox28.Controls.Add(this.label69);
            this.groupBox28.Controls.Add(this.label70);
            this.groupBox28.Controls.Add(this.comboBoxResidenceStreet);
            this.groupBox28.Controls.Add(this.textBoxResidenceHouse);
            this.groupBox28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox28.Location = new System.Drawing.Point(504, 223);
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.Size = new System.Drawing.Size(495, 134);
            this.groupBox28.TabIndex = 4;
            this.groupBox28.TabStop = false;
            this.groupBox28.Text = "Адрес проживания";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(17, 110);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(101, 15);
            this.label67.TabIndex = 26;
            this.label67.Text = "Номер комнаты";
            // 
            // textBoxResidenceRoom
            // 
            this.textBoxResidenceRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceRoom.Location = new System.Drawing.Point(164, 106);
            this.textBoxResidenceRoom.MaxLength = 15;
            this.textBoxResidenceRoom.Name = "textBoxResidenceRoom";
            this.textBoxResidenceRoom.Size = new System.Drawing.Size(325, 21);
            this.textBoxResidenceRoom.TabIndex = 3;
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(17, 81);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(106, 15);
            this.label68.TabIndex = 24;
            this.label68.Text = "Номер квартиры";
            // 
            // textBoxResidenceFlat
            // 
            this.textBoxResidenceFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxResidenceFlat.MaxLength = 15;
            this.textBoxResidenceFlat.Name = "textBoxResidenceFlat";
            this.textBoxResidenceFlat.Size = new System.Drawing.Size(325, 21);
            this.textBoxResidenceFlat.TabIndex = 2;
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(17, 24);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(43, 15);
            this.label69.TabIndex = 20;
            this.label69.Text = "Улица";
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(17, 53);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(79, 15);
            this.label70.TabIndex = 21;
            this.label70.Text = "Номер дома";
            // 
            // comboBoxResidenceStreet
            // 
            this.comboBoxResidenceStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxResidenceStreet.FormattingEnabled = true;
            this.comboBoxResidenceStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxResidenceStreet.Name = "comboBoxResidenceStreet";
            this.comboBoxResidenceStreet.Size = new System.Drawing.Size(325, 23);
            this.comboBoxResidenceStreet.TabIndex = 0;
            this.comboBoxResidenceStreet.DropDownClosed += new System.EventHandler(this.comboBoxResidenceStreet_DropDownClosed);
            this.comboBoxResidenceStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxResidenceStreet_KeyUp);
            this.comboBoxResidenceStreet.Leave += new System.EventHandler(this.comboBoxResidenceStreet_Leave);
            // 
            // textBoxResidenceHouse
            // 
            this.textBoxResidenceHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceHouse.Location = new System.Drawing.Point(164, 50);
            this.textBoxResidenceHouse.MaxLength = 10;
            this.textBoxResidenceHouse.Name = "textBoxResidenceHouse";
            this.textBoxResidenceHouse.Size = new System.Drawing.Size(325, 21);
            this.textBoxResidenceHouse.TabIndex = 1;
            // 
            // dataGridViewTenancyPersons
            // 
            this.dataGridViewTenancyPersons.AllowUserToAddRows = false;
            this.dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            this.dataGridViewTenancyPersons.AllowUserToResizeRows = false;
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
            this.date_of_birth,
            this.id_kinship,
            this.registration_date});
            this.tableLayoutPanel11.SetColumnSpan(this.dataGridViewTenancyPersons, 2);
            this.dataGridViewTenancyPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTenancyPersons.Location = new System.Drawing.Point(3, 363);
            this.dataGridViewTenancyPersons.MultiSelect = false;
            this.dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            this.dataGridViewTenancyPersons.ReadOnly = true;
            this.dataGridViewTenancyPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTenancyPersons.Size = new System.Drawing.Size(996, 358);
            this.dataGridViewTenancyPersons.TabIndex = 0;
            // 
            // surname
            // 
            this.surname.HeaderText = "Фамилия";
            this.surname.MinimumWidth = 140;
            this.surname.Name = "surname";
            this.surname.ReadOnly = true;
            // 
            // name
            // 
            this.name.HeaderText = "Имя";
            this.name.MinimumWidth = 140;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // patronymic
            // 
            this.patronymic.HeaderText = "Отчество";
            this.patronymic.MinimumWidth = 140;
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
            // id_kinship
            // 
            this.id_kinship.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_kinship.HeaderText = "Отношение/связь";
            this.id_kinship.MinimumWidth = 140;
            this.id_kinship.Name = "id_kinship";
            this.id_kinship.ReadOnly = true;
            // 
            // registration_date
            // 
            this.registration_date.HeaderText = "Дата регистрации";
            this.registration_date.MinimumWidth = 140;
            this.registration_date.Name = "registration_date";
            this.registration_date.ReadOnly = true;
            // 
            // TenancyPersonsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(660, 420);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.tableLayoutPanel11);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyPersonsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Участники найма №{0}";
            this.tableLayoutPanel11.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.groupBox27.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.groupBox28.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTenancyPersons)).EndInit();
            this.ResumeLayout(false);

        }

        private Button buttonImportFromMSP;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DataGridViewComboBoxColumn id_kinship;
        private DataGridViewTextBoxColumn registration_date;
    }
}
