namespace Registry.Reporting.SettingForms
{
    partial class StatisticReportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticReportDialog));
            this.vButtonAllObjects = new VIBlend.WinForms.Controls.vButton();
            this.vButtonMunicipalObjects = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.SuspendLayout();
            // 
            // vButtonAllObjects
            // 
            this.vButtonAllObjects.AllowAnimations = true;
            this.vButtonAllObjects.BackColor = System.Drawing.Color.Transparent;
            this.vButtonAllObjects.Location = new System.Drawing.Point(12, 12);
            this.vButtonAllObjects.Name = "vButtonAllObjects";
            this.vButtonAllObjects.RoundedCornersMask = ((byte)(15));
            this.vButtonAllObjects.Size = new System.Drawing.Size(265, 35);
            this.vButtonAllObjects.TabIndex = 0;
            this.vButtonAllObjects.Text = "Все помещения";
            this.vButtonAllObjects.UseVisualStyleBackColor = false;
            this.vButtonAllObjects.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonAllObjects.Click += new System.EventHandler(this.vButtonAllObjects_Click);
            // 
            // vButtonMunicipalObjects
            // 
            this.vButtonMunicipalObjects.AllowAnimations = true;
            this.vButtonMunicipalObjects.BackColor = System.Drawing.Color.Transparent;
            this.vButtonMunicipalObjects.Location = new System.Drawing.Point(12, 53);
            this.vButtonMunicipalObjects.Name = "vButtonMunicipalObjects";
            this.vButtonMunicipalObjects.RoundedCornersMask = ((byte)(15));
            this.vButtonMunicipalObjects.Size = new System.Drawing.Size(265, 35);
            this.vButtonMunicipalObjects.TabIndex = 1;
            this.vButtonMunicipalObjects.Text = "Муниципальные помещения";
            this.vButtonMunicipalObjects.UseVisualStyleBackColor = false;
            this.vButtonMunicipalObjects.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonMunicipalObjects.Click += new System.EventHandler(this.vButtonMunicipalObjects_Click);
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(12, 116);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(265, 35);
            this.vButtonCancel.TabIndex = 2;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // StatisticReportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(289, 158);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonAllObjects);
            this.Controls.Add(this.vButtonMunicipalObjects);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StatisticReportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Формирование статистики";
            this.ResumeLayout(false);

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonAllObjects;
        private VIBlend.WinForms.Controls.vButton vButtonMunicipalObjects;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
    }
}