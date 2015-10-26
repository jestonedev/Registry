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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyViewport));
            tableLayoutPanel9 = new TableLayoutPanel();
            groupBox31 = new GroupBox();
            textBoxDescription = new TextBox();
            groupBox22 = new GroupBox();
            comboBoxExecutor = new ComboBox();
            label41 = new Label();
            comboBoxRentType = new ComboBox();
            label46 = new Label();
            groupBox1 = new GroupBox();
            dataGridViewTenancyPersons = new DataGridView();
            surname = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            patronymic = new DataGridViewTextBoxColumn();
            date_of_birth = new DataGridViewTextBoxColumn();
            id_kinship = new DataGridViewComboBoxColumn();
            groupBoxTenancyContract = new GroupBox();
            tableLayoutPanel10 = new TableLayoutPanel();
            panel6 = new Panel();
            label52 = new Label();
            label51 = new Label();
            dateTimePickerEndDate = new DateTimePicker();
            label50 = new Label();
            dateTimePickerBeginDate = new DateTimePicker();
            label49 = new Label();
            dateTimePickerIssueDate = new DateTimePicker();
            panel5 = new Panel();
            vButtonWarrant = new vButton();
            textBoxSelectedWarrant = new TextBox();
            label82 = new Label();
            label48 = new Label();
            dateTimePickerRegistrationDate = new DateTimePicker();
            textBoxRegistrationNumber = new TextBox();
            label47 = new Label();
            checkBoxContractEnable = new CheckBox();
            groupBox25 = new GroupBox();
            dataGridViewTenancyAgreements = new DataGridView();
            agreement_date = new DataGridViewTextBoxColumn();
            agreement_content = new DataGridViewTextBoxColumn();
            groupBox24 = new GroupBox();
            dataGridViewTenancyReasons = new DataGridView();
            reason_prepared = new DataGridViewTextBoxColumn();
            reason_number = new DataGridViewTextBoxColumn();
            reason_date = new DataGridViewTextBoxColumn();
            groupBoxProtocol = new GroupBox();
            label45 = new Label();
            dateTimePickerProtocolDate = new DateTimePicker();
            label42 = new Label();
            textBoxProtocolNumber = new TextBox();
            checkBoxProtocolEnable = new CheckBox();
            groupBox21 = new GroupBox();
            dataGridViewTenancyAddress = new DataGridView();
            address = new DataGridViewTextBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            living_area = new DataGridViewTextBoxColumn();
            rent_area = new DataGridViewTextBoxColumn();
            tableLayoutPanel9.SuspendLayout();
            groupBox31.SuspendLayout();
            groupBox22.SuspendLayout();
            groupBox1.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).BeginInit();
            groupBoxTenancyContract.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            panel6.SuspendLayout();
            panel5.SuspendLayout();
            groupBox25.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyAgreements)).BeginInit();
            groupBox24.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyReasons)).BeginInit();
            groupBoxProtocol.SuspendLayout();
            groupBox21.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyAddress)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.ColumnCount = 2;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.Controls.Add(groupBox31, 0, 3);
            tableLayoutPanel9.Controls.Add(groupBox22, 0, 0);
            tableLayoutPanel9.Controls.Add(groupBox1, 1, 3);
            tableLayoutPanel9.Controls.Add(groupBoxTenancyContract, 0, 1);
            tableLayoutPanel9.Controls.Add(groupBox25, 1, 2);
            tableLayoutPanel9.Controls.Add(groupBox24, 0, 2);
            tableLayoutPanel9.Controls.Add(groupBoxProtocol, 1, 0);
            tableLayoutPanel9.Controls.Add(groupBox21, 0, 4);
            tableLayoutPanel9.Dock = DockStyle.Fill;
            tableLayoutPanel9.Location = new Point(3, 3);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 5;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.RowStyles.Add(new RowStyle());
            tableLayoutPanel9.Size = new Size(867, 502);
            tableLayoutPanel9.TabIndex = 0;
            // 
            // groupBox31
            // 
            groupBox31.Controls.Add(textBoxDescription);
            groupBox31.Dock = DockStyle.Fill;
            groupBox31.Location = new Point(3, 311);
            groupBox31.Name = "groupBox31";
            groupBox31.Size = new Size(427, 79);
            groupBox31.TabIndex = 6;
            groupBox31.TabStop = false;
            groupBox31.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(3, 17);
            textBoxDescription.MaxLength = 4000;
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(421, 59);
            textBoxDescription.TabIndex = 0;
            textBoxDescription.Enter += selectAll_Enter;
            // 
            // groupBox22
            // 
            groupBox22.Controls.Add(comboBoxExecutor);
            groupBox22.Controls.Add(label41);
            groupBox22.Controls.Add(comboBoxRentType);
            groupBox22.Controls.Add(label46);
            groupBox22.Dock = DockStyle.Fill;
            groupBox22.Location = new Point(3, 3);
            groupBox22.Name = "groupBox22";
            groupBox22.Size = new Size(427, 79);
            groupBox22.TabIndex = 0;
            groupBox22.TabStop = false;
            groupBox22.Text = "Общие сведения";
            // 
            // comboBoxExecutor
            // 
            comboBoxExecutor.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                      | AnchorStyles.Right;
            comboBoxExecutor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxExecutor.FormattingEnabled = true;
            comboBoxExecutor.Location = new Point(168, 48);
            comboBoxExecutor.Name = "comboBoxExecutor";
            comboBoxExecutor.Size = new Size(250, 23);
            comboBoxExecutor.TabIndex = 1;
            // 
            // label41
            // 
            label41.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                             | AnchorStyles.Right;
            label41.AutoSize = true;
            label41.Location = new Point(12, 51);
            label41.Name = "label41";
            label41.Size = new Size(141, 15);
            label41.TabIndex = 1;
            label41.Text = "Составитель договора";
            // 
            // comboBoxRentType
            // 
            comboBoxRentType.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                      | AnchorStyles.Right;
            comboBoxRentType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxRentType.FormattingEnabled = true;
            comboBoxRentType.Location = new Point(168, 19);
            comboBoxRentType.Name = "comboBoxRentType";
            comboBoxRentType.Size = new Size(250, 23);
            comboBoxRentType.TabIndex = 0;
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.Location = new Point(12, 22);
            label46.Name = "label46";
            label46.Size = new Size(108, 15);
            label46.TabIndex = 16;
            label46.Text = "Тип найма жилья";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dataGridViewTenancyPersons);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(436, 311);
            groupBox1.Name = "groupBox1";
            tableLayoutPanel9.SetRowSpan(groupBox1, 2);
            groupBox1.Size = new Size(428, 188);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Участники найма";
            // 
            // dataGridViewTenancyPersons
            // 
            dataGridViewTenancyPersons.AllowUserToAddRows = false;
            dataGridViewTenancyPersons.AllowUserToDeleteRows = false;
            dataGridViewTenancyPersons.AllowUserToResizeRows = false;
            dataGridViewTenancyPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTenancyPersons.BackgroundColor = Color.White;
            dataGridViewTenancyPersons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyPersons.Columns.AddRange(surname, name, patronymic, date_of_birth, id_kinship);
            dataGridViewTenancyPersons.Dock = DockStyle.Fill;
            dataGridViewTenancyPersons.Location = new Point(3, 17);
            dataGridViewTenancyPersons.MultiSelect = false;
            dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            dataGridViewTenancyPersons.ReadOnly = true;
            dataGridViewTenancyPersons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyPersons.Size = new Size(422, 168);
            dataGridViewTenancyPersons.TabIndex = 0;
            dataGridViewTenancyPersons.CellDoubleClick += dataGridViewTenancyPersons_CellDoubleClick;
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
            id_kinship.MinimumWidth = 120;
            id_kinship.Name = "id_kinship";
            id_kinship.ReadOnly = true;
            // 
            // groupBoxTenancyContract
            // 
            tableLayoutPanel9.SetColumnSpan(groupBoxTenancyContract, 2);
            groupBoxTenancyContract.Controls.Add(tableLayoutPanel10);
            groupBoxTenancyContract.Controls.Add(checkBoxContractEnable);
            groupBoxTenancyContract.Dock = DockStyle.Fill;
            groupBoxTenancyContract.Location = new Point(3, 88);
            groupBoxTenancyContract.Name = "groupBoxTenancyContract";
            groupBoxTenancyContract.Size = new Size(861, 109);
            groupBoxTenancyContract.TabIndex = 2;
            groupBoxTenancyContract.TabStop = false;
            groupBoxTenancyContract.Text = "      Договор найма";
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.ColumnCount = 2;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Controls.Add(panel6, 1, 0);
            tableLayoutPanel10.Controls.Add(panel5, 0, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(3, 17);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            tableLayoutPanel10.Size = new Size(855, 89);
            tableLayoutPanel10.TabIndex = 1;
            // 
            // panel6
            // 
            panel6.Controls.Add(label52);
            panel6.Controls.Add(label51);
            panel6.Controls.Add(dateTimePickerEndDate);
            panel6.Controls.Add(label50);
            panel6.Controls.Add(dateTimePickerBeginDate);
            panel6.Controls.Add(label49);
            panel6.Controls.Add(dateTimePickerIssueDate);
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(427, 0);
            panel6.Margin = new Padding(0);
            panel6.Name = "panel6";
            panel6.Size = new Size(428, 90);
            panel6.TabIndex = 1;
            // 
            // label52
            // 
            label52.AutoSize = true;
            label52.Location = new Point(150, 65);
            label52.Name = "label52";
            label52.Size = new Size(21, 15);
            label52.TabIndex = 28;
            label52.Text = "по";
            // 
            // label51
            // 
            label51.AutoSize = true;
            label51.Location = new Point(158, 36);
            label51.Name = "label51";
            label51.Size = new Size(13, 15);
            label51.TabIndex = 27;
            label51.Text = "с";
            // 
            // dateTimePickerEndDate
            // 
            dateTimePickerEndDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            dateTimePickerEndDate.Location = new Point(174, 62);
            dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            dateTimePickerEndDate.ShowCheckBox = true;
            dateTimePickerEndDate.Size = new Size(246, 21);
            dateTimePickerEndDate.TabIndex = 2;
            // 
            // label50
            // 
            label50.AutoSize = true;
            label50.Location = new Point(15, 36);
            label50.Name = "label50";
            label50.Size = new Size(93, 15);
            label50.TabIndex = 25;
            label50.Text = "Срок действия";
            // 
            // dateTimePickerBeginDate
            // 
            dateTimePickerBeginDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                             | AnchorStyles.Right;
            dateTimePickerBeginDate.Location = new Point(174, 33);
            dateTimePickerBeginDate.Name = "dateTimePickerBeginDate";
            dateTimePickerBeginDate.ShowCheckBox = true;
            dateTimePickerBeginDate.Size = new Size(246, 21);
            dateTimePickerBeginDate.TabIndex = 1;
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Location = new Point(15, 7);
            label49.Name = "label49";
            label49.Size = new Size(83, 15);
            label49.TabIndex = 23;
            label49.Text = "Дата выдачи";
            // 
            // dateTimePickerIssueDate
            // 
            dateTimePickerIssueDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                             | AnchorStyles.Right;
            dateTimePickerIssueDate.Location = new Point(174, 4);
            dateTimePickerIssueDate.Name = "dateTimePickerIssueDate";
            dateTimePickerIssueDate.ShowCheckBox = true;
            dateTimePickerIssueDate.Size = new Size(246, 21);
            dateTimePickerIssueDate.TabIndex = 0;
            // 
            // panel5
            // 
            panel5.Controls.Add(vButtonWarrant);
            panel5.Controls.Add(textBoxSelectedWarrant);
            panel5.Controls.Add(label82);
            panel5.Controls.Add(label48);
            panel5.Controls.Add(dateTimePickerRegistrationDate);
            panel5.Controls.Add(textBoxRegistrationNumber);
            panel5.Controls.Add(label47);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 0);
            panel5.Margin = new Padding(0);
            panel5.Name = "panel5";
            panel5.Size = new Size(427, 90);
            panel5.TabIndex = 0;
            // 
            // vButtonWarrant
            // 
            vButtonWarrant.AllowAnimations = true;
            vButtonWarrant.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonWarrant.BackColor = Color.Transparent;
            vButtonWarrant.Location = new Point(391, 62);
            vButtonWarrant.Name = "vButtonWarrant";
            vButtonWarrant.RoundedCornersMask = 15;
            vButtonWarrant.Size = new Size(27, 20);
            vButtonWarrant.TabIndex = 24;
            vButtonWarrant.Text = "...";
            vButtonWarrant.UseVisualStyleBackColor = false;
            vButtonWarrant.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonWarrant.Click += vButtonWarrant_Click;
            // 
            // textBoxSelectedWarrant
            // 
            textBoxSelectedWarrant.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            textBoxSelectedWarrant.Location = new Point(172, 62);
            textBoxSelectedWarrant.Name = "textBoxSelectedWarrant";
            textBoxSelectedWarrant.ReadOnly = true;
            textBoxSelectedWarrant.Size = new Size(213, 21);
            textBoxSelectedWarrant.TabIndex = 2;
            // 
            // label82
            // 
            label82.AutoSize = true;
            label82.Location = new Point(14, 65);
            label82.Name = "label82";
            label82.Size = new Size(92, 15);
            label82.TabIndex = 23;
            label82.Text = "Доверенность";
            // 
            // label48
            // 
            label48.AutoSize = true;
            label48.Location = new Point(14, 36);
            label48.Name = "label48";
            label48.Size = new Size(114, 15);
            label48.TabIndex = 21;
            label48.Text = "Дата регистрации";
            // 
            // dateTimePickerRegistrationDate
            // 
            dateTimePickerRegistrationDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                    | AnchorStyles.Right;
            dateTimePickerRegistrationDate.Location = new Point(172, 33);
            dateTimePickerRegistrationDate.Name = "dateTimePickerRegistrationDate";
            dateTimePickerRegistrationDate.Size = new Size(246, 21);
            dateTimePickerRegistrationDate.TabIndex = 1;
            // 
            // textBoxRegistrationNumber
            // 
            textBoxRegistrationNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                               | AnchorStyles.Right;
            textBoxRegistrationNumber.Location = new Point(172, 4);
            textBoxRegistrationNumber.MaxLength = 255;
            textBoxRegistrationNumber.Name = "textBoxRegistrationNumber";
            textBoxRegistrationNumber.Size = new Size(246, 21);
            textBoxRegistrationNumber.TabIndex = 0;
            textBoxRegistrationNumber.Enter += selectAll_Enter;
            // 
            // label47
            // 
            label47.AutoSize = true;
            label47.Location = new Point(14, 7);
            label47.Name = "label47";
            label47.Size = new Size(152, 15);
            label47.TabIndex = 18;
            label47.Text = "Регистрационный номер";
            // 
            // checkBoxContractEnable
            // 
            checkBoxContractEnable.AutoSize = true;
            checkBoxContractEnable.Checked = true;
            checkBoxContractEnable.CheckState = CheckState.Checked;
            checkBoxContractEnable.Location = new Point(11, 0);
            checkBoxContractEnable.Name = "checkBoxContractEnable";
            checkBoxContractEnable.Size = new Size(15, 14);
            checkBoxContractEnable.TabIndex = 0;
            checkBoxContractEnable.UseVisualStyleBackColor = true;
            checkBoxContractEnable.CheckedChanged += checkBoxProcessEnable_CheckedChanged;
            // 
            // groupBox25
            // 
            groupBox25.Controls.Add(dataGridViewTenancyAgreements);
            groupBox25.Dock = DockStyle.Fill;
            groupBox25.Location = new Point(436, 203);
            groupBox25.Name = "groupBox25";
            groupBox25.Size = new Size(428, 102);
            groupBox25.TabIndex = 4;
            groupBox25.TabStop = false;
            groupBox25.Text = "Соглашения найма";
            // 
            // dataGridViewTenancyAgreements
            // 
            dataGridViewTenancyAgreements.AllowUserToAddRows = false;
            dataGridViewTenancyAgreements.AllowUserToDeleteRows = false;
            dataGridViewTenancyAgreements.AllowUserToResizeRows = false;
            dataGridViewTenancyAgreements.BackgroundColor = Color.White;
            dataGridViewTenancyAgreements.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyAgreements.Columns.AddRange(agreement_date, agreement_content);
            dataGridViewTenancyAgreements.Dock = DockStyle.Fill;
            dataGridViewTenancyAgreements.Location = new Point(3, 17);
            dataGridViewTenancyAgreements.MultiSelect = false;
            dataGridViewTenancyAgreements.Name = "dataGridViewTenancyAgreements";
            dataGridViewTenancyAgreements.ReadOnly = true;
            dataGridViewTenancyAgreements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyAgreements.Size = new Size(422, 82);
            dataGridViewTenancyAgreements.TabIndex = 0;
            dataGridViewTenancyAgreements.CellDoubleClick += dataGridViewTenancyAgreements_CellDoubleClick;
            // 
            // agreement_date
            // 
            agreement_date.HeaderText = "Дата";
            agreement_date.Name = "agreement_date";
            agreement_date.ReadOnly = true;
            // 
            // agreement_content
            // 
            agreement_content.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            agreement_content.HeaderText = "Содержание";
            agreement_content.Name = "agreement_content";
            agreement_content.ReadOnly = true;
            // 
            // groupBox24
            // 
            groupBox24.Controls.Add(dataGridViewTenancyReasons);
            groupBox24.Dock = DockStyle.Fill;
            groupBox24.Location = new Point(3, 203);
            groupBox24.Name = "groupBox24";
            groupBox24.Size = new Size(427, 102);
            groupBox24.TabIndex = 3;
            groupBox24.TabStop = false;
            groupBox24.Text = "Основания найма";
            // 
            // dataGridViewTenancyReasons
            // 
            dataGridViewTenancyReasons.AllowUserToAddRows = false;
            dataGridViewTenancyReasons.AllowUserToDeleteRows = false;
            dataGridViewTenancyReasons.AllowUserToResizeRows = false;
            dataGridViewTenancyReasons.BackgroundColor = Color.White;
            dataGridViewTenancyReasons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyReasons.Columns.AddRange(reason_prepared, reason_number, reason_date);
            dataGridViewTenancyReasons.Dock = DockStyle.Fill;
            dataGridViewTenancyReasons.Location = new Point(3, 17);
            dataGridViewTenancyReasons.MultiSelect = false;
            dataGridViewTenancyReasons.Name = "dataGridViewTenancyReasons";
            dataGridViewTenancyReasons.ReadOnly = true;
            dataGridViewTenancyReasons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyReasons.Size = new Size(421, 82);
            dataGridViewTenancyReasons.TabIndex = 0;
            dataGridViewTenancyReasons.CellDoubleClick += dataGridViewTenancyReasons_CellDoubleClick;
            // 
            // reason_prepared
            // 
            reason_prepared.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            reason_prepared.HeaderText = "Основание";
            reason_prepared.Name = "reason_prepared";
            reason_prepared.ReadOnly = true;
            // 
            // reason_number
            // 
            reason_number.HeaderText = "№";
            reason_number.Name = "reason_number";
            reason_number.ReadOnly = true;
            // 
            // reason_date
            // 
            reason_date.HeaderText = "Дата";
            reason_date.Name = "reason_date";
            reason_date.ReadOnly = true;
            // 
            // groupBoxProtocol
            // 
            groupBoxProtocol.Controls.Add(label45);
            groupBoxProtocol.Controls.Add(dateTimePickerProtocolDate);
            groupBoxProtocol.Controls.Add(label42);
            groupBoxProtocol.Controls.Add(textBoxProtocolNumber);
            groupBoxProtocol.Controls.Add(checkBoxProtocolEnable);
            groupBoxProtocol.Dock = DockStyle.Fill;
            groupBoxProtocol.Location = new Point(436, 3);
            groupBoxProtocol.Name = "groupBoxProtocol";
            groupBoxProtocol.Size = new Size(428, 79);
            groupBoxProtocol.TabIndex = 1;
            groupBoxProtocol.TabStop = false;
            groupBoxProtocol.Text = "      Протокол жилищной комиссии";
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.Location = new Point(12, 52);
            label45.Name = "label45";
            label45.Size = new Size(102, 15);
            label45.TabIndex = 18;
            label45.Text = "Дата протокола";
            // 
            // dateTimePickerProtocolDate
            // 
            dateTimePickerProtocolDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                | AnchorStyles.Right;
            dateTimePickerProtocolDate.Location = new Point(171, 48);
            dateTimePickerProtocolDate.Name = "dateTimePickerProtocolDate";
            dateTimePickerProtocolDate.Size = new Size(246, 21);
            dateTimePickerProtocolDate.TabIndex = 2;
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Location = new Point(12, 22);
            label42.Name = "label42";
            label42.Size = new Size(111, 15);
            label42.TabIndex = 12;
            label42.Text = "Номер протокола";
            // 
            // textBoxProtocolNumber
            // 
            textBoxProtocolNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxProtocolNumber.Location = new Point(171, 19);
            textBoxProtocolNumber.MaxLength = 50;
            textBoxProtocolNumber.Name = "textBoxProtocolNumber";
            textBoxProtocolNumber.Size = new Size(246, 21);
            textBoxProtocolNumber.TabIndex = 1;
            textBoxProtocolNumber.Enter += selectAll_Enter;
            // 
            // checkBoxProtocolEnable
            // 
            checkBoxProtocolEnable.AutoSize = true;
            checkBoxProtocolEnable.Checked = true;
            checkBoxProtocolEnable.CheckState = CheckState.Checked;
            checkBoxProtocolEnable.Location = new Point(11, 0);
            checkBoxProtocolEnable.Name = "checkBoxProtocolEnable";
            checkBoxProtocolEnable.Size = new Size(15, 14);
            checkBoxProtocolEnable.TabIndex = 0;
            checkBoxProtocolEnable.UseVisualStyleBackColor = true;
            checkBoxProtocolEnable.CheckedChanged += checkBoxProtocolEnable_CheckedChanged;
            // 
            // groupBox21
            // 
            groupBox21.Controls.Add(dataGridViewTenancyAddress);
            groupBox21.Dock = DockStyle.Fill;
            groupBox21.Location = new Point(3, 396);
            groupBox21.Name = "groupBox21";
            groupBox21.Size = new Size(427, 103);
            groupBox21.TabIndex = 7;
            groupBox21.TabStop = false;
            groupBox21.Text = "Нанимаемое жилье";
            // 
            // dataGridViewTenancyAddress
            // 
            dataGridViewTenancyAddress.AllowUserToAddRows = false;
            dataGridViewTenancyAddress.AllowUserToDeleteRows = false;
            dataGridViewTenancyAddress.AllowUserToResizeRows = false;
            dataGridViewTenancyAddress.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTenancyAddress.BackgroundColor = Color.White;
            dataGridViewTenancyAddress.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTenancyAddress.Columns.AddRange(address, total_area, living_area, rent_area);
            dataGridViewTenancyAddress.Dock = DockStyle.Fill;
            dataGridViewTenancyAddress.Location = new Point(3, 17);
            dataGridViewTenancyAddress.MultiSelect = false;
            dataGridViewTenancyAddress.Name = "dataGridViewTenancyAddress";
            dataGridViewTenancyAddress.ReadOnly = true;
            dataGridViewTenancyAddress.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyAddress.Size = new Size(421, 83);
            dataGridViewTenancyAddress.TabIndex = 0;
            dataGridViewTenancyAddress.CellDoubleClick += dataGridViewTenancyAddress_CellDoubleClick;
            // 
            // address
            // 
            address.HeaderText = "Адрес";
            address.MinimumWidth = 400;
            address.Name = "address";
            address.ReadOnly = true;
            // 
            // total_area
            // 
            dataGridViewCellStyle1.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle1;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 150;
            total_area.Name = "total_area";
            total_area.ReadOnly = true;
            // 
            // living_area
            // 
            dataGridViewCellStyle2.Format = "#0.0## м²";
            living_area.DefaultCellStyle = dataGridViewCellStyle2;
            living_area.HeaderText = "Жилая площадь";
            living_area.MinimumWidth = 150;
            living_area.Name = "living_area";
            living_area.ReadOnly = true;
            // 
            // rent_area
            // 
            dataGridViewCellStyle3.Format = "#0.0## м²";
            rent_area.DefaultCellStyle = dataGridViewCellStyle3;
            rent_area.HeaderText = "Площадь койко-места";
            rent_area.MinimumWidth = 200;
            rent_area.Name = "rent_area";
            rent_area.ReadOnly = true;
            // 
            // TenancyViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(720, 480);
            BackColor = Color.White;
            ClientSize = new Size(873, 508);
            Controls.Add(tableLayoutPanel9);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyViewport";
            Padding = new Padding(3);
            Text = "Процесс найма №{0}";
            tableLayoutPanel9.ResumeLayout(false);
            groupBox31.ResumeLayout(false);
            groupBox31.PerformLayout();
            groupBox22.ResumeLayout(false);
            groupBox22.PerformLayout();
            groupBox1.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyPersons)).EndInit();
            groupBoxTenancyContract.ResumeLayout(false);
            groupBoxTenancyContract.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            groupBox25.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyAgreements)).EndInit();
            groupBox24.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyReasons)).EndInit();
            groupBoxProtocol.ResumeLayout(false);
            groupBoxProtocol.PerformLayout();
            groupBox21.ResumeLayout(false);
            ((ISupportInitialize)(dataGridViewTenancyAddress)).EndInit();
            ResumeLayout(false);

        }
    }
}
