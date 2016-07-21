using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.ModalEditors
{
    partial class DeptPeriodEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeptPeriodEditor));
            this.label4 = new System.Windows.Forms.Label();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.dateTimePickerDeptPeriodFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerDeptPeriodTo = new System.Windows.Forms.DateTimePicker();
            this.checkBoxSetDeptPeriodFrom = new System.Windows.Forms.CheckBox();
            this.checkBoxSetDeptPeriodTo = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Период предъявления";
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(221, 79);
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
            this.vButtonSave.Location = new System.Drawing.Point(60, 79);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(136, 40);
            this.vButtonSave.TabIndex = 4;
            this.vButtonSave.Text = "Изменить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
            // 
            // dateTimePickerDeptPeriodFrom
            // 
            this.dateTimePickerDeptPeriodFrom.Enabled = false;
            this.dateTimePickerDeptPeriodFrom.Location = new System.Drawing.Point(213, 9);
            this.dateTimePickerDeptPeriodFrom.Name = "dateTimePickerDeptPeriodFrom";
            this.dateTimePickerDeptPeriodFrom.ShowCheckBox = true;
            this.dateTimePickerDeptPeriodFrom.Size = new System.Drawing.Size(190, 21);
            this.dateTimePickerDeptPeriodFrom.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(192, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "с";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(184, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "по";
            // 
            // dateTimePickerDeptPeriodTo
            // 
            this.dateTimePickerDeptPeriodTo.Enabled = false;
            this.dateTimePickerDeptPeriodTo.Location = new System.Drawing.Point(213, 43);
            this.dateTimePickerDeptPeriodTo.Name = "dateTimePickerDeptPeriodTo";
            this.dateTimePickerDeptPeriodTo.ShowCheckBox = true;
            this.dateTimePickerDeptPeriodTo.Size = new System.Drawing.Size(190, 21);
            this.dateTimePickerDeptPeriodTo.TabIndex = 3;
            // 
            // checkBoxSetDeptPeriodFrom
            // 
            this.checkBoxSetDeptPeriodFrom.AutoSize = true;
            this.checkBoxSetDeptPeriodFrom.Location = new System.Drawing.Point(163, 14);
            this.checkBoxSetDeptPeriodFrom.Name = "checkBoxSetDeptPeriodFrom";
            this.checkBoxSetDeptPeriodFrom.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSetDeptPeriodFrom.TabIndex = 0;
            this.checkBoxSetDeptPeriodFrom.UseVisualStyleBackColor = true;
            this.checkBoxSetDeptPeriodFrom.CheckedChanged += new System.EventHandler(this.checkBoxSetDeptPeriodFrom_CheckedChanged);
            // 
            // checkBoxSetDeptPeriodTo
            // 
            this.checkBoxSetDeptPeriodTo.AutoSize = true;
            this.checkBoxSetDeptPeriodTo.Location = new System.Drawing.Point(163, 48);
            this.checkBoxSetDeptPeriodTo.Name = "checkBoxSetDeptPeriodTo";
            this.checkBoxSetDeptPeriodTo.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSetDeptPeriodTo.TabIndex = 2;
            this.checkBoxSetDeptPeriodTo.UseVisualStyleBackColor = true;
            this.checkBoxSetDeptPeriodTo.CheckedChanged += new System.EventHandler(this.checkBoxSetDeptPeriodTo_CheckedChanged);
            // 
            // DeptPeriodEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(415, 126);
            this.Controls.Add(this.checkBoxSetDeptPeriodTo);
            this.Controls.Add(this.checkBoxSetDeptPeriodFrom);
            this.Controls.Add(this.dateTimePickerDeptPeriodTo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePickerDeptPeriodFrom);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSave);
            this.Controls.Add(this.label4);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DeptPeriodEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Изменить период предъявления";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label4;
        private vButton vButtonCancel;
        private vButton vButtonSave;
        private DateTimePicker dateTimePickerDeptPeriodFrom;
        private Label label1;
        private Label label2;
        private DateTimePicker dateTimePickerDeptPeriodTo;
        private CheckBox checkBoxSetDeptPeriodFrom;
        private CheckBox checkBoxSetDeptPeriodTo;
    }
}