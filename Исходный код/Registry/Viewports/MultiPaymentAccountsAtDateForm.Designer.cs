namespace Registry.Viewport
{
    partial class MultiPaymentAccountsAtDateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiPaymentAccountsAtDateForm));
            this.vButtonOk = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.dateTimePickerAt = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // vButtonOk
            // 
            this.vButtonOk.AllowAnimations = true;
            this.vButtonOk.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.vButtonOk.Location = new System.Drawing.Point(8, 38);
            this.vButtonOk.Name = "vButtonOk";
            this.vButtonOk.RoundedCornersMask = ((byte)(15));
            this.vButtonOk.Size = new System.Drawing.Size(117, 35);
            this.vButtonOk.TabIndex = 2;
            this.vButtonOk.Text = "Сформировать";
            this.vButtonOk.UseVisualStyleBackColor = false;
            this.vButtonOk.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(143, 38);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 3;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // dateTimePickerAt
            // 
            this.dateTimePickerAt.Location = new System.Drawing.Point(8, 12);
            this.dateTimePickerAt.Name = "dateTimePickerAt";
            this.dateTimePickerAt.Size = new System.Drawing.Size(252, 20);
            this.dateTimePickerAt.TabIndex = 0;
            // 
            // MultiPaymentAccountsAtDateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(269, 78);
            this.Controls.Add(this.vButtonOk);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.dateTimePickerAt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultiPaymentAccountsAtDateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Формировать иски на дату";
            this.ResumeLayout(false);

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonOk;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.DateTimePicker dateTimePickerAt;
    }
}