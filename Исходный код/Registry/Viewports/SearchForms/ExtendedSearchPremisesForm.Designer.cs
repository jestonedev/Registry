using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.SearchForms
{
    partial class ExtendedSearchPremisesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedSearchPremisesForm));
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxHouseEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxStreetEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.checkBoxPremisesNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxPremisesNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxFloorEnable = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownFloor = new System.Windows.Forms.NumericUpDown();
            this.checkBoxCadastralNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxFundType = new System.Windows.Forms.ComboBox();
            this.checkBoxFundTypeEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.checkBoxStateEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownIDPremises = new System.Windows.Forms.NumericUpDown();
            this.checkBoxIDPremisesEnable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxRegion = new System.Windows.Forms.ComboBox();
            this.checkBoxRegionEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxTenantSNPEnable = new System.Windows.Forms.CheckBox();
            this.textBoxTenantSNP = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBoxContractNumberEnable = new System.Windows.Forms.CheckBox();
            this.textBoxContractNumber = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxOwnershipType = new System.Windows.Forms.ComboBox();
            this.checkBoxOwnershipTypeEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.checkBoxOwnershipNumberEnable = new System.Windows.Forms.CheckBox();
            this.textBoxOwnershipNumber = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxRestrictionNumber = new System.Windows.Forms.TextBox();
            this.checkBoxRestrictionNumberEnable = new System.Windows.Forms.CheckBox();
            this.comboBoxRestrictionType = new System.Windows.Forms.ComboBox();
            this.checkBoxRestrictionTypeEnable = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDPremises)).BeginInit();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(641, 430);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 31;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(518, 430);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 30;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // checkBoxHouseEnable
            // 
            this.checkBoxHouseEnable.AutoSize = true;
            this.checkBoxHouseEnable.Location = new System.Drawing.Point(17, 155);
            this.checkBoxHouseEnable.Name = "checkBoxHouseEnable";
            this.checkBoxHouseEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseEnable.TabIndex = 6;
            this.checkBoxHouseEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // checkBoxStreetEnable
            // 
            this.checkBoxStreetEnable.AutoSize = true;
            this.checkBoxStreetEnable.Location = new System.Drawing.Point(17, 113);
            this.checkBoxStreetEnable.Name = "checkBoxStreetEnable";
            this.checkBoxStreetEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetEnable.TabIndex = 4;
            this.checkBoxStreetEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 25;
            this.label2.Text = "Номер дома";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 24;
            this.label1.Text = "Улица";
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Enabled = false;
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(42, 109);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(437, 23);
            this.comboBoxStreet.TabIndex = 5;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.Enter += new System.EventHandler(this.selectAll_Enter);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // checkBoxPremisesNumEnable
            // 
            this.checkBoxPremisesNumEnable.AutoSize = true;
            this.checkBoxPremisesNumEnable.Location = new System.Drawing.Point(17, 196);
            this.checkBoxPremisesNumEnable.Name = "checkBoxPremisesNumEnable";
            this.checkBoxPremisesNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPremisesNumEnable.TabIndex = 8;
            this.checkBoxPremisesNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxPremisesNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxPremisesNumEnable_CheckedChanged);
            // 
            // textBoxPremisesNum
            // 
            this.textBoxPremisesNum.Enabled = false;
            this.textBoxPremisesNum.Location = new System.Drawing.Point(42, 193);
            this.textBoxPremisesNum.MaxLength = 4;
            this.textBoxPremisesNum.Name = "textBoxPremisesNum";
            this.textBoxPremisesNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxPremisesNum.TabIndex = 9;
            this.textBoxPremisesNum.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 15);
            this.label3.TabIndex = 28;
            this.label3.Text = "Номер помещения";
            // 
            // checkBoxFloorEnable
            // 
            this.checkBoxFloorEnable.AutoSize = true;
            this.checkBoxFloorEnable.Location = new System.Drawing.Point(17, 237);
            this.checkBoxFloorEnable.Name = "checkBoxFloorEnable";
            this.checkBoxFloorEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFloorEnable.TabIndex = 10;
            this.checkBoxFloorEnable.UseVisualStyleBackColor = true;
            this.checkBoxFloorEnable.CheckedChanged += new System.EventHandler(this.checkBoxFloorEnable_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 217);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 15);
            this.label4.TabIndex = 31;
            this.label4.Text = "Этаж";
            // 
            // numericUpDownFloor
            // 
            this.numericUpDownFloor.Enabled = false;
            this.numericUpDownFloor.Location = new System.Drawing.Point(42, 234);
            this.numericUpDownFloor.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownFloor.Name = "numericUpDownFloor";
            this.numericUpDownFloor.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownFloor.TabIndex = 11;
            this.numericUpDownFloor.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxCadastralNumEnable
            // 
            this.checkBoxCadastralNumEnable.AutoSize = true;
            this.checkBoxCadastralNumEnable.Location = new System.Drawing.Point(17, 278);
            this.checkBoxCadastralNumEnable.Name = "checkBoxCadastralNumEnable";
            this.checkBoxCadastralNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCadastralNumEnable.TabIndex = 12;
            this.checkBoxCadastralNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxCadastralNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxCadastralNumEnable_CheckedChanged);
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Enabled = false;
            this.textBoxCadastralNum.Location = new System.Drawing.Point(42, 275);
            this.textBoxCadastralNum.MaxLength = 20;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(437, 21);
            this.textBoxCadastralNum.TabIndex = 13;
            this.textBoxCadastralNum.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 258);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 15);
            this.label5.TabIndex = 34;
            this.label5.Text = "Кадастровый номер";
            // 
            // comboBoxFundType
            // 
            this.comboBoxFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFundType.Enabled = false;
            this.comboBoxFundType.FormattingEnabled = true;
            this.comboBoxFundType.Location = new System.Drawing.Point(42, 398);
            this.comboBoxFundType.Name = "comboBoxFundType";
            this.comboBoxFundType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxFundType.TabIndex = 19;
            // 
            // checkBoxFundTypeEnable
            // 
            this.checkBoxFundTypeEnable.AutoSize = true;
            this.checkBoxFundTypeEnable.Location = new System.Drawing.Point(17, 402);
            this.checkBoxFundTypeEnable.Name = "checkBoxFundTypeEnable";
            this.checkBoxFundTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFundTypeEnable.TabIndex = 18;
            this.checkBoxFundTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxFundTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxFundTypeEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 381);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 15);
            this.label6.TabIndex = 37;
            this.label6.Text = "Тип жилого фонда";
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Enabled = false;
            this.textBoxHouse.Location = new System.Drawing.Point(42, 152);
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(437, 21);
            this.textBoxHouse.TabIndex = 7;
            this.textBoxHouse.Enter += new System.EventHandler(this.selectAll_Enter);
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
            this.label7.Size = new System.Drawing.Size(140, 15);
            this.label7.TabIndex = 41;
            this.label7.Text = "Состояние помещения";
            // 
            // numericUpDownIDPremises
            // 
            this.numericUpDownIDPremises.Enabled = false;
            this.numericUpDownIDPremises.Location = new System.Drawing.Point(42, 25);
            this.numericUpDownIDPremises.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownIDPremises.Name = "numericUpDownIDPremises";
            this.numericUpDownIDPremises.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownIDPremises.TabIndex = 1;
            this.numericUpDownIDPremises.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // checkBoxIDPremisesEnable
            // 
            this.checkBoxIDPremisesEnable.AutoSize = true;
            this.checkBoxIDPremisesEnable.Location = new System.Drawing.Point(17, 29);
            this.checkBoxIDPremisesEnable.Name = "checkBoxIDPremisesEnable";
            this.checkBoxIDPremisesEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIDPremisesEnable.TabIndex = 0;
            this.checkBoxIDPremisesEnable.UseVisualStyleBackColor = true;
            this.checkBoxIDPremisesEnable.CheckedChanged += new System.EventHandler(this.checkBoxIDBuildingEnable_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(189, 15);
            this.label9.TabIndex = 47;
            this.label9.Text = "Реестровый номер помещения";
            // 
            // comboBoxRegion
            // 
            this.comboBoxRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRegion.Enabled = false;
            this.comboBoxRegion.FormattingEnabled = true;
            this.comboBoxRegion.Location = new System.Drawing.Point(42, 66);
            this.comboBoxRegion.Name = "comboBoxRegion";
            this.comboBoxRegion.Size = new System.Drawing.Size(437, 23);
            this.comboBoxRegion.TabIndex = 3;
            // 
            // checkBoxRegionEnable
            // 
            this.checkBoxRegionEnable.AutoSize = true;
            this.checkBoxRegionEnable.Location = new System.Drawing.Point(17, 70);
            this.checkBoxRegionEnable.Name = "checkBoxRegionEnable";
            this.checkBoxRegionEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRegionEnable.TabIndex = 2;
            this.checkBoxRegionEnable.UseVisualStyleBackColor = true;
            this.checkBoxRegionEnable.CheckedChanged += new System.EventHandler(this.checkBoxRegionEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 15);
            this.label8.TabIndex = 46;
            this.label8.Text = "Жилой район";
            // 
            // checkBoxTenantSNPEnable
            // 
            this.checkBoxTenantSNPEnable.AutoSize = true;
            this.checkBoxTenantSNPEnable.Location = new System.Drawing.Point(17, 360);
            this.checkBoxTenantSNPEnable.Name = "checkBoxTenantSNPEnable";
            this.checkBoxTenantSNPEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxTenantSNPEnable.TabIndex = 16;
            this.checkBoxTenantSNPEnable.UseVisualStyleBackColor = true;
            this.checkBoxTenantSNPEnable.CheckedChanged += new System.EventHandler(this.checkBoxTenantSNPEnable_CheckedChanged);
            // 
            // textBoxTenantSNP
            // 
            this.textBoxTenantSNP.Enabled = false;
            this.textBoxTenantSNP.Location = new System.Drawing.Point(42, 357);
            this.textBoxTenantSNP.MaxLength = 255;
            this.textBoxTenantSNP.Name = "textBoxTenantSNP";
            this.textBoxTenantSNP.Size = new System.Drawing.Size(437, 21);
            this.textBoxTenantSNP.TabIndex = 17;
            this.textBoxTenantSNP.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 340);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 15);
            this.label11.TabIndex = 52;
            this.label11.Text = "ФИО нанимателя";
            // 
            // checkBoxContractNumberEnable
            // 
            this.checkBoxContractNumberEnable.AutoSize = true;
            this.checkBoxContractNumberEnable.Location = new System.Drawing.Point(17, 319);
            this.checkBoxContractNumberEnable.Name = "checkBoxContractNumberEnable";
            this.checkBoxContractNumberEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxContractNumberEnable.TabIndex = 14;
            this.checkBoxContractNumberEnable.UseVisualStyleBackColor = true;
            this.checkBoxContractNumberEnable.CheckedChanged += new System.EventHandler(this.checkBoxContractNumberEnable_CheckedChanged);
            // 
            // textBoxContractNumber
            // 
            this.textBoxContractNumber.Enabled = false;
            this.textBoxContractNumber.Location = new System.Drawing.Point(42, 316);
            this.textBoxContractNumber.MaxLength = 16;
            this.textBoxContractNumber.Name = "textBoxContractNumber";
            this.textBoxContractNumber.Size = new System.Drawing.Size(437, 21);
            this.textBoxContractNumber.TabIndex = 15;
            this.textBoxContractNumber.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 299);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(143, 15);
            this.label10.TabIndex = 49;
            this.label10.Text = "Номер договора найма";
            // 
            // comboBoxOwnershipType
            // 
            this.comboBoxOwnershipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOwnershipType.Enabled = false;
            this.comboBoxOwnershipType.FormattingEnabled = true;
            this.comboBoxOwnershipType.Location = new System.Drawing.Point(518, 274);
            this.comboBoxOwnershipType.Name = "comboBoxOwnershipType";
            this.comboBoxOwnershipType.Size = new System.Drawing.Size(240, 23);
            this.comboBoxOwnershipType.TabIndex = 23;
            // 
            // checkBoxOwnershipTypeEnable
            // 
            this.checkBoxOwnershipTypeEnable.AutoSize = true;
            this.checkBoxOwnershipTypeEnable.Location = new System.Drawing.Point(494, 278);
            this.checkBoxOwnershipTypeEnable.Name = "checkBoxOwnershipTypeEnable";
            this.checkBoxOwnershipTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxOwnershipTypeEnable.TabIndex = 22;
            this.checkBoxOwnershipTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxOwnershipTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxOwnershipTypeEnable_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(491, 258);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(105, 15);
            this.label12.TabIndex = 56;
            this.label12.Text = "Тип ограничения";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Enabled = false;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(518, 28);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(240, 228);
            this.checkedListBox1.TabIndex = 21;
            // 
            // checkBoxOwnershipNumberEnable
            // 
            this.checkBoxOwnershipNumberEnable.AutoSize = true;
            this.checkBoxOwnershipNumberEnable.Location = new System.Drawing.Point(494, 318);
            this.checkBoxOwnershipNumberEnable.Name = "checkBoxOwnershipNumberEnable";
            this.checkBoxOwnershipNumberEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxOwnershipNumberEnable.TabIndex = 24;
            this.checkBoxOwnershipNumberEnable.UseVisualStyleBackColor = true;
            this.checkBoxOwnershipNumberEnable.CheckedChanged += new System.EventHandler(this.checkBoxOwnershipNumberEnable_CheckedChanged);
            // 
            // textBoxOwnershipNumber
            // 
            this.textBoxOwnershipNumber.Enabled = false;
            this.textBoxOwnershipNumber.Location = new System.Drawing.Point(518, 315);
            this.textBoxOwnershipNumber.MaxLength = 255;
            this.textBoxOwnershipNumber.Name = "textBoxOwnershipNumber";
            this.textBoxOwnershipNumber.Size = new System.Drawing.Size(240, 21);
            this.textBoxOwnershipNumber.TabIndex = 25;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(491, 298);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(123, 15);
            this.label13.TabIndex = 59;
            this.label13.Text = "Номер ограничения";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(491, 382);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 15);
            this.label14.TabIndex = 65;
            this.label14.Text = "Номер реквизита";
            // 
            // textBoxRestrictionNumber
            // 
            this.textBoxRestrictionNumber.Enabled = false;
            this.textBoxRestrictionNumber.Location = new System.Drawing.Point(518, 400);
            this.textBoxRestrictionNumber.MaxLength = 255;
            this.textBoxRestrictionNumber.Name = "textBoxRestrictionNumber";
            this.textBoxRestrictionNumber.Size = new System.Drawing.Size(240, 21);
            this.textBoxRestrictionNumber.TabIndex = 29;
            // 
            // checkBoxRestrictionNumberEnable
            // 
            this.checkBoxRestrictionNumberEnable.AutoSize = true;
            this.checkBoxRestrictionNumberEnable.Location = new System.Drawing.Point(494, 403);
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
            this.comboBoxRestrictionType.Location = new System.Drawing.Point(518, 357);
            this.comboBoxRestrictionType.Name = "comboBoxRestrictionType";
            this.comboBoxRestrictionType.Size = new System.Drawing.Size(240, 23);
            this.comboBoxRestrictionType.TabIndex = 27;
            // 
            // checkBoxRestrictionTypeEnable
            // 
            this.checkBoxRestrictionTypeEnable.AutoSize = true;
            this.checkBoxRestrictionTypeEnable.Location = new System.Drawing.Point(493, 360);
            this.checkBoxRestrictionTypeEnable.Name = "checkBoxRestrictionTypeEnable";
            this.checkBoxRestrictionTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxRestrictionTypeEnable.TabIndex = 26;
            this.checkBoxRestrictionTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxRestrictionTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxRestrictionTypeEnable_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(486, 339);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(157, 15);
            this.label15.TabIndex = 62;
            this.label15.Text = "Тип права собственности";
            // 
            // ExtendedSearchPremisesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(764, 468);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBoxRestrictionNumber);
            this.Controls.Add(this.checkBoxRestrictionNumberEnable);
            this.Controls.Add(this.comboBoxRestrictionType);
            this.Controls.Add(this.checkBoxRestrictionTypeEnable);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBoxOwnershipNumber);
            this.Controls.Add(this.checkBoxOwnershipNumberEnable);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.comboBoxOwnershipType);
            this.Controls.Add(this.checkBoxOwnershipTypeEnable);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.checkBoxTenantSNPEnable);
            this.Controls.Add(this.textBoxTenantSNP);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.checkBoxContractNumberEnable);
            this.Controls.Add(this.textBoxContractNumber);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.numericUpDownIDPremises);
            this.Controls.Add(this.checkBoxIDPremisesEnable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.comboBoxRegion);
            this.Controls.Add(this.checkBoxRegionEnable);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.checkBoxStateEnable);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxHouse);
            this.Controls.Add(this.comboBoxFundType);
            this.Controls.Add(this.checkBoxFundTypeEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBoxCadastralNumEnable);
            this.Controls.Add(this.textBoxCadastralNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBoxFloorEnable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownFloor);
            this.Controls.Add(this.checkBoxPremisesNumEnable);
            this.Controls.Add(this.textBoxPremisesNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxHouseEnable);
            this.Controls.Add(this.checkBoxStreetEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxStreet);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendedSearchPremisesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация помещений";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDPremises)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vButton vButtonCancel;
        private vButton vButtonSearch;
        private CheckBox checkBoxHouseEnable;
        private CheckBox checkBoxStreetEnable;
        private Label label2;
        private Label label1;
        private ComboBox comboBoxStreet;
        private CheckBox checkBoxPremisesNumEnable;
        private TextBox textBoxPremisesNum;
        private Label label3;
        private CheckBox checkBoxFloorEnable;
        private Label label4;
        private NumericUpDown numericUpDownFloor;
        private CheckBox checkBoxCadastralNumEnable;
        private TextBox textBoxCadastralNum;
        private Label label5;
        private ComboBox comboBoxFundType;
        private CheckBox checkBoxFundTypeEnable;
        private Label label6;
        private TextBox textBoxHouse;
        private CheckBox checkBoxStateEnable;
        private Label label7;
        private NumericUpDown numericUpDownIDPremises;
        private CheckBox checkBoxIDPremisesEnable;
        private Label label9;
        private ComboBox comboBoxRegion;
        private CheckBox checkBoxRegionEnable;
        private Label label8;
        private CheckBox checkBoxTenantSNPEnable;
        private TextBox textBoxTenantSNP;
        private Label label11;
        private CheckBox checkBoxContractNumberEnable;
        private TextBox textBoxContractNumber;
        private Label label10;
        private ComboBox comboBoxOwnershipType;
        private CheckBox checkBoxOwnershipTypeEnable;
        private Label label12;
        private CheckedListBox checkedListBox1;
        private CheckBox checkBoxOwnershipNumberEnable;
        private TextBox textBoxOwnershipNumber;
        private Label label13;
        private Label label14;
        private TextBox textBoxRestrictionNumber;
        private CheckBox checkBoxRestrictionNumberEnable;
        private ComboBox comboBoxRestrictionType;
        private CheckBox checkBoxRestrictionTypeEnable;
        private Label label15;
    }
}