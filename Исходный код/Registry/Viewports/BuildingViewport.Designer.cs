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
    internal partial class BuildingViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel;
        private TableLayoutPanel tableLayoutPanel2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private NumericUpDown numericUpDownFloors;
        private NumericUpDown numericUpDownBalanceCost;
        private NumericUpDown numericUpDownCadastralCost;
        private NumericUpDown numericUpDownStartupYear;
        private NumericUpDown numericUpDownLivingArea;
        private NumericUpDown numericUpDownTotalArea;
        private NumericUpDown numericUpDownMunicipalArea;
        private NumericUpDown numericUpDownPremisesCount;
        private NumericUpDown numericUpDownRoomsCount;
        private NumericUpDown numericUpDownApartmentsCount;
        private NumericUpDown numericUpDownSharedApartmentsCount;
        private NumericUpDown numericUpDownCommercialPremisesCount;
        private NumericUpDown numericUpDownSpecialPremisesCount;
        private NumericUpDown numericUpDownSocialPremisesCount;
        private NumericUpDown numericUpDownOtherPremisesCount;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label20;
        private Label label40;
        private Panel panel1;
        private Panel panel2;
        private TextBox textBoxHouse;
        private ComboBox comboBoxCurrentFundType;
        private ComboBox comboBoxState;
        private TextBox textBoxCadastralNum;
        private DataGridView dataGridViewRestrictions;
        private DataGridView dataGridViewOwnerships;
        private ComboBox comboBoxStreet;
        private ComboBox comboBoxStructureType;
        private CheckBox checkBoxImprovement;
        private CheckBox checkBoxElevator;
        private DataGridViewTextBoxColumn restriction_number;
        private DataGridViewTextBoxColumn restriction_date;
        private DataGridViewTextBoxColumn restriction_description;
        private DataGridViewComboBoxColumn id_restriction_type;
        private DataGridViewTextBoxColumn ownership_number;
        private DataGridViewTextBoxColumn ownership_date;
        private DataGridViewTextBoxColumn ownership_description;
        private DataGridViewComboBoxColumn id_ownership_type;
        private NumericUpDown numericUpDownWear;
        private Label label21;
        private Panel panel3;
        private VIBlend.WinForms.Controls.vButton vButtonRestrictionEdit;
        private VIBlend.WinForms.Controls.vButton vButtonRestrictionDelete;
        private VIBlend.WinForms.Controls.vButton vButtonRestrictionAdd;
        private Panel panel4;
        private VIBlend.WinForms.Controls.vButton vButtonOwnershipEdit;
        private VIBlend.WinForms.Controls.vButton vButtonOwnershipDelete;
        private VIBlend.WinForms.Controls.vButton vButtonOwnershipAdd;
        private DateTimePicker dateTimePickerStateDate;
        private Label label22;
        private CheckBox checkBoxRubbishChute;
        #endregion Components


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildingViewport));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numericUpDownWear = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.numericUpDownFloors = new System.Windows.Forms.NumericUpDown();
            this.dateTimePickerStateDate = new System.Windows.Forms.DateTimePicker();
            this.label22 = new System.Windows.Forms.Label();
            this.numericUpDownStartupYear = new System.Windows.Forms.NumericUpDown();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.comboBoxStructureType = new System.Windows.Forms.ComboBox();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.comboBoxCurrentFundType = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBoxHeatingType = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.checkBoxRadioNetwork = new System.Windows.Forms.CheckBox();
            this.checkBoxHotWaterSupply = new System.Windows.Forms.CheckBox();
            this.checkBoxElectricity = new System.Windows.Forms.CheckBox();
            this.checkBoxCanalization = new System.Windows.Forms.CheckBox();
            this.checkBoxPlumbing = new System.Windows.Forms.CheckBox();
            this.checkBoxRubbishChute = new System.Windows.Forms.CheckBox();
            this.label40 = new System.Windows.Forms.Label();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.numericUpDownBalanceCost = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCadastralCost = new System.Windows.Forms.NumericUpDown();
            this.checkBoxImprovement = new System.Windows.Forms.CheckBox();
            this.checkBoxElevator = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownMunPremisesPercentage = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.numericUpDownMunPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.numericUpDownPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRoomsCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownApartmentsCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSharedApartmentsCount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownOtherPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSpecialPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCommercialPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSocialPremisesCount = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownTotalArea = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.numericUpDownLivingArea = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMunicipalArea = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.vButtonRestrictionEdit = new VIBlend.WinForms.Controls.vButton();
            this.vButtonRestrictionDelete = new VIBlend.WinForms.Controls.vButton();
            this.vButtonRestrictionAdd = new VIBlend.WinForms.Controls.vButton();
            this.dataGridViewRestrictions = new System.Windows.Forms.DataGridView();
            this.restriction_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_restriction_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.vButtonOwnershipEdit = new VIBlend.WinForms.Controls.vButton();
            this.vButtonOwnershipDelete = new VIBlend.WinForms.Controls.vButton();
            this.vButtonOwnershipAdd = new VIBlend.WinForms.Controls.vButton();
            this.dataGridViewOwnerships = new System.Windows.Forms.DataGridView();
            this.ownership_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_ownership_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxRoomsBTI = new System.Windows.Forms.TextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunPremisesPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoomsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownApartmentsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharedApartmentsCount)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOtherPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpecialPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCommercialPremisesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSocialPremisesCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBox3, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.groupBox6, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.groupBox7, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 1, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 113F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1256, 971);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.tableLayoutPanel.SetColumnSpan(this.groupBox4, 2);
            this.groupBox4.Controls.Add(this.tableLayoutPanel2);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1250, 254);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Общие сведения";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoScroll = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1244, 234);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.numericUpDownWear);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.numericUpDownFloors);
            this.panel1.Controls.Add(this.dateTimePickerStateDate);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.numericUpDownStartupYear);
            this.panel1.Controls.Add(this.comboBoxStreet);
            this.panel1.Controls.Add(this.comboBoxStructureType);
            this.panel1.Controls.Add(this.textBoxHouse);
            this.panel1.Controls.Add(this.comboBoxCurrentFundType);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(616, 228);
            this.panel1.TabIndex = 0;
            // 
            // numericUpDownWear
            // 
            this.numericUpDownWear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownWear.DecimalPlaces = 2;
            this.numericUpDownWear.Location = new System.Drawing.Point(174, 151);
            this.numericUpDownWear.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownWear.Name = "numericUpDownWear";
            this.numericUpDownWear.Size = new System.Drawing.Size(438, 21);
            this.numericUpDownWear.TabIndex = 5;
            this.numericUpDownWear.ThousandsSeparator = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 154);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(59, 15);
            this.label21.TabIndex = 36;
            this.label21.Text = "Износ, %";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Улица";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Номер дома";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Этажность дома";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 96);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(160, 15);
            this.label17.TabIndex = 3;
            this.label17.Text = "Год ввода в эксплуатацию";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 125);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(155, 15);
            this.label18.TabIndex = 4;
            this.label18.Text = "Тип строения (материал)";
            // 
            // numericUpDownFloors
            // 
            this.numericUpDownFloors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownFloors.Location = new System.Drawing.Point(175, 65);
            this.numericUpDownFloors.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownFloors.Name = "numericUpDownFloors";
            this.numericUpDownFloors.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownFloors.TabIndex = 2;
            // 
            // dateTimePickerStateDate
            // 
            this.dateTimePickerStateDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStateDate.Location = new System.Drawing.Point(175, 229);
            this.dateTimePickerStateDate.Name = "dateTimePickerStateDate";
            this.dateTimePickerStateDate.ShowCheckBox = true;
            this.dateTimePickerStateDate.Size = new System.Drawing.Size(437, 21);
            this.dateTimePickerStateDate.TabIndex = 7;
            this.dateTimePickerStateDate.Visible = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(10, 222);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(147, 15);
            this.label22.TabIndex = 37;
            this.label22.Text = "Состояние установлено";
            this.label22.Visible = false;
            // 
            // numericUpDownStartupYear
            // 
            this.numericUpDownStartupYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownStartupYear.Location = new System.Drawing.Point(175, 93);
            this.numericUpDownStartupYear.Maximum = new decimal(new int[] {
            2014,
            0,
            0,
            0});
            this.numericUpDownStartupYear.Minimum = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.numericUpDownStartupYear.Name = "numericUpDownStartupYear";
            this.numericUpDownStartupYear.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownStartupYear.TabIndex = 3;
            this.numericUpDownStartupYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(175, 7);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStreet.TabIndex = 0;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // comboBoxStructureType
            // 
            this.comboBoxStructureType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStructureType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStructureType.FormattingEnabled = true;
            this.comboBoxStructureType.Location = new System.Drawing.Point(175, 121);
            this.comboBoxStructureType.Name = "comboBoxStructureType";
            this.comboBoxStructureType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStructureType.TabIndex = 4;
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHouse.Location = new System.Drawing.Point(175, 37);
            this.textBoxHouse.MaxLength = 20;
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(437, 21);
            this.textBoxHouse.TabIndex = 1;
            this.textBoxHouse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxHouse_KeyPress);
            // 
            // comboBoxCurrentFundType
            // 
            this.comboBoxCurrentFundType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCurrentFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurrentFundType.Enabled = false;
            this.comboBoxCurrentFundType.ForeColor = System.Drawing.Color.Black;
            this.comboBoxCurrentFundType.FormattingEnabled = true;
            this.comboBoxCurrentFundType.Location = new System.Drawing.Point(174, 178);
            this.comboBoxCurrentFundType.Name = "comboBoxCurrentFundType";
            this.comboBoxCurrentFundType.Size = new System.Drawing.Size(438, 23);
            this.comboBoxCurrentFundType.TabIndex = 6;
            this.comboBoxCurrentFundType.Visible = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 180);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(90, 15);
            this.label19.TabIndex = 35;
            this.label19.Text = "Текущий фонд";
            this.label19.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.comboBoxHeatingType);
            this.panel2.Controls.Add(this.label25);
            this.panel2.Controls.Add(this.checkBoxRadioNetwork);
            this.panel2.Controls.Add(this.checkBoxHotWaterSupply);
            this.panel2.Controls.Add(this.checkBoxElectricity);
            this.panel2.Controls.Add(this.checkBoxCanalization);
            this.panel2.Controls.Add(this.checkBoxPlumbing);
            this.panel2.Controls.Add(this.checkBoxRubbishChute);
            this.panel2.Controls.Add(this.label40);
            this.panel2.Controls.Add(this.comboBoxState);
            this.panel2.Controls.Add(this.numericUpDownBalanceCost);
            this.panel2.Controls.Add(this.numericUpDownCadastralCost);
            this.panel2.Controls.Add(this.checkBoxImprovement);
            this.panel2.Controls.Add(this.checkBoxElevator);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.textBoxCadastralNum);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(625, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(616, 228);
            this.panel2.TabIndex = 1;
            // 
            // comboBoxHeatingType
            // 
            this.comboBoxHeatingType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxHeatingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHeatingType.FormattingEnabled = true;
            this.comboBoxHeatingType.Location = new System.Drawing.Point(175, 120);
            this.comboBoxHeatingType.Name = "comboBoxHeatingType";
            this.comboBoxHeatingType.Size = new System.Drawing.Size(433, 23);
            this.comboBoxHeatingType.TabIndex = 4;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(16, 125);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(72, 15);
            this.label25.TabIndex = 1;
            this.label25.Text = "Отопление";
            // 
            // checkBoxRadioNetwork
            // 
            this.checkBoxRadioNetwork.AutoSize = true;
            this.checkBoxRadioNetwork.Location = new System.Drawing.Point(176, 204);
            this.checkBoxRadioNetwork.Name = "checkBoxRadioNetwork";
            this.checkBoxRadioNetwork.Size = new System.Drawing.Size(89, 19);
            this.checkBoxRadioNetwork.TabIndex = 12;
            this.checkBoxRadioNetwork.Text = "Радиосеть";
            this.checkBoxRadioNetwork.UseVisualStyleBackColor = true;
            // 
            // checkBoxHotWaterSupply
            // 
            this.checkBoxHotWaterSupply.AutoSize = true;
            this.checkBoxHotWaterSupply.Location = new System.Drawing.Point(19, 179);
            this.checkBoxHotWaterSupply.Name = "checkBoxHotWaterSupply";
            this.checkBoxHotWaterSupply.Size = new System.Drawing.Size(124, 19);
            this.checkBoxHotWaterSupply.TabIndex = 8;
            this.checkBoxHotWaterSupply.Text = "Горяч. водоснаб.";
            this.checkBoxHotWaterSupply.UseVisualStyleBackColor = true;
            // 
            // checkBoxElectricity
            // 
            this.checkBoxElectricity.AutoSize = true;
            this.checkBoxElectricity.Location = new System.Drawing.Point(333, 179);
            this.checkBoxElectricity.Name = "checkBoxElectricity";
            this.checkBoxElectricity.Size = new System.Drawing.Size(115, 19);
            this.checkBoxElectricity.TabIndex = 10;
            this.checkBoxElectricity.Text = "Электричество";
            this.checkBoxElectricity.UseVisualStyleBackColor = true;
            // 
            // checkBoxCanalization
            // 
            this.checkBoxCanalization.AutoSize = true;
            this.checkBoxCanalization.Location = new System.Drawing.Point(176, 178);
            this.checkBoxCanalization.Name = "checkBoxCanalization";
            this.checkBoxCanalization.Size = new System.Drawing.Size(103, 19);
            this.checkBoxCanalization.TabIndex = 9;
            this.checkBoxCanalization.Text = "Канализация";
            this.checkBoxCanalization.UseVisualStyleBackColor = true;
            // 
            // checkBoxPlumbing
            // 
            this.checkBoxPlumbing.AutoSize = true;
            this.checkBoxPlumbing.Location = new System.Drawing.Point(19, 204);
            this.checkBoxPlumbing.Name = "checkBoxPlumbing";
            this.checkBoxPlumbing.Size = new System.Drawing.Size(97, 19);
            this.checkBoxPlumbing.TabIndex = 11;
            this.checkBoxPlumbing.Text = "Водопровод";
            this.checkBoxPlumbing.UseVisualStyleBackColor = true;
            // 
            // checkBoxRubbishChute
            // 
            this.checkBoxRubbishChute.AutoSize = true;
            this.checkBoxRubbishChute.Location = new System.Drawing.Point(333, 154);
            this.checkBoxRubbishChute.Name = "checkBoxRubbishChute";
            this.checkBoxRubbishChute.Size = new System.Drawing.Size(111, 19);
            this.checkBoxRubbishChute.TabIndex = 7;
            this.checkBoxRubbishChute.Text = "Мусоропровод";
            this.checkBoxRubbishChute.UseVisualStyleBackColor = true;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(16, 95);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(119, 15);
            this.label40.TabIndex = 31;
            this.label40.Text = "Текущее состояние";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.FormattingEnabled = true;
            this.comboBoxState.Location = new System.Drawing.Point(175, 91);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(433, 23);
            this.comboBoxState.TabIndex = 3;
            // 
            // numericUpDownBalanceCost
            // 
            this.numericUpDownBalanceCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceCost.DecimalPlaces = 2;
            this.numericUpDownBalanceCost.Location = new System.Drawing.Point(175, 63);
            this.numericUpDownBalanceCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            this.numericUpDownBalanceCost.Size = new System.Drawing.Size(433, 21);
            this.numericUpDownBalanceCost.TabIndex = 2;
            this.numericUpDownBalanceCost.ThousandsSeparator = true;
            // 
            // numericUpDownCadastralCost
            // 
            this.numericUpDownCadastralCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCadastralCost.DecimalPlaces = 2;
            this.numericUpDownCadastralCost.Location = new System.Drawing.Point(175, 35);
            this.numericUpDownCadastralCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            this.numericUpDownCadastralCost.Size = new System.Drawing.Size(433, 21);
            this.numericUpDownCadastralCost.TabIndex = 1;
            this.numericUpDownCadastralCost.ThousandsSeparator = true;
            // 
            // checkBoxImprovement
            // 
            this.checkBoxImprovement.AutoSize = true;
            this.checkBoxImprovement.Location = new System.Drawing.Point(176, 154);
            this.checkBoxImprovement.Name = "checkBoxImprovement";
            this.checkBoxImprovement.Size = new System.Drawing.Size(126, 19);
            this.checkBoxImprovement.TabIndex = 6;
            this.checkBoxImprovement.Text = "Благоустройство";
            this.checkBoxImprovement.UseVisualStyleBackColor = true;
            // 
            // checkBoxElevator
            // 
            this.checkBoxElevator.AutoSize = true;
            this.checkBoxElevator.Location = new System.Drawing.Point(19, 154);
            this.checkBoxElevator.Name = "checkBoxElevator";
            this.checkBoxElevator.Size = new System.Drawing.Size(118, 19);
            this.checkBoxElevator.TabIndex = 5;
            this.checkBoxElevator.Text = "Наличие лифта";
            this.checkBoxElevator.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 11);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(126, 15);
            this.label14.TabIndex = 32;
            this.label14.Text = "Кадастровый номер";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 39);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(150, 15);
            this.label15.TabIndex = 33;
            this.label15.Text = "Кадастровая стоимость";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 67);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(143, 15);
            this.label16.TabIndex = 34;
            this.label16.Text = "Балансовая стоимость";
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCadastralNum.Location = new System.Drawing.Point(175, 7);
            this.textBoxCadastralNum.MaxLength = 20;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(433, 21);
            this.textBoxCadastralNum.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownMunPremisesPercentage);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.numericUpDownMunPremisesCount);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.numericUpDownPremisesCount);
            this.groupBox1.Controls.Add(this.numericUpDownRoomsCount);
            this.groupBox1.Controls.Add(this.numericUpDownApartmentsCount);
            this.groupBox1.Controls.Add(this.numericUpDownSharedApartmentsCount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 263);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(622, 164);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Количество жилых помещений";
            // 
            // numericUpDownMunPremisesPercentage
            // 
            this.numericUpDownMunPremisesPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownMunPremisesPercentage.DecimalPlaces = 2;
            this.numericUpDownMunPremisesPercentage.Location = new System.Drawing.Point(550, 134);
            this.numericUpDownMunPremisesPercentage.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownMunPremisesPercentage.Name = "numericUpDownMunPremisesPercentage";
            this.numericUpDownMunPremisesPercentage.ReadOnly = true;
            this.numericUpDownMunPremisesPercentage.Size = new System.Drawing.Size(68, 21);
            this.numericUpDownMunPremisesPercentage.TabIndex = 10;
            this.numericUpDownMunPremisesPercentage.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(526, 136);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(18, 15);
            this.label24.TabIndex = 11;
            this.label24.Text = "%";
            // 
            // numericUpDownMunPremisesCount
            // 
            this.numericUpDownMunPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownMunPremisesCount.Location = new System.Drawing.Point(181, 134);
            this.numericUpDownMunPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownMunPremisesCount.Name = "numericUpDownMunPremisesCount";
            this.numericUpDownMunPremisesCount.ReadOnly = true;
            this.numericUpDownMunPremisesCount.Size = new System.Drawing.Size(340, 21);
            this.numericUpDownMunPremisesCount.TabIndex = 8;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(16, 136);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(158, 15);
            this.label23.TabIndex = 9;
            this.label23.Text = "Всего муниципальных ЖП";
            // 
            // numericUpDownPremisesCount
            // 
            this.numericUpDownPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownPremisesCount.Location = new System.Drawing.Point(181, 18);
            this.numericUpDownPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPremisesCount.Name = "numericUpDownPremisesCount";
            this.numericUpDownPremisesCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownPremisesCount.TabIndex = 0;
            // 
            // numericUpDownRoomsCount
            // 
            this.numericUpDownRoomsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownRoomsCount.Location = new System.Drawing.Point(181, 47);
            this.numericUpDownRoomsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownRoomsCount.Name = "numericUpDownRoomsCount";
            this.numericUpDownRoomsCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownRoomsCount.TabIndex = 1;
            // 
            // numericUpDownApartmentsCount
            // 
            this.numericUpDownApartmentsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownApartmentsCount.Location = new System.Drawing.Point(181, 76);
            this.numericUpDownApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownApartmentsCount.Name = "numericUpDownApartmentsCount";
            this.numericUpDownApartmentsCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownApartmentsCount.TabIndex = 2;
            // 
            // numericUpDownSharedApartmentsCount
            // 
            this.numericUpDownSharedApartmentsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSharedApartmentsCount.Location = new System.Drawing.Point(181, 105);
            this.numericUpDownSharedApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSharedApartmentsCount.Name = "numericUpDownSharedApartmentsCount";
            this.numericUpDownSharedApartmentsCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownSharedApartmentsCount.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Всего";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "Квартир";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Комнат";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(147, 15);
            this.label7.TabIndex = 7;
            this.label7.Text = "Квартир с подселением";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDownOtherPremisesCount);
            this.groupBox3.Controls.Add(this.numericUpDownSpecialPremisesCount);
            this.groupBox3.Controls.Add(this.numericUpDownCommercialPremisesCount);
            this.groupBox3.Controls.Add(this.numericUpDownSocialPremisesCount);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(631, 263);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(622, 164);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Количество помещений по типу найма";
            // 
            // numericUpDownOtherPremisesCount
            // 
            this.numericUpDownOtherPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownOtherPremisesCount.Location = new System.Drawing.Point(175, 105);
            this.numericUpDownOtherPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownOtherPremisesCount.Name = "numericUpDownOtherPremisesCount";
            this.numericUpDownOtherPremisesCount.ReadOnly = true;
            this.numericUpDownOtherPremisesCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownOtherPremisesCount.TabIndex = 3;
            // 
            // numericUpDownSpecialPremisesCount
            // 
            this.numericUpDownSpecialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSpecialPremisesCount.Location = new System.Drawing.Point(175, 76);
            this.numericUpDownSpecialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSpecialPremisesCount.Name = "numericUpDownSpecialPremisesCount";
            this.numericUpDownSpecialPremisesCount.ReadOnly = true;
            this.numericUpDownSpecialPremisesCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownSpecialPremisesCount.TabIndex = 2;
            // 
            // numericUpDownCommercialPremisesCount
            // 
            this.numericUpDownCommercialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCommercialPremisesCount.Location = new System.Drawing.Point(175, 47);
            this.numericUpDownCommercialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownCommercialPremisesCount.Name = "numericUpDownCommercialPremisesCount";
            this.numericUpDownCommercialPremisesCount.ReadOnly = true;
            this.numericUpDownCommercialPremisesCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownCommercialPremisesCount.TabIndex = 1;
            // 
            // numericUpDownSocialPremisesCount
            // 
            this.numericUpDownSocialPremisesCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSocialPremisesCount.Location = new System.Drawing.Point(175, 18);
            this.numericUpDownSocialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSocialPremisesCount.Name = "numericUpDownSocialPremisesCount";
            this.numericUpDownSocialPremisesCount.ReadOnly = true;
            this.numericUpDownSocialPremisesCount.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownSocialPremisesCount.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 107);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 15);
            this.label10.TabIndex = 4;
            this.label10.Text = "Свободные";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 78);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(135, 15);
            this.label11.TabIndex = 5;
            this.label11.Text = "Специализированный";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 49);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 15);
            this.label12.TabIndex = 6;
            this.label12.Text = "Коммерческий";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 15);
            this.label13.TabIndex = 7;
            this.label13.Text = "Социальный";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDownTotalArea);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.numericUpDownLivingArea);
            this.groupBox2.Controls.Add(this.numericUpDownMunicipalArea);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 433);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(622, 107);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Площадь";
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new System.Drawing.Point(181, 18);
            this.numericUpDownTotalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownTotalArea.TabIndex = 0;
            this.numericUpDownTotalArea.ThousandsSeparator = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(16, 20);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(46, 15);
            this.label20.TabIndex = 5;
            this.label20.Text = "Общая";
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new System.Drawing.Point(181, 47);
            this.numericUpDownLivingArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            this.numericUpDownLivingArea.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownLivingArea.TabIndex = 1;
            this.numericUpDownLivingArea.ThousandsSeparator = true;
            // 
            // numericUpDownMunicipalArea
            // 
            this.numericUpDownMunicipalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownMunicipalArea.DecimalPlaces = 3;
            this.numericUpDownMunicipalArea.Location = new System.Drawing.Point(181, 76);
            this.numericUpDownMunicipalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            this.numericUpDownMunicipalArea.ReadOnly = true;
            this.numericUpDownMunicipalArea.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownMunicipalArea.TabIndex = 2;
            this.numericUpDownMunicipalArea.ThousandsSeparator = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Муниципальных ЖП";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "Жилая";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.panel3);
            this.groupBox6.Controls.Add(this.dataGridViewRestrictions);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(3, 546);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(622, 422);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Реквизиты";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.vButtonRestrictionEdit);
            this.panel3.Controls.Add(this.vButtonRestrictionDelete);
            this.panel3.Controls.Add(this.vButtonRestrictionAdd);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(581, 17);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(38, 402);
            this.panel3.TabIndex = 2;
            // 
            // vButtonRestrictionEdit
            // 
            this.vButtonRestrictionEdit.AllowAnimations = true;
            this.vButtonRestrictionEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonRestrictionEdit.BackColor = System.Drawing.Color.Transparent;
            this.vButtonRestrictionEdit.Image = ((System.Drawing.Image)(resources.GetObject("vButtonRestrictionEdit.Image")));
            this.vButtonRestrictionEdit.Location = new System.Drawing.Point(3, 57);
            this.vButtonRestrictionEdit.Name = "vButtonRestrictionEdit";
            this.vButtonRestrictionEdit.RoundedCornersMask = ((byte)(15));
            this.vButtonRestrictionEdit.Size = new System.Drawing.Size(32, 25);
            this.vButtonRestrictionEdit.TabIndex = 2;
            this.vButtonRestrictionEdit.UseVisualStyleBackColor = false;
            this.vButtonRestrictionEdit.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRestrictionEdit.Click += new System.EventHandler(this.vButtonRestrictionEdit_Click);
            // 
            // vButtonRestrictionDelete
            // 
            this.vButtonRestrictionDelete.AllowAnimations = true;
            this.vButtonRestrictionDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonRestrictionDelete.BackColor = System.Drawing.Color.Transparent;
            this.vButtonRestrictionDelete.Image = ((System.Drawing.Image)(resources.GetObject("vButtonRestrictionDelete.Image")));
            this.vButtonRestrictionDelete.Location = new System.Drawing.Point(3, 30);
            this.vButtonRestrictionDelete.Name = "vButtonRestrictionDelete";
            this.vButtonRestrictionDelete.RoundedCornersMask = ((byte)(15));
            this.vButtonRestrictionDelete.Size = new System.Drawing.Size(32, 25);
            this.vButtonRestrictionDelete.TabIndex = 1;
            this.vButtonRestrictionDelete.UseVisualStyleBackColor = false;
            this.vButtonRestrictionDelete.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRestrictionDelete.Click += new System.EventHandler(this.vButtonRestrictionDelete_Click);
            // 
            // vButtonRestrictionAdd
            // 
            this.vButtonRestrictionAdd.AllowAnimations = true;
            this.vButtonRestrictionAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonRestrictionAdd.BackColor = System.Drawing.Color.Transparent;
            this.vButtonRestrictionAdd.Image = ((System.Drawing.Image)(resources.GetObject("vButtonRestrictionAdd.Image")));
            this.vButtonRestrictionAdd.Location = new System.Drawing.Point(3, 3);
            this.vButtonRestrictionAdd.Name = "vButtonRestrictionAdd";
            this.vButtonRestrictionAdd.RoundedCornersMask = ((byte)(15));
            this.vButtonRestrictionAdd.Size = new System.Drawing.Size(32, 25);
            this.vButtonRestrictionAdd.TabIndex = 0;
            this.vButtonRestrictionAdd.UseVisualStyleBackColor = false;
            this.vButtonRestrictionAdd.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRestrictionAdd.Click += new System.EventHandler(this.vButtonRestrictionAdd_Click);
            // 
            // dataGridViewRestrictions
            // 
            this.dataGridViewRestrictions.AllowUserToAddRows = false;
            this.dataGridViewRestrictions.AllowUserToDeleteRows = false;
            this.dataGridViewRestrictions.AllowUserToResizeRows = false;
            this.dataGridViewRestrictions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewRestrictions.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRestrictions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRestrictions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.restriction_number,
            this.restriction_date,
            this.restriction_description,
            this.id_restriction_type});
            this.dataGridViewRestrictions.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            this.dataGridViewRestrictions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRestrictions.Size = new System.Drawing.Size(577, 402);
            this.dataGridViewRestrictions.TabIndex = 0;
            this.dataGridViewRestrictions.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRestrictions_CellDoubleClick);
            this.dataGridViewRestrictions.Resize += new System.EventHandler(this.dataGridViewRestrictions_Resize);
            // 
            // restriction_number
            // 
            this.restriction_number.HeaderText = "Номер";
            this.restriction_number.MinimumWidth = 100;
            this.restriction_number.Name = "restriction_number";
            this.restriction_number.ReadOnly = true;
            // 
            // restriction_date
            // 
            this.restriction_date.HeaderText = "Дата";
            this.restriction_date.MinimumWidth = 100;
            this.restriction_date.Name = "restriction_date";
            this.restriction_date.ReadOnly = true;
            // 
            // restriction_description
            // 
            this.restriction_description.HeaderText = "Наименование";
            this.restriction_description.MinimumWidth = 200;
            this.restriction_description.Name = "restriction_description";
            this.restriction_description.ReadOnly = true;
            this.restriction_description.Width = 200;
            // 
            // id_restriction_type
            // 
            this.id_restriction_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_restriction_type.HeaderText = "Тип права собственности";
            this.id_restriction_type.MinimumWidth = 200;
            this.id_restriction_type.Name = "id_restriction_type";
            this.id_restriction_type.ReadOnly = true;
            this.id_restriction_type.Width = 200;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.panel4);
            this.groupBox7.Controls.Add(this.dataGridViewOwnerships);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(631, 546);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(622, 422);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Ограничения";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.vButtonOwnershipEdit);
            this.panel4.Controls.Add(this.vButtonOwnershipDelete);
            this.panel4.Controls.Add(this.vButtonOwnershipAdd);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(581, 17);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(38, 402);
            this.panel4.TabIndex = 3;
            // 
            // vButtonOwnershipEdit
            // 
            this.vButtonOwnershipEdit.AllowAnimations = true;
            this.vButtonOwnershipEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonOwnershipEdit.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOwnershipEdit.Image = ((System.Drawing.Image)(resources.GetObject("vButtonOwnershipEdit.Image")));
            this.vButtonOwnershipEdit.Location = new System.Drawing.Point(3, 57);
            this.vButtonOwnershipEdit.Name = "vButtonOwnershipEdit";
            this.vButtonOwnershipEdit.RoundedCornersMask = ((byte)(15));
            this.vButtonOwnershipEdit.Size = new System.Drawing.Size(32, 25);
            this.vButtonOwnershipEdit.TabIndex = 2;
            this.vButtonOwnershipEdit.UseVisualStyleBackColor = false;
            this.vButtonOwnershipEdit.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOwnershipEdit.Click += new System.EventHandler(this.vButtonOwnershipEdit_Click);
            // 
            // vButtonOwnershipDelete
            // 
            this.vButtonOwnershipDelete.AllowAnimations = true;
            this.vButtonOwnershipDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonOwnershipDelete.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOwnershipDelete.Image = ((System.Drawing.Image)(resources.GetObject("vButtonOwnershipDelete.Image")));
            this.vButtonOwnershipDelete.Location = new System.Drawing.Point(3, 30);
            this.vButtonOwnershipDelete.Name = "vButtonOwnershipDelete";
            this.vButtonOwnershipDelete.RoundedCornersMask = ((byte)(15));
            this.vButtonOwnershipDelete.Size = new System.Drawing.Size(32, 25);
            this.vButtonOwnershipDelete.TabIndex = 1;
            this.vButtonOwnershipDelete.UseVisualStyleBackColor = false;
            this.vButtonOwnershipDelete.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOwnershipDelete.Click += new System.EventHandler(this.vButtonOwnershipDelete_Click);
            // 
            // vButtonOwnershipAdd
            // 
            this.vButtonOwnershipAdd.AllowAnimations = true;
            this.vButtonOwnershipAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonOwnershipAdd.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOwnershipAdd.Image = ((System.Drawing.Image)(resources.GetObject("vButtonOwnershipAdd.Image")));
            this.vButtonOwnershipAdd.Location = new System.Drawing.Point(3, 3);
            this.vButtonOwnershipAdd.Name = "vButtonOwnershipAdd";
            this.vButtonOwnershipAdd.RoundedCornersMask = ((byte)(15));
            this.vButtonOwnershipAdd.Size = new System.Drawing.Size(32, 25);
            this.vButtonOwnershipAdd.TabIndex = 0;
            this.vButtonOwnershipAdd.UseVisualStyleBackColor = false;
            this.vButtonOwnershipAdd.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOwnershipAdd.Click += new System.EventHandler(this.vButtonOwnershipAdd_Click);
            // 
            // dataGridViewOwnerships
            // 
            this.dataGridViewOwnerships.AllowUserToAddRows = false;
            this.dataGridViewOwnerships.AllowUserToDeleteRows = false;
            this.dataGridViewOwnerships.AllowUserToResizeRows = false;
            this.dataGridViewOwnerships.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewOwnerships.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewOwnerships.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOwnerships.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ownership_number,
            this.ownership_date,
            this.ownership_description,
            this.id_ownership_type});
            this.dataGridViewOwnerships.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            this.dataGridViewOwnerships.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOwnerships.Size = new System.Drawing.Size(577, 402);
            this.dataGridViewOwnerships.TabIndex = 0;
            this.dataGridViewOwnerships.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOwnerships_CellDoubleClick);
            this.dataGridViewOwnerships.Resize += new System.EventHandler(this.dataGridViewOwnerships_Resize);
            // 
            // ownership_number
            // 
            this.ownership_number.HeaderText = "Номер";
            this.ownership_number.MinimumWidth = 100;
            this.ownership_number.Name = "ownership_number";
            this.ownership_number.ReadOnly = true;
            // 
            // ownership_date
            // 
            this.ownership_date.HeaderText = "Дата";
            this.ownership_date.MinimumWidth = 100;
            this.ownership_date.Name = "ownership_date";
            this.ownership_date.ReadOnly = true;
            // 
            // ownership_description
            // 
            this.ownership_description.HeaderText = "Наименование";
            this.ownership_description.MinimumWidth = 200;
            this.ownership_description.Name = "ownership_description";
            this.ownership_description.ReadOnly = true;
            this.ownership_description.Width = 200;
            // 
            // id_ownership_type
            // 
            this.id_ownership_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_ownership_type.HeaderText = "Тип ограничения";
            this.id_ownership_type.MinimumWidth = 200;
            this.id_ownership_type.Name = "id_ownership_type";
            this.id_ownership_type.ReadOnly = true;
            this.id_ownership_type.Width = 200;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox8, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(631, 433);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(622, 107);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxRoomsBTI);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(305, 101);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Приватиз. кв. по данным БТИ";
            // 
            // textBoxRoomsBTI
            // 
            this.textBoxRoomsBTI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRoomsBTI.Location = new System.Drawing.Point(0, 20);
            this.textBoxRoomsBTI.MaxLength = 1512;
            this.textBoxRoomsBTI.Multiline = true;
            this.textBoxRoomsBTI.Name = "textBoxRoomsBTI";
            this.textBoxRoomsBTI.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxRoomsBTI.Size = new System.Drawing.Size(305, 81);
            this.textBoxRoomsBTI.TabIndex = 0;
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.Controls.Add(this.textBoxDescription);
            this.groupBox8.Location = new System.Drawing.Point(314, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(305, 101);
            this.groupBox8.TabIndex = 1;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Доп. сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(0, 20);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(305, 81);
            this.textBoxDescription.TabIndex = 0;
            // 
            // BuildingViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(940, 660);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1262, 977);
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BuildingViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Здание";
            this.tableLayoutPanel.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunPremisesPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoomsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownApartmentsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharedApartmentsCount)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOtherPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpecialPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCommercialPremisesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSocialPremisesCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        private NumericUpDown numericUpDownMunPremisesPercentage;
        private Label label24;
        private NumericUpDown numericUpDownMunPremisesCount;
        private Label label23;
        private CheckBox checkBoxRadioNetwork;
        private CheckBox checkBoxHotWaterSupply;
        private CheckBox checkBoxElectricity;
        private CheckBox checkBoxCanalization;
        private CheckBox checkBoxPlumbing;
        private ComboBox comboBoxHeatingType;
        private Label label25;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox5;
        private TextBox textBoxRoomsBTI;
        private GroupBox groupBox8;
        private TextBox textBoxDescription;
    }
}
