﻿using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.SearchForms
{
    partial class SimpleSearchBuildingForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleSearchBuildingForm));
            this.textBoxCriteria = new System.Windows.Forms.TextBox();
            this.comboBoxCriteriaType = new System.Windows.Forms.ComboBox();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxMunicipalOnly = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxCriteria
            // 
            this.textBoxCriteria.Location = new System.Drawing.Point(196, 10);
            this.textBoxCriteria.MaxLength = 255;
            this.textBoxCriteria.Name = "textBoxCriteria";
            this.textBoxCriteria.Size = new System.Drawing.Size(348, 21);
            this.textBoxCriteria.TabIndex = 0;
            this.textBoxCriteria.Enter += new System.EventHandler(this.textBoxCriteria_Enter);
            // 
            // comboBoxCriteriaType
            // 
            this.comboBoxCriteriaType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCriteriaType.FormattingEnabled = true;
            this.comboBoxCriteriaType.Items.AddRange(new object[] {
            "по адресу",
            "по ФИО нанимателя",
            "по ФИО участника",
            "по кадастровому номеру",
            "по номеру договора"});
            this.comboBoxCriteriaType.Location = new System.Drawing.Point(10, 9);
            this.comboBoxCriteriaType.Name = "comboBoxCriteriaType";
            this.comboBoxCriteriaType.Size = new System.Drawing.Size(178, 23);
            this.comboBoxCriteriaType.TabIndex = 1;
            this.comboBoxCriteriaType.DropDownClosed += new System.EventHandler(this.comboBoxCriteriaType_DropDownClosed);
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(379, 39);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 4;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(241, 39);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 3;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // checkBoxMunicipalOnly
            // 
            this.checkBoxMunicipalOnly.AutoSize = true;
            this.checkBoxMunicipalOnly.Location = new System.Drawing.Point(12, 46);
            this.checkBoxMunicipalOnly.Name = "checkBoxMunicipalOnly";
            this.checkBoxMunicipalOnly.Size = new System.Drawing.Size(163, 19);
            this.checkBoxMunicipalOnly.TabIndex = 2;
            this.checkBoxMunicipalOnly.Text = "Только муниципальные";
            this.checkBoxMunicipalOnly.UseVisualStyleBackColor = true;
            // 
            // SimpleSearchBuildingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(556, 85);
            this.Controls.Add(this.checkBoxMunicipalOnly);
            this.Controls.Add(this.textBoxCriteria);
            this.Controls.Add(this.comboBoxCriteriaType);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimpleSearchBuildingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация зданий";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vButton vButtonCancel;
        private vButton vButtonSearch;
        private TextBox textBoxCriteria;
        private ComboBox comboBoxCriteriaType;
        private CheckBox checkBoxMunicipalOnly;
    }
}