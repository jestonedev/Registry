namespace Registry.Reporting
{
    partial class ClaimsFilterForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimsFilterForm));
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.numericUpDownIDClaim = new System.Windows.Forms.NumericUpDown();
            this.checkBoxIDClaimEnable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownIDProcess = new System.Windows.Forms.NumericUpDown();
            this.checkBoxIDProcessEnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerTransferFrom = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerTransferTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStartDeptTo = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dateTimePickerStartDeptFrom = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDeptTo = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDeptFrom = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDClaim)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDProcess)).BeginInit();
            this.SuspendLayout();
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.vButton2.Location = new System.Drawing.Point(118, 220);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(117, 35);
            this.vButton2.TabIndex = 16;
            this.vButton2.Text = "Сформировать";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton1.Location = new System.Drawing.Point(253, 220);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(117, 35);
            this.vButton1.TabIndex = 17;
            this.vButton1.Text = "Отменить";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // numericUpDownIDClaim
            // 
            this.numericUpDownIDClaim.Enabled = false;
            this.numericUpDownIDClaim.Location = new System.Drawing.Point(44, 27);
            this.numericUpDownIDClaim.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownIDClaim.Name = "numericUpDownIDClaim";
            this.numericUpDownIDClaim.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownIDClaim.TabIndex = 1;
            // 
            // checkBoxIDClaimEnable
            // 
            this.checkBoxIDClaimEnable.AutoSize = true;
            this.checkBoxIDClaimEnable.Location = new System.Drawing.Point(20, 30);
            this.checkBoxIDClaimEnable.Name = "checkBoxIDClaimEnable";
            this.checkBoxIDClaimEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIDClaimEnable.TabIndex = 0;
            this.checkBoxIDClaimEnable.UseVisualStyleBackColor = true;
            this.checkBoxIDClaimEnable.CheckedChanged += new System.EventHandler(this.checkBoxIDClaimEnable_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(213, 15);
            this.label9.TabIndex = 30;
            this.label9.Text = "Внутренний номер исковой работы";
            // 
            // numericUpDownIDProcess
            // 
            this.numericUpDownIDProcess.Enabled = false;
            this.numericUpDownIDProcess.Location = new System.Drawing.Point(44, 68);
            this.numericUpDownIDProcess.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.numericUpDownIDProcess.Name = "numericUpDownIDProcess";
            this.numericUpDownIDProcess.Size = new System.Drawing.Size(437, 21);
            this.numericUpDownIDProcess.TabIndex = 3;
            // 
            // checkBoxIDProcessEnable
            // 
            this.checkBoxIDProcessEnable.AutoSize = true;
            this.checkBoxIDProcessEnable.Location = new System.Drawing.Point(20, 71);
            this.checkBoxIDProcessEnable.Name = "checkBoxIDProcessEnable";
            this.checkBoxIDProcessEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxIDProcessEnable.TabIndex = 2;
            this.checkBoxIDProcessEnable.UseVisualStyleBackColor = true;
            this.checkBoxIDProcessEnable.CheckedChanged += new System.EventHandler(this.checkBoxIDProcessEnable_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(213, 15);
            this.label1.TabIndex = 33;
            this.label1.Text = "Внутренний номер процесса найма";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 15);
            this.label2.TabIndex = 36;
            this.label2.Text = "Дата передачи иска";
            // 
            // dateTimePickerTransferFrom
            // 
            this.dateTimePickerTransferFrom.Checked = false;
            this.dateTimePickerTransferFrom.Location = new System.Drawing.Point(44, 109);
            this.dateTimePickerTransferFrom.Name = "dateTimePickerTransferFrom";
            this.dateTimePickerTransferFrom.ShowCheckBox = true;
            this.dateTimePickerTransferFrom.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerTransferFrom.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "с";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(259, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "по";
            // 
            // dateTimePickerTransferTo
            // 
            this.dateTimePickerTransferTo.Checked = false;
            this.dateTimePickerTransferTo.Location = new System.Drawing.Point(283, 109);
            this.dateTimePickerTransferTo.Name = "dateTimePickerTransferTo";
            this.dateTimePickerTransferTo.ShowCheckBox = true;
            this.dateTimePickerTransferTo.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerTransferTo.TabIndex = 7;
            // 
            // dateTimePickerStartDeptTo
            // 
            this.dateTimePickerStartDeptTo.Checked = false;
            this.dateTimePickerStartDeptTo.Location = new System.Drawing.Point(283, 150);
            this.dateTimePickerStartDeptTo.Name = "dateTimePickerStartDeptTo";
            this.dateTimePickerStartDeptTo.ShowCheckBox = true;
            this.dateTimePickerStartDeptTo.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerStartDeptTo.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "с";
            // 
            // dateTimePickerStartDeptFrom
            // 
            this.dateTimePickerStartDeptFrom.Checked = false;
            this.dateTimePickerStartDeptFrom.Location = new System.Drawing.Point(44, 150);
            this.dateTimePickerStartDeptFrom.Name = "dateTimePickerStartDeptFrom";
            this.dateTimePickerStartDeptFrom.ShowCheckBox = true;
            this.dateTimePickerStartDeptFrom.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerStartDeptFrom.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 133);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(182, 15);
            this.label6.TabIndex = 41;
            this.label6.Text = "Начало периода задолжности";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(259, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 15);
            this.label7.TabIndex = 10;
            this.label7.Text = "по";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(259, 194);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 15);
            this.label8.TabIndex = 14;
            this.label8.Text = "по";
            // 
            // dateTimePickerEndDeptTo
            // 
            this.dateTimePickerEndDeptTo.Checked = false;
            this.dateTimePickerEndDeptTo.Location = new System.Drawing.Point(283, 191);
            this.dateTimePickerEndDeptTo.Name = "dateTimePickerEndDeptTo";
            this.dateTimePickerEndDeptTo.ShowCheckBox = true;
            this.dateTimePickerEndDeptTo.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerEndDeptTo.TabIndex = 15;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(22, 194);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(13, 15);
            this.label10.TabIndex = 12;
            this.label10.Text = "с";
            // 
            // dateTimePickerEndDeptFrom
            // 
            this.dateTimePickerEndDeptFrom.Checked = false;
            this.dateTimePickerEndDeptFrom.Location = new System.Drawing.Point(44, 191);
            this.dateTimePickerEndDeptFrom.Name = "dateTimePickerEndDeptFrom";
            this.dateTimePickerEndDeptFrom.ShowCheckBox = true;
            this.dateTimePickerEndDeptFrom.Size = new System.Drawing.Size(198, 21);
            this.dateTimePickerEndDeptFrom.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 174);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(202, 15);
            this.label11.TabIndex = 46;
            this.label11.Text = "Окончание периода задолжности";
            // 
            // ClaimsFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(489, 262);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dateTimePickerEndDeptTo);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.dateTimePickerEndDeptFrom);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dateTimePickerStartDeptTo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dateTimePickerStartDeptFrom);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePickerTransferTo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateTimePickerTransferFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownIDProcess);
            this.Controls.Add(this.checkBoxIDProcessEnable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownIDClaim);
            this.Controls.Add(this.checkBoxIDClaimEnable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClaimsFilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Критерии отчета по исковой работе";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDClaim)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIDProcess)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButton2;
        private VIBlend.WinForms.Controls.vButton vButton1;
        private System.Windows.Forms.NumericUpDown numericUpDownIDClaim;
        private System.Windows.Forms.CheckBox checkBoxIDClaimEnable;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDownIDProcess;
        private System.Windows.Forms.CheckBox checkBoxIDProcessEnable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerTransferFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePickerTransferTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerStartDeptTo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dateTimePickerStartDeptFrom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndDeptTo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndDeptFrom;
        private System.Windows.Forms.Label label11;
    }
}