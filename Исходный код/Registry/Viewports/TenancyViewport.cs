using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal sealed class TenancyViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel9 = new TableLayoutPanel();
        private TableLayoutPanel tableLayoutPanel10 = new TableLayoutPanel();
        private GroupBox groupBox18 = new GroupBox();
        private GroupBox groupBox19 = new GroupBox();
        private GroupBox groupBox20 = new GroupBox();
        private GroupBox groupBox21 = new GroupBox();
        private GroupBox groupBox22 = new GroupBox();
        private GroupBox groupBox24 = new GroupBox();
        private GroupBox groupBox25 = new GroupBox();
        private Panel panel5 = new Panel();
        private Panel panel6 = new Panel();
        private DataGridView dataGridViewPersons = new DataGridView();
        private DataGridViewTextBoxColumn field_surname = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_name = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_patronymic = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_date_of_birth = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_kinship = new DataGridViewComboBoxColumn();
        private DataGridView dataGridViewContractReasons = new DataGridView();
        private DataGridViewTextBoxColumn field_contract_reason_prepared = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_number = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_date = new DataGridViewTextBoxColumn();
        private DataGridView dataGridViewAgreements = new DataGridView();
        private DataGridViewTextBoxColumn field_agreement_date = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_agreement_content = new DataGridViewTextBoxColumn();
        private CheckBox checkBoxContractEnable = new CheckBox();
        private CheckBox checkBoxResidenceWarrantEnable = new CheckBox();
        private CheckBox checkBoxKumiOrderEnable = new CheckBox();
        private CheckBox checkBoxIsContractPermanent = new CheckBox();
        private Label label41 = new Label();
        private Label label42 = new Label();
        private Label label43 = new Label();
        private Label label44 = new Label();
        private Label label45 = new Label();
        private Label label46 = new Label();
        private Label label47 = new Label();
        private Label label48 = new Label();
        private Label label49 = new Label();
        private Label label50 = new Label();
        private Label label51 = new Label();
        private Label label52 = new Label();
        private TextBox textBoxResidenceWarrantNumber = new TextBox();
        private TextBox textBoxKumiOrderNumber = new TextBox();
        private TextBox textBoxDescription = new TextBox();
        private TextBox textBoxRegistrationNumber = new TextBox();
        private DateTimePicker dateTimePickerResidenceWarrantDate = new DateTimePicker();
        private DateTimePicker dateTimePickerKumiOrderDate = new DateTimePicker();
        private DateTimePicker dateTimePickerRegistrationDate = new DateTimePicker();
        private DateTimePicker dateTimePickerIssueDate = new DateTimePicker();
        private DateTimePicker dateTimePickerBeginDate = new DateTimePicker();
        private DateTimePicker dateTimePickerEndDate = new DateTimePicker();
        private ComboBox comboBoxExecutor = new ComboBox();
        private ComboBox comboBoxRentType = new ComboBox();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        public TenancyViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageTenancy";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Запись о найме №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        private void ConstructViewport()
        {
            this.tableLayoutPanel9.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox25.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContractReasons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAgreements)).BeginInit();
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Controls.Add(this.groupBox18, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.groupBox19, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBox21, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.groupBox20, 1, 1);
            this.tableLayoutPanel9.Controls.Add(this.groupBox22, 1, 2);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 3;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(990, 665);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.groupBox25, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.groupBox24, 1, 1);
            this.tableLayoutPanel10.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 2;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(978, 324);
            this.tableLayoutPanel10.TabIndex = 1;
            // 
            // groupBox18
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.groupBox18, 2);
            this.groupBox18.Controls.Add(this.tableLayoutPanel10);
            this.groupBox18.Controls.Add(this.checkBoxContractEnable);
            this.groupBox18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox18.Location = new System.Drawing.Point(3, 3);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(984, 343);
            this.groupBox18.TabIndex = 0;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "      Договор найма";
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.label44);
            this.groupBox19.Controls.Add(this.label43);
            this.groupBox19.Controls.Add(this.textBoxResidenceWarrantNumber);
            this.groupBox19.Controls.Add(this.dateTimePickerResidenceWarrantDate);
            this.groupBox19.Controls.Add(this.checkBoxResidenceWarrantEnable);
            this.groupBox19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox19.Location = new System.Drawing.Point(3, 352);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(489, 77);
            this.groupBox19.TabIndex = 1;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "      Ордер на проживание";
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.label45);
            this.groupBox20.Controls.Add(this.dateTimePickerKumiOrderDate);
            this.groupBox20.Controls.Add(this.label42);
            this.groupBox20.Controls.Add(this.textBoxKumiOrderNumber);
            this.groupBox20.Controls.Add(this.checkBoxKumiOrderEnable);
            this.groupBox20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox20.Location = new System.Drawing.Point(498, 352);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(489, 77);
            this.groupBox20.TabIndex = 2;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "      Распоряжение КУМИ";
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.dataGridViewPersons);
            this.groupBox21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox21.Location = new System.Drawing.Point(3, 435);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(489, 227);
            this.groupBox21.TabIndex = 3;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "Участники найма";
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.comboBoxExecutor);
            this.groupBox22.Controls.Add(this.label41);
            this.groupBox22.Controls.Add(this.textBoxDescription);
            this.groupBox22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox22.Location = new System.Drawing.Point(498, 435);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(489, 227);
            this.groupBox22.TabIndex = 8;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Дополнительные сведения";
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.dataGridViewContractReasons);
            this.groupBox24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox24.Location = new System.Drawing.Point(492, 93);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(483, 228);
            this.groupBox24.TabIndex = 6;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Основания найма";
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.dataGridViewAgreements);
            this.groupBox25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox25.Location = new System.Drawing.Point(3, 93);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(483, 228);
            this.groupBox25.TabIndex = 7;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Соглашения найма";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label48);
            this.panel5.Controls.Add(this.dateTimePickerRegistrationDate);
            this.panel5.Controls.Add(this.textBoxRegistrationNumber);
            this.panel5.Controls.Add(this.label47);
            this.panel5.Controls.Add(this.comboBoxRentType);
            this.panel5.Controls.Add(this.label46);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(489, 90);
            this.panel5.TabIndex = 8;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.checkBoxIsContractPermanent);
            this.panel6.Controls.Add(this.label52);
            this.panel6.Controls.Add(this.label51);
            this.panel6.Controls.Add(this.dateTimePickerEndDate);
            this.panel6.Controls.Add(this.label50);
            this.panel6.Controls.Add(this.dateTimePickerBeginDate);
            this.panel6.Controls.Add(this.label49);
            this.panel6.Controls.Add(this.dateTimePickerIssueDate);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(489, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(489, 90);
            this.panel6.TabIndex = 9;
            // 
            // dataGridViewPersons
            // 
            this.dataGridViewPersons.AllowUserToAddRows = false;
            this.dataGridViewPersons.AllowUserToDeleteRows = false;
            this.dataGridViewPersons.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_surname,
            this.field_name,
            this.field_patronymic,
            this.field_date_of_birth,
            this.field_id_kinship});
            this.dataGridViewPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPersons.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewPersons.Name = "dataGridView12";
            this.dataGridViewPersons.ReadOnly = true;
            this.dataGridViewPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPersons.Size = new System.Drawing.Size(483, 208);
            this.dataGridViewPersons.TabIndex = 0;
            this.dataGridViewPersons.MultiSelect = false;
            // 
            // field_surname
            // 
            this.field_surname.HeaderText = "Фамилия";
            this.field_surname.Name = "surname";
            // 
            // field_name
            // 
            this.field_name.HeaderText = "Имя";
            this.field_name.Name = "name";
            // 
            // field_patronymic
            // 
            this.field_patronymic.HeaderText = "Отчество";
            this.field_patronymic.Name = "patronymic";
            // 
            // field_date_of_birth
            // 
            this.field_date_of_birth.HeaderText = "Дата рождения";
            this.field_date_of_birth.MinimumWidth = 120;
            this.field_date_of_birth.Name = "date_of_birth";
            // 
            // field_id_kinship
            // 
            this.field_id_kinship.HeaderText = "Отношение/связь";
            this.field_id_kinship.Name = "id_kinship";
            // 
            // dataGridViewContractReasons
            // 
            this.dataGridViewContractReasons.AllowUserToAddRows = false;
            this.dataGridViewContractReasons.AllowUserToDeleteRows = false;
            this.dataGridViewContractReasons.Dock = DockStyle.Fill;
            this.dataGridViewContractReasons.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewContractReasons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewContractReasons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_contract_reason_prepared,
            this.field_reason_number,
            this.field_reason_date});
            this.dataGridViewContractReasons.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewContractReasons.Name = "dataGridView14";
            this.dataGridViewContractReasons.ReadOnly = true;
            this.dataGridViewContractReasons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewContractReasons.Size = new System.Drawing.Size(477, 209);
            this.dataGridViewContractReasons.TabIndex = 0;
            this.dataGridViewContractReasons.MultiSelect = false;
            // 
            // field_contract_reason_prepared
            // 
            this.field_contract_reason_prepared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_contract_reason_prepared.HeaderText = "Основание";
            this.field_contract_reason_prepared.Name = "contract_reason_prepared";
            // 
            // field_reason_number
            // 
            this.field_reason_number.HeaderText = "№";
            this.field_reason_number.Name = "reason_number";
            // 
            // field_reason_date
            // 
            this.field_reason_date.HeaderText = "Дата";
            this.field_reason_date.Name = "reason_date";
            // 
            // dataGridViewAgreements
            // 
            this.dataGridViewAgreements.AllowUserToAddRows = false;
            this.dataGridViewAgreements.AllowUserToDeleteRows = false;
            this.dataGridViewAgreements.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewAgreements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAgreements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_agreement_date,
            this.field_agreement_content});
            this.dataGridViewAgreements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAgreements.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewAgreements.Name = "dataGridView15";
            this.dataGridViewAgreements.ReadOnly = true;
            this.dataGridViewAgreements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAgreements.Size = new System.Drawing.Size(477, 209);
            this.dataGridViewAgreements.TabIndex = 0;
            this.dataGridViewAgreements.MultiSelect = false;
            // 
            // field_agreement_date
            // 
            this.field_agreement_date.HeaderText = "Дата";
            this.field_agreement_date.Name = "agreement_date";
            // 
            // field_agreement_content
            // 
            this.field_agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_agreement_content.HeaderText = "Содержание";
            this.field_agreement_content.Name = "agreement_content";
            // 
            // checkBoxContractEnable
            // 
            this.checkBoxContractEnable.AutoSize = true;
            this.checkBoxContractEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxContractEnable.Name = "checkBoxContractEnable";
            this.checkBoxContractEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxContractEnable.TabIndex = 0;
            this.checkBoxContractEnable.UseVisualStyleBackColor = true;
            // 
            // checkBoxResidenceWarrantEnable
            // 
            this.checkBoxResidenceWarrantEnable.AutoSize = true;
            this.checkBoxResidenceWarrantEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxResidenceWarrantEnable.Name = "checkBoxResidenceWarrantEnable";
            this.checkBoxResidenceWarrantEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxResidenceWarrantEnable.TabIndex = 1;
            this.checkBoxResidenceWarrantEnable.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsContractPermanent
            // 
            this.checkBoxIsContractPermanent.AutoSize = true;
            this.checkBoxIsContractPermanent.Location = new System.Drawing.Point(18, 65);
            this.checkBoxIsContractPermanent.Name = "checkBoxIsContractPermanent";
            this.checkBoxIsContractPermanent.Size = new System.Drawing.Size(88, 17);
            this.checkBoxIsContractPermanent.TabIndex = 2;
            this.checkBoxIsContractPermanent.Text = "Бессрочный";
            this.checkBoxIsContractPermanent.UseVisualStyleBackColor = true;
            // 
            // checkBoxKumiOrderEnable
            // 
            this.checkBoxKumiOrderEnable.AutoSize = true;
            this.checkBoxKumiOrderEnable.Location = new System.Drawing.Point(11, 0);
            this.checkBoxKumiOrderEnable.Name = "checkBoxKumiOrderEnable";
            this.checkBoxKumiOrderEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxKumiOrderEnable.TabIndex = 1;
            this.checkBoxKumiOrderEnable.UseVisualStyleBackColor = true;
            // 
            // label41
            // 
            this.label41.Anchor = 
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(8, 184);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(209, 13);
            this.label41.TabIndex = 1;
            this.label41.Text = "Исполнитель, подготовивший документ";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(12, 22);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(118, 13);
            this.label42.TabIndex = 12;
            this.label42.Text = "Номер распоряжения";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(17, 22);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(80, 13);
            this.label43.TabIndex = 14;
            this.label43.Text = "Номер ордера";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(17, 54);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(72, 13);
            this.label44.TabIndex = 16;
            this.label44.Text = "Дата ордера";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(12, 54);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(110, 13);
            this.label45.TabIndex = 18;
            this.label45.Text = "Дата распоряжения";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(14, 7);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(61, 13);
            this.label46.TabIndex = 16;
            this.label46.Text = "Тип найма";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(14, 36);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(133, 13);
            this.label47.TabIndex = 18;
            this.label47.Text = "Регистрационный номер";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(14, 65);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(100, 13);
            this.label48.TabIndex = 21;
            this.label48.Text = "Дата регистрации";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(15, 7);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(73, 13);
            this.label49.TabIndex = 23;
            this.label49.Text = "Дата выдачи";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(15, 36);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(82, 13);
            this.label50.TabIndex = 25;
            this.label50.Text = "Срок действия";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(143, 36);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(13, 13);
            this.label51.TabIndex = 27;
            this.label51.Text = "с";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(137, 65);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(19, 13);
            this.label52.TabIndex = 28;
            this.label52.Text = "по";
            // 
            // textBoxResidenceWarrantNumber
            // 
            this.textBoxResidenceWarrantNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResidenceWarrantNumber.Location = new System.Drawing.Point(164, 19);
            this.textBoxResidenceWarrantNumber.Name = "textBoxResidenceWarrantNumber";
            this.textBoxResidenceWarrantNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxResidenceWarrantNumber.TabIndex = 0;// 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(9, 16);
            this.textBoxDescription.MaxLength = 255;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(471, 165);
            this.textBoxDescription.TabIndex = 0;
            // 
            // textBoxKumiOrderNumber
            // 
            this.textBoxKumiOrderNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxKumiOrderNumber.Location = new System.Drawing.Point(159, 19);
            this.textBoxKumiOrderNumber.Name = "textBoxKumiOrderNumber";
            this.textBoxKumiOrderNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxKumiOrderNumber.TabIndex = 0;
            // 
            // textBoxRegistrationNumber
            // 
            this.textBoxRegistrationNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRegistrationNumber.Location = new System.Drawing.Point(161, 33);
            this.textBoxRegistrationNumber.Name = "textBoxRegistrationNumber";
            this.textBoxRegistrationNumber.Size = new System.Drawing.Size(319, 20);
            this.textBoxRegistrationNumber.TabIndex = 1;
            // 
            // dateTimePickerResidenceWarrantDate
            // 
            this.dateTimePickerResidenceWarrantDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerResidenceWarrantDate.Location = new System.Drawing.Point(164, 48);
            this.dateTimePickerResidenceWarrantDate.Name = "dateTimePickerResidenceWarrantDate";
            this.dateTimePickerResidenceWarrantDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerResidenceWarrantDate.TabIndex = 1;
            // 
            // dateTimePickerKumiOrderDate
            // 
            this.dateTimePickerKumiOrderDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerKumiOrderDate.Location = new System.Drawing.Point(159, 48);
            this.dateTimePickerKumiOrderDate.Name = "dateTimePickerKumiOrderDate";
            this.dateTimePickerKumiOrderDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerKumiOrderDate.TabIndex = 1;
            // 
            // dateTimePickerRegistrationDate
            // 
            this.dateTimePickerRegistrationDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRegistrationDate.Location = new System.Drawing.Point(161, 62);
            this.dateTimePickerRegistrationDate.Name = "dateTimePickerRegistrationDate";
            this.dateTimePickerRegistrationDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerRegistrationDate.TabIndex = 2;
            // 
            // dateTimePickerIssueDate
            // 
            this.dateTimePickerIssueDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIssueDate.Location = new System.Drawing.Point(162, 4);
            this.dateTimePickerIssueDate.Name = "dateTimePickerIssueDate";
            this.dateTimePickerIssueDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerIssueDate.TabIndex = 0;
            // 
            // dateTimePickerBeginDate
            // 
            this.dateTimePickerBeginDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerBeginDate.Location = new System.Drawing.Point(162, 33);
            this.dateTimePickerBeginDate.Name = "dateTimePickerBeginDate";
            this.dateTimePickerBeginDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerBeginDate.TabIndex = 1;
            // 
            // dateTimePickerEndDate
            // 
            this.dateTimePickerEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDate.Location = new System.Drawing.Point(162, 62);
            this.dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            this.dateTimePickerEndDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerEndDate.TabIndex = 3;
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(9, 200);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(471, 21);
            this.comboBoxExecutor.TabIndex = 1;
            // 
            // comboBoxRentType
            // 
            this.comboBoxRentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRentType.FormattingEnabled = true;
            this.comboBoxRentType.Location = new System.Drawing.Point(161, 4);
            this.comboBoxRentType.Name = "comboBoxRentType";
            this.comboBoxRentType.Size = new System.Drawing.Size(319, 21);
            this.comboBoxRentType.TabIndex = 0;

            this.tableLayoutPanel9.ResumeLayout(false);
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.groupBox24.ResumeLayout(false);
            this.groupBox25.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContractReasons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAgreements)).EndInit();
        }
    }
}
