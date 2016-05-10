namespace Registry.Reporting
{
    partial class ActPremiseExtInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActPremiseExtInfoForm));
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxOpenDate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.vButton2.Location = new System.Drawing.Point(22, 71);
            this.vButton2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(156, 43);
            this.vButton2.TabIndex = 18;
            this.vButton2.Text = "Сформировать";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton1.Location = new System.Drawing.Point(202, 71);
            this.vButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(156, 43);
            this.vButton1.TabIndex = 19;
            this.vButton1.Text = "Отменить";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // checkBoxOpenDate
            // 
            this.checkBoxOpenDate.AutoSize = true;
            this.checkBoxOpenDate.Location = new System.Drawing.Point(22, 25);
            this.checkBoxOpenDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxOpenDate.Name = "checkBoxOpenDate";
            this.checkBoxOpenDate.Size = new System.Drawing.Size(214, 21);
            this.checkBoxOpenDate.TabIndex = 20;
            this.checkBoxOpenDate.Text = "Печатать с открытой датой";
            this.checkBoxOpenDate.UseVisualStyleBackColor = true;
            // 
            // ActPremiseExtInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(417, 124);
            this.Controls.Add(this.checkBoxOpenDate);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActPremiseExtInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Дополнительные характеристики помещения";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButton2;
        private VIBlend.WinForms.Controls.vButton vButton1;
        private System.Windows.Forms.CheckBox checkBoxOpenDate;
    }
}