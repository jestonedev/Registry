﻿namespace Registry.Reporting
{
    partial class SelectRegionsForm
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
            this.checkedListBoxRegions = new System.Windows.Forms.CheckedListBox();
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxCheckAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkedListBoxRegions
            // 
            this.checkedListBoxRegions.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkedListBoxRegions.Location = new System.Drawing.Point(0, 0);
            this.checkedListBoxRegions.Name = "checkedListBoxRegions";
            this.checkedListBoxRegions.Size = new System.Drawing.Size(242, 214);
            this.checkedListBoxRegions.TabIndex = 0;
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton1.Location = new System.Drawing.Point(129, 242);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(100, 30);
            this.vButton1.TabIndex = 1;
            this.vButton1.Text = "Отменить";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.Location = new System.Drawing.Point(13, 242);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(100, 30);
            this.vButton2.TabIndex = 2;
            this.vButton2.Text = "Сформировать";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            this.vButton2.Click += new System.EventHandler(this.vButton2_Click);
            // 
            // checkBoxCheckAll
            // 
            this.checkBoxCheckAll.AutoSize = true;
            this.checkBoxCheckAll.Location = new System.Drawing.Point(12, 219);
            this.checkBoxCheckAll.Name = "checkBoxCheckAll";
            this.checkBoxCheckAll.Size = new System.Drawing.Size(97, 17);
            this.checkBoxCheckAll.TabIndex = 3;
            this.checkBoxCheckAll.Text = "Выделить все";
            this.checkBoxCheckAll.UseVisualStyleBackColor = true;
            this.checkBoxCheckAll.CheckedChanged += new System.EventHandler(this.checkBoxCheckAll_CheckedChanged);
            // 
            // SelectRegions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(242, 278);
            this.Controls.Add(this.checkBoxCheckAll);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.Controls.Add(this.checkedListBoxRegions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectRegions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выберите жилые районы";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxRegions;
        private VIBlend.WinForms.Controls.vButton vButton1;
        private VIBlend.WinForms.Controls.vButton vButton2;
        private System.Windows.Forms.CheckBox checkBoxCheckAll;

    }
}