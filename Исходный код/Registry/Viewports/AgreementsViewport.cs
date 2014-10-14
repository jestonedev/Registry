using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using CustomControls;

namespace Registry.Viewport
{
    internal sealed class AgreementsViewport: Viewport
    {
        #region Components  
        private TableLayoutPanel tableLayoutPanel12 = new TableLayoutPanel();
        private GroupBox groupBox29 = new GroupBox();
        private GroupBox groupBox30 = new GroupBox();
        private TabControl tabControl1 = new TabControl();
        private Panel panel7 = new System.Windows.Forms.Panel();
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_agreement = new DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_agreement_date = new DateGridViewDateTimeColumn();
        private DataGridViewTextBoxColumn field_agreement_content = new DataGridViewTextBoxColumn();
        private DataGridView dataGridViewPersons = new DataGridView();
        private DataGridViewTextBoxColumn field_surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private TextBox textBoxAgreementContent = new TextBox();
        private TextBox textBoxAgreementWarrant = new TextBox();
        private TextBox textBoxIncludePoint = new TextBox();
        private TextBox textBoxExcludePoint = new TextBox();
        private TextBox textBoxExcludeSNP = new TextBox();
        private TextBox textBoxExplainPoint = new TextBox();
        private TextBox textBoxExplainContent = new TextBox();
        private TextBox textBoxTerminatePoint = new TextBox();
        private TabPage tabPageExclude = new TabPage();
        private TabPage tabPageInclude = new TabPage();
        private TabPage tabPageExplain = new TabPage();
        private TabPage tabPageTerminate = new TabPage();
        private Label label71 = new Label();
        private Label label72 = new Label();
        private Label label73 = new Label();
        private Label label74 = new Label();
        private Label label75 = new Label();
        private Label label76 = new Label();
        private Label label77 = new Label();
        private Label label78 = new Label();
        private Label label79 = new Label();
        private Label label80 = new Label();
        private ComboBox comboBoxExecutor = new ComboBox();
        private ComboBox comboBoxIncludeKinship = new ComboBox();
        private VIBlend.WinForms.Controls.vButton vButtonSelectWarrant = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonExcludePaste = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonIncludePaste = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonExplainPaste = new VIBlend.WinForms.Controls.vButton();
        private VIBlend.WinForms.Controls.vButton vButtonTerminatePaste = new VIBlend.WinForms.Controls.vButton();
        private DateTimePicker dateTimePickerAgreementDate = new DateTimePicker();
        private DateTimePicker dateTimePickerIncludeDateOfBirth = new DateTimePicker();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        public AgreementsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageAgreements";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Соглашения найма №{0}";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            this.tableLayoutPanel12.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox29.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tabPageExclude.SuspendLayout();
            this.tabPageInclude.SuspendLayout();
            this.tabPageExplain.SuspendLayout();
            this.tabPageTerminate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).BeginInit();
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 2;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel12.Controls.Add(this.groupBox30, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.dataGridView, 0, 2);
            this.tableLayoutPanel12.Controls.Add(this.panel7, 0, 0);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 3;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.72365F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.27634F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(990, 537);
            this.tableLayoutPanel12.TabIndex = 0;
            // 
            // groupBox29
            // 
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
            this.groupBox29.Size = new System.Drawing.Size(489, 104);
            this.groupBox29.TabIndex = 0;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Общие сведения";
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.textBoxAgreementContent);
            this.groupBox30.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox30.Location = new System.Drawing.Point(498, 3);
            this.groupBox30.Name = "groupBox30";
            this.tableLayoutPanel12.SetRowSpan(this.groupBox30, 2);
            this.groupBox30.Size = new System.Drawing.Size(489, 232);
            this.groupBox30.TabIndex = 1;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Содержание";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageExclude);
            this.tabControl1.Controls.Add(this.tabPageInclude);
            this.tabControl1.Controls.Add(this.tabPageExplain);
            this.tabControl1.Controls.Add(this.tabPageTerminate);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 110);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(495, 128);
            this.tabControl1.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.groupBox29);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(3, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(489, 104);
            this.panel7.TabIndex = 7;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = 
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_agreement,
            this.field_agreement_date,
            this.field_agreement_content});
            this.tableLayoutPanel12.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Location = new System.Drawing.Point(3, 256);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(984, 278);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.ReadOnly = true;
            // 
            // field_id_agreement
            // 
            this.field_id_agreement.HeaderText = "Номер соглашения";
            this.field_id_agreement.MinimumWidth = 100;
            this.field_id_agreement.Name = "id_agreement";
            this.field_id_agreement.Visible = false;
            // 
            // field_agreement_date
            // 
            this.field_agreement_date.HeaderText = "Дата соглашения";
            this.field_agreement_date.MinimumWidth = 100;
            this.field_agreement_date.Name = "agreement_date";
            // 
            // field_agreement_content
            // 
            this.field_agreement_content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.field_agreement_content.FillWeight = 500F;
            this.field_agreement_content.HeaderText = "Содержание";
            this.field_agreement_content.MinimumWidth = 100;
            this.field_agreement_content.Name = "agreement_content";
            // 
            // dataGridViewPersons
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
            this.field_date_of_birth});
            this.dataGridViewPersons.Location = new System.Drawing.Point(3, 32);
            this.dataGridViewPersons.Name = "dataGridView18";
            this.dataGridViewPersons.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPersons.Size = new System.Drawing.Size(481, 82);
            this.dataGridViewPersons.TabIndex = 2;
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
            this.field_date_of_birth.MinimumWidth = 120;
            this.field_date_of_birth.Name = "date_of_birth";
            this.field_date_of_birth.ReadOnly = true;
            // 
            // textBoxAgreementContent
            // 
            this.textBoxAgreementContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAgreementContent.Location = new System.Drawing.Point(3, 16);
            this.textBoxAgreementContent.Multiline = true;
            this.textBoxAgreementContent.Name = "textBoxAgreementContent";
            this.textBoxAgreementContent.Size = new System.Drawing.Size(483, 228);
            this.textBoxAgreementContent.TabIndex = 1;
            // 
            // textBoxAgreementWarrant
            // 
            this.textBoxAgreementWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAgreementWarrant.Location = new System.Drawing.Point(164, 48);
            this.textBoxAgreementWarrant.Name = "textBoxAgreementWarrant";
            this.textBoxAgreementWarrant.Size = new System.Drawing.Size(286, 20);
            this.textBoxAgreementWarrant.TabIndex = 1;
            // 
            // textBoxIncludePoint
            // 
            this.textBoxIncludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIncludePoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxIncludePoint.Name = "textBoxIncludePoint";
            this.textBoxIncludePoint.Size = new System.Drawing.Size(286, 20);
            this.textBoxIncludePoint.TabIndex = 0;
            // 
            // textBoxExcludePoint
            // 
            this.textBoxExcludePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludePoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxExcludePoint.Name = "textBoxExcludePoint";
            this.textBoxExcludePoint.Size = new System.Drawing.Size(285, 20);
            this.textBoxExcludePoint.TabIndex = 0;
            // 
            // textBoxExcludeSNP
            // 
            this.textBoxExcludeSNP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcludeSNP.Location = new System.Drawing.Point(163, 32);
            this.textBoxExcludeSNP.Name = "textBoxExcludeSNP";
            this.textBoxExcludeSNP.Size = new System.Drawing.Size(285, 20);
            this.textBoxExcludeSNP.TabIndex = 1;
            // 
            // textBoxExplainPoint
            // 
            this.textBoxExplainPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainPoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxExplainPoint.Name = "textBoxExplainPoint";
            this.textBoxExplainPoint.Size = new System.Drawing.Size(285, 20);
            this.textBoxExplainPoint.TabIndex = 0;
            // 
            // textBoxExplainContent
            // 
            this.textBoxExplainContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExplainContent.Location = new System.Drawing.Point(7, 32);
            this.textBoxExplainContent.Multiline = true;
            this.textBoxExplainContent.Name = "textBoxExplainContent";
            this.textBoxExplainContent.Size = new System.Drawing.Size(475, 83);
            this.textBoxExplainContent.TabIndex = 1;
            // 
            // textBoxTerminatePoint
            // 
            this.textBoxTerminatePoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTerminatePoint.Location = new System.Drawing.Point(163, 6);
            this.textBoxTerminatePoint.Name = "textBoxTerminatePoint";
            this.textBoxTerminatePoint.Size = new System.Drawing.Size(285, 20);
            this.textBoxTerminatePoint.TabIndex = 0;
            // 
            // tabPageExclude
            // 
            this.tabPageExclude.Controls.Add(this.dataGridViewPersons);
            this.tabPageExclude.Controls.Add(this.vButtonExcludePaste);
            this.tabPageExclude.Controls.Add(this.textBoxIncludePoint);
            this.tabPageExclude.Controls.Add(this.label74);
            this.tabPageExclude.Location = new System.Drawing.Point(4, 22);
            this.tabPageExclude.Name = "tabPageExclude";
            this.tabPageExclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExclude.Size = new System.Drawing.Size(487, 117);
            this.tabPageExclude.TabIndex = 0;
            this.tabPageExclude.Text = "Исключить";
            this.tabPageExclude.UseVisualStyleBackColor = true;
            // 
            // tabPageInclude
            // 
            this.tabPageInclude.Controls.Add(this.dateTimePickerIncludeDateOfBirth);
            this.tabPageInclude.Controls.Add(this.comboBoxIncludeKinship);
            this.tabPageInclude.Controls.Add(this.label76);
            this.tabPageInclude.Controls.Add(this.label77);
            this.tabPageInclude.Controls.Add(this.textBoxExcludeSNP);
            this.tabPageInclude.Controls.Add(this.textBoxExcludePoint);
            this.tabPageInclude.Controls.Add(this.label78);
            this.tabPageInclude.Controls.Add(this.vButtonIncludePaste);
            this.tabPageInclude.Controls.Add(this.label75);
            this.tabPageInclude.Location = new System.Drawing.Point(4, 22);
            this.tabPageInclude.Name = "tabPageInclude";
            this.tabPageInclude.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInclude.Size = new System.Drawing.Size(487, 117);
            this.tabPageInclude.TabIndex = 1;
            this.tabPageInclude.Text = "Включить";
            this.tabPageInclude.UseVisualStyleBackColor = true;
            // 
            // tabPageExplain
            // 
            this.tabPageExplain.Controls.Add(this.textBoxExplainContent);
            this.tabPageExplain.Controls.Add(this.textBoxExplainPoint);
            this.tabPageExplain.Controls.Add(this.vButtonExplainPaste);
            this.tabPageExplain.Controls.Add(this.label79);
            this.tabPageExplain.Location = new System.Drawing.Point(4, 22);
            this.tabPageExplain.Name = "tabPageExplain";
            this.tabPageExplain.Size = new System.Drawing.Size(487, 117);
            this.tabPageExplain.TabIndex = 2;
            this.tabPageExplain.Text = "Изложить";
            this.tabPageExplain.UseVisualStyleBackColor = true;
            // 
            // tabPageTerminate
            // 
            this.tabPageTerminate.Controls.Add(this.vButtonTerminatePaste);
            this.tabPageTerminate.Controls.Add(this.textBoxTerminatePoint);
            this.tabPageTerminate.Controls.Add(this.label80);
            this.tabPageTerminate.Location = new System.Drawing.Point(4, 22);
            this.tabPageTerminate.Name = "tabPageTerminate";
            this.tabPageTerminate.Size = new System.Drawing.Size(487, 117);
            this.tabPageTerminate.TabIndex = 3;
            this.tabPageTerminate.Text = "Расторгнуть";
            this.tabPageTerminate.UseVisualStyleBackColor = true;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Location = new System.Drawing.Point(17, 23);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(97, 13);
            this.label71.TabIndex = 33;
            this.label71.Text = "Дата соглашения";
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(17, 51);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(95, 13);
            this.label72.TabIndex = 35;
            this.label72.Text = "По доверенности";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(17, 80);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(74, 13);
            this.label73.TabIndex = 38;
            this.label73.Text = "Исполнитель";
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(12, 9);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(55, 13);
            this.label74.TabIndex = 37;
            this.label74.Text = "Подпункт";
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(12, 9);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(55, 13);
            this.label75.TabIndex = 40;
            this.label75.Text = "Подпункт";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(12, 93);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(99, 13);
            this.label76.TabIndex = 46;
            this.label76.Text = "Отношение/связь";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(12, 65);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(86, 13);
            this.label77.TabIndex = 45;
            this.label77.Text = "Дата рождения";
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(12, 35);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(34, 13);
            this.label78.TabIndex = 43;
            this.label78.Text = "ФИО";
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(12, 9);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(55, 13);
            this.label79.TabIndex = 40;
            this.label79.Text = "Подпункт";
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(12, 9);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(55, 13);
            this.label80.TabIndex = 43;
            this.label80.Text = "Подпункт";
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(164, 77);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(319, 21);
            this.comboBoxExecutor.TabIndex = 3;
            // 
            // comboBoxIncludeKinship
            // 
            this.comboBoxIncludeKinship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxIncludeKinship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIncludeKinship.FormattingEnabled = true;
            this.comboBoxIncludeKinship.Location = new System.Drawing.Point(163, 90);
            this.comboBoxIncludeKinship.Name = "comboBoxIncludeKinship";
            this.comboBoxIncludeKinship.Size = new System.Drawing.Size(285, 21);
            this.comboBoxIncludeKinship.TabIndex = 3;
            // 
            // vButtonSelectWarrant
            // 
            this.vButtonSelectWarrant.AllowAnimations = true;
            this.vButtonSelectWarrant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonSelectWarrant.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSelectWarrant.Location = new System.Drawing.Point(456, 48);
            this.vButtonSelectWarrant.Name = "vButtonSelectWarrant";
            this.vButtonSelectWarrant.RoundedCornersMask = ((byte)(15));
            this.vButtonSelectWarrant.Size = new System.Drawing.Size(27, 20);
            this.vButtonSelectWarrant.TabIndex = 2;
            this.vButtonSelectWarrant.Text = "...";
            this.vButtonSelectWarrant.UseVisualStyleBackColor = false;
            this.vButtonSelectWarrant.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButtonExcludePaste
            // 
            this.vButtonExcludePaste.AllowAnimations = true;
            this.vButtonExcludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExcludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExcludePaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonExcludePaste.Name = "vButton2";
            this.vButtonExcludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExcludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExcludePaste.TabIndex = 1;
            this.vButtonExcludePaste.Text = "→";
            this.vButtonExcludePaste.UseVisualStyleBackColor = false;
            this.vButtonExcludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButtonIncludePaste
            // 
            this.vButtonIncludePaste.AllowAnimations = true;
            this.vButtonIncludePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonIncludePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonIncludePaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonIncludePaste.Name = "vButton3";
            this.vButtonIncludePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonIncludePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonIncludePaste.TabIndex = 4;
            this.vButtonIncludePaste.Text = "→";
            this.vButtonIncludePaste.UseVisualStyleBackColor = false;
            this.vButtonIncludePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButtonExplainPaste
            // 
            this.vButtonExplainPaste.AllowAnimations = true;
            this.vButtonExplainPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExplainPaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExplainPaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonExplainPaste.Name = "vButton4";
            this.vButtonExplainPaste.RoundedCornersMask = ((byte)(15));
            this.vButtonExplainPaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonExplainPaste.TabIndex = 2;
            this.vButtonExplainPaste.Text = "→";
            this.vButtonExplainPaste.UseVisualStyleBackColor = false;
            this.vButtonExplainPaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButtonTerminatePaste
            // 
            this.vButtonTerminatePaste.AllowAnimations = true;
            this.vButtonTerminatePaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonTerminatePaste.BackColor = System.Drawing.Color.Transparent;
            this.vButtonTerminatePaste.Location = new System.Drawing.Point(455, 6);
            this.vButtonTerminatePaste.Name = "vButton5";
            this.vButtonTerminatePaste.RoundedCornersMask = ((byte)(15));
            this.vButtonTerminatePaste.Size = new System.Drawing.Size(27, 20);
            this.vButtonTerminatePaste.TabIndex = 1;
            this.vButtonTerminatePaste.Text = "→";
            this.vButtonTerminatePaste.UseVisualStyleBackColor = false;
            this.vButtonTerminatePaste.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // dateTimePickerAgreementDate
            // 
            this.dateTimePickerAgreementDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAgreementDate.Location = new System.Drawing.Point(164, 19);
            this.dateTimePickerAgreementDate.Name = "dateTimePickerAgreementDate";
            this.dateTimePickerAgreementDate.Size = new System.Drawing.Size(319, 20);
            this.dateTimePickerAgreementDate.TabIndex = 0;
            // 
            // dateTimePickerIncludeDateOfBirth
            // 
            this.dateTimePickerIncludeDateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerIncludeDateOfBirth.Location = new System.Drawing.Point(163, 61);
            this.dateTimePickerIncludeDateOfBirth.Name = "dateTimePickerIncludeDateOfBirth";
            this.dateTimePickerIncludeDateOfBirth.Size = new System.Drawing.Size(285, 20);
            this.dateTimePickerIncludeDateOfBirth.TabIndex = 2;

            this.tableLayoutPanel12.ResumeLayout(false);
            this.groupBox30.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tabPageExclude.ResumeLayout(false);
            this.tabPageExclude.PerformLayout();
            this.tabPageInclude.ResumeLayout(false);
            this.tabPageInclude.PerformLayout();
            this.tabPageExplain.ResumeLayout(false);
            this.tabPageExplain.PerformLayout();
            this.tabPageTerminate.ResumeLayout(false);
            this.tabPageTerminate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPersons)).EndInit();
        }
    }
}
