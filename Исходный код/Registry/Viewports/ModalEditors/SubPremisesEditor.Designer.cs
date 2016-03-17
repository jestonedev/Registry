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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLivingArea)).BeginInit();
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
            this.label3.Location = new System.Drawing.Point(73, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Примечание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(177, 154);
            this.textBoxDescription.MaxLength = 65535;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(223, 66);
            this.textBoxDescription.TabIndex = 5;
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
            this.vButtonCancel.Location = new System.Drawing.Point(222, 231);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(136, 40);
            this.vButtonCancel.TabIndex = 7;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSave
            // 
            this.vButtonSave.AllowAnimations = true;
            this.vButtonSave.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSave.Location = new System.Drawing.Point(61, 231);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(136, 40);
            this.vButtonSave.TabIndex = 6;
            this.vButtonSave.Text = "Добавить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
            // 
            // numericUpDownTotalArea
            // 
            this.numericUpDownTotalArea.DecimalPlaces = 3;
            this.numericUpDownTotalArea.Location = new System.Drawing.Point(177, 40);
            this.numericUpDownTotalArea.Name = "numericUpDownTotalArea";
            this.numericUpDownTotalArea.Size = new System.Drawing.Size(223, 21);
            this.numericUpDownTotalArea.TabIndex = 1;
            this.numericUpDownTotalArea.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // dateTimePickerStateDate
            // 
            this.dateTimePickerStateDate.Location = new System.Drawing.Point(177, 126);
            this.dateTimePickerStateDate.Name = "dateTimePickerStateDate";
            this.dateTimePickerStateDate.ShowCheckBox = true;
            this.dateTimePickerStateDate.Size = new System.Drawing.Size(223, 21);
            this.dateTimePickerStateDate.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Состояние установлено";
            // 
            // numericUpDownLivingArea
            // 
            this.numericUpDownLivingArea.DecimalPlaces = 3;
            this.numericUpDownLivingArea.Location = new System.Drawing.Point(177, 68);
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
            // SubPremisesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(415, 280);
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
    }
}