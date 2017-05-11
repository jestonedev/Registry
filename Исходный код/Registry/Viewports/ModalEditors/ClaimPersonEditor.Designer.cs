using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.ModalEditors
{
    partial class ClaimPersonEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimPersonEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSurname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerDateOfBirth = new System.Windows.Forms.DateTimePicker();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPatronymic = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPlaceOfBirth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxWorkPlace = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxIsClaimer = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Фамилия";
            // 
            // textBoxSurname
            // 
            this.textBoxSurname.Location = new System.Drawing.Point(150, 9);
            this.textBoxSurname.MaxLength = 50;
            this.textBoxSurname.Name = "textBoxSurname";
            this.textBoxSurname.Size = new System.Drawing.Size(250, 21);
            this.textBoxSurname.TabIndex = 0;
            this.textBoxSurname.Enter += new System.EventHandler(this.selectAll_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Дата рождения";
            // 
            // dateTimePickerDateOfBirth
            // 
            this.dateTimePickerDateOfBirth.Location = new System.Drawing.Point(150, 90);
            this.dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            this.dateTimePickerDateOfBirth.Size = new System.Drawing.Size(250, 21);
            this.dateTimePickerDateOfBirth.TabIndex = 3;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(221, 197);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(136, 40);
            this.vButtonCancel.TabIndex = 8;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSave
            // 
            this.vButtonSave.AllowAnimations = true;
            this.vButtonSave.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSave.Location = new System.Drawing.Point(60, 197);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(136, 40);
            this.vButtonSave.TabIndex = 7;
            this.vButtonSave.Text = "Добавить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(150, 36);
            this.textBoxName.MaxLength = 50;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(250, 21);
            this.textBoxName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Имя";
            // 
            // textBoxPatronymic
            // 
            this.textBoxPatronymic.Location = new System.Drawing.Point(150, 63);
            this.textBoxPatronymic.MaxLength = 255;
            this.textBoxPatronymic.Name = "textBoxPatronymic";
            this.textBoxPatronymic.Size = new System.Drawing.Size(250, 21);
            this.textBoxPatronymic.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Отчество";
            // 
            // textBoxPlaceOfBirth
            // 
            this.textBoxPlaceOfBirth.Location = new System.Drawing.Point(150, 117);
            this.textBoxPlaceOfBirth.MaxLength = 1024;
            this.textBoxPlaceOfBirth.Name = "textBoxPlaceOfBirth";
            this.textBoxPlaceOfBirth.Size = new System.Drawing.Size(250, 21);
            this.textBoxPlaceOfBirth.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Место рождения";
            // 
            // textBoxWorkPlace
            // 
            this.textBoxWorkPlace.Location = new System.Drawing.Point(150, 144);
            this.textBoxWorkPlace.MaxLength = 1024;
            this.textBoxWorkPlace.Name = "textBoxWorkPlace";
            this.textBoxWorkPlace.Size = new System.Drawing.Size(250, 21);
            this.textBoxWorkPlace.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "Место работы";
            // 
            // checkBoxIsClaimer
            // 
            this.checkBoxIsClaimer.AutoSize = true;
            this.checkBoxIsClaimer.Location = new System.Drawing.Point(150, 172);
            this.checkBoxIsClaimer.Name = "checkBoxIsClaimer";
            this.checkBoxIsClaimer.Size = new System.Drawing.Size(100, 19);
            this.checkBoxIsClaimer.TabIndex = 6;
            this.checkBoxIsClaimer.Text = "Наниматель";
            this.checkBoxIsClaimer.UseVisualStyleBackColor = true;
            // 
            // ClaimPersonEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(415, 241);
            this.Controls.Add(this.checkBoxIsClaimer);
            this.Controls.Add(this.textBoxWorkPlace);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxPlaceOfBirth);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxPatronymic);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSave);
            this.Controls.Add(this.dateTimePickerDateOfBirth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSurname);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimPersonEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавить участника";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox textBoxSurname;
        private Label label2;
        private DateTimePicker dateTimePickerDateOfBirth;
        private vButton vButtonCancel;
        private vButton vButtonSave;
        private TextBox textBoxName;
        private Label label3;
        private TextBox textBoxPatronymic;
        private Label label4;
        private TextBox textBoxPlaceOfBirth;
        private Label label5;
        private TextBox textBoxWorkPlace;
        private Label label6;
        private CheckBox checkBoxIsClaimer;
    }
}