namespace Registry.Reporting
{
    partial class TenancyExcerptSettingsForm
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
            this.dateTimePickerRegistryInsert = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerExcertFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.vButtonOk = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxIsCultureMemorial = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxExcerptNumber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // dateTimePickerRegistryInsert
            // 
            this.dateTimePickerRegistryInsert.Location = new System.Drawing.Point(12, 61);
            this.dateTimePickerRegistryInsert.Name = "dateTimePickerRegistryInsert";
            this.dateTimePickerRegistryInsert.Size = new System.Drawing.Size(213, 20);
            this.dateTimePickerRegistryInsert.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "Дата внесения в реестр";
            // 
            // dateTimePickerExcertFrom
            // 
            this.dateTimePickerExcertFrom.Location = new System.Drawing.Point(12, 102);
            this.dateTimePickerExcertFrom.Name = "dateTimePickerExcertFrom";
            this.dateTimePickerExcertFrom.Size = new System.Drawing.Size(213, 20);
            this.dateTimePickerExcertFrom.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 62;
            this.label2.Text = "Выписка от";
            // 
            // vButtonOk
            // 
            this.vButtonOk.AllowAnimations = true;
            this.vButtonOk.BackColor = System.Drawing.Color.Transparent;
            this.vButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.vButtonOk.Location = new System.Drawing.Point(7, 155);
            this.vButtonOk.Name = "vButtonOk";
            this.vButtonOk.RoundedCornersMask = ((byte)(15));
            this.vButtonOk.Size = new System.Drawing.Size(100, 30);
            this.vButtonOk.TabIndex = 4;
            this.vButtonOk.Text = "Сформировать";
            this.vButtonOk.UseVisualStyleBackColor = false;
            this.vButtonOk.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(123, 155);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(100, 30);
            this.vButtonCancel.TabIndex = 5;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.ULTRABLUE;
            // 
            // checkBoxIsCultureMemorial
            // 
            this.checkBoxIsCultureMemorial.AutoSize = true;
            this.checkBoxIsCultureMemorial.Location = new System.Drawing.Point(11, 130);
            this.checkBoxIsCultureMemorial.Name = "checkBoxIsCultureMemorial";
            this.checkBoxIsCultureMemorial.Size = new System.Drawing.Size(191, 17);
            this.checkBoxIsCultureMemorial.TabIndex = 3;
            this.checkBoxIsCultureMemorial.Text = "Является памятником культуры";
            this.checkBoxIsCultureMemorial.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 64;
            this.label3.Text = "Номер выписки";
            // 
            // textBoxExcerptNumber
            // 
            this.textBoxExcerptNumber.Location = new System.Drawing.Point(11, 22);
            this.textBoxExcerptNumber.Name = "textBoxExcerptNumber";
            this.textBoxExcerptNumber.Size = new System.Drawing.Size(214, 20);
            this.textBoxExcerptNumber.TabIndex = 0;
            // 
            // TenancyExcerptSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(231, 196);
            this.Controls.Add(this.textBoxExcerptNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxIsCultureMemorial);
            this.Controls.Add(this.vButtonOk);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.dateTimePickerExcertFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePickerRegistryInsert);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TenancyExcerptSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выписка";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerRegistryInsert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerExcertFrom;
        private System.Windows.Forms.Label label2;
        private VIBlend.WinForms.Controls.vButton vButtonOk;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.CheckBox checkBoxIsCultureMemorial;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxExcerptNumber;
    }
}