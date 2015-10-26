using System;
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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyAgreementsViewport));
            tableLayoutPanel12 = new TableLayoutPanel();
            panel7 = new Panel();
            groupBox29 = new GroupBox();
            comboBoxExecutor = new ComboBox();
            label73 = new Label();
            vButtonSelectWarrant = new vButton();
            textBoxAgreementWarrant = new TextBox();
            label72 = new Label();
            dateTimePickerAgreementDate = new DateTimePicker();
            label71 = new Label();
            groupBox30 = new GroupBox();
            textBoxAgreementContent = new TextBox();
            tabControl1 = new TabControl();
            tabPageExclude = new TabPage();
            textBoxGeneralExcludePoint = new TextBox();
            label3 = new Label();
            dataGridViewTenancyPersons = new DataGridView();
            surname = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            patronymic = new DataGridViewTextBoxColumn();
            date_of_birth = new DataGridViewTextBoxColumn();
            vButtonExcludePaste = new vButton();
            textBoxExcludePoint = new TextBox();
            label74 = new Label();
            tabPageInclude = new TabPage();
            textBoxGeneralIncludePoint = new TextBox();
            label2 = new Label();
            dateTimePickerIncludeDateOfBirth = new DateTimePicker();
            comboBoxIncludeKinship = new ComboBox();
            label76 = new Label();
            label77 = new Label();
            textBoxIncludeSNP = new TextBox();
            textBoxIncludePoint = new TextBox();
            label78 = new Label();
            vButtonIncludePaste = new vButton();
            label75 = new Label();
            tabPageExplain = new TabPage();
            textBoxExplainContent = new TextBox();
            textBoxExplainPoint = new TextBox();
            vButtonExplainPaste = new vButton();
            label79 = new Label();
            tabPageTerminate = new TabPage();
            dateTimePickerTerminateDate = new DateTimePicker();
            label1 = new Label();
            vButtonTerminatePaste = new vButton();
            textBoxTerminateAgreement = new TextBox();
            label80 = new Label();
            dataGridView = new DataGridView();
            id_agreement = new DataGridViewTextBoxColumn();
            agreement_date = new DataGridViewDateTimeColumn();
            agreement_content = new DataGridViewTextBoxColumn();
            tableLayoutPanel12.SuspendLayout();
            panel7.SuspendLayout();
            groupBox29.SuspendLayout();
            groupBox30.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPageExclude.SuspendLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).BeginInit();
            tabPageInclude.SuspendLayout();
            tabPageExplain.SuspendLayout();
            tabPageTerminate.SuspendLayout();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 2;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel12.Controls.Add(panel7, 0, 0);
            tableLayoutPanel12.Controls.Add(groupBox30, 1, 0);
            tableLayoutPanel12.Controls.Add(tabControl1, 0, 1);
            tableLayoutPanel12.Controls.Add(dataGridView, 0, 2);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(3, 3);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 3;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Absolute, 179F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel12.Size = new Size(888, 518);
            tableLayoutPanel12.TabIndex = 0;
            // 
            // panel7
            // 
            panel7.Controls.Add(groupBox29);
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(3, 3);
            panel7.Name = "panel7";
            panel7.Size = new Size(438, 104);
            panel7.TabIndex = 0;
            // 
            // groupBox29
            // 
            groupBox29.Controls.Add(comboBoxExecutor);
            groupBox29.Controls.Add(label73);
            groupBox29.Controls.Add(vButtonSelectWarrant);
            groupBox29.Controls.Add(textBoxAgreementWarrant);
            groupBox29.Controls.Add(label72);
            groupBox29.Controls.Add(dateTimePickerAgreementDate);
            groupBox29.Controls.Add(label71);
            groupBox29.Dock = DockStyle.Fill;
            groupBox29.Location = new Point(0, 0);
            groupBox29.Name = "groupBox29";
            groupBox29.Size = new Size(438, 104);
            groupBox29.TabIndex = 0;
            groupBox29.TabStop = false;
            groupBox29.Text = "Общие сведения";
            // 
            // comboBoxExecutor
            // 
            comboBoxExecutor.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                      | AnchorStyles.Right;
            comboBoxExecutor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxExecutor.FormattingEnabled = true;
            comboBoxExecutor.Location = new Point(164, 77);
            comboBoxExecutor.Name = "comboBoxExecutor";
            comboBoxExecutor.Size = new Size(268, 23);
            comboBoxExecutor.TabIndex = 3;
            // 
            // label73
            // 
            label73.AutoSize = true;
            label73.Location = new Point(17, 80);
            label73.Name = "label73";
            label73.Size = new Size(85, 15);
            label73.TabIndex = 38;
            label73.Text = "Исполнитель";
            // 
            // vButtonSelectWarrant
            // 
            vButtonSelectWarrant.AllowAnimations = true;
            vButtonSelectWarrant.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonSelectWarrant.BackColor = Color.Transparent;
            vButtonSelectWarrant.Location = new Point(405, 48);
            vButtonSelectWarrant.Name = "vButtonSelectWarrant";
            vButtonSelectWarrant.RoundedCornersMask = 15;
            vButtonSelectWarrant.Size = new Size(27, 20);
            vButtonSelectWarrant.TabIndex = 2;
            vButtonSelectWarrant.Text = "...";
            vButtonSelectWarrant.UseVisualStyleBackColor = false;
            vButtonSelectWarrant.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonSelectWarrant.Click += vButtonSelectWarrant_Click;
            // 
            // textBoxAgreementWarrant
            // 
            textBoxAgreementWarrant.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                             | AnchorStyles.Right;
            textBoxAgreementWarrant.Location = new Point(164, 48);
            textBoxAgreementWarrant.Name = "textBoxAgreementWarrant";
            textBoxAgreementWarrant.ReadOnly = true;
            textBoxAgreementWarrant.Size = new Size(235, 21);
            textBoxAgreementWarrant.TabIndex = 1;
            // 
            // label72
            // 
            label72.AutoSize = true;
            label72.Location = new Point(17, 51);
            label72.Name = "label72";
            label72.Size = new Size(109, 15);
            label72.TabIndex = 35;
            label72.Text = "По доверенности";
            // 
            // dateTimePickerAgreementDate
            // 
            dateTimePickerAgreementDate.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                 | AnchorStyles.Right;
            dateTimePickerAgreementDate.Location = new Point(164, 19);
            dateTimePickerAgreementDate.Name = "dateTimePickerAgreementDate";
            dateTimePickerAgreementDate.Size = new Size(268, 21);
            dateTimePickerAgreementDate.TabIndex = 0;
            // 
            // label71
            // 
            label71.AutoSize = true;
            label71.Location = new Point(17, 23);
            label71.Name = "label71";
            label71.Size = new Size(109, 15);
            label71.TabIndex = 33;
            label71.Text = "Дата соглашения";
            // 
            // groupBox30
            // 
            groupBox30.Controls.Add(textBoxAgreementContent);
            groupBox30.Dock = DockStyle.Fill;
            groupBox30.Location = new Point(447, 3);
            groupBox30.Name = "groupBox30";
            tableLayoutPanel12.SetRowSpan(groupBox30, 2);
            groupBox30.Size = new Size(438, 283);
            groupBox30.TabIndex = 1;
            groupBox30.TabStop = false;
            groupBox30.Text = "Содержание";
            // 
            // textBoxAgreementContent
            // 
            textBoxAgreementContent.Dock = DockStyle.Fill;
            textBoxAgreementContent.Location = new Point(3, 17);
            textBoxAgreementContent.MaxLength = 4000;
            textBoxAgreementContent.Multiline = true;
            textBoxAgreementContent.Name = "textBoxAgreementContent";
            textBoxAgreementContent.Size = new Size(432, 263);
            textBoxAgreementContent.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageExclude);
            tabControl1.Controls.Add(tabPageInclude);
            tabControl1.Controls.Add(tabPageExplain);
            tabControl1.Controls.Add(tabPageTerminate);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 110);
            tabControl1.Margin = new Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(0, 0);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(444, 179);
            tabControl1.TabIndex = 0;
            // 
            // tabPageExclude
            // 
            tabPageExclude.BackColor = Color.White;
            tabPageExclude.Controls.Add(textBoxGeneralExcludePoint);
            tabPageExclude.Controls.Add(label3);
            tabPageExclude.Controls.Add(dataGridViewTenancyPersons);
            tabPageExclude.Controls.Add(vButtonExcludePaste);
            tabPageExclude.Controls.Add(textBoxExcludePoint);
            tabPageExclude.Controls.Add(label74);
            tabPageExclude.Location = new Point(4, 24);
            tabPageExclude.Name = "tabPageExclude";
            tabPageExclude.Padding = new Padding(3);
            tabPageExclude.Size = new Size(436, 151);
            tabPageExclude.TabIndex = 0;
            tabPageExclude.Text = "Исключить";
            // 
            // textBoxGeneralExcludePoint
            // 
            textBoxGeneralExcludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                | AnchorStyles.Right;
            textBoxGeneralExcludePoint.Location = new Point(164, 6);
            textBoxGeneralExcludePoint.Name = "textBoxGeneralExcludePoint";
            textBoxGeneralExcludePoint.Size = new Size(234, 21);
            textBoxGeneralExcludePoint.TabIndex = 0;
            textBoxGeneralExcludePoint.Enter += selectAll_Enter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 9);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 50;
            label3.Text = "Пункт";
            // 
            // dataGridViewTenancyPersons
            // 
            dataGridViewTenancyPersons.AllowUserToAddRows = false;
            dataGridViewTenancyPersons.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
                                                 | AnchorStyles.Left)
                                                | AnchorStyles.Right;
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
            dataGridViewTenancyPersons.Columns.AddRange(surname, name, patronymic, date_of_birth);
            dataGridViewTenancyPersons.Location = new Point(3, 61);
            dataGridViewTenancyPersons.MultiSelect = false;
            dataGridViewTenancyPersons.Name = "dataGridViewTenancyPersons";
            dataGridViewTenancyPersons.ReadOnly = true;
            dataGridViewTenancyPersons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTenancyPersons.Size = new Size(433, 83);
            dataGridViewTenancyPersons.TabIndex = 3;
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
            date_of_birth.MinimumWidth = 140;
            date_of_birth.Name = "date_of_birth";
            date_of_birth.ReadOnly = true;
            // 
            // vButtonExcludePaste
            // 
            vButtonExcludePaste.AllowAnimations = true;
            vButtonExcludePaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonExcludePaste.BackColor = Color.Transparent;
            vButtonExcludePaste.Location = new Point(404, 6);
            vButtonExcludePaste.Name = "vButtonExcludePaste";
            vButtonExcludePaste.RoundedCornersMask = 15;
            vButtonExcludePaste.Size = new Size(27, 20);
            vButtonExcludePaste.TabIndex = 2;
            vButtonExcludePaste.Text = "→";
            vButtonExcludePaste.UseVisualStyleBackColor = false;
            vButtonExcludePaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonExcludePaste.Click += vButtonExcludePaste_Click;
            // 
            // textBoxExcludePoint
            // 
            textBoxExcludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                         | AnchorStyles.Right;
            textBoxExcludePoint.Location = new Point(164, 34);
            textBoxExcludePoint.Name = "textBoxExcludePoint";
            textBoxExcludePoint.Size = new Size(235, 21);
            textBoxExcludePoint.TabIndex = 1;
            textBoxExcludePoint.Enter += selectAll_Enter;
            // 
            // label74
            // 
            label74.AutoSize = true;
            label74.Location = new Point(13, 37);
            label74.Name = "label74";
            label74.Size = new Size(62, 15);
            label74.TabIndex = 37;
            label74.Text = "Подпункт";
            // 
            // tabPageInclude
            // 
            tabPageInclude.BackColor = Color.White;
            tabPageInclude.Controls.Add(textBoxGeneralIncludePoint);
            tabPageInclude.Controls.Add(label2);
            tabPageInclude.Controls.Add(dateTimePickerIncludeDateOfBirth);
            tabPageInclude.Controls.Add(comboBoxIncludeKinship);
            tabPageInclude.Controls.Add(label76);
            tabPageInclude.Controls.Add(label77);
            tabPageInclude.Controls.Add(textBoxIncludeSNP);
            tabPageInclude.Controls.Add(textBoxIncludePoint);
            tabPageInclude.Controls.Add(label78);
            tabPageInclude.Controls.Add(vButtonIncludePaste);
            tabPageInclude.Controls.Add(label75);
            tabPageInclude.Location = new Point(4, 24);
            tabPageInclude.Name = "tabPageInclude";
            tabPageInclude.Padding = new Padding(3);
            tabPageInclude.Size = new Size(436, 151);
            tabPageInclude.TabIndex = 1;
            tabPageInclude.Text = "Включить";
            // 
            // textBoxGeneralIncludePoint
            // 
            textBoxGeneralIncludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                | AnchorStyles.Right;
            textBoxGeneralIncludePoint.Location = new Point(164, 6);
            textBoxGeneralIncludePoint.Name = "textBoxGeneralIncludePoint";
            textBoxGeneralIncludePoint.Size = new Size(234, 21);
            textBoxGeneralIncludePoint.TabIndex = 0;
            textBoxGeneralIncludePoint.Enter += selectAll_Enter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 9);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 48;
            label2.Text = "Пункт";
            // 
            // dateTimePickerIncludeDateOfBirth
            // 
            dateTimePickerIncludeDateOfBirth.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                                      | AnchorStyles.Right;
            dateTimePickerIncludeDateOfBirth.Location = new Point(164, 90);
            dateTimePickerIncludeDateOfBirth.Name = "dateTimePickerIncludeDateOfBirth";
            dateTimePickerIncludeDateOfBirth.Size = new Size(234, 21);
            dateTimePickerIncludeDateOfBirth.TabIndex = 3;
            // 
            // comboBoxIncludeKinship
            // 
            comboBoxIncludeKinship.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                            | AnchorStyles.Right;
            comboBoxIncludeKinship.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxIncludeKinship.FormattingEnabled = true;
            comboBoxIncludeKinship.Location = new Point(164, 118);
            comboBoxIncludeKinship.Name = "comboBoxIncludeKinship";
            comboBoxIncludeKinship.Size = new Size(234, 23);
            comboBoxIncludeKinship.TabIndex = 4;
            // 
            // label76
            // 
            label76.AutoSize = true;
            label76.Location = new Point(13, 122);
            label76.Name = "label76";
            label76.Size = new Size(110, 15);
            label76.TabIndex = 46;
            label76.Text = "Отношение/связь";
            // 
            // label77
            // 
            label77.AutoSize = true;
            label77.Location = new Point(13, 93);
            label77.Name = "label77";
            label77.Size = new Size(98, 15);
            label77.TabIndex = 45;
            label77.Text = "Дата рождения";
            // 
            // textBoxIncludeSNP
            // 
            textBoxIncludeSNP.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                       | AnchorStyles.Right;
            textBoxIncludeSNP.Location = new Point(164, 62);
            textBoxIncludeSNP.Name = "textBoxIncludeSNP";
            textBoxIncludeSNP.Size = new Size(234, 21);
            textBoxIncludeSNP.TabIndex = 2;
            textBoxIncludeSNP.Enter += selectAll_Enter;
            // 
            // textBoxIncludePoint
            // 
            textBoxIncludePoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                         | AnchorStyles.Right;
            textBoxIncludePoint.Location = new Point(164, 34);
            textBoxIncludePoint.Name = "textBoxIncludePoint";
            textBoxIncludePoint.Size = new Size(234, 21);
            textBoxIncludePoint.TabIndex = 1;
            textBoxIncludePoint.Enter += selectAll_Enter;
            // 
            // label78
            // 
            label78.AutoSize = true;
            label78.Location = new Point(13, 65);
            label78.Name = "label78";
            label78.Size = new Size(36, 15);
            label78.TabIndex = 43;
            label78.Text = "ФИО";
            // 
            // vButtonIncludePaste
            // 
            vButtonIncludePaste.AllowAnimations = true;
            vButtonIncludePaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonIncludePaste.BackColor = Color.Transparent;
            vButtonIncludePaste.Location = new Point(404, 6);
            vButtonIncludePaste.Name = "vButtonIncludePaste";
            vButtonIncludePaste.RoundedCornersMask = 15;
            vButtonIncludePaste.Size = new Size(27, 20);
            vButtonIncludePaste.TabIndex = 4;
            vButtonIncludePaste.Text = "→";
            vButtonIncludePaste.UseVisualStyleBackColor = false;
            vButtonIncludePaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonIncludePaste.Click += vButtonIncludePaste_Click;
            // 
            // label75
            // 
            label75.AutoSize = true;
            label75.Location = new Point(13, 37);
            label75.Name = "label75";
            label75.Size = new Size(62, 15);
            label75.TabIndex = 40;
            label75.Text = "Подпункт";
            // 
            // tabPageExplain
            // 
            tabPageExplain.BackColor = Color.White;
            tabPageExplain.Controls.Add(textBoxExplainContent);
            tabPageExplain.Controls.Add(textBoxExplainPoint);
            tabPageExplain.Controls.Add(vButtonExplainPaste);
            tabPageExplain.Controls.Add(label79);
            tabPageExplain.Location = new Point(4, 24);
            tabPageExplain.Name = "tabPageExplain";
            tabPageExplain.Size = new Size(436, 151);
            tabPageExplain.TabIndex = 2;
            tabPageExplain.Text = "Изложить";
            // 
            // textBoxExplainContent
            // 
            textBoxExplainContent.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
                                            | AnchorStyles.Left)
                                           | AnchorStyles.Right;
            textBoxExplainContent.Location = new Point(7, 32);
            textBoxExplainContent.Multiline = true;
            textBoxExplainContent.Name = "textBoxExplainContent";
            textBoxExplainContent.Size = new Size(424, 103);
            textBoxExplainContent.TabIndex = 1;
            // 
            // textBoxExplainPoint
            // 
            textBoxExplainPoint.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                         | AnchorStyles.Right;
            textBoxExplainPoint.Location = new Point(163, 6);
            textBoxExplainPoint.Name = "textBoxExplainPoint";
            textBoxExplainPoint.Size = new Size(234, 21);
            textBoxExplainPoint.TabIndex = 0;
            textBoxExplainPoint.Enter += selectAll_Enter;
            // 
            // vButtonExplainPaste
            // 
            vButtonExplainPaste.AllowAnimations = true;
            vButtonExplainPaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonExplainPaste.BackColor = Color.Transparent;
            vButtonExplainPaste.Location = new Point(404, 6);
            vButtonExplainPaste.Name = "vButtonExplainPaste";
            vButtonExplainPaste.RoundedCornersMask = 15;
            vButtonExplainPaste.Size = new Size(27, 20);
            vButtonExplainPaste.TabIndex = 2;
            vButtonExplainPaste.Text = "→";
            vButtonExplainPaste.UseVisualStyleBackColor = false;
            vButtonExplainPaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonExplainPaste.Click += vButtonExplainPaste_Click;
            // 
            // label79
            // 
            label79.AutoSize = true;
            label79.Location = new Point(12, 9);
            label79.Name = "label79";
            label79.Size = new Size(62, 15);
            label79.TabIndex = 40;
            label79.Text = "Подпункт";
            // 
            // tabPageTerminate
            // 
            tabPageTerminate.BackColor = Color.White;
            tabPageTerminate.Controls.Add(dateTimePickerTerminateDate);
            tabPageTerminate.Controls.Add(label1);
            tabPageTerminate.Controls.Add(vButtonTerminatePaste);
            tabPageTerminate.Controls.Add(textBoxTerminateAgreement);
            tabPageTerminate.Controls.Add(label80);
            tabPageTerminate.Location = new Point(4, 24);
            tabPageTerminate.Name = "tabPageTerminate";
            tabPageTerminate.Size = new Size(436, 151);
            tabPageTerminate.TabIndex = 3;
            tabPageTerminate.Text = "Расторгнуть";
            // 
            // dateTimePickerTerminateDate
            // 
            dateTimePickerTerminateDate.Location = new Point(163, 34);
            dateTimePickerTerminateDate.Name = "dateTimePickerTerminateDate";
            dateTimePickerTerminateDate.Size = new Size(234, 21);
            dateTimePickerTerminateDate.TabIndex = 45;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 38);
            label1.Name = "label1";
            label1.Size = new Size(118, 15);
            label1.TabIndex = 44;
            label1.Text = "Дата расторжения";
            // 
            // vButtonTerminatePaste
            // 
            vButtonTerminatePaste.AllowAnimations = true;
            vButtonTerminatePaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            vButtonTerminatePaste.BackColor = Color.Transparent;
            vButtonTerminatePaste.Location = new Point(404, 6);
            vButtonTerminatePaste.Name = "vButtonTerminatePaste";
            vButtonTerminatePaste.RoundedCornersMask = 15;
            vButtonTerminatePaste.Size = new Size(27, 20);
            vButtonTerminatePaste.TabIndex = 1;
            vButtonTerminatePaste.Text = "→";
            vButtonTerminatePaste.UseVisualStyleBackColor = false;
            vButtonTerminatePaste.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            vButtonTerminatePaste.Click += vButtonTerminatePaste_Click;
            // 
            // textBoxTerminateAgreement
            // 
            textBoxTerminateAgreement.Anchor = (AnchorStyles.Top | AnchorStyles.Left)
                                               | AnchorStyles.Right;
            textBoxTerminateAgreement.Location = new Point(163, 6);
            textBoxTerminateAgreement.Name = "textBoxTerminateAgreement";
            textBoxTerminateAgreement.Size = new Size(234, 21);
            textBoxTerminateAgreement.TabIndex = 0;
            textBoxTerminateAgreement.Enter += selectAll_Enter;
            // 
            // label80
            // 
            label80.AutoSize = true;
            label80.Location = new Point(12, 10);
            label80.Name = "label80";
            label80.Size = new Size(110, 15);
            label80.TabIndex = 43;
            label80.Text = "По какой причине";
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
                                   | AnchorStyles.Left)
                                  | AnchorStyles.Right;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_agreement, agreement_date, agreement_content);
            tableLayoutPanel12.SetColumnSpan(dataGridView, 2);
            dataGridView.Location = new Point(3, 292);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(882, 223);
            dataGridView.TabIndex = 2;
            dataGridView.DataError += dataGridView_DataError;
            // 
            // id_agreement
            // 
            id_agreement.HeaderText = "Номер соглашения";
            id_agreement.MinimumWidth = 100;
            id_agreement.Name = "id_agreement";
            id_agreement.ReadOnly = true;
            id_agreement.Visible = false;
            // 
            // agreement_date
            // 
            agreement_date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            agreement_date.HeaderText = "Дата соглашения";
            agreement_date.MinimumWidth = 150;
            agreement_date.Name = "agreement_date";
            agreement_date.ReadOnly = true;
            agreement_date.Width = 150;
            // 
            // agreement_content
            // 
            agreement_content.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            agreement_content.FillWeight = 500F;
            agreement_content.HeaderText = "Содержание";
            agreement_content.MinimumWidth = 100;
            agreement_content.Name = "agreement_content";
            agreement_content.ReadOnly = true;
            // 
            // TenancyAgreementsViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new Size(660, 360);
            BackColor = Color.White;
            ClientSize = new Size(894, 524);
            Controls.Add(tableLayoutPanel12);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyAgreementsViewport";
            Padding = new Padding(3);
            Text = "Соглашения найма №{0}";
            tableLayoutPanel12.ResumeLayout(false);
            panel7.ResumeLayout(false);
            groupBox29.ResumeLayout(false);
            groupBox29.PerformLayout();
            groupBox30.ResumeLayout(false);
            groupBox30.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPageExclude.ResumeLayout(false);
            tabPageExclude.PerformLayout();
            ((ISupportInitialize)(dataGridViewTenancyPersons)).EndInit();
            tabPageInclude.ResumeLayout(false);
            tabPageInclude.PerformLayout();
            tabPageExplain.ResumeLayout(false);
            tabPageExplain.PerformLayout();
            tabPageTerminate.ResumeLayout(false);
            tabPageTerminate.PerformLayout();
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
