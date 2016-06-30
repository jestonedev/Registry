using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class PremisesViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;
        private GroupBox groupBox8;
        private GroupBox groupBox9;
        private GroupBox groupBox10;
        private GroupBox groupBox11;
        private GroupBox groupBox13;
        private GroupBox groupBoxRooms;
        private Panel panel3 = new Panel();
        private Panel panel4 = new Panel();
        private DataGridView dataGridViewRestrictions;
        private DataGridView dataGridViewOwnerships;
        private DataGridView dataGridViewRooms;
        private NumericUpDown numericUpDownFloor;
        private NumericUpDown numericUpDownBalanceCost;
        private NumericUpDown numericUpDownCadastralCost;
        private NumericUpDown numericUpDownLivingArea;
        private NumericUpDown numericUpDownTotalArea;
        private NumericUpDown numericUpDownNumBeds;
        private Label label19;
        private Label label20;
        private Label label21;
        private Label label22;
        private Label label23;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label29;
        private Label label38;
        private Label label39;
        private ComboBox comboBoxHouse;
        private ComboBox comboBoxStreet;
        private TextBox textBoxPremisesNumber;
        private TextBox textBoxDescription;
        private TextBox textBoxSubPremisesNumber;
        private TextBox textBoxCadastralNum;
        private ComboBox comboBoxPremisesType;
        private ComboBox comboBoxPremisesKind;
        private ComboBox comboBoxCurrentFundType;
        private ComboBox comboBoxState;
        private Label label1;
        private NumericUpDown numericUpDownNumRooms;
        private NumericUpDown numericUpDownMunicipalArea;
        private Label label2;
        private NumericUpDown numericUpDownHeight;
        private Label label3;
        private DateTimePicker dateTimePickerRegDate;
        private Label label4;
        private CheckBox checkBoxIsMemorial;
        private Panel panel1;
        private VIBlend.WinForms.Controls.vButton vButtonRestrictionAdd;
        private VIBlend.WinForms.Controls.vButton vButtonRestrictionEdit;
        private VIBlend.WinForms.Controls.vButton vButtonRestrictionDelete;
        private Panel panel2;
        private VIBlend.WinForms.Controls.vButton vButtonOwnershipEdit;
        private VIBlend.WinForms.Controls.vButton vButtonOwnershipDelete;
        private VIBlend.WinForms.Controls.vButton vButtonOwnershipAdd;
        private Panel panel5;
        private VIBlend.WinForms.Controls.vButton vButtonRoomEdit;
        private VIBlend.WinForms.Controls.vButton vButtonRoomDelete;
        private VIBlend.WinForms.Controls.vButton vButtonRoomAdd;
        private DataGridViewTextBoxColumn id_restriction;
        private DataGridViewTextBoxColumn restriction_number;
        private DataGridViewTextBoxColumn restriction_date;
        private DataGridViewTextBoxColumn restriction_description;
        private DataGridViewComboBoxColumn id_restriction_type;
        private DataGridViewTextBoxColumn restriction_relation;
        private DataGridViewTextBoxColumn id_ownership_right;
        private DataGridViewTextBoxColumn ownership_number;
        private DataGridViewTextBoxColumn ownership_date;
        private DataGridViewTextBoxColumn ownership_description;
        private DataGridViewComboBoxColumn id_ownership_type;
        private DataGridViewTextBoxColumn ownership_relation;
        private TextBox textBoxAccount;
        private Label label5;
        private DateTimePicker dateTimePickerStateDate;
        private Label label6;
        #endregion Components


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PremisesViewport));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label29 = new System.Windows.Forms.Label();
            this.textBoxSubPremisesNumber = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.vButtonRestrictionEdit = new VIBlend.WinForms.Controls.vButton();
            this.vButtonRestrictionDelete = new VIBlend.WinForms.Controls.vButton();
            this.vButtonRestrictionAdd = new VIBlend.WinForms.Controls.vButton();
            this.dataGridViewRestrictions = new System.Windows.Forms.DataGridView();
            this.id_restriction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restriction_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_restriction_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.restriction_relation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.vButtonOwnershipEdit = new VIBlend.WinForms.Controls.vButton();
            this.vButtonOwnershipDelete = new VIBlend.WinForms.Controls.vButton();
            this.vButtonOwnershipAdd = new VIBlend.WinForms.Controls.vButton();
            this.dataGridViewOwnerships = new System.Windows.Forms.DataGridView();
            this.id_ownership_right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownership_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_ownership_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ownership_relation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dateTimePickerRegDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPremisesKind = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.numericUpDownNumRooms = new System.Windows.Forms.NumericUpDown();
            this.comboBoxPremisesType = new System.Windows.Forms.ComboBox();
            this.textBoxPremisesNumber = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.numericUpDownNumBeds = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownFloor = new System.Windows.Forms.NumericUpDown();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBoxHouse = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dateTimePickerStateDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxIsMemorial = new System.Windows.Forms.CheckBox();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.label38 = new System.Windows.Forms.Label();
            this.comboBoxCurrentFundType = new System.Windows.Forms.ComboBox();
            this.numericUpDownBalanceCost = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.numericUpDownCadastralCost = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownMunicipalArea = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownLivingArea = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTotalArea = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBoxRooms = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.vButtonRoomEdit = new VIBlend.WinForms.Controls.vButton();
            this.vButtonRoomDelete = new VIBlend.WinForms.Controls.vButton();
            this.vButtonRoomAdd = new VIBlend.WinForms.Controls.vButton();
            this.dataGridViewRooms = new System.Windows.Forms.DataGridView();
            this.sub_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_id_state = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.current_fund = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_cadastral_cost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_balance_cost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumRooms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumBeds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            this.groupBoxRooms.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).BeginInit();
            this.SuspendLayout();
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(10, 97);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(53, 13);
            this.label29.TabIndex = 0;
            this.label29.Text = "Комнаты";
            // 
            // textBoxSubPremisesNumber
            // 
            this.textBoxSubPremisesNumber.Location = new System.Drawing.Point(0, 0);
            this.textBoxSubPremisesNumber.Name = "textBoxSubPremisesNumber";
            this.textBoxSubPremisesNumber.Size = new System.Drawing.Size(100, 20);
            this.textBoxSubPremisesNumber.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox13, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupBox9, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox10, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.groupBox8, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBoxRooms, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 238F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(918, 665);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.textBoxDescription);
            this.groupBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox13.Location = new System.Drawing.Point(3, 381);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(453, 74);
            this.groupBox13.TabIndex = 4;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxDescription.MaxLength = 65535;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(447, 54);
            this.textBoxDescription.TabIndex = 0;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.panel1);
            this.groupBox9.Controls.Add(this.dataGridViewRestrictions);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(3, 461);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(453, 201);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Реквизиты";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.vButtonRestrictionEdit);
            this.panel1.Controls.Add(this.vButtonRestrictionDelete);
            this.panel1.Controls.Add(this.vButtonRestrictionAdd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(412, 17);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(38, 181);
            this.panel1.TabIndex = 1;
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
            this.id_restriction,
            this.restriction_number,
            this.restriction_date,
            this.restriction_description,
            this.id_restriction_type,
            this.restriction_relation});
            this.dataGridViewRestrictions.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewRestrictions.Name = "dataGridViewRestrictions";
            this.dataGridViewRestrictions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRestrictions.Size = new System.Drawing.Size(408, 181);
            this.dataGridViewRestrictions.TabIndex = 0;
            this.dataGridViewRestrictions.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRestrictions_CellDoubleClick);
            this.dataGridViewRestrictions.Resize += new System.EventHandler(this.dataGridViewRestrictions_Resize);
            // 
            // id_restriction
            // 
            this.id_restriction.HeaderText = "Идентификатор";
            this.id_restriction.Name = "id_restriction";
            this.id_restriction.Visible = false;
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
            // restriction_relation
            // 
            this.restriction_relation.HeaderText = "Принадлежность";
            this.restriction_relation.MinimumWidth = 150;
            this.restriction_relation.Name = "restriction_relation";
            this.restriction_relation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.restriction_relation.Width = 150;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.panel2);
            this.groupBox10.Controls.Add(this.dataGridViewOwnerships);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Location = new System.Drawing.Point(462, 461);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(453, 201);
            this.groupBox10.TabIndex = 6;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Ограничения";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.vButtonOwnershipEdit);
            this.panel2.Controls.Add(this.vButtonOwnershipDelete);
            this.panel2.Controls.Add(this.vButtonOwnershipAdd);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(412, 17);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(38, 181);
            this.panel2.TabIndex = 2;
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
            this.id_ownership_right,
            this.ownership_number,
            this.ownership_date,
            this.ownership_description,
            this.id_ownership_type,
            this.ownership_relation});
            this.dataGridViewOwnerships.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewOwnerships.Name = "dataGridViewOwnerships";
            this.dataGridViewOwnerships.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOwnerships.Size = new System.Drawing.Size(408, 181);
            this.dataGridViewOwnerships.TabIndex = 0;
            this.dataGridViewOwnerships.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOwnerships_CellDoubleClick);
            this.dataGridViewOwnerships.Resize += new System.EventHandler(this.dataGridViewOwnerships_Resize);
            // 
            // id_ownership_right
            // 
            this.id_ownership_right.HeaderText = "Идентификатор";
            this.id_ownership_right.Name = "id_ownership_right";
            this.id_ownership_right.Visible = false;
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
            // ownership_relation
            // 
            this.ownership_relation.HeaderText = "Принадлежность";
            this.ownership_relation.MinimumWidth = 150;
            this.ownership_relation.Name = "ownership_relation";
            this.ownership_relation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ownership_relation.Width = 150;
            // 
            // groupBox8
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox8, 2);
            this.groupBox8.Controls.Add(this.tableLayoutPanel4);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(912, 232);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Общие сведения";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel4, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(906, 212);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dateTimePickerRegDate);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.comboBoxPremisesKind);
            this.panel3.Controls.Add(this.label28);
            this.panel3.Controls.Add(this.numericUpDownNumRooms);
            this.panel3.Controls.Add(this.comboBoxPremisesType);
            this.panel3.Controls.Add(this.textBoxPremisesNumber);
            this.panel3.Controls.Add(this.label27);
            this.panel3.Controls.Add(this.label21);
            this.panel3.Controls.Add(this.numericUpDownNumBeds);
            this.panel3.Controls.Add(this.numericUpDownFloor);
            this.panel3.Controls.Add(this.comboBoxStreet);
            this.panel3.Controls.Add(this.label20);
            this.panel3.Controls.Add(this.comboBoxHouse);
            this.panel3.Controls.Add(this.label19);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(447, 206);
            this.panel3.TabIndex = 1;
            // 
            // dateTimePickerRegDate
            // 
            this.dateTimePickerRegDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerRegDate.Location = new System.Drawing.Point(169, 179);
            this.dateTimePickerRegDate.Name = "dateTimePickerRegDate";
            this.dateTimePickerRegDate.Size = new System.Drawing.Size(272, 21);
            this.dateTimePickerRegDate.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Дата включения в РМИ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Количество комнат";
            // 
            // comboBoxPremisesKind
            // 
            this.comboBoxPremisesKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPremisesKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPremisesKind.FormattingEnabled = true;
            this.comboBoxPremisesKind.Location = new System.Drawing.Point(169, 206);
            this.comboBoxPremisesKind.Name = "comboBoxPremisesKind";
            this.comboBoxPremisesKind.Size = new System.Drawing.Size(272, 23);
            this.comboBoxPremisesKind.TabIndex = 4;
            this.comboBoxPremisesKind.Visible = false;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(10, 210);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(99, 15);
            this.label28.TabIndex = 5;
            this.label28.Text = "Вид помещения";
            this.label28.Visible = false;
            // 
            // numericUpDownNumRooms
            // 
            this.numericUpDownNumRooms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownNumRooms.Location = new System.Drawing.Point(169, 123);
            this.numericUpDownNumRooms.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownNumRooms.Name = "numericUpDownNumRooms";
            this.numericUpDownNumRooms.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownNumRooms.TabIndex = 5;
            // 
            // comboBoxPremisesType
            // 
            this.comboBoxPremisesType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPremisesType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxPremisesType.FormattingEnabled = true;
            this.comboBoxPremisesType.Items.AddRange(new object[] {
            "Номер квартиры"});
            this.comboBoxPremisesType.Location = new System.Drawing.Point(9, 66);
            this.comboBoxPremisesType.Name = "comboBoxPremisesType";
            this.comboBoxPremisesType.Size = new System.Drawing.Size(154, 23);
            this.comboBoxPremisesType.TabIndex = 2;
            // 
            // textBoxPremisesNumber
            // 
            this.textBoxPremisesNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPremisesNumber.Location = new System.Drawing.Point(169, 67);
            this.textBoxPremisesNumber.MaxLength = 255;
            this.textBoxPremisesNumber.Name = "textBoxPremisesNumber";
            this.textBoxPremisesNumber.Size = new System.Drawing.Size(272, 21);
            this.textBoxPremisesNumber.TabIndex = 3;
            this.textBoxPremisesNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPremisesNumber_KeyPress);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(10, 154);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(145, 15);
            this.label27.TabIndex = 4;
            this.label27.Text = "Количество койко-мест";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 98);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(38, 15);
            this.label21.TabIndex = 6;
            this.label21.Text = "Этаж";
            // 
            // numericUpDownNumBeds
            // 
            this.numericUpDownNumBeds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownNumBeds.Location = new System.Drawing.Point(169, 151);
            this.numericUpDownNumBeds.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownNumBeds.Name = "numericUpDownNumBeds";
            this.numericUpDownNumBeds.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownNumBeds.TabIndex = 6;
            // 
            // numericUpDownFloor
            // 
            this.numericUpDownFloor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownFloor.Location = new System.Drawing.Point(169, 95);
            this.numericUpDownFloor.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownFloor.Name = "numericUpDownFloor";
            this.numericUpDownFloor.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownFloor.TabIndex = 4;
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxStreet.Location = new System.Drawing.Point(169, 7);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(272, 23);
            this.comboBoxStreet.TabIndex = 0;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.VisibleChanged += new System.EventHandler(this.comboBoxStreet_VisibleChanged);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 41);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(79, 15);
            this.label20.TabIndex = 7;
            this.label20.Text = "Номер дома";
            // 
            // comboBoxHouse
            // 
            this.comboBoxHouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxHouse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHouse.Location = new System.Drawing.Point(169, 37);
            this.comboBoxHouse.Name = "comboBoxHouse";
            this.comboBoxHouse.Size = new System.Drawing.Size(272, 23);
            this.comboBoxHouse.TabIndex = 1;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 11);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(43, 15);
            this.label19.TabIndex = 8;
            this.label19.Text = "Улица";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dateTimePickerStateDate);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.checkBoxIsMemorial);
            this.panel4.Controls.Add(this.textBoxAccount);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.label39);
            this.panel4.Controls.Add(this.comboBoxState);
            this.panel4.Controls.Add(this.label38);
            this.panel4.Controls.Add(this.comboBoxCurrentFundType);
            this.panel4.Controls.Add(this.numericUpDownBalanceCost);
            this.panel4.Controls.Add(this.label22);
            this.panel4.Controls.Add(this.textBoxCadastralNum);
            this.panel4.Controls.Add(this.numericUpDownCadastralCost);
            this.panel4.Controls.Add(this.label23);
            this.panel4.Controls.Add(this.label24);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(456, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(447, 206);
            this.panel4.TabIndex = 2;
            // 
            // dateTimePickerStateDate
            // 
            this.dateTimePickerStateDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStateDate.Location = new System.Drawing.Point(170, 206);
            this.dateTimePickerStateDate.Name = "dateTimePickerStateDate";
            this.dateTimePickerStateDate.ShowCheckBox = true;
            this.dateTimePickerStateDate.Size = new System.Drawing.Size(272, 21);
            this.dateTimePickerStateDate.TabIndex = 6;
            this.dateTimePickerStateDate.TabStop = false;
            this.dateTimePickerStateDate.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 209);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(147, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "Состояние установлено";
            this.label6.Visible = false;
            // 
            // checkBoxIsMemorial
            // 
            this.checkBoxIsMemorial.AutoSize = true;
            this.checkBoxIsMemorial.Location = new System.Drawing.Point(19, 181);
            this.checkBoxIsMemorial.Name = "checkBoxIsMemorial";
            this.checkBoxIsMemorial.Size = new System.Drawing.Size(141, 19);
            this.checkBoxIsMemorial.TabIndex = 8;
            this.checkBoxIsMemorial.Text = "Памятник культуры";
            this.checkBoxIsMemorial.UseVisualStyleBackColor = true;
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAccount.Location = new System.Drawing.Point(170, 38);
            this.textBoxAccount.MaxLength = 20;
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(272, 21);
            this.textBoxAccount.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 15);
            this.label5.TabIndex = 13;
            this.label5.Text = "Лицевой счет ФКР";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(16, 127);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(119, 15);
            this.label39.TabIndex = 0;
            this.label39.Text = "Текущее состояние";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.FormattingEnabled = true;
            this.comboBoxState.Location = new System.Drawing.Point(170, 123);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(272, 23);
            this.comboBoxState.TabIndex = 5;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(16, 154);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(90, 15);
            this.label38.TabIndex = 2;
            this.label38.Text = "Текущий фонд";
            // 
            // comboBoxCurrentFundType
            // 
            this.comboBoxCurrentFundType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCurrentFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurrentFundType.Enabled = false;
            this.comboBoxCurrentFundType.ForeColor = System.Drawing.Color.Black;
            this.comboBoxCurrentFundType.FormattingEnabled = true;
            this.comboBoxCurrentFundType.Location = new System.Drawing.Point(170, 151);
            this.comboBoxCurrentFundType.Name = "comboBoxCurrentFundType";
            this.comboBoxCurrentFundType.Size = new System.Drawing.Size(272, 23);
            this.comboBoxCurrentFundType.TabIndex = 7;
            // 
            // numericUpDownBalanceCost
            // 
            this.numericUpDownBalanceCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownBalanceCost.DecimalPlaces = 2;
            this.numericUpDownBalanceCost.Location = new System.Drawing.Point(170, 95);
            this.numericUpDownBalanceCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            this.numericUpDownBalanceCost.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownBalanceCost.TabIndex = 3;
            this.numericUpDownBalanceCost.ThousandsSeparator = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(16, 98);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(143, 15);
            this.label22.TabIndex = 9;
            this.label22.Text = "Балансовая стоимость";
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCadastralNum.Location = new System.Drawing.Point(170, 8);
            this.textBoxCadastralNum.MaxLength = 20;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(272, 21);
            this.textBoxCadastralNum.TabIndex = 0;
            // 
            // numericUpDownCadastralCost
            // 
            this.numericUpDownCadastralCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownCadastralCost.DecimalPlaces = 2;
            this.numericUpDownCadastralCost.Location = new System.Drawing.Point(170, 68);
            this.numericUpDownCadastralCost.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            this.numericUpDownCadastralCost.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownCadastralCost.TabIndex = 2;
            this.numericUpDownCadastralCost.ThousandsSeparator = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(16, 11);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(126, 15);
            this.label23.TabIndex = 10;
            this.label23.Text = "Кадастровый номер";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(16, 70);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(150, 15);
            this.label24.TabIndex = 11;
            this.label24.Text = "Кадастровая стоимость";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Controls.Add(this.groupBox11, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 241);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(453, 134);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.numericUpDownHeight);
            this.groupBox11.Controls.Add(this.label3);
            this.groupBox11.Controls.Add(this.numericUpDownMunicipalArea);
            this.groupBox11.Controls.Add(this.label2);
            this.groupBox11.Controls.Add(this.numericUpDownLivingArea);
            this.groupBox11.Controls.Add(this.numericUpDownTotalArea);
            this.groupBox11.Controls.Add(this.label25);
            this.groupBox11.Controls.Add(this.label26);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(0, 0);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(453, 134);
            this.groupBox11.TabIndex = 1;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Геометрия помещения";
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownHeight.DecimalPlaces = 3;
            this.numericUpDownHeight.Location = new System.Drawing.Point(175, 105);
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownHeight.TabIndex = 3;
            this.numericUpDownHeight.ThousandsSeparator = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 15);
            this.label3.TabIndex = 17;
            this.label3.Text = "Высота помещения";
            // 
            // numericUpDownMunicipalArea
            // 
            this.numericUpDownMunicipalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownMunicipalArea.DecimalPlaces = 3;
            this.numericUpDownMunicipalArea.Location = new System.Drawing.Point(175, 76);
            this.numericUpDownMunicipalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownMunicipalArea.Name = "numericUpDownMunicipalArea";
            this.numericUpDownMunicipalArea.ReadOnly = true;
            this.numericUpDownMunicipalArea.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownMunicipalArea.TabIndex = 2;
            this.numericUpDownMunicipalArea.ThousandsSeparator = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 15);
            this.label2.TabIndex = 15;
            this.label2.Text = "Площадь мун. комнат";
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new System.Drawing.Point(175, 47);
            this.numericUpDownLivingArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            this.numericUpDownLivingArea.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownLivingArea.TabIndex = 1;
            this.numericUpDownLivingArea.ThousandsSeparator = true;
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new System.Drawing.Point(175, 18);
            this.numericUpDownTotalArea.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(272, 21);
            this.numericUpDownTotalArea.TabIndex = 0;
            this.numericUpDownTotalArea.ThousandsSeparator = true;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(16, 50);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(100, 15);
            this.label25.TabIndex = 12;
            this.label25.Text = "Жилая площадь";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(16, 21);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(100, 15);
            this.label26.TabIndex = 13;
            this.label26.Text = "Общая площадь";
            // 
            // groupBoxRooms
            // 
            this.groupBoxRooms.Controls.Add(this.panel5);
            this.groupBoxRooms.Controls.Add(this.dataGridViewRooms);
            this.groupBoxRooms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRooms.Location = new System.Drawing.Point(462, 241);
            this.groupBoxRooms.Name = "groupBoxRooms";
            this.tableLayoutPanel3.SetRowSpan(this.groupBoxRooms, 2);
            this.groupBoxRooms.Size = new System.Drawing.Size(453, 214);
            this.groupBoxRooms.TabIndex = 3;
            this.groupBoxRooms.TabStop = false;
            this.groupBoxRooms.Text = "Комнаты";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.vButtonRoomEdit);
            this.panel5.Controls.Add(this.vButtonRoomDelete);
            this.panel5.Controls.Add(this.vButtonRoomAdd);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(412, 17);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(38, 194);
            this.panel5.TabIndex = 3;
            // 
            // vButtonRoomEdit
            // 
            this.vButtonRoomEdit.AllowAnimations = true;
            this.vButtonRoomEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonRoomEdit.BackColor = System.Drawing.Color.Transparent;
            this.vButtonRoomEdit.Image = ((System.Drawing.Image)(resources.GetObject("vButtonRoomEdit.Image")));
            this.vButtonRoomEdit.Location = new System.Drawing.Point(3, 57);
            this.vButtonRoomEdit.Name = "vButtonRoomEdit";
            this.vButtonRoomEdit.RoundedCornersMask = ((byte)(15));
            this.vButtonRoomEdit.Size = new System.Drawing.Size(32, 25);
            this.vButtonRoomEdit.TabIndex = 2;
            this.vButtonRoomEdit.UseVisualStyleBackColor = false;
            this.vButtonRoomEdit.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRoomEdit.Click += new System.EventHandler(this.vButtonSubPremisesEdit_Click);
            // 
            // vButtonRoomDelete
            // 
            this.vButtonRoomDelete.AllowAnimations = true;
            this.vButtonRoomDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonRoomDelete.BackColor = System.Drawing.Color.Transparent;
            this.vButtonRoomDelete.Image = ((System.Drawing.Image)(resources.GetObject("vButtonRoomDelete.Image")));
            this.vButtonRoomDelete.Location = new System.Drawing.Point(3, 30);
            this.vButtonRoomDelete.Name = "vButtonRoomDelete";
            this.vButtonRoomDelete.RoundedCornersMask = ((byte)(15));
            this.vButtonRoomDelete.Size = new System.Drawing.Size(32, 25);
            this.vButtonRoomDelete.TabIndex = 1;
            this.vButtonRoomDelete.UseVisualStyleBackColor = false;
            this.vButtonRoomDelete.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRoomDelete.Click += new System.EventHandler(this.vButtonSubPremisesDelete_Click);
            // 
            // vButtonRoomAdd
            // 
            this.vButtonRoomAdd.AllowAnimations = true;
            this.vButtonRoomAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonRoomAdd.BackColor = System.Drawing.Color.Transparent;
            this.vButtonRoomAdd.Image = ((System.Drawing.Image)(resources.GetObject("vButtonRoomAdd.Image")));
            this.vButtonRoomAdd.Location = new System.Drawing.Point(3, 3);
            this.vButtonRoomAdd.Name = "vButtonRoomAdd";
            this.vButtonRoomAdd.RoundedCornersMask = ((byte)(15));
            this.vButtonRoomAdd.Size = new System.Drawing.Size(32, 25);
            this.vButtonRoomAdd.TabIndex = 0;
            this.vButtonRoomAdd.UseVisualStyleBackColor = false;
            this.vButtonRoomAdd.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonRoomAdd.Click += new System.EventHandler(this.vButtonSubPremisesAdd_Click);
            // 
            // dataGridViewRooms
            // 
            this.dataGridViewRooms.AllowUserToAddRows = false;
            this.dataGridViewRooms.AllowUserToDeleteRows = false;
            this.dataGridViewRooms.AllowUserToResizeRows = false;
            this.dataGridViewRooms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewRooms.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRooms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sub_premises_num,
            this.sub_premises_total_area,
            this.sub_premises_id_state,
            this.current_fund,
            this.sub_premises_cadastral_num,
            this.sub_premises_cadastral_cost,
            this.sub_premises_balance_cost,
            this.sub_premises_account});
            this.dataGridViewRooms.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewRooms.Name = "dataGridViewRooms";
            this.dataGridViewRooms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRooms.Size = new System.Drawing.Size(408, 193);
            this.dataGridViewRooms.TabIndex = 0;
            this.dataGridViewRooms.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRooms_CellDoubleClick);
            this.dataGridViewRooms.Resize += new System.EventHandler(this.dataGridViewRooms_Resize);
            // 
            // sub_premises_num
            // 
            this.sub_premises_num.HeaderText = "Номер";
            this.sub_premises_num.MinimumWidth = 100;
            this.sub_premises_num.Name = "sub_premises_num";
            this.sub_premises_num.ReadOnly = true;
            // 
            // sub_premises_total_area
            // 
            dataGridViewCellStyle1.Format = "#0.0## м²";
            this.sub_premises_total_area.DefaultCellStyle = dataGridViewCellStyle1;
            this.sub_premises_total_area.HeaderText = "Общая площадь";
            this.sub_premises_total_area.MinimumWidth = 100;
            this.sub_premises_total_area.Name = "sub_premises_total_area";
            this.sub_premises_total_area.ReadOnly = true;
            // 
            // sub_premises_id_state
            // 
            this.sub_premises_id_state.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.sub_premises_id_state.HeaderText = "Текущее состояние";
            this.sub_premises_id_state.MinimumWidth = 150;
            this.sub_premises_id_state.Name = "sub_premises_id_state";
            this.sub_premises_id_state.ReadOnly = true;
            this.sub_premises_id_state.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.sub_premises_id_state.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.sub_premises_id_state.Width = 150;
            // 
            // current_fund
            // 
            this.current_fund.HeaderText = "Текущий фонд";
            this.current_fund.MinimumWidth = 150;
            this.current_fund.Name = "current_fund";
            this.current_fund.Width = 150;
            // 
            // sub_premises_cadastral_num
            // 
            this.sub_premises_cadastral_num.HeaderText = "Кадастровый номер";
            this.sub_premises_cadastral_num.MinimumWidth = 100;
            this.sub_premises_cadastral_num.Name = "sub_premises_cadastral_num";
            // 
            // sub_premises_cadastral_cost
            // 
            this.sub_premises_cadastral_cost.HeaderText = "Кадастровая стоимость";
            this.sub_premises_cadastral_cost.MinimumWidth = 100;
            this.sub_premises_cadastral_cost.Name = "sub_premises_cadastral_cost";
            // 
            // sub_premises_balance_cost
            // 
            this.sub_premises_balance_cost.HeaderText = "Балансовая стоимость";
            this.sub_premises_balance_cost.MinimumWidth = 100;
            this.sub_premises_balance_cost.Name = "sub_premises_balance_cost";
            // 
            // sub_premises_account
            // 
            this.sub_premises_account.HeaderText = "Лицевой счет ФКР";
            this.sub_premises_account.MinimumWidth = 100;
            this.sub_premises_account.Name = "sub_premises_account";
            // 
            // PremisesViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(665, 580);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(924, 671);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PremisesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Помещение";
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRestrictions)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOwnerships)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumRooms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumBeds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMunicipalArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            this.groupBoxRooms.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRooms)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn sub_premises_total_area;
        private DataGridViewComboBoxColumn sub_premises_id_state;
        private DataGridViewTextBoxColumn current_fund;
        private DataGridViewTextBoxColumn sub_premises_cadastral_num;
        private DataGridViewTextBoxColumn sub_premises_cadastral_cost;
        private DataGridViewTextBoxColumn sub_premises_balance_cost;
        private DataGridViewTextBoxColumn sub_premises_account;
    }
}
