using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport
{
    partial class OwnershipsEditor
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(OwnershipsEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOwnershipNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerOwnershipDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOwnershipDescription = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxIdOwnershipType = new System.Windows.Forms.ComboBox();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Номер ограничения";
            // 
            // textBoxOwnershipNumber
            // 
            this.textBoxOwnershipNumber.Location = new System.Drawing.Point(177, 9);
            this.textBoxOwnershipNumber.MaxLength = 10;
            this.textBoxOwnershipNumber.Name = "textBoxOwnershipNumber";
            this.textBoxOwnershipNumber.Size = new System.Drawing.Size(223, 21);
            this.textBoxOwnershipNumber.TabIndex = 0;
            this.textBoxOwnershipNumber.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Дата ограничения";
            // 
            // dateTimePickerOwnershipDate
            // 
            this.dateTimePickerOwnershipDate.Location = new System.Drawing.Point(177, 39);
            this.dateTimePickerOwnershipDate.Name = "dateTimePickerOwnershipDate";
            this.dateTimePickerOwnershipDate.Size = new System.Drawing.Size(223, 21);
            this.dateTimePickerOwnershipDate.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(73, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Наименование";
            // 
            // textBoxOwnershipDescription
            // 
            this.textBoxOwnershipDescription.Location = new System.Drawing.Point(177, 100);
            this.textBoxOwnershipDescription.MaxLength = 255;
            this.textBoxOwnershipDescription.Multiline = true;
            this.textBoxOwnershipDescription.Name = "textBoxOwnershipDescription";
            this.textBoxOwnershipDescription.Size = new System.Drawing.Size(223, 66);
            this.textBoxOwnershipDescription.TabIndex = 3;
            this.textBoxOwnershipDescription.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(61, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Тип ограничения";
            // 
            // comboBoxIdOwnershipType
            // 
            this.comboBoxIdOwnershipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIdOwnershipType.FormattingEnabled = true;
            this.comboBoxIdOwnershipType.Location = new System.Drawing.Point(177, 69);
            this.comboBoxIdOwnershipType.Name = "comboBoxIdOwnershipType";
            this.comboBoxIdOwnershipType.Size = new System.Drawing.Size(223, 23);
            this.comboBoxIdOwnershipType.TabIndex = 2;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(222, 177);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(136, 40);
            this.vButtonCancel.TabIndex = 5;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSave
            // 
            this.vButtonSave.AllowAnimations = true;
            this.vButtonSave.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSave.Location = new System.Drawing.Point(61, 177);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(136, 40);
            this.vButtonSave.TabIndex = 4;
            this.vButtonSave.Text = "Добавить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
            // 
            // OwnershipsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(415, 223);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSave);
            this.Controls.Add(this.comboBoxIdOwnershipType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxOwnershipDescription);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateTimePickerOwnershipDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxOwnershipNumber);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OwnershipsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавить ограничение";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox textBoxOwnershipNumber;
        private Label label2;
        private DateTimePicker dateTimePickerOwnershipDate;
        private Label label3;
        private TextBox textBoxOwnershipDescription;
        private Label label4;
        private ComboBox comboBoxIdOwnershipType;
        private vButton vButtonCancel;
        private vButton vButtonSave;
    }
}