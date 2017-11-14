using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.ModalEditors
{
    partial class TenancyReasonsEditorMultiMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyReasonsEditorMultiMaster));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxReasonNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerReasonDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxIdReasonType = new System.Windows.Forms.ComboBox();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxDeletePrevReasons = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Номер основания";
            // 
            // textBoxReasonNumber
            // 
            this.textBoxReasonNumber.Location = new System.Drawing.Point(144, 9);
            this.textBoxReasonNumber.MaxLength = 50;
            this.textBoxReasonNumber.Name = "textBoxReasonNumber";
            this.textBoxReasonNumber.Size = new System.Drawing.Size(451, 21);
            this.textBoxReasonNumber.TabIndex = 0;
            this.textBoxReasonNumber.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Дата основания";
            // 
            // dateTimePickerReasonDate
            // 
            this.dateTimePickerReasonDate.Location = new System.Drawing.Point(144, 39);
            this.dateTimePickerReasonDate.Name = "dateTimePickerReasonDate";
            this.dateTimePickerReasonDate.Size = new System.Drawing.Size(451, 21);
            this.dateTimePickerReasonDate.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Тип основания";
            // 
            // comboBoxIdReasonType
            // 
            this.comboBoxIdReasonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIdReasonType.FormattingEnabled = true;
            this.comboBoxIdReasonType.Location = new System.Drawing.Point(144, 69);
            this.comboBoxIdReasonType.Name = "comboBoxIdReasonType";
            this.comboBoxIdReasonType.Size = new System.Drawing.Size(451, 23);
            this.comboBoxIdReasonType.TabIndex = 2;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(459, 98);
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
            this.vButtonSave.Location = new System.Drawing.Point(318, 98);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(136, 40);
            this.vButtonSave.TabIndex = 4;
            this.vButtonSave.Text = "Добавить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
            // 
            // checkBoxDeletePrevReasons
            // 
            this.checkBoxDeletePrevReasons.AutoSize = true;
            this.checkBoxDeletePrevReasons.Location = new System.Drawing.Point(55, 108);
            this.checkBoxDeletePrevReasons.Name = "checkBoxDeletePrevReasons";
            this.checkBoxDeletePrevReasons.Size = new System.Drawing.Size(228, 19);
            this.checkBoxDeletePrevReasons.TabIndex = 3;
            this.checkBoxDeletePrevReasons.Text = "Удалить существующие основания";
            this.checkBoxDeletePrevReasons.UseVisualStyleBackColor = true;
            // 
            // TenancyReasonsEditorMultiMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(607, 148);
            this.Controls.Add(this.checkBoxDeletePrevReasons);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSave);
            this.Controls.Add(this.comboBoxIdReasonType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dateTimePickerReasonDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxReasonNumber);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyReasonsEditorMultiMaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавить основание найма";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox textBoxReasonNumber;
        private Label label2;
        private DateTimePicker dateTimePickerReasonDate;
        private Label label4;
        private ComboBox comboBoxIdReasonType;
        private vButton vButtonCancel;
        private vButton vButtonSave;
        private CheckBox checkBoxDeletePrevReasons;
    }
}