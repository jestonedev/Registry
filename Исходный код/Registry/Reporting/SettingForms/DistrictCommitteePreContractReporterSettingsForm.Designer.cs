namespace Registry.Reporting.SettingForms
{
    partial class DistrictCommitteePreContractReporterSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DistrictCommitteePreContractReporterSettingsForm));
            this.vButtonOk = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.comboBoxPreamble = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxCommittee = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // vButtonOk
            // 
            this.vButtonOk.AllowAnimations = true;
            this.vButtonOk.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.vButtonOk.Location = new System.Drawing.Point(134, 101);
            this.vButtonOk.Name = "vButtonOk";
            this.vButtonOk.RoundedCornersMask = ((byte)(15));
            this.vButtonOk.Size = new System.Drawing.Size(117, 35);
            this.vButtonOk.TabIndex = 4;
            this.vButtonOk.Text = "Сформировать";
            this.vButtonOk.UseVisualStyleBackColor = false;
            this.vButtonOk.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonOk.Click += new System.EventHandler(this.vButtonOk_Click);
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(269, 101);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 5;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // comboBoxPreamble
            // 
            this.comboBoxPreamble.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPreamble.FormattingEnabled = true;
            this.comboBoxPreamble.Location = new System.Drawing.Point(12, 66);
            this.comboBoxPreamble.Name = "comboBoxPreamble";
            this.comboBoxPreamble.Size = new System.Drawing.Size(494, 23);
            this.comboBoxPreamble.TabIndex = 65;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 15);
            this.label1.TabIndex = 66;
            this.label1.Text = "Преамбула";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 68;
            this.label2.Text = "Комитет";
            // 
            // comboBoxCommittee
            // 
            this.comboBoxCommittee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCommittee.FormattingEnabled = true;
            this.comboBoxCommittee.Location = new System.Drawing.Point(9, 22);
            this.comboBoxCommittee.Name = "comboBoxCommittee";
            this.comboBoxCommittee.Size = new System.Drawing.Size(494, 23);
            this.comboBoxCommittee.TabIndex = 67;
            // 
            // DistrictCommitteePreContractReporterSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(518, 143);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxCommittee);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxPreamble);
            this.Controls.Add(this.vButtonOk);
            this.Controls.Add(this.vButtonCancel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DistrictCommitteePreContractReporterSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Предварительный договор";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonOk;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.ComboBox comboBoxPreamble;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxCommittee;
    }
}