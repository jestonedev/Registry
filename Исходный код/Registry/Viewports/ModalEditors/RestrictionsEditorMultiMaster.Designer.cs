using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport
{
    partial class RestrictionsEditorMultiMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RestrictionsEditorMultiMaster));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRestrictionNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerRestrictionDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRestrictionDescription = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxIdRestrictionType = new System.Windows.Forms.ComboBox();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Номер реквизита";
            // 
            // textBoxRestrictionNumber
            // 
            this.textBoxRestrictionNumber.Location = new System.Drawing.Point(177, 9);
            this.textBoxRestrictionNumber.MaxLength = 10;
            this.textBoxRestrictionNumber.Name = "textBoxRestrictionNumber";
            this.textBoxRestrictionNumber.Size = new System.Drawing.Size(223, 21);
            this.textBoxRestrictionNumber.TabIndex = 0;
            this.textBoxRestrictionNumber.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Дата реквизита";
            // 
            // dateTimePickerRestrictionDate
            // 
            this.dateTimePickerRestrictionDate.Location = new System.Drawing.Point(177, 39);
            this.dateTimePickerRestrictionDate.Name = "dateTimePickerRestrictionDate";
            this.dateTimePickerRestrictionDate.Size = new System.Drawing.Size(223, 21);
            this.dateTimePickerRestrictionDate.TabIndex = 1;
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
            // textBoxRestrictionDescription
            // 
            this.textBoxRestrictionDescription.Location = new System.Drawing.Point(177, 100);
            this.textBoxRestrictionDescription.MaxLength = 255;
            this.textBoxRestrictionDescription.Multiline = true;
            this.textBoxRestrictionDescription.Name = "textBoxRestrictionDescription";
            this.textBoxRestrictionDescription.Size = new System.Drawing.Size(223, 66);
            this.textBoxRestrictionDescription.TabIndex = 3;
            this.textBoxRestrictionDescription.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Тип права собственности";
            // 
            // comboBoxIdRestrictionType
            // 
            this.comboBoxIdRestrictionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIdRestrictionType.FormattingEnabled = true;
            this.comboBoxIdRestrictionType.Location = new System.Drawing.Point(177, 69);
            this.comboBoxIdRestrictionType.Name = "comboBoxIdRestrictionType";
            this.comboBoxIdRestrictionType.Size = new System.Drawing.Size(223, 23);
            this.comboBoxIdRestrictionType.TabIndex = 2;
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
            // RestrictionsEditorMultiMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(415, 223);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSave);
            this.Controls.Add(this.comboBoxIdRestrictionType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxRestrictionDescription);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateTimePickerRestrictionDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxRestrictionNumber);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RestrictionsEditorMultiMaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавить реквизит";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox textBoxRestrictionNumber;
        private Label label2;
        private DateTimePicker dateTimePickerRestrictionDate;
        private Label label3;
        private TextBox textBoxRestrictionDescription;
        private Label label4;
        private ComboBox comboBoxIdRestrictionType;
        private vButton vButtonCancel;
        private vButton vButtonSave;
    }
}