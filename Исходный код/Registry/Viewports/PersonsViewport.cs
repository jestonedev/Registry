using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal sealed class PersonsViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel11 = new TableLayoutPanel();
        private GroupBox groupBox23 = new GroupBox();
        private GroupBox groupBox26 = new GroupBox();
        private GroupBox groupBox27 = new GroupBox();
        private GroupBox groupBox28 = new GroupBox();
        private DataGridView dataGridViewPersons = new DataGridView();
        private DataGridViewTextBoxColumn field_surname = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_name = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_patronymic = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_date_of_birth = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_kinship = new DataGridViewComboBoxColumn();
        private Label label53 = new Label();
        private Label label54 = new Label();
        private Label label55 = new Label();
        private Label label56 = new Label();
        private Label label57 = new Label();
        private Label label58 = new Label();
        private Label label59 = new Label();
        private Label label60 = new Label();
        private Label label61 = new Label();
        private Label label62 = new Label();
        private Label label63 = new Label();
        private Label label64 = new Label();
        private Label label65 = new Label();
        private Label label66 = new Label();
        private Label label67 = new Label();
        private Label label68 = new Label();
        private Label label69 = new Label();
        private Label label70 = new Label();
        private Label label81 = new Label();
        private TextBox textBoxSurname = new TextBox();
        private TextBox textBoxName = new TextBox();
        private TextBox textBoxPatronymic = new TextBox();
        private TextBox textBoxDocumentSeria = new TextBox();
        private TextBox textBoxDocumentNumber = new TextBox();
        private TextBox textBoxRegistrationHouse = new TextBox();
        private TextBox textBoxRegistrationFlat = new TextBox();
        private TextBox textBoxRegistrationRoom = new TextBox();
        private TextBox textBoxResidenceRoom = new TextBox();
        private TextBox textBoxResidenceFlat = new TextBox();
        private TextBox textBoxResidenceHouse = new TextBox();
        private TextBox textBoxPersonalAccount = new TextBox();
        private ComboBox comboBoxKinship = new ComboBox();
        private ComboBox comboBoxDocumentType = new ComboBox();
        private ComboBox comboBoxIssuedBy = new ComboBox();
        private ComboBox comboBoxRegistrationStreet = new ComboBox();
        private ComboBox comboBoxResidenceStreet = new ComboBox();
        private DateTimePicker dateTimePickerDateOfBirth = new DateTimePicker();
        private DateTimePicker dateTimePickerDateOfDocumentIssue = new DateTimePicker();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        public PersonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPagePersons";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Участники найма №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            this.tableLayoutPanel11.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox28.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).BeginInit();
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.Controls.Add(this.groupBox23, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox27, 0, 1);
            this.tableLayoutPanel11.Controls.Add(this.dataGridViewPersons, 0, 2);
            this.tableLayoutPanel11.Controls.Add(this.groupBox26, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.groupBox28, 1, 1);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 3;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.Size = new System.Drawing.Size(990, 665);
            this.tableLayoutPanel11.TabIndex = 0;
            // 
            // groupBox23
            // 
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
            this.groupBox23.Size = new System.Drawing.Size(489, 194);
            this.groupBox23.TabIndex = 6;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Личные данные";
            // 
            // groupBox26
            // 
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
            this.groupBox26.Location = new System.Drawing.Point(498, 3);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(489, 194);
            this.groupBox26.TabIndex = 7;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Документ, удостоверяющий личность";
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
            this.groupBox27.Location = new System.Drawing.Point(3, 173);
            this.groupBox27.Name = "groupBox27";
            this.groupBox27.Size = new System.Drawing.Size(489, 134);
            this.groupBox27.TabIndex = 8;
            this.groupBox27.TabStop = false;
            this.groupBox27.Text = "Адрес регистрации";
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
            this.groupBox28.Location = new System.Drawing.Point(498, 173);
            this.groupBox28.Name = "groupBox28";
            this.groupBox28.Size = new System.Drawing.Size(489, 134);
            this.groupBox28.TabIndex = 9;
            this.groupBox28.TabStop = false;
            this.groupBox28.Text = "Адрес проживания";
            // 
            // dataGridView13
            // 
            this.dataGridViewPersons.AllowUserToAddRows = false;
            this.dataGridViewPersons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewPersons.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPersons.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewPersons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = 
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridViewPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_surname,
            this.field_name,
            this.field_patronymic,
            this.field_date_of_birth,
            this.field_id_kinship});
            this.tableLayoutPanel11.SetColumnSpan(this.dataGridViewPersons, 2);
            this.dataGridViewPersons.Name = "dataGridView13";
            this.dataGridViewPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPersons.TabIndex = 0;
            this.dataGridViewPersons.MultiSelect = false;
            // 
            // field_surname
            // 
            this.field_surname.HeaderText = "Фамилия";
            this.field_surname.MinimumWidth = 100;
            this.field_surname.Name = "surname";
            this.field_surname.ReadOnly = true;
            // 
            // field_name
            // 
            this.field_name.HeaderText = "Имя";
            this.field_name.MinimumWidth = 100;
            this.field_name.Name = "name";
            this.field_name.ReadOnly = true;
            // 
            // field_patronymic
            // 
            this.field_patronymic.HeaderText = "Отчество";
            this.field_patronymic.MinimumWidth = 100;
            this.field_patronymic.Name = "patronymic";
            this.field_patronymic.ReadOnly = true;
            // 
            // field_date_of_birth
            // 
            this.field_date_of_birth.HeaderText = "Дата рождения";
            this.field_date_of_birth.MinimumWidth = 100;
            this.field_date_of_birth.Name = "date_of_birth";
            this.field_date_of_birth.ReadOnly = true;
            // 
            // field_id_kinship
            // 
            this.field_id_kinship.HeaderText = "Отношение/связь";
            this.field_id_kinship.MinimumWidth = 100;
            this.field_id_kinship.Name = "id_kinship";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(17, 22);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(56, 13);
            this.label53.TabIndex = 20;
            this.label53.Text = "Фамилия";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(17, 51);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(29, 13);
            this.label54.TabIndex = 22;
            this.label54.Text = "Имя";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(17, 80);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(54, 13);
            this.label55.TabIndex = 24;
            this.label55.Text = "Отчество";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(17, 110);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(86, 13);
            this.label56.TabIndex = 26;
            this.label56.Text = "Дата рождения";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(17, 138);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(99, 13);
            this.label57.TabIndex = 28;
            this.label57.Text = "Отношение/связь";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(17, 22);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(83, 13);
            this.label58.TabIndex = 30;
            this.label58.Text = "Вид документа";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(17, 51);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(38, 13);
            this.label59.TabIndex = 32;
            this.label59.Text = "Серия";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(17, 80);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(41, 13);
            this.label60.TabIndex = 34;
            this.label60.Text = "Номер";
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(17, 110);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(73, 13);
            this.label61.TabIndex = 36;
            this.label61.Text = "Дата выдачи";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(17, 138);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(63, 13);
            this.label62.TabIndex = 38;
            this.label62.Text = "Кем выдан";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(17, 23);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(39, 13);
            this.label63.TabIndex = 12;
            this.label63.Text = "Улица";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(17, 52);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(70, 13);
            this.label64.TabIndex = 13;
            this.label64.Text = "Номер дома";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(17, 81);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(93, 13);
            this.label65.TabIndex = 16;
            this.label65.Text = "Номер квартиры";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(17, 110);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(93, 13);
            this.label66.TabIndex = 18;
            this.label66.Text = "Номер комнаты";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(17, 110);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(89, 13);
            this.label67.TabIndex = 26;
            this.label67.Text = "Номер комнаты";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(17, 81);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(93, 13);
            this.label68.TabIndex = 24;
            this.label68.Text = "Номер квартиры";
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(17, 23);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(39, 13);
            this.label69.TabIndex = 20;
            this.label69.Text = "Улица";
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(17, 52);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(70, 13);
            this.label70.TabIndex = 21;
            this.label70.Text = "Номер дома";
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(17, 167);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(71, 13);
            this.label81.TabIndex = 30;
            this.label81.Text = "Личный счет";
            // 
            // textBoxSurname
            // 
            this.textBoxSurname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSurname.Location = new System.Drawing.Point(164, 19);
            this.textBoxSurname.Name = "textBoxSurname";
            this.textBoxSurname.Size = new System.Drawing.Size(319, 20);
            this.textBoxSurname.TabIndex = 0;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(164, 48);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(319, 20);
            this.textBoxName.TabIndex = 1;
            // 
            // textBoxPatronymic
            // 
            this.textBoxPatronymic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPatronymic.Location = new System.Drawing.Point(164, 77);
            this.textBoxPatronymic.Name = "textBoxPatronymic";
            this.textBoxPatronymic.Size = new System.Drawing.Size(319, 20);
            this.textBoxPatronymic.TabIndex = 2;
            // 
            // textBoxDocumentSeria
            // 
            this.textBoxDocumentSeria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentSeria.Location = new System.Drawing.Point(164, 48);
            this.textBoxDocumentSeria.Name = "textBoxDocumentSeria";
            this.textBoxDocumentSeria.Size = new System.Drawing.Size(319, 20);
            this.textBoxDocumentSeria.TabIndex = 1;
            // 
            // textBoxDocumentNumber
            // 
            this.textBoxDocumentNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocumentNumber.Location = new System.Drawing.Point(164, 77);
            this.textBoxDocumentNumber.Name = "textBoxDocumentNumber";
            this.textBoxDocumentNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxDocumentNumber.TabIndex = 2;
            // 
            // textBoxRegistrationHouse
            // 
            this.textBoxRegistrationHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationHouse.Location = new System.Drawing.Point(164, 49);
            this.textBoxRegistrationHouse.MaxLength = 10;
            this.textBoxRegistrationHouse.Name = "textBoxRegistrationHouse";
            this.textBoxRegistrationHouse.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationHouse.TabIndex = 1;
            // 
            // textBoxRegistrationFlat
            // 
            this.textBoxRegistrationFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxRegistrationFlat.MaxLength = 15;
            this.textBoxRegistrationFlat.Name = "textBoxRegistrationFlat";
            this.textBoxRegistrationFlat.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationFlat.TabIndex = 2;
            // 
            // textBoxRegistrationRoom
            // 
            this.textBoxRegistrationRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationRoom.Location = new System.Drawing.Point(164, 107);
            this.textBoxRegistrationRoom.MaxLength = 15;
            this.textBoxRegistrationRoom.Name = "textBoxRegistrationRoom";
            this.textBoxRegistrationRoom.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationRoom.TabIndex = 3;
            // 
            // textBoxResidenceRoom
            // 
            this.textBoxResidenceRoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceRoom.Location = new System.Drawing.Point(164, 107);
            this.textBoxResidenceRoom.MaxLength = 15;
            this.textBoxResidenceRoom.Name = "textBoxResidenceRoom";
            this.textBoxResidenceRoom.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceRoom.TabIndex = 3;
            // 
            // textBoxResidenceFlat
            // 
            this.textBoxResidenceFlat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceFlat.Location = new System.Drawing.Point(164, 78);
            this.textBoxResidenceFlat.MaxLength = 15;
            this.textBoxResidenceFlat.Name = "textBoxResidenceFlat";
            this.textBoxResidenceFlat.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceFlat.TabIndex = 2;
            // 
            // textBoxResidenceHouse
            // 
            this.textBoxResidenceHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceHouse.Location = new System.Drawing.Point(164, 49);
            this.textBoxResidenceHouse.MaxLength = 10;
            this.textBoxResidenceHouse.Name = "textBoxResidenceHouse";
            this.textBoxResidenceHouse.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceHouse.TabIndex = 1;
            // 
            // textBoxPersonalAccount
            // 
            this.textBoxPersonalAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPersonalAccount.Location = new System.Drawing.Point(164, 164);
            this.textBoxPersonalAccount.Name = "textBoxPersonalAccount";
            this.textBoxPersonalAccount.Size = new System.Drawing.Size(319, 20);
            this.textBoxPersonalAccount.TabIndex = 29;
            // 
            // comboBoxKinship
            // 
            this.comboBoxKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKinship.FormattingEnabled = true;
            this.comboBoxKinship.Location = new System.Drawing.Point(164, 135);
            this.comboBoxKinship.Name = "comboBoxKinship";
            this.comboBoxKinship.Size = new System.Drawing.Size(319, 21);
            this.comboBoxKinship.TabIndex = 4;
            // 
            // comboBoxDocumentType
            // 
            this.comboBoxDocumentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDocumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDocumentType.FormattingEnabled = true;
            this.comboBoxDocumentType.Location = new System.Drawing.Point(164, 19);
            this.comboBoxDocumentType.Name = "comboBoxDocumentType";
            this.comboBoxDocumentType.Size = new System.Drawing.Size(319, 21);
            this.comboBoxDocumentType.TabIndex = 0;
            // 
            // comboBoxIssuedBy
            // 
            this.comboBoxIssuedBy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIssuedBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIssuedBy.FormattingEnabled = true;
            this.comboBoxIssuedBy.Location = new System.Drawing.Point(164, 135);
            this.comboBoxIssuedBy.Name = "comboBoxIssuedBy";
            this.comboBoxIssuedBy.Size = new System.Drawing.Size(319, 21);
            this.comboBoxIssuedBy.TabIndex = 4;
            // 
            // comboBoxRegistrationStreet
            // 
            this.comboBoxRegistrationStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRegistrationStreet.FormattingEnabled = true;
            this.comboBoxRegistrationStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxRegistrationStreet.Name = "comboBoxRegistrationStreet";
            this.comboBoxRegistrationStreet.Size = new System.Drawing.Size(319, 21);
            this.comboBoxRegistrationStreet.TabIndex = 0;
            // 
            // comboBoxResidenceStreet
            // 
            this.comboBoxResidenceStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxResidenceStreet.FormattingEnabled = true;
            this.comboBoxResidenceStreet.Location = new System.Drawing.Point(164, 20);
            this.comboBoxResidenceStreet.Name = "comboBoxResidenceStreet";
            this.comboBoxResidenceStreet.Size = new System.Drawing.Size(319, 21);
            this.comboBoxResidenceStreet.TabIndex = 0;
            // 
            // dateTimePickerDateOfBirth
            // 
            this.dateTimePickerDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfBirth.Location = new System.Drawing.Point(164, 106);
            this.dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            this.dateTimePickerDateOfBirth.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerDateOfBirth.TabIndex = 3;
            // 
            // dateTimePickerDateOfDocumentIssue
            // 
            this.dateTimePickerDateOfDocumentIssue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateOfDocumentIssue.Location = new System.Drawing.Point(164, 106);
            this.dateTimePickerDateOfDocumentIssue.Name = "dateTimePickerDateOfDocumentIssue";
            this.dateTimePickerDateOfDocumentIssue.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerDateOfDocumentIssue.TabIndex = 3;

            this.tableLayoutPanel11.ResumeLayout(false);
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.groupBox27.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.groupBox28.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).EndInit();
        }
    }
}
