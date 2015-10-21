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
        private GroupBox groupBox5;
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
        private TextBox textBoxDescription;
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
        #endregion Components


        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(BuildingViewport));
            this.tableLayoutPanel = new TableLayoutPanel();
            this.groupBox4 = new GroupBox();
            this.tableLayoutPanel2 = new TableLayoutPanel();
            this.panel1 = new Panel();
            this.numericUpDownWear = new NumericUpDown();
            this.label21 = new Label();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label17 = new Label();
            this.label18 = new Label();
            this.numericUpDownFloors = new NumericUpDown();
            this.numericUpDownStartupYear = new NumericUpDown();
            this.comboBoxStreet = new ComboBox();
            this.comboBoxStructureType = new ComboBox();
            this.textBoxHouse = new TextBox();
            this.panel2 = new Panel();
            this.dateTimePickerStateDate = new DateTimePicker();
            this.label22 = new Label();
            this.label40 = new Label();
            this.comboBoxState = new ComboBox();
            this.comboBoxCurrentFundType = new ComboBox();
            this.numericUpDownBalanceCost = new NumericUpDown();
            this.numericUpDownCadastralCost = new NumericUpDown();
            this.checkBoxImprovement = new CheckBox();
            this.checkBoxElevator = new CheckBox();
            this.label14 = new Label();
            this.label15 = new Label();
            this.label16 = new Label();
            this.label19 = new Label();
            this.textBoxCadastralNum = new TextBox();
            this.groupBox1 = new GroupBox();
            this.numericUpDownPremisesCount = new NumericUpDown();
            this.numericUpDownRoomsCount = new NumericUpDown();
            this.numericUpDownApartmentsCount = new NumericUpDown();
            this.numericUpDownSharedApartmentsCount = new NumericUpDown();
            this.label4 = new Label();
            this.label5 = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.groupBox3 = new GroupBox();
            this.numericUpDownOtherPremisesCount = new NumericUpDown();
            this.numericUpDownSpecialPremisesCount = new NumericUpDown();
            this.numericUpDownCommercialPremisesCount = new NumericUpDown();
            this.numericUpDownSocialPremisesCount = new NumericUpDown();
            this.label10 = new Label();
            this.label11 = new Label();
            this.label12 = new Label();
            this.label13 = new Label();
            this.groupBox2 = new GroupBox();
            this.numericUpDownTotalArea = new NumericUpDown();
            this.label20 = new Label();
            this.numericUpDownLivingArea = new NumericUpDown();
            this.numericUpDownMunicipalArea = new NumericUpDown();
            this.label8 = new Label();
            this.label9 = new Label();
            this.groupBox5 = new GroupBox();
            this.textBoxDescription = new TextBox();
            this.groupBox6 = new GroupBox();
            this.panel3 = new Panel();
            this.vButtonRestrictionEdit = new vButton();
            this.vButtonRestrictionDelete = new vButton();
            this.vButtonRestrictionAdd = new vButton();
            this.dataGridViewRestrictions = new DataGridView();
            this.restriction_number = new DataGridViewTextBoxColumn();
            this.restriction_date = new DataGridViewTextBoxColumn();
            this.restriction_description = new DataGridViewTextBoxColumn();
            this.id_restriction_type = new DataGridViewComboBoxColumn();
            this.groupBox7 = new GroupBox();
            this.panel4 = new Panel();
            this.vButtonOwnershipEdit = new vButton();
            this.vButtonOwnershipDelete = new vButton();
            this.vButtonOwnershipAdd = new vButton();
            this.dataGridViewOwnerships = new DataGridView();
            this.ownership_number = new DataGridViewTextBoxColumn();
            this.ownership_date = new DataGridViewTextBoxColumn();
            this.ownership_description = new DataGridViewTextBoxColumn();
            this.id_ownership_type = new DataGridViewComboBoxColumn();
            this.checkBoxRubbishChute = new CheckBox();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDownWear)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownFloors)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownStartupYear)).BeginInit();
            this.panel2.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDownBalanceCost)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownCadastralCost)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDownPremisesCount)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownRoomsCount)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownApartmentsCount)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownSharedApartmentsCount)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDownOtherPremisesCount)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownSpecialPremisesCount)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownCommercialPremisesCount)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownSocialPremisesCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((ISupportInitialize)(this.numericUpDownMunicipalArea)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.panel3.SuspendLayout();
            ((ISupportInitialize)(this.dataGridViewRestrictions)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.panel4.SuspendLayout();
            ((ISupportInitialize)(this.dataGridViewOwnerships)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBox3, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.groupBox5, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.groupBox6, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.groupBox7, 1, 3);
            this.tableLayoutPanel.Dock = DockStyle.Fill;
            this.tableLayoutPanel.Location = new Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 235F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new Size(1002, 596);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.tableLayoutPanel.SetColumnSpan(this.groupBox4, 2);
            this.groupBox4.Controls.Add(this.tableLayoutPanel2);
            this.groupBox4.Dock = DockStyle.Fill;
            this.groupBox4.Location = new Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new Size(996, 229);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Общие сведения";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel2.Dock = DockStyle.Fill;
            this.tableLayoutPanel2.Location = new Point(3, 17);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            this.tableLayoutPanel2.Size = new Size(990, 209);
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
            this.panel1.Controls.Add(this.numericUpDownStartupYear);
            this.panel1.Controls.Add(this.comboBoxStreet);
            this.panel1.Controls.Add(this.comboBoxStructureType);
            this.panel1.Controls.Add(this.textBoxHouse);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(489, 203);
            this.panel1.TabIndex = 0;
            // 
            // numericUpDownWear
            // 
            this.numericUpDownWear.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownWear.DecimalPlaces = 2;
            this.numericUpDownWear.Location = new Point(174, 149);
            this.numericUpDownWear.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownWear.Name = "numericUpDownWear";
            this.numericUpDownWear.Size = new Size(311, 21);
            this.numericUpDownWear.TabIndex = 5;
            this.numericUpDownWear.ThousandsSeparator = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new Point(10, 152);
            this.label21.Name = "label21";
            this.label21.Size = new Size(59, 15);
            this.label21.TabIndex = 36;
            this.label21.Text = "Износ, %";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new Size(43, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Улица";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(10, 38);
            this.label2.Name = "label2";
            this.label2.Size = new Size(79, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Номер дома";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(10, 67);
            this.label3.Name = "label3";
            this.label3.Size = new Size(105, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Этажность дома";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new Point(10, 96);
            this.label17.Name = "label17";
            this.label17.Size = new Size(160, 15);
            this.label17.TabIndex = 3;
            this.label17.Text = "Год ввода в эксплуатацию";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new Point(10, 125);
            this.label18.Name = "label18";
            this.label18.Size = new Size(155, 15);
            this.label18.TabIndex = 4;
            this.label18.Text = "Тип строения (материал)";
            // 
            // numericUpDownFloors
            // 
            this.numericUpDownFloors.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownFloors.Location = new Point(175, 65);
            this.numericUpDownFloors.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownFloors.Name = "numericUpDownFloors";
            this.numericUpDownFloors.Size = new Size(310, 21);
            this.numericUpDownFloors.TabIndex = 2;
            // 
            // numericUpDownStartupYear
            // 
            this.numericUpDownStartupYear.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownStartupYear.Location = new Point(175, 93);
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
            this.numericUpDownStartupYear.Size = new Size(310, 21);
            this.numericUpDownStartupYear.TabIndex = 3;
            this.numericUpDownStartupYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new Point(175, 7);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new Size(310, 23);
            this.comboBoxStreet.TabIndex = 0;
            this.comboBoxStreet.DropDownClosed += new EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.KeyUp += new KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new EventHandler(this.comboBoxStreet_Leave);
            // 
            // comboBoxStructureType
            // 
            this.comboBoxStructureType.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.comboBoxStructureType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxStructureType.FormattingEnabled = true;
            this.comboBoxStructureType.Location = new Point(175, 121);
            this.comboBoxStructureType.Name = "comboBoxStructureType";
            this.comboBoxStructureType.Size = new Size(310, 23);
            this.comboBoxStructureType.TabIndex = 4;
           // 
            // textBoxHouse
            // 
            this.textBoxHouse.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.textBoxHouse.Location = new Point(175, 37);
            this.textBoxHouse.MaxLength = 20;
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new Size(310, 21);
            this.textBoxHouse.TabIndex = 1;
            this.textBoxHouse.KeyPress += new KeyPressEventHandler(this.textBoxHouse_KeyPress);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBoxRubbishChute);
            this.panel2.Controls.Add(this.dateTimePickerStateDate);
            this.panel2.Controls.Add(this.label22);
            this.panel2.Controls.Add(this.label40);
            this.panel2.Controls.Add(this.comboBoxState);
            this.panel2.Controls.Add(this.comboBoxCurrentFundType);
            this.panel2.Controls.Add(this.numericUpDownBalanceCost);
            this.panel2.Controls.Add(this.numericUpDownCadastralCost);
            this.panel2.Controls.Add(this.checkBoxImprovement);
            this.panel2.Controls.Add(this.checkBoxElevator);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.label19);
            this.panel2.Controls.Add(this.textBoxCadastralNum);
            this.panel2.Dock = DockStyle.Fill;
            this.panel2.Location = new Point(498, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(489, 203);
            this.panel2.TabIndex = 1;
            // 
            // dateTimePickerStateDate
            // 
            this.dateTimePickerStateDate.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.dateTimePickerStateDate.Location = new Point(175, 121);
            this.dateTimePickerStateDate.Name = "dateTimePickerStateDate";
            this.dateTimePickerStateDate.ShowCheckBox = true;
            this.dateTimePickerStateDate.Size = new Size(310, 21);
            this.dateTimePickerStateDate.TabIndex = 4;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new Point(16, 124);
            this.label22.Name = "label22";
            this.label22.Size = new Size(147, 15);
            this.label22.TabIndex = 37;
            this.label22.Text = "Состояние установлено";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new Point(16, 95);
            this.label40.Name = "label40";
            this.label40.Size = new Size(119, 15);
            this.label40.TabIndex = 31;
            this.label40.Text = "Текущее состояние";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxState.FormattingEnabled = true;
            this.comboBoxState.Location = new Point(175, 91);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new Size(310, 23);
            this.comboBoxState.TabIndex = 3;
           // 
            // comboBoxCurrentFundType
            // 
            this.comboBoxCurrentFundType.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.comboBoxCurrentFundType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxCurrentFundType.Enabled = false;
            this.comboBoxCurrentFundType.ForeColor = Color.Black;
            this.comboBoxCurrentFundType.FormattingEnabled = true;
            this.comboBoxCurrentFundType.Location = new Point(175, 149);
            this.comboBoxCurrentFundType.Name = "comboBoxCurrentFundType";
            this.comboBoxCurrentFundType.Size = new Size(310, 23);
            this.comboBoxCurrentFundType.TabIndex = 5;
            this.comboBoxCurrentFundType.Visible = false;
            // 
            // numericUpDownBalanceCost
            // 
            this.numericUpDownBalanceCost.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownBalanceCost.DecimalPlaces = 2;
            this.numericUpDownBalanceCost.Location = new Point(175, 63);
            this.numericUpDownBalanceCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            this.numericUpDownBalanceCost.Size = new Size(310, 21);
            this.numericUpDownBalanceCost.TabIndex = 2;
            this.numericUpDownBalanceCost.ThousandsSeparator = true;
            // 
            // numericUpDownCadastralCost
            // 
            this.numericUpDownCadastralCost.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownCadastralCost.DecimalPlaces = 2;
            this.numericUpDownCadastralCost.Location = new Point(175, 35);
            this.numericUpDownCadastralCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            this.numericUpDownCadastralCost.Size = new Size(310, 21);
            this.numericUpDownCadastralCost.TabIndex = 1;
            this.numericUpDownCadastralCost.ThousandsSeparator = true;
            // 
            // checkBoxImprovement
            // 
            this.checkBoxImprovement.AutoSize = true;
            this.checkBoxImprovement.Location = new Point(175, 177);
            this.checkBoxImprovement.Name = "checkBoxImprovement";
            this.checkBoxImprovement.Size = new Size(126, 19);
            this.checkBoxImprovement.TabIndex = 7;
            this.checkBoxImprovement.Text = "Благоустройство";
            this.checkBoxImprovement.UseVisualStyleBackColor = true;
            // 
            // checkBoxElevator
            // 
            this.checkBoxElevator.AutoSize = true;
            this.checkBoxElevator.Location = new Point(19, 177);
            this.checkBoxElevator.Name = "checkBoxElevator";
            this.checkBoxElevator.Size = new Size(118, 19);
            this.checkBoxElevator.TabIndex = 6;
            this.checkBoxElevator.Text = "Наличие лифта";
            this.checkBoxElevator.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new Point(16, 11);
            this.label14.Name = "label14";
            this.label14.Size = new Size(126, 15);
            this.label14.TabIndex = 32;
            this.label14.Text = "Кадастровый номер";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new Point(16, 39);
            this.label15.Name = "label15";
            this.label15.Size = new Size(150, 15);
            this.label15.TabIndex = 33;
            this.label15.Text = "Кадастровая стоимость";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new Point(16, 67);
            this.label16.Name = "label16";
            this.label16.Size = new Size(143, 15);
            this.label16.TabIndex = 34;
            this.label16.Text = "Балансовая стоимость";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new Point(16, 151);
            this.label19.Name = "label19";
            this.label19.Size = new Size(90, 15);
            this.label19.TabIndex = 35;
            this.label19.Text = "Текущий фонд";
            this.label19.Visible = false;
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.textBoxCadastralNum.Location = new Point(175, 7);
            this.textBoxCadastralNum.MaxLength = 20;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new Size(310, 21);
            this.textBoxCadastralNum.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownPremisesCount);
            this.groupBox1.Controls.Add(this.numericUpDownRoomsCount);
            this.groupBox1.Controls.Add(this.numericUpDownApartmentsCount);
            this.groupBox1.Controls.Add(this.numericUpDownSharedApartmentsCount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Dock = DockStyle.Fill;
            this.groupBox1.Location = new Point(3, 238);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(495, 134);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Количество жилых помещений";
            // 
            // numericUpDownPremisesCount
            // 
            this.numericUpDownPremisesCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownPremisesCount.Location = new Point(181, 18);
            this.numericUpDownPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPremisesCount.Name = "numericUpDownPremisesCount";
            this.numericUpDownPremisesCount.Size = new Size(310, 21);
            this.numericUpDownPremisesCount.TabIndex = 0;
            // 
            // numericUpDownRoomsCount
            // 
            this.numericUpDownRoomsCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownRoomsCount.Location = new Point(181, 47);
            this.numericUpDownRoomsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownRoomsCount.Name = "numericUpDownRoomsCount";
            this.numericUpDownRoomsCount.Size = new Size(310, 21);
            this.numericUpDownRoomsCount.TabIndex = 1;
            // 
            // numericUpDownApartmentsCount
            // 
            this.numericUpDownApartmentsCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownApartmentsCount.Location = new Point(181, 76);
            this.numericUpDownApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownApartmentsCount.Name = "numericUpDownApartmentsCount";
            this.numericUpDownApartmentsCount.Size = new Size(310, 21);
            this.numericUpDownApartmentsCount.TabIndex = 2;
            // 
            // numericUpDownSharedApartmentsCount
            // 
            this.numericUpDownSharedApartmentsCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownSharedApartmentsCount.Location = new Point(181, 105);
            this.numericUpDownSharedApartmentsCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSharedApartmentsCount.Name = "numericUpDownSharedApartmentsCount";
            this.numericUpDownSharedApartmentsCount.Size = new Size(310, 21);
            this.numericUpDownSharedApartmentsCount.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new Point(16, 20);
            this.label4.Name = "label4";
            this.label4.Size = new Size(40, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Всего";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new Point(16, 78);
            this.label5.Name = "label5";
            this.label5.Size = new Size(57, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "Квартир";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new Point(16, 49);
            this.label6.Name = "label6";
            this.label6.Size = new Size(52, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Комнат";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new Point(16, 107);
            this.label7.Name = "label7";
            this.label7.Size = new Size(147, 15);
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
            this.groupBox3.Dock = DockStyle.Fill;
            this.groupBox3.Location = new Point(504, 238);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(495, 134);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Количество помещений по типу найма";
            // 
            // numericUpDownOtherPremisesCount
            // 
            this.numericUpDownOtherPremisesCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownOtherPremisesCount.Location = new Point(175, 105);
            this.numericUpDownOtherPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownOtherPremisesCount.Name = "numericUpDownOtherPremisesCount";
            this.numericUpDownOtherPremisesCount.ReadOnly = true;
            this.numericUpDownOtherPremisesCount.Size = new Size(310, 21);
            this.numericUpDownOtherPremisesCount.TabIndex = 3;
            // 
            // numericUpDownSpecialPremisesCount
            // 
            this.numericUpDownSpecialPremisesCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownSpecialPremisesCount.Location = new Point(175, 76);
            this.numericUpDownSpecialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSpecialPremisesCount.Name = "numericUpDownSpecialPremisesCount";
            this.numericUpDownSpecialPremisesCount.ReadOnly = true;
            this.numericUpDownSpecialPremisesCount.Size = new Size(310, 21);
            this.numericUpDownSpecialPremisesCount.TabIndex = 2;
            // 
            // numericUpDownCommercialPremisesCount
            // 
            this.numericUpDownCommercialPremisesCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownCommercialPremisesCount.Location = new Point(175, 47);
            this.numericUpDownCommercialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownCommercialPremisesCount.Name = "numericUpDownCommercialPremisesCount";
            this.numericUpDownCommercialPremisesCount.ReadOnly = true;
            this.numericUpDownCommercialPremisesCount.Size = new Size(310, 21);
            this.numericUpDownCommercialPremisesCount.TabIndex = 1;
            // 
            // numericUpDownSocialPremisesCount
            // 
            this.numericUpDownSocialPremisesCount.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownSocialPremisesCount.Location = new Point(175, 18);
            this.numericUpDownSocialPremisesCount.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownSocialPremisesCount.Name = "numericUpDownSocialPremisesCount";
            this.numericUpDownSocialPremisesCount.ReadOnly = true;
            this.numericUpDownSocialPremisesCount.Size = new Size(310, 21);
            this.numericUpDownSocialPremisesCount.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new Point(16, 107);
            this.label10.Name = "label10";
            this.label10.Size = new Size(50, 15);
            this.label10.TabIndex = 4;
            this.label10.Text = "Прочие";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new Point(16, 78);
            this.label11.Name = "label11";
            this.label11.Size = new Size(135, 15);
            this.label11.TabIndex = 5;
            this.label11.Text = "Специализированный";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new Point(16, 49);
            this.label12.Name = "label12";
            this.label12.Size = new Size(93, 15);
            this.label12.TabIndex = 6;
            this.label12.Text = "Коммерческий";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new Point(16, 20);
            this.label13.Name = "label13";
            this.label13.Size = new Size(80, 15);
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
            this.groupBox2.Dock = DockStyle.Fill;
            this.groupBox2.Location = new Point(3, 378);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(495, 104);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Площадь";
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new Point(181, 18);
            this.numericUpDownTotalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new Size(310, 21);
            this.numericUpDownTotalArea.TabIndex = 0;
            this.numericUpDownTotalArea.ThousandsSeparator = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new Point(16, 20);
            this.label20.Name = "label20";
            this.label20.Size = new Size(46, 15);
            this.label20.TabIndex = 5;
            this.label20.Text = "Общая";
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new Point(181, 47);
            this.numericUpDownLivingArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            this.numericUpDownLivingArea.Size = new Size(310, 21);
            this.numericUpDownLivingArea.TabIndex = 1;
            this.numericUpDownLivingArea.ThousandsSeparator = true;
            // 
            // numericUpDownMunicipalArea
            // 
            this.numericUpDownMunicipalArea.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.numericUpDownMunicipalArea.DecimalPlaces = 3;
            this.numericUpDownMunicipalArea.Location = new Point(181, 76);
            this.numericUpDownMunicipalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            this.numericUpDownMunicipalArea.ReadOnly = true;
            this.numericUpDownMunicipalArea.Size = new Size(310, 21);
            this.numericUpDownMunicipalArea.TabIndex = 2;
            this.numericUpDownMunicipalArea.ThousandsSeparator = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new Point(16, 78);
            this.label8.Name = "label8";
            this.label8.Size = new Size(124, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Муниципальных ЖП";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new Point(16, 49);
            this.label9.Name = "label9";
            this.label9.Size = new Size(46, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "Жилая";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxDescription);
            this.groupBox5.Dock = DockStyle.Fill;
            this.groupBox5.Location = new Point(504, 378);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new Size(495, 104);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = DockStyle.Fill;
            this.textBoxDescription.Location = new Point(3, 17);
            this.textBoxDescription.MaxLength = 255;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new Size(489, 84);
            this.textBoxDescription.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.panel3);
            this.groupBox6.Controls.Add(this.dataGridViewRestrictions);
            this.groupBox6.Dock = DockStyle.Fill;
            this.groupBox6.Location = new Point(3, 488);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new Size(495, 105);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Реквизиты";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.vButtonRestrictionEdit);
            this.panel3.Controls.Add(this.vButtonRestrictionDelete);
            this.panel3.Controls.Add(this.vButtonRestrictionAdd);
            this.panel3.Dock = DockStyle.Right;
            this.panel3.Location = new Point(454, 17);
            this.panel3.Margin = new Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(38, 85);
            this.panel3.TabIndex = 2;
            // 
            // vButtonRestrictionEdit
            // 
            this.vButtonRestrictionEdit.AllowAnimations = true;
            this.vButtonRestrictionEdit.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.vButtonRestrictionEdit.BackColor = Color.Transparent;
            this.vButtonRestrictionEdit.Image = ((Image)(resources.GetObject("vButtonRestrictionEdit.Image")));
            this.vButtonRestrictionEdit.Location = new Point(3, 57);
            this.vButtonRestrictionEdit.Name = "vButtonRestrictionEdit";
            this.vButtonRestrictionEdit.RoundedCornersMask = ((byte)(15));
            this.vButtonRestrictionEdit.Size = new Size(32, 25);
            this.vButtonRestrictionEdit.TabIndex = 2;
            this.vButtonRestrictionEdit.UseVisualStyleBackColor = false;
            this.vButtonRestrictionEdit.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRestrictionEdit.Click += new EventHandler(this.vButtonRestrictionEdit_Click);
            // 
            // vButtonRestrictionDelete
            // 
            this.vButtonRestrictionDelete.AllowAnimations = true;
            this.vButtonRestrictionDelete.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.vButtonRestrictionDelete.BackColor = Color.Transparent;
            this.vButtonRestrictionDelete.Image = ((Image)(resources.GetObject("vButtonRestrictionDelete.Image")));
            this.vButtonRestrictionDelete.Location = new Point(3, 30);
            this.vButtonRestrictionDelete.Name = "vButtonRestrictionDelete";
            this.vButtonRestrictionDelete.RoundedCornersMask = ((byte)(15));
            this.vButtonRestrictionDelete.Size = new Size(32, 25);
            this.vButtonRestrictionDelete.TabIndex = 1;
            this.vButtonRestrictionDelete.UseVisualStyleBackColor = false;
            this.vButtonRestrictionDelete.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRestrictionDelete.Click += new EventHandler(this.vButtonRestrictionDelete_Click);
            // 
            // vButtonRestrictionAdd
            // 
            this.vButtonRestrictionAdd.AllowAnimations = true;
            this.vButtonRestrictionAdd.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.vButtonRestrictionAdd.BackColor = Color.Transparent;
            this.vButtonRestrictionAdd.Image = ((Image)(resources.GetObject("vButtonRestrictionAdd.Image")));
            this.vButtonRestrictionAdd.Location = new Point(3, 3);
            this.vButtonRestrictionAdd.Name = "vButtonRestrictionAdd";
            this.vButtonRestrictionAdd.RoundedCornersMask = ((byte)(15));
            this.vButtonRestrictionAdd.Size = new Size(32, 25);
            this.vButtonRestrictionAdd.TabIndex = 0;
            this.vButtonRestrictionAdd.UseVisualStyleBackColor = false;
            this.vButtonRestrictionAdd.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRestrictionAdd.Click += new EventHandler(this.vButtonRestrictionAdd_Click);
            // 
            // dataGridViewRestrictions
            // 
            this.dataGridViewRestrictions.AllowUserToAddRows = false;
            this.dataGridViewRestrictions.AllowUserToDeleteRows = false;
            this.dataGridViewRestrictions.AllowUserToResizeRows = false;
            this.dataGridViewRestrictions.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.dataGridViewRestrictions.BackgroundColor = Color.White;
            this.dataGridViewRestrictions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRestrictions.Columns.AddRange(new DataGridViewColumn[] {
            this.restriction_number,
            this.restriction_date,
            this.restriction_description,
            this.id_restriction_type});
            this.dataGridViewRestrictions.Location = new Point(3, 17);
            this.dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            this.dataGridViewRestrictions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRestrictions.Size = new Size(450, 85);
            this.dataGridViewRestrictions.TabIndex = 0;
            this.dataGridViewRestrictions.CellDoubleClick += new DataGridViewCellEventHandler(this.dataGridViewRestrictions_CellDoubleClick);
            this.dataGridViewRestrictions.Resize += new EventHandler(this.dataGridViewRestrictions_Resize);
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
            this.id_restriction_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
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
            this.groupBox7.Dock = DockStyle.Fill;
            this.groupBox7.Location = new Point(504, 488);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new Size(495, 105);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Ограничения";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.vButtonOwnershipEdit);
            this.panel4.Controls.Add(this.vButtonOwnershipDelete);
            this.panel4.Controls.Add(this.vButtonOwnershipAdd);
            this.panel4.Dock = DockStyle.Right;
            this.panel4.Location = new Point(454, 17);
            this.panel4.Margin = new Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new Size(38, 85);
            this.panel4.TabIndex = 3;
            // 
            // vButtonOwnershipEdit
            // 
            this.vButtonOwnershipEdit.AllowAnimations = true;
            this.vButtonOwnershipEdit.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.vButtonOwnershipEdit.BackColor = Color.Transparent;
            this.vButtonOwnershipEdit.Image = ((Image)(resources.GetObject("vButtonOwnershipEdit.Image")));
            this.vButtonOwnershipEdit.Location = new Point(3, 57);
            this.vButtonOwnershipEdit.Name = "vButtonOwnershipEdit";
            this.vButtonOwnershipEdit.RoundedCornersMask = ((byte)(15));
            this.vButtonOwnershipEdit.Size = new Size(32, 25);
            this.vButtonOwnershipEdit.TabIndex = 2;
            this.vButtonOwnershipEdit.UseVisualStyleBackColor = false;
            this.vButtonOwnershipEdit.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOwnershipEdit.Click += new EventHandler(this.vButtonOwnershipEdit_Click);
            // 
            // vButtonOwnershipDelete
            // 
            this.vButtonOwnershipDelete.AllowAnimations = true;
            this.vButtonOwnershipDelete.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.vButtonOwnershipDelete.BackColor = Color.Transparent;
            this.vButtonOwnershipDelete.Image = ((Image)(resources.GetObject("vButtonOwnershipDelete.Image")));
            this.vButtonOwnershipDelete.Location = new Point(3, 30);
            this.vButtonOwnershipDelete.Name = "vButtonOwnershipDelete";
            this.vButtonOwnershipDelete.RoundedCornersMask = ((byte)(15));
            this.vButtonOwnershipDelete.Size = new Size(32, 25);
            this.vButtonOwnershipDelete.TabIndex = 1;
            this.vButtonOwnershipDelete.UseVisualStyleBackColor = false;
            this.vButtonOwnershipDelete.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOwnershipDelete.Click += new EventHandler(this.vButtonOwnershipDelete_Click);
            // 
            // vButtonOwnershipAdd
            // 
            this.vButtonOwnershipAdd.AllowAnimations = true;
            this.vButtonOwnershipAdd.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.vButtonOwnershipAdd.BackColor = Color.Transparent;
            this.vButtonOwnershipAdd.Image = ((Image)(resources.GetObject("vButtonOwnershipAdd.Image")));
            this.vButtonOwnershipAdd.Location = new Point(3, 3);
            this.vButtonOwnershipAdd.Name = "vButtonOwnershipAdd";
            this.vButtonOwnershipAdd.RoundedCornersMask = ((byte)(15));
            this.vButtonOwnershipAdd.Size = new Size(32, 25);
            this.vButtonOwnershipAdd.TabIndex = 0;
            this.vButtonOwnershipAdd.UseVisualStyleBackColor = false;
            this.vButtonOwnershipAdd.VIBlendTheme = VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOwnershipAdd.Click += new EventHandler(this.vButtonOwnershipAdd_Click);
            // 
            // dataGridViewOwnerships
            // 
            this.dataGridViewOwnerships.AllowUserToAddRows = false;
            this.dataGridViewOwnerships.AllowUserToDeleteRows = false;
            this.dataGridViewOwnerships.AllowUserToResizeRows = false;
            this.dataGridViewOwnerships.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.dataGridViewOwnerships.BackgroundColor = Color.White;
            this.dataGridViewOwnerships.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOwnerships.Columns.AddRange(new DataGridViewColumn[] {
            this.ownership_number,
            this.ownership_date,
            this.ownership_description,
            this.id_ownership_type});
            this.dataGridViewOwnerships.Location = new Point(3, 17);
            this.dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            this.dataGridViewOwnerships.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOwnerships.Size = new Size(450, 85);
            this.dataGridViewOwnerships.TabIndex = 0;
            this.dataGridViewOwnerships.CellDoubleClick += new DataGridViewCellEventHandler(this.dataGridViewOwnerships_CellDoubleClick);
            this.dataGridViewOwnerships.Resize += new EventHandler(this.dataGridViewOwnerships_Resize);
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
            this.id_ownership_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_ownership_type.HeaderText = "Тип ограничения";
            this.id_ownership_type.MinimumWidth = 200;
            this.id_ownership_type.Name = "id_ownership_type";
            this.id_ownership_type.ReadOnly = true;
            this.id_ownership_type.Width = 200;
            // 
            // checkBoxRubbishChute
            // 
            this.checkBoxRubbishChute.AutoSize = true;
            this.checkBoxRubbishChute.Location = new Point(331, 177);
            this.checkBoxRubbishChute.Name = "checkBoxRubbishChute";
            this.checkBoxRubbishChute.Size = new Size(111, 19);
            this.checkBoxRubbishChute.TabIndex = 38;
            this.checkBoxRubbishChute.Text = "Мусоропровод";
            this.checkBoxRubbishChute.UseVisualStyleBackColor = true;
            // 
            // BuildingViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(630, 570);
            this.BackColor = Color.White;
            this.ClientSize = new Size(1008, 602);
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BuildingViewport";
            this.Padding = new Padding(3);
            this.Text = "Здание";
            this.tableLayoutPanel.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((ISupportInitialize)(this.numericUpDownWear)).EndInit();
            ((ISupportInitialize)(this.numericUpDownFloors)).EndInit();
            ((ISupportInitialize)(this.numericUpDownStartupYear)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((ISupportInitialize)(this.numericUpDownBalanceCost)).EndInit();
            ((ISupportInitialize)(this.numericUpDownCadastralCost)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((ISupportInitialize)(this.numericUpDownPremisesCount)).EndInit();
            ((ISupportInitialize)(this.numericUpDownRoomsCount)).EndInit();
            ((ISupportInitialize)(this.numericUpDownApartmentsCount)).EndInit();
            ((ISupportInitialize)(this.numericUpDownSharedApartmentsCount)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((ISupportInitialize)(this.numericUpDownOtherPremisesCount)).EndInit();
            ((ISupportInitialize)(this.numericUpDownSpecialPremisesCount)).EndInit();
            ((ISupportInitialize)(this.numericUpDownCommercialPremisesCount)).EndInit();
            ((ISupportInitialize)(this.numericUpDownSocialPremisesCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            ((ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((ISupportInitialize)(this.numericUpDownMunicipalArea)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((ISupportInitialize)(this.dataGridViewRestrictions)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((ISupportInitialize)(this.dataGridViewOwnerships)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
