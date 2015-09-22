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
            this.checkBoxHasAqueduct = new System.Windows.Forms.CheckBox();
            this.checkBoxHasHotWater = new System.Windows.Forms.CheckBox();
            this.checkBoxHasSewerage = new System.Windows.Forms.CheckBox();
            this.checkBoxHasLighting = new System.Windows.Forms.CheckBox();
            this.checkBoxHasRadio = new System.Windows.Forms.CheckBox();
            this.groupBoxHeating = new System.Windows.Forms.GroupBox();
            this.checkBoxHasHeating = new System.Windows.Forms.CheckBox();
            this.radioButtonLocalHeating = new System.Windows.Forms.RadioButton();
            this.radioButtonCentralHeating = new System.Windows.Forms.RadioButton();
            this.radioButtonStoveHeating = new System.Windows.Forms.RadioButton();
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.groupBoxHeating.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxHasAqueduct
            // 
            this.checkBoxHasAqueduct.AutoSize = true;
            this.checkBoxHasAqueduct.Location = new System.Drawing.Point(12, 12);
            this.checkBoxHasAqueduct.Name = "checkBoxHasAqueduct";
            this.checkBoxHasAqueduct.Size = new System.Drawing.Size(87, 17);
            this.checkBoxHasAqueduct.TabIndex = 0;
            this.checkBoxHasAqueduct.Text = "Водопровод";
            this.checkBoxHasAqueduct.UseVisualStyleBackColor = true;
            // 
            // checkBoxHasHotWater
            // 
            this.checkBoxHasHotWater.AutoSize = true;
            this.checkBoxHasHotWater.Location = new System.Drawing.Point(12, 35);
            this.checkBoxHasHotWater.Name = "checkBoxHasHotWater";
            this.checkBoxHasHotWater.Size = new System.Drawing.Size(150, 17);
            this.checkBoxHasHotWater.TabIndex = 1;
            this.checkBoxHasHotWater.Text = "Горячее водоснабжение";
            this.checkBoxHasHotWater.UseVisualStyleBackColor = true;
            // 
            // checkBoxHasSewerage
            // 
            this.checkBoxHasSewerage.AutoSize = true;
            this.checkBoxHasSewerage.Location = new System.Drawing.Point(12, 58);
            this.checkBoxHasSewerage.Name = "checkBoxHasSewerage";
            this.checkBoxHasSewerage.Size = new System.Drawing.Size(93, 17);
            this.checkBoxHasSewerage.TabIndex = 2;
            this.checkBoxHasSewerage.Text = "Канализация";
            this.checkBoxHasSewerage.UseVisualStyleBackColor = true;
            // 
            // checkBoxHasLighting
            // 
            this.checkBoxHasLighting.AutoSize = true;
            this.checkBoxHasLighting.Location = new System.Drawing.Point(12, 81);
            this.checkBoxHasLighting.Name = "checkBoxHasLighting";
            this.checkBoxHasLighting.Size = new System.Drawing.Size(125, 17);
            this.checkBoxHasLighting.TabIndex = 3;
            this.checkBoxHasLighting.Text = "Электроосвещение";
            this.checkBoxHasLighting.UseVisualStyleBackColor = true;
            // 
            // checkBoxHasRadio
            // 
            this.checkBoxHasRadio.AutoSize = true;
            this.checkBoxHasRadio.Location = new System.Drawing.Point(12, 104);
            this.checkBoxHasRadio.Name = "checkBoxHasRadio";
            this.checkBoxHasRadio.Size = new System.Drawing.Size(166, 17);
            this.checkBoxHasRadio.TabIndex = 5;
            this.checkBoxHasRadio.Text = "Радиотрансляционная сеть";
            this.checkBoxHasRadio.UseVisualStyleBackColor = true;
            // 
            // groupBoxHeating
            // 
            this.groupBoxHeating.Controls.Add(this.checkBoxHasHeating);
            this.groupBoxHeating.Controls.Add(this.radioButtonLocalHeating);
            this.groupBoxHeating.Controls.Add(this.radioButtonCentralHeating);
            this.groupBoxHeating.Controls.Add(this.radioButtonStoveHeating);
            this.groupBoxHeating.Location = new System.Drawing.Point(208, 28);
            this.groupBoxHeating.Name = "groupBoxHeating";
            this.groupBoxHeating.Size = new System.Drawing.Size(180, 93);
            this.groupBoxHeating.TabIndex = 6;
            this.groupBoxHeating.TabStop = false;
            this.groupBoxHeating.Text = "      Отопление";
            // 
            // checkBoxHasHeating
            // 
            this.checkBoxHasHeating.AutoSize = true;
            this.checkBoxHasHeating.Location = new System.Drawing.Point(10, 1);
            this.checkBoxHasHeating.Name = "checkBoxHasHeating";
            this.checkBoxHasHeating.Size = new System.Drawing.Size(15, 14);
            this.checkBoxHasHeating.TabIndex = 20;
            this.checkBoxHasHeating.UseVisualStyleBackColor = true;
            this.checkBoxHasHeating.CheckedChanged += new System.EventHandler(this.checkBoxHasHeating_CheckedChanged);
            // 
            // radioButtonLocalHeating
            // 
            this.radioButtonLocalHeating.AutoSize = true;
            this.radioButtonLocalHeating.Enabled = false;
            this.radioButtonLocalHeating.Location = new System.Drawing.Point(10, 46);
            this.radioButtonLocalHeating.Name = "radioButtonLocalHeating";
            this.radioButtonLocalHeating.Size = new System.Drawing.Size(69, 17);
            this.radioButtonLocalHeating.TabIndex = 2;
            this.radioButtonLocalHeating.Text = "Местное";
            this.radioButtonLocalHeating.UseVisualStyleBackColor = true;
            // 
            // radioButtonCentralHeating
            // 
            this.radioButtonCentralHeating.AutoSize = true;
            this.radioButtonCentralHeating.Enabled = false;
            this.radioButtonCentralHeating.Location = new System.Drawing.Point(10, 68);
            this.radioButtonCentralHeating.Name = "radioButtonCentralHeating";
            this.radioButtonCentralHeating.Size = new System.Drawing.Size(92, 17);
            this.radioButtonCentralHeating.TabIndex = 1;
            this.radioButtonCentralHeating.Text = "Центральное";
            this.radioButtonCentralHeating.UseVisualStyleBackColor = true;
            // 
            // radioButtonStoveHeating
            // 
            this.radioButtonStoveHeating.AutoSize = true;
            this.radioButtonStoveHeating.Enabled = false;
            this.radioButtonStoveHeating.Location = new System.Drawing.Point(10, 23);
            this.radioButtonStoveHeating.Name = "radioButtonStoveHeating";
            this.radioButtonStoveHeating.Size = new System.Drawing.Size(62, 17);
            this.radioButtonStoveHeating.TabIndex = 0;
            this.radioButtonStoveHeating.Text = "Печное";
            this.radioButtonStoveHeating.UseVisualStyleBackColor = true;
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.vButton2.Location = new System.Drawing.Point(73, 127);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(117, 35);
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
            this.vButton1.Location = new System.Drawing.Point(208, 127);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(117, 35);
            this.vButton1.TabIndex = 19;
            this.vButton1.Text = "Отменить";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // ActPremiseExtInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(395, 167);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.Controls.Add(this.groupBoxHeating);
            this.Controls.Add(this.checkBoxHasRadio);
            this.Controls.Add(this.checkBoxHasLighting);
            this.Controls.Add(this.checkBoxHasSewerage);
            this.Controls.Add(this.checkBoxHasHotWater);
            this.Controls.Add(this.checkBoxHasAqueduct);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActPremiseExtInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Дополнительные характеристики помещения";
            this.groupBoxHeating.ResumeLayout(false);
            this.groupBoxHeating.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxHasAqueduct;
        private System.Windows.Forms.CheckBox checkBoxHasHotWater;
        private System.Windows.Forms.CheckBox checkBoxHasSewerage;
        private System.Windows.Forms.CheckBox checkBoxHasLighting;
        private System.Windows.Forms.CheckBox checkBoxHasRadio;
        private System.Windows.Forms.GroupBox groupBoxHeating;
        private System.Windows.Forms.RadioButton radioButtonLocalHeating;
        private System.Windows.Forms.RadioButton radioButtonCentralHeating;
        private System.Windows.Forms.RadioButton radioButtonStoveHeating;
        private VIBlend.WinForms.Controls.vButton vButton2;
        private VIBlend.WinForms.Controls.vButton vButton1;
        private System.Windows.Forms.CheckBox checkBoxHasHeating;
    }
}