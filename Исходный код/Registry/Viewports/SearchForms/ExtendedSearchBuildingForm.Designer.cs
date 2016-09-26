using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.SearchForms
{
    partial class ExtendedSearchBuildingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedSearchBuildingForm));
            this.numericUpDownIDBuilding = new System.Windows.Forms.NumericUpDown();
            this.checkBoxIDBuildingEnable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxRegion = new System.Windows.Forms.ComboBox();
            this.checkBoxRegionEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxStateEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxImprovementEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxImprovement = new System.Windows.Forms.CheckBox();
            this.checkBoxElevatorEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxElevator = new System.Windows.Forms.CheckBox();
            this.comboBoxStructureType = new System.Windows.Forms.ComboBox();
            this.checkBoxStructureTypeEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxStartupYearEnable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownStartupYear = new System.Windows.Forms.NumericUpDown();
            this.checkBoxCadastralNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxFloorsEnable = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxHouseEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxStreetEnable = new System.Windows.Forms.CheckBox();
            this.numericUpDownFloors = new System.Windows.Forms.NumericUpDown();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.comboBoxOwnershipType = new System.Windows.Forms.ComboBox();
            this.checkBoxOwnershipTypeEnable = new System.Windows.Forms.CheckBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxRestrictionNumber = new System.Windows.Forms.TextBox();
            this.checkBoxRestrictionNumberEnable = new System.Windows.Forms.CheckBox();
            this.comboBoxRestrictionType = new System.Windows.Forms.ComboBox();
            this.checkBoxRestrictionTypeEnable = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxOwnershipNumber = new System.Windows.Forms.TextBox();
            this.checkBoxOwnershipNumberEnable = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDBuilding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownIDBuilding
            // 
            this.numericUpDownIDBuilding.Enabled = false;
            this.numericUpDownIDBuilding.Location = new System.Drawing.Point(42, 26);
            this.numericUpDownIDBuilding.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownIDBuilding.Name = "numericUpDownIDBuilding";
            this.numericUpDownIDBuilding.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownIDBuilding.TabIndex = 1;
            this.numericUpDownIDBuilding.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxIDBuildingEnable
            // 
            this.checkBoxIDBuildingEnable.AutoSize = true;
            this.checkBoxIDBuildingEnable.Location = new System.Drawing.Point(17, 29);
            this.checkBoxIDBuildingEnable.Name = "checkBoxIDBuildingEnable";
            this.checkBoxIDBuildingEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIDBuildingEnable.TabIndex = 0;
            this.checkBoxIDBuildingEnable.UseVisualStyleBackColor = true;
            this.checkBoxIDBuildingEnable.CheckedChanged += new System.EventHandler(this.checkBoxIDBuildingEnable_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(163, 15);
            this.label9.TabIndex = 27;
            this.label9.Text = "Реестровый номер здания";
            // 
            // comboBoxRegion
            // 
            this.comboBoxRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegion.Enabled = false;
            this.comboBoxRegion.FormattingEnabled = true;
            this.comboBoxRegion.Location = new System.Drawing.Point(42, 67);
            this.comboBoxRegion.Name = "comboBoxRegion";
            this.comboBoxRegion.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRegion.TabIndex = 3;
            // 
            // checkBoxRegionEnable
            // 
            this.checkBoxRegionEnable.AutoSize = true;
            this.checkBoxRegionEnable.Location = new System.Drawing.Point(17, 71);
            this.checkBoxRegionEnable.Name = "checkBoxRegionEnable";
            this.checkBoxRegionEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegionEnable.TabIndex = 2;
            this.checkBoxRegionEnable.UseVisualStyleBackColor = true;
            this.checkBoxRegionEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegionEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 15);
            this.label8.TabIndex = 24;
            this.label8.Text = "Жилой район";
            // 
            // checkBoxStateEnable
            // 
            this.checkBoxStateEnable.AutoSize = true;
            this.checkBoxStateEnable.Location = new System.Drawing.Point(494, 29);
            this.checkBoxStateEnable.Name = "checkBoxStateEnable";
            this.checkBoxStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStateEnable.TabIndex = 20;
            this.checkBoxStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxStateEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(491, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "Состояние здания";
            // 
            // checkBoxImprovementEnable
            // 
            this.checkBoxImprovementEnable.AutoSize = true;
            this.checkBoxImprovementEnable.Location = new System.Drawing.Point(17, 375);
            this.checkBoxImprovementEnable.Name = "checkBoxImprovementEnable";
            this.checkBoxImprovementEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxImprovementEnable.TabIndex = 18;
            this.checkBoxImprovementEnable.UseVisualStyleBackColor = true;
            this.checkBoxImprovementEnable.CheckedChanged += new System.EventHandler(this.checkBoxImprovementEnable_CheckedChanged);
            // 
            // checkBoxImprovement
            // 
            this.checkBoxImprovement.AutoSize = true;
            this.checkBoxImprovement.Enabled = false;
            this.checkBoxImprovement.Location = new System.Drawing.Point(41, 373);
            this.checkBoxImprovement.Name = "checkBoxImprovement";
            this.checkBoxImprovement.Size = new System.Drawing.Size(126, 19);
            this.checkBoxImprovement.TabIndex = 19;
            this.checkBoxImprovement.Text = "Благоустройство";
            this.checkBoxImprovement.UseVisualStyleBackColor = true;
            // 
            // checkBoxElevatorEnable
            // 
            this.checkBoxElevatorEnable.AutoSize = true;
            this.checkBoxElevatorEnable.Location = new System.Drawing.Point(17, 350);
            this.checkBoxElevatorEnable.Name = "checkBoxElevatorEnable";
            this.checkBoxElevatorEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxElevatorEnable.TabIndex = 16;
            this.checkBoxElevatorEnable.UseVisualStyleBackColor = true;
            this.checkBoxElevatorEnable.CheckedChanged += new System.EventHandler(this.checkBoxElevatorEnable_CheckedChanged);
            // 
            // checkBoxElevator
            // 
            this.checkBoxElevator.AutoSize = true;
            this.checkBoxElevator.Enabled = false;
            this.checkBoxElevator.Location = new System.Drawing.Point(41, 348);
            this.checkBoxElevator.Name = "checkBoxElevator";
            this.checkBoxElevator.Size = new System.Drawing.Size(118, 19);
            this.checkBoxElevator.TabIndex = 17;
            this.checkBoxElevator.Text = "Наличие лифта";
            this.checkBoxElevator.UseVisualStyleBackColor = true;
            // 
            // comboBoxStructureType
            // 
            this.comboBoxStructureType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStructureType.Enabled = false;
            this.comboBoxStructureType.FormattingEnabled = true;
            this.comboBoxStructureType.Location = new System.Drawing.Point(41, 317);
            this.comboBoxStructureType.Name = "comboBoxStructureType";
            this.comboBoxStructureType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStructureType.TabIndex = 15;
            // 
            // checkBoxStructureTypeEnable
            // 
            this.checkBoxStructureTypeEnable.AutoSize = true;
            this.checkBoxStructureTypeEnable.Location = new System.Drawing.Point(17, 321);
            this.checkBoxStructureTypeEnable.Name = "checkBoxStructureTypeEnable";
            this.checkBoxStructureTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStructureTypeEnable.TabIndex = 14;
            this.checkBoxStructureTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxStructureTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxFundTypeEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 300);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "Тип строения";
            // 
            // checkBoxStartupYearEnable
            // 
            this.checkBoxStartupYearEnable.AutoSize = true;
            this.checkBoxStartupYearEnable.Location = new System.Drawing.Point(17, 279);
            this.checkBoxStartupYearEnable.Name = "checkBoxStartupYearEnable";
            this.checkBoxStartupYearEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStartupYearEnable.TabIndex = 12;
            this.checkBoxStartupYearEnable.UseVisualStyleBackColor = true;
            this.checkBoxStartupYearEnable.CheckedChanged += new System.EventHandler(this.checkBoxStartupYearEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 15);
            this.label5.TabIndex = 15;
            this.label5.Text = "Год ввода в эксплуатацию";
            // 
            // numericUpDownStartupYear
            // 
            this.numericUpDownStartupYear.Enabled = false;
            this.numericUpDownStartupYear.Location = new System.Drawing.Point(42, 276);
            this.numericUpDownStartupYear.Maximum = new decimal(new int[] {
            1900,
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
            this.numericUpDownStartupYear.TabIndex = 13;
            this.numericUpDownStartupYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            this.numericUpDownStartupYear.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxCadastralNumEnable
            // 
            this.checkBoxCadastralNumEnable.AutoSize = true;
            this.checkBoxCadastralNumEnable.Location = new System.Drawing.Point(17, 238);
            this.checkBoxCadastralNumEnable.Name = "checkBoxCadastralNumEnable";
            this.checkBoxCadastralNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCadastralNumEnable.TabIndex = 10;
            this.checkBoxCadastralNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxCadastralNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxCadastralNumEnable_CheckedChanged);
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Enabled = false;
            this.textBoxCadastralNum.Location = new System.Drawing.Point(42, 235);
            this.textBoxCadastralNum.MaxLength = 20;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxCadastralNum.TabIndex = 11;
            this.textBoxCadastralNum.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 218);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Кадастровый номер";
            // 
            // checkBoxFloorsEnable
            // 
            this.checkBoxFloorsEnable.AutoSize = true;
            this.checkBoxFloorsEnable.Location = new System.Drawing.Point(17, 197);
            this.checkBoxFloorsEnable.Name = "checkBoxFloorsEnable";
            this.checkBoxFloorsEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFloorsEnable.TabIndex = 8;
            this.checkBoxFloorsEnable.UseVisualStyleBackColor = true;
            this.checkBoxFloorsEnable.CheckedChanged += new System.EventHandler(this.checkBoxFloorsEnable_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Этажность";
            // 
            // checkBoxHouseEnable
            // 
            this.checkBoxHouseEnable.AutoSize = true;
            this.checkBoxHouseEnable.Location = new System.Drawing.Point(17, 156);
            this.checkBoxHouseEnable.Name = "checkBoxHouseEnable";
            this.checkBoxHouseEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseEnable.TabIndex = 6;
            this.checkBoxHouseEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // checkBoxStreetEnable
            // 
            this.checkBoxStreetEnable.AutoSize = true;
            this.checkBoxStreetEnable.Location = new System.Drawing.Point(17, 114);
            this.checkBoxStreetEnable.Name = "checkBoxStreetEnable";
            this.checkBoxStreetEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetEnable.TabIndex = 4;
            this.checkBoxStreetEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // numericUpDownFloors
            // 
            this.numericUpDownFloors.Enabled = false;
            this.numericUpDownFloors.Location = new System.Drawing.Point(42, 194);
            this.numericUpDownFloors.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownFloors.Name = "numericUpDownFloors";
            this.numericUpDownFloors.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownFloors.TabIndex = 9;
            this.numericUpDownFloors.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Enabled = false;
            this.textBoxHouse.Location = new System.Drawing.Point(42, 153);
            this.textBoxHouse.MaxLength = 4;
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(437, 21);
            this.textBoxHouse.TabIndex = 7;
            this.textBoxHouse.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Номер дома";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Улица";
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Enabled = false;
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(42, 110);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStreet.TabIndex = 5;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.Enter += new System.EventHandler(this.selectAll_Enter);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // comboBoxOwnershipType
            // 
            this.comboBoxOwnershipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOwnershipType.Enabled = false;
            this.comboBoxOwnershipType.FormattingEnabled = true;
            this.comboBoxOwnershipType.Location = new System.Drawing.Point(518, 192);
            this.comboBoxOwnershipType.Name = "comboBoxOwnershipType";
            this.comboBoxOwnershipType.Size = new System.Drawing.Size(238, 23);
            this.comboBoxOwnershipType.TabIndex = 23;
            // 
            // checkBoxOwnershipTypeEnable
            // 
            this.checkBoxOwnershipTypeEnable.AutoSize = true;
            this.checkBoxOwnershipTypeEnable.Location = new System.Drawing.Point(491, 195);
            this.checkBoxOwnershipTypeEnable.Name = "checkBoxOwnershipTypeEnable";
            this.checkBoxOwnershipTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxOwnershipTypeEnable.TabIndex = 22;
            this.checkBoxOwnershipTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxOwnershipTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxOwnershipTypeEnable_CheckedChanged);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Enabled = false;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(518, 26);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(238, 148);
            this.checkedListBox1.TabIndex = 21;
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.Location = new System.Drawing.Point(501, 353);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(117, 35);
            this.vButton1.TabIndex = 30;
            this.vButton1.Text = "Поиск";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButton1.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton2.Location = new System.Drawing.Point(639, 353);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(117, 35);
            this.vButton2.TabIndex = 31;
            this.vButton2.Text = "Отмена";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(486, 301);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 15);
            this.label14.TabIndex = 77;
            this.label14.Text = "Номер реквизита";
            // 
            // textBoxRestrictionNumber
            // 
            this.textBoxRestrictionNumber.Enabled = false;
            this.textBoxRestrictionNumber.Location = new System.Drawing.Point(518, 319);
            this.textBoxRestrictionNumber.MaxLength = 255;
            this.textBoxRestrictionNumber.Name = "textBoxRestrictionNumber";
            this.textBoxRestrictionNumber.Size = new System.Drawing.Size(238, 21);
            this.textBoxRestrictionNumber.TabIndex = 29;
            // 
            // checkBoxRestrictionNumberEnable
            // 
            this.checkBoxRestrictionNumberEnable.AutoSize = true;
            this.checkBoxRestrictionNumberEnable.Location = new System.Drawing.Point(491, 322);
            this.checkBoxRestrictionNumberEnable.Name = "checkBoxRestrictionNumberEnable";
            this.checkBoxRestrictionNumberEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRestrictionNumberEnable.TabIndex = 28;
            this.checkBoxRestrictionNumberEnable.UseVisualStyleBackColor = true;
            this.checkBoxRestrictionNumberEnable.CheckedChanged += new System.EventHandler(this.checkBoxRestrictionNumberEnable_CheckedChanged);
            // 
            // comboBoxRestrictionType
            // 
            this.comboBoxRestrictionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRestrictionType.Enabled = false;
            this.comboBoxRestrictionType.FormattingEnabled = true;
            this.comboBoxRestrictionType.Location = new System.Drawing.Point(518, 274);
            this.comboBoxRestrictionType.Name = "comboBoxRestrictionType";
            this.comboBoxRestrictionType.Size = new System.Drawing.Size(238, 23);
            this.comboBoxRestrictionType.TabIndex = 27;
            // 
            // checkBoxRestrictionTypeEnable
            // 
            this.checkBoxRestrictionTypeEnable.AutoSize = true;
            this.checkBoxRestrictionTypeEnable.Location = new System.Drawing.Point(490, 277);
            this.checkBoxRestrictionTypeEnable.Name = "checkBoxRestrictionTypeEnable";
            this.checkBoxRestrictionTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRestrictionTypeEnable.TabIndex = 26;
            this.checkBoxRestrictionTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxRestrictionTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxRestrictionTypeEnable_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(481, 256);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(157, 15);
            this.label15.TabIndex = 76;
            this.label15.Text = "Тип права собственности";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(486, 215);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(123, 15);
            this.label13.TabIndex = 75;
            this.label13.Text = "Номер ограничения";
            // 
            // textBoxOwnershipNumber
            // 
            this.textBoxOwnershipNumber.Enabled = false;
            this.textBoxOwnershipNumber.Location = new System.Drawing.Point(518, 232);
            this.textBoxOwnershipNumber.MaxLength = 255;
            this.textBoxOwnershipNumber.Name = "textBoxOwnershipNumber";
            this.textBoxOwnershipNumber.Size = new System.Drawing.Size(238, 21);
            this.textBoxOwnershipNumber.TabIndex = 25;
            // 
            // checkBoxOwnershipNumberEnable
            // 
            this.checkBoxOwnershipNumberEnable.AutoSize = true;
            this.checkBoxOwnershipNumberEnable.Location = new System.Drawing.Point(491, 235);
            this.checkBoxOwnershipNumberEnable.Name = "checkBoxOwnershipNumberEnable";
            this.checkBoxOwnershipNumberEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxOwnershipNumberEnable.TabIndex = 24;
            this.checkBoxOwnershipNumberEnable.UseVisualStyleBackColor = true;
            this.checkBoxOwnershipNumberEnable.CheckedChanged += new System.EventHandler(this.checkBoxOwnershipNumberEnable_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(488, 176);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 15);
            this.label10.TabIndex = 74;
            this.label10.Text = "Тип ограничения";
            // 
            // ExtendedSearchBuildingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(768, 396);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBoxRestrictionNumber);
            this.Controls.Add(this.checkBoxRestrictionNumberEnable);
            this.Controls.Add(this.comboBoxRestrictionType);
            this.Controls.Add(this.checkBoxRestrictionTypeEnable);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBoxOwnershipNumber);
            this.Controls.Add(this.checkBoxOwnershipNumberEnable);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.comboBoxOwnershipType);
            this.Controls.Add(this.checkBoxOwnershipTypeEnable);
            this.Controls.Add(this.numericUpDownIDBuilding);
            this.Controls.Add(this.checkBoxIDBuildingEnable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.comboBoxRegion);
            this.Controls.Add(this.checkBoxRegionEnable);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.checkBoxStateEnable);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxImprovementEnable);
            this.Controls.Add(this.checkBoxImprovement);
            this.Controls.Add(this.checkBoxElevatorEnable);
            this.Controls.Add(this.checkBoxElevator);
            this.Controls.Add(this.comboBoxStructureType);
            this.Controls.Add(this.checkBoxStructureTypeEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBoxStartupYearEnable);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownStartupYear);
            this.Controls.Add(this.checkBoxCadastralNumEnable);
            this.Controls.Add(this.textBoxCadastralNum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBoxFloorsEnable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxHouseEnable);
            this.Controls.Add(this.checkBoxStreetEnable);
            this.Controls.Add(this.numericUpDownFloors);
            this.Controls.Add(this.textBoxHouse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxStreet);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendedSearchBuildingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация зданий";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDBuilding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox comboBoxStreet;
        private Label label1;
        private Label label2;
        private TextBox textBoxHouse;
        private NumericUpDown numericUpDownFloors;
        private CheckBox checkBoxStreetEnable;
        private CheckBox checkBoxHouseEnable;
        private Label label3;
        private CheckBox checkBoxFloorsEnable;
        private CheckBox checkBoxCadastralNumEnable;
        private TextBox textBoxCadastralNum;
        private Label label4;
        private CheckBox checkBoxStartupYearEnable;
        private Label label5;
        private NumericUpDown numericUpDownStartupYear;
        private CheckBox checkBoxStructureTypeEnable;
        private Label label6;
        private ComboBox comboBoxStructureType;
        private CheckBox checkBoxElevator;
        private CheckBox checkBoxElevatorEnable;
        private CheckBox checkBoxImprovementEnable;
        private CheckBox checkBoxImprovement;
        private CheckBox checkBoxStateEnable;
        private Label label7;
        private ComboBox comboBoxRegion;
        private CheckBox checkBoxRegionEnable;
        private Label label8;
        private CheckBox checkBoxIDBuildingEnable;
        private Label label9;
        private NumericUpDown numericUpDownIDBuilding;
        private ComboBox comboBoxOwnershipType;
        private CheckBox checkBoxOwnershipTypeEnable;
        private CheckedListBox checkedListBox1;
        private vButton vButton1;
        private vButton vButton2;
        private Label label14;
        private TextBox textBoxRestrictionNumber;
        private CheckBox checkBoxRestrictionNumberEnable;
        private ComboBox comboBoxRestrictionType;
        private CheckBox checkBoxRestrictionTypeEnable;
        private Label label15;
        private Label label13;
        private TextBox textBoxOwnershipNumber;
        private CheckBox checkBoxOwnershipNumberEnable;
        private Label label10;

    }
}