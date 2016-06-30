using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport
{
    partial class SubPremisesEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubPremisesEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSubPremisesNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxIdState = new System.Windows.Forms.ComboBox();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.numericUpDownTotalArea = new System.Windows.Forms.NumericUpDown();
            this.dateTimePickerStateDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownLivingArea = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxCadastralNum = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownBalanceCost = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownCadastralCost = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Номер комнаты";
            // 
            // textBoxSubPremisesNum
            // 
            this.textBoxSubPremisesNum.Location = new System.Drawing.Point(177, 9);
            this.textBoxSubPremisesNum.MaxLength = 20;
            this.textBoxSubPremisesNum.Name = "textBoxSubPremisesNum";
            this.textBoxSubPremisesNum.Size = new System.Drawing.Size(223, 21);
            this.textBoxSubPremisesNum.TabIndex = 0;
            this.textBoxSubPremisesNum.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Общая площадь";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(73, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Примечание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(177, 239);
            this.textBoxDescription.MaxLength = 65535;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(223, 66);
            this.textBoxDescription.TabIndex = 8;
            this.textBoxDescription.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Текущее состояние";
            // 
            // comboBoxIdState
            // 
            this.comboBoxIdState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIdState.FormattingEnabled = true;
            this.comboBoxIdState.Location = new System.Drawing.Point(177, 96);
            this.comboBoxIdState.Name = "comboBoxIdState";
            this.comboBoxIdState.Size = new System.Drawing.Size(223, 23);
            this.comboBoxIdState.TabIndex = 3;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(221, 316);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(136, 40);
            this.vButtonCancel.TabIndex = 10;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSave
            // 
            this.vButtonSave.AllowAnimations = true;
            this.vButtonSave.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSave.Location = new System.Drawing.Point(60, 316);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(136, 40);
            this.vButtonSave.TabIndex = 9;
            this.vButtonSave.Text = "Добавить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new System.Drawing.Point(177, 40);
            this.numericUpDownTotalArea.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(223, 21);
            this.numericUpDownTotalArea.TabIndex = 1;
            this.numericUpDownTotalArea.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // dateTimePickerStateDate
            // 
            this.dateTimePickerStateDate.Location = new System.Drawing.Point(157, 491);
            this.dateTimePickerStateDate.Name = "dateTimePickerStateDate";
            this.dateTimePickerStateDate.ShowCheckBox = true;
            this.dateTimePickerStateDate.Size = new System.Drawing.Size(223, 21);
            this.dateTimePickerStateDate.TabIndex = 4;
            this.dateTimePickerStateDate.TabStop = false;
            this.dateTimePickerStateDate.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 495);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Состояние установлено";
            this.label5.Visible = false;
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new System.Drawing.Point(177, 68);
            this.numericUpDownLivingArea.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownLivingArea.Name = "numericUpDownLivingArea";
            this.numericUpDownLivingArea.Size = new System.Drawing.Size(223, 21);
            this.numericUpDownLivingArea.TabIndex = 2;
            this.numericUpDownLivingArea.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(68, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Жилая площадь";
            // 
            // textBoxCadastralNum
            // 
            this.textBoxCadastralNum.Location = new System.Drawing.Point(177, 125);
            this.textBoxCadastralNum.MaxLength = 20;
            this.textBoxCadastralNum.Name = "textBoxCadastralNum";
            this.textBoxCadastralNum.Size = new System.Drawing.Size(223, 21);
            this.textBoxCadastralNum.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Кадастровый номер";
            // 
            // numericUpDownBalanceCost
            // 
            this.numericUpDownBalanceCost.DecimalPlaces = 2;
            this.numericUpDownBalanceCost.Location = new System.Drawing.Point(177, 182);
            this.numericUpDownBalanceCost.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownBalanceCost.Name = "numericUpDownBalanceCost";
            this.numericUpDownBalanceCost.Size = new System.Drawing.Size(223, 21);
            this.numericUpDownBalanceCost.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(143, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Балансовая стоимость";
            // 
            // numericUpDownCadastralCost
            // 
            this.numericUpDownCadastralCost.DecimalPlaces = 2;
            this.numericUpDownCadastralCost.Location = new System.Drawing.Point(177, 154);
            this.numericUpDownCadastralCost.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownCadastralCost.Name = "numericUpDownCadastralCost";
            this.numericUpDownCadastralCost.Size = new System.Drawing.Size(223, 21);
            this.numericUpDownCadastralCost.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(150, 15);
            this.label9.TabIndex = 16;
            this.label9.Text = "Кадастровая стоимость";
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Location = new System.Drawing.Point(177, 209);
            this.textBoxAccount.MaxLength = 255;
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(223, 21);
            this.textBoxAccount.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(51, 212);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 15);
            this.label10.TabIndex = 19;
            this.label10.Text = "Лицевой счет ФКР";
            // 
            // SubPremisesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(415, 365);
            this.Controls.Add(this.textBoxAccount);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.numericUpDownBalanceCost);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numericUpDownCadastralCost);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxCadastralNum);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericUpDownLivingArea);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dateTimePickerStateDate);
            this.Controls.Add(this.numericUpDownTotalArea);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSave);
            this.Controls.Add(this.comboBoxIdState);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSubPremisesNum);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SubPremisesEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавить комнату";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBalanceCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCadastralCost)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox textBoxSubPremisesNum;
        private Label label2;
        private Label label3;
        private TextBox textBoxDescription;
        private Label label4;
        private ComboBox comboBoxIdState;
        private vButton vButtonCancel;
        private vButton vButtonSave;
        private NumericUpDown numericUpDownTotalArea;
        private DateTimePicker dateTimePickerStateDate;
        private Label label5;
        private NumericUpDown numericUpDownLivingArea;
        private Label label6;
        private TextBox textBoxCadastralNum;
        private Label label7;
        private NumericUpDown numericUpDownBalanceCost;
        private Label label8;
        private NumericUpDown numericUpDownCadastralCost;
        private Label label9;
        private TextBox textBoxAccount;
        private Label label10;
    }
}