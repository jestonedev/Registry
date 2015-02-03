namespace Registry.Reporting
{
    partial class RegistryExcerptSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistryExcerptSettingsForm));
            this.dateTimePickerExcertFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.vButtonOk = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxExcerptNumber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // dateTimePickerExcertFrom
            // 
            this.dateTimePickerExcertFrom.Location = new System.Drawing.Point(12, 66);
            this.dateTimePickerExcertFrom.Name = "dateTimePickerExcertFrom";
            this.dateTimePickerExcertFrom.Size = new System.Drawing.Size(248, 21);
            this.dateTimePickerExcertFrom.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 15);
            this.label2.TabIndex = 62;
            this.label2.Text = "Выписка от";
            // 
            // vButtonOk
            // 
            this.vButtonOk.AllowAnimations = true;
            this.vButtonOk.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.vButtonOk.Location = new System.Drawing.Point(9, 98);
            this.vButtonOk.Name = "vButtonOk";
            this.vButtonOk.RoundedCornersMask = ((byte)(15));
            this.vButtonOk.Size = new System.Drawing.Size(117, 35);
            this.vButtonOk.TabIndex = 4;
            this.vButtonOk.Text = "Сформировать";
            this.vButtonOk.UseVisualStyleBackColor = false;
            this.vButtonOk.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(144, 98);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 5;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 15);
            this.label3.TabIndex = 64;
            this.label3.Text = "Номер выписки";
            // 
            // textBoxExcerptNumber
            // 
            this.textBoxExcerptNumber.Location = new System.Drawing.Point(12, 25);
            this.textBoxExcerptNumber.Name = "textBoxExcerptNumber";
            this.textBoxExcerptNumber.Size = new System.Drawing.Size(249, 21);
            this.textBoxExcerptNumber.TabIndex = 0;
            // 
            // TenancyExcerptSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(269, 143);
            this.Controls.Add(this.textBoxExcerptNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.vButtonOk);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.dateTimePickerExcertFrom);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TenancyExcerptSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выписка";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerExcertFrom;
        private System.Windows.Forms.Label label2;
        private VIBlend.WinForms.Controls.vButton vButtonOk;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxExcerptNumber;
    }
}