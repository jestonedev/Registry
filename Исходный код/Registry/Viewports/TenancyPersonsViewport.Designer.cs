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
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        private DataGridViewTextBoxColumn date_of_birth;
        private DataGridViewComboBoxColumn id_kinship;
        private DateTimePicker dateTimePickerExcludeDate;
        private Label label2;
        private DateTimePicker dateTimePickerIncludeDate;
        private Label label1;
        #endregion Components

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyPersonsViewport));
            tableLayoutPanel11 = new TableLayoutPanel();
            groupBox23 = new GroupBox();
            textBoxPersonalAccount = new TextBox();
            label81 = new Label();
            dateTimePickerDateOfBirth = new DateTimePicker();
            comboBoxKinship = new ComboBox();
            label57 = new Label();
            label56 = new Label();
            textBoxPatronymic = new TextBox();
            label55 = new Label();
            textBoxName = new TextBox();
            label54 = new Label();
            textBoxSurname = new TextBox();
            label53 = new Label();
            groupBox27 = new GroupBox();
            label66 = new Label();
            textBoxRegistrationRoom = new TextBox();
            label65 = new Label();
            textBoxRegistrationFlat = new TextBox();
            label63 = new Label();
            label64 = new Label();
            comboBoxRegistrationStreet = new ComboBox();
            textBoxRegistrationHouse = new TextBox();
            groupBox26 = new GroupBox();
            dateTimePickerExcludeDate = new DateTimePicker();
            label2 = new Label();
            dateTimePickerIncludeDate = new DateTimePicker();
            label1 = new Label();
            comboBoxIssuedBy = new ComboBox();
            label62 = new Label();
            dateTimePickerDateOfDocumentIssue = new DateTimePicker();
            label61 = new Label();
            textBoxDocumentNumber = new TextBox();
            label60 = new Label();
            textBoxDocumentSeria = new TextBox();
            label59 = new Label();
            comboBoxDocumentType = new ComboBox();
            label58 = new Label();
            groupBox28 = new GroupBox();
            label67 = new Label();
            textBoxResidenceRoom = new TextBox();
            label68 = new Label();
            textBoxResidenceFlat = new TextBox();
            label69 = new Label();
            label70 = new Label();
            comboBoxResidenceStreet = new ComboBox();
            textBoxResidenceHouse = new TextBox();
            dataGridViewTenancyPersons = new DataGridView();
            surname = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            patronymic = new DataGridViewTextBoxColumn();
            date_of_birth = new DataGridViewTextBoxColumn();
            id_kinship = new DataGridViewComboBoxColumn();
            tableLayoutPanel11.SuspendLayout();
            groupBox23.SuspendLayout();
            groupBox27.SuspendLayout();
            groupBox26.SuspendLayout();
            groupBox28.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.ColumnCount = 2;
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel11.Controls.Add(groupBox23, 0, 0);
            tableLayoutPanel11.Controls.Add(groupBox27, 0, 1);
            tableLayoutPanel11.Controls.Add(groupBox26, 1, 0);
            tableLayoutPanel11.Controls.Add(groupBox28, 1, 1);
            tableLayoutPanel11.Controls.Add(dataGridViewTenancyPersons, 0, 2);
            tableLayoutPanel11.Dock = DockStyle.Fill;
            tableLayoutPanel11.Location = new Point(3, 3);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 3;
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel11.Size = new Size(813, 564);
            tableLayoutPanel11.TabIndex = 0;
            // 
            // groupBox23
            // 
            groupBox23.Controls.Add(textBoxPersonalAccount);
            groupBox23.Controls.Add(label81);
            groupBox23.Controls.Add(dateTimePickerDateOfBirth);
            groupBox23.Controls.Add(comboBoxKinship);
            groupBox23.Controls.Add(label57);
            groupBox23.Controls.Add(label56);
            groupBox23.Controls.Add(textBoxPatronymic);
            groupBox23.Controls.Add(label55);
            groupBox23.Controls.Add(textBoxName);
            groupBox23.Controls.Add(label54);
            groupBox23.Controls.Add(textBoxSurname);
            groupBox23.Controls.Add(label53);
            groupBox23.Dock = DockStyle.Fill;
            groupBox23.Location = new Point(3, 3);
            groupBox23.Name = "groupBox23";
            groupBox23.Size = new Size(400, 214);
            groupBox23.TabIndex = 1;
            groupBox23.TabStop = false;
            groupBox23.Text = "Личные данные";
            // 
            // textBoxPersonalAccount
            // 
            textBoxPersonalAccount.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            textBoxPersonalAccount.Location = new Point(164, 157);
            textBoxPersonalAccount.MaxLength = 255;
            textBoxPersonalAccount.Name = "textBoxPersonalAccount";
            textBoxPersonalAccount.Size = new Size(230, 21);
            textBoxPersonalAccount.TabIndex = 29;
            // 
            // label81
            // 
            label81.AutoSize = true;
            label81.Location = new Point(17, 160);
            label81.Name = "label81";
            label81.Size = new Size(86, 15);
            label81.TabIndex = 30;
            label81.Text = "Лицевой счет";
            // 
            // dateTimePickerDateOfBirth
            // 
            dateTimePickerDateOfBirth.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                               | AnchorStyles.Right;
            dateTimePickerDateOfBirth.Location = new Point(164, 100);
            dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            dateTimePickerDateOfBirth.ShowCheckBox = true;
            dateTimePickerDateOfBirth.Size = new Size(230, 21);
            dateTimePickerDateOfBirth.TabIndex = 3;
            // 
            // comboBoxKinship
            // 
            comboBoxKinship.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                     | AnchorStyles.Right;
            comboBoxKinship.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxKinship.FormattingEnabled = true;
            comboBoxKinship.Location = new Point(164, 127);
            comboBoxKinship.Name = "comboBoxKinship";
            comboBoxKinship.Size = new Size(230, 23);
            comboBoxKinship.TabIndex = 4;
            // 
            // label57
            // 
            label57.AutoSize = true;
            label57.Location = new Point(17, 131);
            label57.Name = "label57";
            label57.Size = new Size(110, 15);
            label57.TabIndex = 28;
            label57.Text = "Отношение/связь";
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Location = new Point(17, 103);
            label56.Name = "label56";
            label56.Size = new Size(98, 15);
            label56.TabIndex = 26;
            label56.Text = "Дата рождения";
            // 
            // textBoxPatronymic
            // 
            textBoxPatronymic.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                       | AnchorStyles.Right;
            textBoxPatronymic.Location = new Point(164, 73);
            textBoxPatronymic.MaxLength = 255;
            textBoxPatronymic.Name = "textBoxPatronymic";
            textBoxPatronymic.Size = new Size(230, 21);
            textBoxPatronymic.TabIndex = 2;
            textBoxPatronymic.Leave += textBoxSNP_Leave;
            // 
            // label55
            // 
            label55.AutoSize = true;
            label55.Location = new Point(17, 76);
            label55.Name = "label55";
            label55.Size = new Size(63, 15);
            label55.TabIndex = 24;
            label55.Text = "Отчество";
            // 
            // textBoxName
            // 
            textBoxName.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                 | AnchorStyles.Right;
            textBoxName.Location = new Point(164, 46);
            textBoxName.MaxLength = 50;
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(230, 21);
            textBoxName.TabIndex = 1;
            textBoxName.Leave += textBoxSNP_Leave;
            // 
            // label54
            // 
            label54.AutoSize = true;
            label54.Location = new Point(17, 49);
            label54.Name = "label54";
            label54.Size = new Size(32, 15);
            label54.TabIndex = 22;
            label54.Text = "Имя";
            // 
            // textBoxSurname
            // 
            textBoxSurname.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                    | AnchorStyles.Right;
            textBoxSurname.Location = new Point(164, 19);
            textBoxSurname.MaxLength = 50;
            textBoxSurname.Name = "textBoxSurname";
            textBoxSurname.Size = new Size(230, 21);
            textBoxSurname.TabIndex = 0;
            textBoxSurname.Leave += textBoxSNP_Leave;
            // 
            // label53
            // 
            label53.AutoSize = true;
            label53.Location = new Point(17, 22);
            label53.Name = "label53";
            label53.Size = new Size(62, 15);
            label53.TabIndex = 20;
            label53.Text = "Фамилия";
            // 
            // groupBox27
            // 
            groupBox27.Controls.Add(label66);
            groupBox27.Controls.Add(textBoxRegistrationRoom);
            groupBox27.Controls.Add(label65);
            groupBox27.Controls.Add(textBoxRegistrationFlat);
            groupBox27.Controls.Add(label63);
            groupBox27.Controls.Add(label64);
            groupBox27.Controls.Add(comboBoxRegistrationStreet);
            groupBox27.Controls.Add(textBoxRegistrationHouse);
            groupBox27.Dock = DockStyle.Fill;
            groupBox27.Location = new Point(3, 223);
            groupBox27.Name = "groupBox27";
            groupBox27.Size = new Size(400, 134);
            groupBox27.TabIndex = 3;
            groupBox27.TabStop = false;
            groupBox27.Text = "Адрес регистрации";
            // 
            // label66
            // 
            label66.AutoSize = true;
            label66.Location = new Point(17, 109);
            label66.Name = "label66";
            label66.Size = new Size(101, 15);
            label66.TabIndex = 18;
            label66.Text = "Номер комнаты";
            // 
            // textBoxRegistrationRoom
            // 
            textBoxRegistrationRoom.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                             | AnchorStyles.Right;
            textBoxRegistrationRoom.Location = new Point(164, 106);
            textBoxRegistrationRoom.MaxLength = 15;
            textBoxRegistrationRoom.Name = "textBoxRegistrationRoom";
            textBoxRegistrationRoom.Size = new Size(230, 21);
            textBoxRegistrationRoom.TabIndex = 3;
            // 
            // label65
            // 
            label65.AutoSize = true;
            label65.Location = new Point(17, 81);
            label65.Name = "label65";
            label65.Size = new Size(106, 15);
            label65.TabIndex = 16;
            label65.Text = "Номер квартиры";
            // 
            // textBoxRegistrationFlat
            // 
            textBoxRegistrationFlat.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                             | AnchorStyles.Right;
            textBoxRegistrationFlat.Location = new Point(164, 78);
            textBoxRegistrationFlat.MaxLength = 15;
            textBoxRegistrationFlat.Name = "textBoxRegistrationFlat";
            textBoxRegistrationFlat.Size = new Size(230, 21);
            textBoxRegistrationFlat.TabIndex = 2;
            // 
            // label63
            // 
            label63.AutoSize = true;
            label63.Location = new Point(17, 24);
            label63.Name = "label63";
            label63.Size = new Size(43, 15);
            label63.TabIndex = 12;
            label63.Text = "Улица";
            // 
            // label64
            // 
            label64.AutoSize = true;
            label64.Location = new Point(17, 53);
            label64.Name = "label64";
            label64.Size = new Size(79, 15);
            label64.TabIndex = 13;
            label64.Text = "Номер дома";
            // 
            // comboBoxRegistrationStreet
            // 
            comboBoxRegistrationStreet.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                | AnchorStyles.Right;
            comboBoxRegistrationStreet.FormattingEnabled = true;
            comboBoxRegistrationStreet.Location = new Point(164, 20);
            comboBoxRegistrationStreet.Name = "comboBoxRegistrationStreet";
            comboBoxRegistrationStreet.Size = new Size(230, 23);
            comboBoxRegistrationStreet.TabIndex = 0;
            comboBoxRegistrationStreet.DropDownClosed += comboBoxRegistrationStreet_DropDownClosed;
            comboBoxRegistrationStreet.KeyUp += comboBoxRegistrationStreet_KeyUp;
            comboBoxRegistrationStreet.Leave += comboBoxRegistrationStreet_Leave;
            // 
            // textBoxRegistrationHouse
            // 
            textBoxRegistrationHouse.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                              | AnchorStyles.Right;
            textBoxRegistrationHouse.Location = new Point(164, 50);
            textBoxRegistrationHouse.MaxLength = 10;
            textBoxRegistrationHouse.Name = "textBoxRegistrationHouse";
            textBoxRegistrationHouse.Size = new Size(230, 21);
            textBoxRegistrationHouse.TabIndex = 1;
            // 
            // groupBox26
            // 
            groupBox26.Controls.Add(dateTimePickerExcludeDate);
            groupBox26.Controls.Add(label2);
            groupBox26.Controls.Add(dateTimePickerIncludeDate);
            groupBox26.Controls.Add(label1);
            groupBox26.Controls.Add(comboBoxIssuedBy);
            groupBox26.Controls.Add(label62);
            groupBox26.Controls.Add(dateTimePickerDateOfDocumentIssue);
            groupBox26.Controls.Add(label61);
            groupBox26.Controls.Add(textBoxDocumentNumber);
            groupBox26.Controls.Add(label60);
            groupBox26.Controls.Add(textBoxDocumentSeria);
            groupBox26.Controls.Add(label59);
            groupBox26.Controls.Add(comboBoxDocumentType);
            groupBox26.Controls.Add(label58);
            groupBox26.Dock = DockStyle.Fill;
            groupBox26.Location = new Point(409, 3);
            groupBox26.Name = "groupBox26";
            groupBox26.Size = new Size(401, 214);
            groupBox26.TabIndex = 2;
            groupBox26.TabStop = false;
            groupBox26.Text = "Документ, удостоверяющий личность";
            // 
            // dateTimePickerExcludeDate
            // 
            dateTimePickerExcludeDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                               | AnchorStyles.Right;
            dateTimePickerExcludeDate.Location = new Point(164, 184);
            dateTimePickerExcludeDate.Name = "dateTimePickerExcludeDate";
            dateTimePickerExcludeDate.ShowCheckBox = true;
            dateTimePickerExcludeDate.Size = new Size(231, 21);
            dateTimePickerExcludeDate.TabIndex = 41;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 187);
            label2.Name = "label2";
            label2.Size = new Size(109, 15);
            label2.TabIndex = 42;
            label2.Text = "Дата исключения";
            // 
            // dateTimePickerIncludeDate
            // 
            dateTimePickerIncludeDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                               | AnchorStyles.Right;
            dateTimePickerIncludeDate.Location = new Point(164, 157);
            dateTimePickerIncludeDate.Name = "dateTimePickerIncludeDate";
            dateTimePickerIncludeDate.ShowCheckBox = true;
            dateTimePickerIncludeDate.Size = new Size(231, 21);
            dateTimePickerIncludeDate.TabIndex = 39;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 160);
            label1.Name = "label1";
            label1.Size = new Size(103, 15);
            label1.TabIndex = 40;
            label1.Text = "Дата включения";
            // 
            // comboBoxIssuedBy
            // 
            comboBoxIssuedBy.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                      | AnchorStyles.Right;
            comboBoxIssuedBy.FormattingEnabled = true;
            comboBoxIssuedBy.Location = new Point(164, 128);
            comboBoxIssuedBy.Name = "comboBoxIssuedBy";
            comboBoxIssuedBy.Size = new Size(231, 23);
            comboBoxIssuedBy.TabIndex = 4;
            comboBoxIssuedBy.DropDownClosed += comboBoxIssuedBy_DropDownClosed;
            comboBoxIssuedBy.KeyUp += comboBoxIssuedBy_KeyUp;
            comboBoxIssuedBy.Leave += comboBoxIssuedBy_Leave;
            // 
            // label62
            // 
            label62.AutoSize = true;
            label62.Location = new Point(17, 132);
            label62.Name = "label62";
            label62.Size = new Size(71, 15);
            label62.TabIndex = 38;
            label62.Text = "Кем выдан";
            // 
            // dateTimePickerDateOfDocumentIssue
            // 
            dateTimePickerDateOfDocumentIssue.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                       | AnchorStyles.Right;
            dateTimePickerDateOfDocumentIssue.Location = new Point(164, 101);
            dateTimePickerDateOfDocumentIssue.Name = "dateTimePickerDateOfDocumentIssue";
            dateTimePickerDateOfDocumentIssue.ShowCheckBox = true;
            dateTimePickerDateOfDocumentIssue.Size = new Size(231, 21);
            dateTimePickerDateOfDocumentIssue.TabIndex = 3;
            // 
            // label61
            // 
            label61.AutoSize = true;
            label61.Location = new Point(17, 104);
            label61.Name = "label61";
            label61.Size = new Size(83, 15);
            label61.TabIndex = 36;
            label61.Text = "Дата выдачи";
            // 
            // textBoxDocumentNumber
            // 
            textBoxDocumentNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxDocumentNumber.Location = new Point(164, 74);
            textBoxDocumentNumber.MaxLength = 8;
            textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            textBoxDocumentNumber.Size = new Size(231, 21);
            textBoxDocumentNumber.TabIndex = 2;
            // 
            // label60
            // 
            label60.AutoSize = true;
            label60.Location = new Point(17, 77);
            label60.Name = "label60";
            label60.Size = new Size(46, 15);
            label60.TabIndex = 34;
            label60.Text = "Номер";
            // 
            // textBoxDocumentSeria
            // 
            textBoxDocumentSeria.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                          | AnchorStyles.Right;
            textBoxDocumentSeria.Location = new Point(164, 47);
            textBoxDocumentSeria.MaxLength = 8;
            textBoxDocumentSeria.Name = "textBoxDocumentSeria";
            textBoxDocumentSeria.Size = new Size(231, 21);
            textBoxDocumentSeria.TabIndex = 1;
            // 
            // label59
            // 
            label59.AutoSize = true;
            label59.Location = new Point(17, 50);
            label59.Name = "label59";
            label59.Size = new Size(43, 15);
            label59.TabIndex = 32;
            label59.Text = "Серия";
            // 
            // comboBoxDocumentType
            // 
            comboBoxDocumentType.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                          | AnchorStyles.Right;
            comboBoxDocumentType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDocumentType.FormattingEnabled = true;
            comboBoxDocumentType.Location = new Point(164, 18);
            comboBoxDocumentType.Name = "comboBoxDocumentType";
            comboBoxDocumentType.Size = new Size(231, 23);
            comboBoxDocumentType.TabIndex = 0;
            // 
            // label58
            // 
            label58.AutoSize = true;
            label58.Location = new Point(17, 22);
            label58.Name = "label58";
            label58.Size = new Size(94, 15);
            label58.TabIndex = 30;
            label58.Text = "Вид документа";
            // 
            // groupBox28
            // 
            groupBox28.Controls.Add(label67);
            groupBox28.Controls.Add(textBoxResidenceRoom);
            groupBox28.Controls.Add(label68);
            groupBox28.Controls.Add(textBoxResidenceFlat);
            groupBox28.Controls.Add(label69);
            groupBox28.Controls.Add(label70);
            groupBox28.Controls.Add(comboBoxResidenceStreet);
            groupBox28.Controls.Add(textBoxResidenceHouse);
            groupBox28.Dock = DockStyle.Fill;
            groupBox28.Location = new Point(409, 223);
            groupBox28.Name = "groupBox28";
            groupBox28.Size = new Size(401, 134);
            groupBox28.TabIndex = 4;
            groupBox28.TabStop = false;
            groupBox28.Text = "Адрес проживания";
            // 
            // label67
            // 
            label67.AutoSize = true;
            label67.Location = new Point(17, 110);
            label67.Name = "label67";
            label67.Size = new Size(101, 15);
            label67.TabIndex = 26;
            label67.Text = "Номер комнаты";
            // 
            // textBoxResidenceRoom
            // 
            textBoxResidenceRoom.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                          | AnchorStyles.Right;
            textBoxResidenceRoom.Location = new Point(164, 106);
            textBoxResidenceRoom.MaxLength = 15;
            textBoxResidenceRoom.Name = "textBoxResidenceRoom";
            textBoxResidenceRoom.Size = new Size(231, 21);
            textBoxResidenceRoom.TabIndex = 3;
            // 
            // label68
            // 
            label68.AutoSize = true;
            label68.Location = new Point(17, 81);
            label68.Name = "label68";
            label68.Size = new Size(106, 15);
            label68.TabIndex = 24;
            label68.Text = "Номер квартиры";
            // 
            // textBoxResidenceFlat
            // 
            textBoxResidenceFlat.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                          | AnchorStyles.Right;
            textBoxResidenceFlat.Location = new Point(164, 78);
            textBoxResidenceFlat.MaxLength = 15;
            textBoxResidenceFlat.Name = "textBoxResidenceFlat";
            textBoxResidenceFlat.Size = new Size(231, 21);
            textBoxResidenceFlat.TabIndex = 2;
            // 
            // label69
            // 
            label69.AutoSize = true;
            label69.Location = new Point(17, 24);
            label69.Name = "label69";
            label69.Size = new Size(43, 15);
            label69.TabIndex = 20;
            label69.Text = "Улица";
            // 
            // label70
            // 
            label70.AutoSize = true;
            label70.Location = new Point(17, 53);
            label70.Name = "label70";
            label70.Size = new Size(79, 15);
            label70.TabIndex = 21;
            label70.Text = "Номер дома";
            // 
            // comboBoxResidenceStreet
            // 
            comboBoxResidenceStreet.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                             | AnchorStyles.Right;
            comboBoxResidenceStreet.FormattingEnabled = true;
            comboBoxResidenceStreet.Location = new Point(164, 20);
            comboBoxResidenceStreet.Name = "comboBoxResidenceStreet";
            comboBoxResidenceStreet.Size = new Size(231, 23);
            comboBoxResidenceStreet.TabIndex = 0;
            comboBoxResidenceStreet.DropDownClosed += comboBoxResidenceStreet_DropDownClosed;
            comboBoxResidenceStreet.KeyUp += comboBoxResidenceStreet_KeyUp;
            comboBoxResidenceStreet.Leave += comboBoxResidenceStreet_Leave;
            // 
            // textBoxResidenceHouse
            // 
            textBoxResidenceHouse.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxResidenceHouse.Location = new Point(164, 50);
            textBoxResidenceHouse.MaxLength = 10;
            textBoxResidenceHouse.Name = "textBoxResidenceHouse";
            textBoxResidenceHouse.Size = new Size(231, 21);
            textBoxResidenceHouse.TabIndex = 1;
            // 
            // dataGridViewTenancyPersons
            // 
            dataGridViewTenancyPersons.AllowUserToAddRows = false;
            dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            dataGridViewTenancyPersons.AllowUserToResizeRows = false;
            dataGridViewTenancyPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTenancyPersons.BackgroundColor = Color.White;
            dataGridViewTenancyPersons.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridViewTenancyPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyPersons.Columns.AddRange(surname, name, patronymic, date_of_birth, id_kinship);
            tableLayoutPanel11.SetColumnSpan(dataGridViewTenancyPersons, 2);
            dataGridViewTenancyPersons.Dock = DockStyle.Fill;
            dataGridViewTenancyPersons.Location = new Point(3, 363);
            dataGridViewTenancyPersons.MultiSelect = false;
            dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            dataGridViewTenancyPersons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyPersons.Size = new Size(807, 198);
            dataGridViewTenancyPersons.TabIndex = 0;
            dataGridViewTenancyPersons.DataError += dataGridViewTenancyPersons_DataError;
            // 
            // surname
            // 
            surname.HeaderText = "Фамилия";
            surname.MinimumWidth = 100;
            surname.Name = "surname";
            surname.ReadOnly = true;
            // 
            // name
            // 
            name.HeaderText = "Имя";
            name.MinimumWidth = 100;
            name.Name = "name";
            name.ReadOnly = true;
            // 
            // patronymic
            // 
            patronymic.HeaderText = "Отчество";
            patronymic.MinimumWidth = 100;
            patronymic.Name = "patronymic";
            patronymic.ReadOnly = true;
            // 
            // date_of_birth
            // 
            date_of_birth.HeaderText = "Дата рождения";
            date_of_birth.MinimumWidth = 130;
            date_of_birth.Name = "date_of_birth";
            date_of_birth.ReadOnly = true;
            // 
            // id_kinship
            // 
            id_kinship.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_kinship.HeaderText = "Отношение/связь";
            id_kinship.MinimumWidth = 100;
            id_kinship.Name = "id_kinship";
            // 
            // TenancyPersonsViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(660, 420);
            BackColor = Color.White;
            ClientSize = new Size(819, 570);
            Controls.Add(tableLayoutPanel11);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyPersonsViewport";
            Padding = new Padding(3);
            Text = "Участники найма №{0}";
            tableLayoutPanel11.ResumeLayout(false);
            groupBox23.ResumeLayout(false);
            groupBox23.PerformLayout();
            groupBox27.ResumeLayout(false);
            groupBox27.PerformLayout();
            groupBox26.ResumeLayout(false);
            groupBox26.PerformLayout();
            groupBox28.ResumeLayout(false);
            groupBox28.PerformLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).EndInit();
            ResumeLayout(false);
        }
    }
}
