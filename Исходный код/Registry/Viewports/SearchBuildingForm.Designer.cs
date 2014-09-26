namespace Registry
{
    partial class SearchBuildingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxHouse = new System.Windows.Forms.TextBox();
            this.numericUpDownFloors = new System.Windows.Forms.NumericUpDown();
            this.checkBoxStreetEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxHouseEnable = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxFloorsEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxCadastralNumEnable = new System.Windows.Forms.CheckBox();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxStartupYearEnable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownStartupYear = new System.Windows.Forms.NumericUpDown();
            this.checkBoxFundTypeEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxFundType = new System.Windows.Forms.ComboBox();
            this.checkBoxElevator = new System.Windows.Forms.CheckBox();
            this.checkBoxElevatorEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxImprovementEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxImprovement = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).BeginInit();
            this.SuspendLayout();
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.Location = new System.Drawing.Point(102, 292);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(100, 30);
            this.vButton1.TabIndex = 16;
            this.vButton1.Text = "Поиск";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButton1.Click += new System.EventHandler(this.vButton1_Click);
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton2.Location = new System.Drawing.Point(220, 292);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(100, 30);
            this.vButton2.TabIndex = 17;
            this.vButton2.Text = "Отмена";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.Enabled = false;
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(36, 25);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(375, 21);
            this.comboBoxStreet.TabIndex = 1;
            this.comboBoxStreet.DropDownClosed += new System.EventHandler(this.comboBoxStreet_DropDownClosed);
            this.comboBoxStreet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxStreet_KeyUp);
            this.comboBoxStreet.Leave += new System.EventHandler(this.comboBoxStreet_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Улица";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Номер дома";
            // 
            // textBoxHouse
            // 
            this.textBoxHouse.Enabled = false;
            this.textBoxHouse.Location = new System.Drawing.Point(36, 65);
            this.textBoxHouse.MaxLength = 4;
            this.textBoxHouse.Name = "textBoxHouse";
            this.textBoxHouse.Size = new System.Drawing.Size(375, 20);
            this.textBoxHouse.TabIndex = 3;
            // 
            // numericUpDownFloors
            // 
            this.numericUpDownFloors.Enabled = false;
            this.numericUpDownFloors.Location = new System.Drawing.Point(36, 104);
            this.numericUpDownFloors.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownFloors.Name = "numericUpDownFloors";
            this.numericUpDownFloors.Size = new System.Drawing.Size(375, 20);
            this.numericUpDownFloors.TabIndex = 5;
            // 
            // checkBoxStreetEnable
            // 
            this.checkBoxStreetEnable.AutoSize = true;
            this.checkBoxStreetEnable.Location = new System.Drawing.Point(15, 28);
            this.checkBoxStreetEnable.Name = "checkBoxStreetEnable";
            this.checkBoxStreetEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStreetEnable.TabIndex = 0;
            this.checkBoxStreetEnable.UseVisualStyleBackColor = true;
            this.checkBoxStreetEnable.CheckedChanged += new System.EventHandler(this.checkBoxStreetEnable_CheckedChanged);
            // 
            // checkBoxHouseEnable
            // 
            this.checkBoxHouseEnable.AutoSize = true;
            this.checkBoxHouseEnable.Location = new System.Drawing.Point(15, 68);
            this.checkBoxHouseEnable.Name = "checkBoxHouseEnable";
            this.checkBoxHouseEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHouseEnable.TabIndex = 2;
            this.checkBoxHouseEnable.UseVisualStyleBackColor = true;
            this.checkBoxHouseEnable.CheckedChanged += new System.EventHandler(this.checkBoxHouseEnable_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Этажность";
            // 
            // checkBoxFloorsEnable
            // 
            this.checkBoxFloorsEnable.AutoSize = true;
            this.checkBoxFloorsEnable.Location = new System.Drawing.Point(15, 107);
            this.checkBoxFloorsEnable.Name = "checkBoxFloorsEnable";
            this.checkBoxFloorsEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFloorsEnable.TabIndex = 4;
            this.checkBoxFloorsEnable.UseVisualStyleBackColor = true;
            this.checkBoxFloorsEnable.CheckedChanged += new System.EventHandler(this.checkBoxFloorsEnable_CheckedChanged);
            // 
            // checkBoxCadastralNumEnable
            // 
            this.checkBoxCadastralNumEnable.AutoSize = true;
            this.checkBoxCadastralNumEnable.Location = new System.Drawing.Point(15, 146);
            this.checkBoxCadastralNumEnable.Name = "checkBoxCadastralNumEnable";
            this.checkBoxCadastralNumEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCadastralNumEnable.TabIndex = 6;
            this.checkBoxCadastralNumEnable.UseVisualStyleBackColor = true;
            this.checkBoxCadastralNumEnable.CheckedChanged += new System.EventHandler(this.checkBoxCadastralNumEnable_CheckedChanged);
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Enabled = false;
            this.textBoxCadastralNum.Location = new System.Drawing.Point(36, 143);
            this.textBoxCadastralNum.MaxLength = 15;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(375, 20);
            this.textBoxCadastralNum.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Кадастровый номер";
            // 
            // checkBoxStartupYearEnable
            // 
            this.checkBoxStartupYearEnable.AutoSize = true;
            this.checkBoxStartupYearEnable.Location = new System.Drawing.Point(15, 185);
            this.checkBoxStartupYearEnable.Name = "checkBoxStartupYearEnable";
            this.checkBoxStartupYearEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStartupYearEnable.TabIndex = 8;
            this.checkBoxStartupYearEnable.UseVisualStyleBackColor = true;
            this.checkBoxStartupYearEnable.CheckedChanged += new System.EventHandler(this.checkBoxStartupYearEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 166);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Год ввода в эксплуатацию";
            // 
            // numericUpDownStartupYear
            // 
            this.numericUpDownStartupYear.Enabled = false;
            this.numericUpDownStartupYear.Location = new System.Drawing.Point(36, 182);
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
            this.numericUpDownStartupYear.Size = new System.Drawing.Size(375, 20);
            this.numericUpDownStartupYear.TabIndex = 9;
            this.numericUpDownStartupYear.Value = new decimal(new int[] {
            1900,
            0,
            0,
            0});
            // 
            // checkBoxFundTypeEnable
            // 
            this.checkBoxFundTypeEnable.AutoSize = true;
            this.checkBoxFundTypeEnable.Location = new System.Drawing.Point(15, 224);
            this.checkBoxFundTypeEnable.Name = "checkBoxFundTypeEnable";
            this.checkBoxFundTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFundTypeEnable.TabIndex = 10;
            this.checkBoxFundTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxFundTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxFundTypeEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 205);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Вид найма";
            // 
            // comboBoxFundType
            // 
            this.comboBoxFundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFundType.Enabled = false;
            this.comboBoxFundType.FormattingEnabled = true;
            this.comboBoxFundType.Location = new System.Drawing.Point(36, 221);
            this.comboBoxFundType.Name = "comboBoxFundType";
            this.comboBoxFundType.Size = new System.Drawing.Size(375, 21);
            this.comboBoxFundType.TabIndex = 11;
            // 
            // checkBoxElevator
            // 
            this.checkBoxElevator.AutoSize = true;
            this.checkBoxElevator.Enabled = false;
            this.checkBoxElevator.Location = new System.Drawing.Point(36, 248);
            this.checkBoxElevator.Name = "checkBoxElevator";
            this.checkBoxElevator.Size = new System.Drawing.Size(103, 17);
            this.checkBoxElevator.TabIndex = 13;
            this.checkBoxElevator.Text = "Наличие лифта";
            this.checkBoxElevator.UseVisualStyleBackColor = true;
            // 
            // checkBoxElevatorEnable
            // 
            this.checkBoxElevatorEnable.AutoSize = true;
            this.checkBoxElevatorEnable.Location = new System.Drawing.Point(15, 249);
            this.checkBoxElevatorEnable.Name = "checkBoxElevatorEnable";
            this.checkBoxElevatorEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxElevatorEnable.TabIndex = 12;
            this.checkBoxElevatorEnable.UseVisualStyleBackColor = true;
            this.checkBoxElevatorEnable.CheckedChanged += new System.EventHandler(this.checkBoxElevatorEnable_CheckedChanged);
            // 
            // checkBoxImprovementEnable
            // 
            this.checkBoxImprovementEnable.AutoSize = true;
            this.checkBoxImprovementEnable.Location = new System.Drawing.Point(15, 270);
            this.checkBoxImprovementEnable.Name = "checkBoxImprovementEnable";
            this.checkBoxImprovementEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxImprovementEnable.TabIndex = 14;
            this.checkBoxImprovementEnable.UseVisualStyleBackColor = true;
            this.checkBoxImprovementEnable.CheckedChanged += new System.EventHandler(this.checkBoxImprovementEnable_CheckedChanged);
            // 
            // checkBoxImprovement
            // 
            this.checkBoxImprovement.AutoSize = true;
            this.checkBoxImprovement.Enabled = false;
            this.checkBoxImprovement.Location = new System.Drawing.Point(36, 269);
            this.checkBoxImprovement.Name = "checkBoxImprovement";
            this.checkBoxImprovement.Size = new System.Drawing.Size(113, 17);
            this.checkBoxImprovement.TabIndex = 15;
            this.checkBoxImprovement.Text = "Благоустройство";
            this.checkBoxImprovement.UseVisualStyleBackColor = true;
            // 
            // SearchBuildingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(423, 329);
            this.Controls.Add(this.checkBoxImprovementEnable);
            this.Controls.Add(this.checkBoxImprovement);
            this.Controls.Add(this.checkBoxElevatorEnable);
            this.Controls.Add(this.checkBoxElevator);
            this.Controls.Add(this.comboBoxFundType);
            this.Controls.Add(this.checkBoxFundTypeEnable);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SearchBuildingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация зданий";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFloors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartupYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButton1;
        private VIBlend.WinForms.Controls.vButton vButton2;
        private System.Windows.Forms.ComboBox comboBoxStreet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxHouse;
        private System.Windows.Forms.NumericUpDown numericUpDownFloors;
        private System.Windows.Forms.CheckBox checkBoxStreetEnable;
        private System.Windows.Forms.CheckBox checkBoxHouseEnable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxFloorsEnable;
        private System.Windows.Forms.CheckBox checkBoxCadastralNumEnable;
        private System.Windows.Forms.TextBox textBoxCadastralNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxStartupYearEnable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownStartupYear;
        private System.Windows.Forms.CheckBox checkBoxFundTypeEnable;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxFundType;
        private System.Windows.Forms.CheckBox checkBoxElevator;
        private System.Windows.Forms.CheckBox checkBoxElevatorEnable;
        private System.Windows.Forms.CheckBox checkBoxImprovementEnable;
        private System.Windows.Forms.CheckBox checkBoxImprovement;

    }
}