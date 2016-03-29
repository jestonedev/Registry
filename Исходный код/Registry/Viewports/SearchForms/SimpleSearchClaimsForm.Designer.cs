using System.ComponentModel;
using System.Windows.Forms;
using VIBlend.WinForms.Controls;

namespace Registry.Viewport.SearchForms
{
    partial class SimpleSearchClaimsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleSearchClaimsForm));
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.dateTimePickerAtDate = new System.Windows.Forms.DateTimePicker();
            this.label103 = new System.Windows.Forms.Label();
            this.label91 = new System.Windows.Forms.Label();
            this.comboBoxState = new System.Windows.Forms.ComboBox();
            this.checkBoxAtDateChecked = new System.Windows.Forms.CheckBox();
            this.checkBoxStateEnable = new System.Windows.Forms.CheckBox();
            this.checkBoxAccountChecked = new System.Windows.Forms.CheckBox();
            this.textBoxAccount = new System.Windows.Forms.TextBox();
            this.dateTimePickerDateStartStateTo = new System.Windows.Forms.DateTimePicker();
            this.comboBoxDateStartStateExpr = new System.Windows.Forms.ComboBox();
            this.checkBoxDateStartStateEnable = new System.Windows.Forms.CheckBox();
            this.dateTimePickerDateStartStateFrom = new System.Windows.Forms.DateTimePicker();
            this.textBoxSRN = new System.Windows.Forms.TextBox();
            this.checkBoxSRNEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxLastState = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(212, 308);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 8;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(74, 308);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 7;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Enabled = false;
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(34, 272);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(352, 21);
            this.dateTimePickerAtDate.TabIndex = 6;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(11, 163);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(139, 15);
            this.label103.TabIndex = 53;
            this.label103.Text = "Номер лицевого счета";
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(12, 254);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(130, 15);
            this.label91.TabIndex = 52;
            this.label91.Text = "Дата формирования";
            // 
            // comboBoxState
            // 
            this.comboBoxState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxState.Enabled = false;
            this.comboBoxState.Location = new System.Drawing.Point(30, 40);
            this.comboBoxState.Name = "comboBoxState";
            this.comboBoxState.Size = new System.Drawing.Size(352, 23);
            this.comboBoxState.TabIndex = 1;
            // 
            // checkBoxAtDateChecked
            // 
            this.checkBoxAtDateChecked.AutoSize = true;
            this.checkBoxAtDateChecked.Location = new System.Drawing.Point(13, 276);
            this.checkBoxAtDateChecked.Name = "checkBoxAtDateChecked";
            this.checkBoxAtDateChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAtDateChecked.TabIndex = 5;
            this.checkBoxAtDateChecked.UseVisualStyleBackColor = true;
            this.checkBoxAtDateChecked.CheckedChanged += new System.EventHandler(this.checkBoxAtDateChecked_CheckedChanged);
            // 
            // checkBoxStateEnable
            // 
            this.checkBoxStateEnable.AutoSize = true;
            this.checkBoxStateEnable.Location = new System.Drawing.Point(9, 44);
            this.checkBoxStateEnable.Name = "checkBoxStateEnable";
            this.checkBoxStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStateEnable.TabIndex = 0;
            this.checkBoxStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxLastStateChecked_CheckedChanged);
            // 
            // checkBoxAccountChecked
            // 
            this.checkBoxAccountChecked.AutoSize = true;
            this.checkBoxAccountChecked.Location = new System.Drawing.Point(13, 185);
            this.checkBoxAccountChecked.Name = "checkBoxAccountChecked";
            this.checkBoxAccountChecked.Size = new System.Drawing.Size(15, 14);
            this.checkBoxAccountChecked.TabIndex = 1;
            this.checkBoxAccountChecked.UseVisualStyleBackColor = true;
            this.checkBoxAccountChecked.CheckedChanged += new System.EventHandler(this.checkBoxAccountChecked_CheckedChanged);
            // 
            // textBoxAccount
            // 
            this.textBoxAccount.Enabled = false;
            this.textBoxAccount.Location = new System.Drawing.Point(34, 181);
            this.textBoxAccount.Name = "textBoxAccount";
            this.textBoxAccount.Size = new System.Drawing.Size(352, 21);
            this.textBoxAccount.TabIndex = 2;
            // 
            // dateTimePickerDateStartStateTo
            // 
            this.dateTimePickerDateStartStateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateStartStateTo.Enabled = false;
            this.dateTimePickerDateStartStateTo.Location = new System.Drawing.Point(241, 91);
            this.dateTimePickerDateStartStateTo.Name = "dateTimePickerDateStartStateTo";
            this.dateTimePickerDateStartStateTo.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerDateStartStateTo.TabIndex = 5;
            // 
            // comboBoxDateStartStateExpr
            // 
            this.comboBoxDateStartStateExpr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDateStartStateExpr.Enabled = false;
            this.comboBoxDateStartStateExpr.FormattingEnabled = true;
            this.comboBoxDateStartStateExpr.Items.AddRange(new object[] {
            "≥",
            "≤",
            "=",
            "между"});
            this.comboBoxDateStartStateExpr.Location = new System.Drawing.Point(30, 90);
            this.comboBoxDateStartStateExpr.Name = "comboBoxDateStartStateExpr";
            this.comboBoxDateStartStateExpr.Size = new System.Drawing.Size(51, 23);
            this.comboBoxDateStartStateExpr.TabIndex = 3;
            // 
            // checkBoxDateStartStateEnable
            // 
            this.checkBoxDateStartStateEnable.AutoSize = true;
            this.checkBoxDateStartStateEnable.Location = new System.Drawing.Point(9, 94);
            this.checkBoxDateStartStateEnable.Name = "checkBoxDateStartStateEnable";
            this.checkBoxDateStartStateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDateStartStateEnable.TabIndex = 2;
            this.checkBoxDateStartStateEnable.UseVisualStyleBackColor = true;
            this.checkBoxDateStartStateEnable.CheckedChanged += new System.EventHandler(this.checkBoxDateStartStateEnable_CheckedChanged);
            // 
            // dateTimePickerDateStartStateFrom
            // 
            this.dateTimePickerDateStartStateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDateStartStateFrom.Enabled = false;
            this.dateTimePickerDateStartStateFrom.Location = new System.Drawing.Point(90, 91);
            this.dateTimePickerDateStartStateFrom.Name = "dateTimePickerDateStartStateFrom";
            this.dateTimePickerDateStartStateFrom.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerDateStartStateFrom.TabIndex = 4;
            // 
            // textBoxSRN
            // 
            this.textBoxSRN.Enabled = false;
            this.textBoxSRN.Location = new System.Drawing.Point(34, 224);
            this.textBoxSRN.Name = "textBoxSRN";
            this.textBoxSRN.Size = new System.Drawing.Size(352, 21);
            this.textBoxSRN.TabIndex = 4;
            // 
            // checkBoxSRNEnable
            // 
            this.checkBoxSRNEnable.AutoSize = true;
            this.checkBoxSRNEnable.Location = new System.Drawing.Point(13, 228);
            this.checkBoxSRNEnable.Name = "checkBoxSRNEnable";
            this.checkBoxSRNEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSRNEnable.TabIndex = 3;
            this.checkBoxSRNEnable.UseVisualStyleBackColor = true;
            this.checkBoxSRNEnable.CheckedChanged += new System.EventHandler(this.checkBoxSRNEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 15);
            this.label6.TabIndex = 201;
            this.label6.Text = "СРН";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxLastState);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxState);
            this.groupBox1.Controls.Add(this.dateTimePickerDateStartStateTo);
            this.groupBox1.Controls.Add(this.comboBoxDateStartStateExpr);
            this.groupBox1.Controls.Add(this.checkBoxStateEnable);
            this.groupBox1.Controls.Add(this.checkBoxDateStartStateEnable);
            this.groupBox1.Controls.Add(this.dateTimePickerDateStartStateFrom);
            this.groupBox1.Location = new System.Drawing.Point(4, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 149);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Состояние";
            // 
            // checkBoxLastState
            // 
            this.checkBoxLastState.AutoSize = true;
            this.checkBoxLastState.Location = new System.Drawing.Point(9, 118);
            this.checkBoxLastState.Name = "checkBoxLastState";
            this.checkBoxLastState.Size = new System.Drawing.Size(292, 19);
            this.checkBoxLastState.TabIndex = 6;
            this.checkBoxLastState.Text = "Фильтровать только по текущему состоянию";
            this.checkBoxLastState.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 15);
            this.label3.TabIndex = 193;
            this.label3.Text = "Дата установки";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 15);
            this.label2.TabIndex = 55;
            this.label2.Text = "Вид состояния";
            // 
            // SimpleSearchClaimsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(402, 351);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxSRN);
            this.Controls.Add(this.checkBoxSRNEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxAccount);
            this.Controls.Add(this.checkBoxAccountChecked);
            this.Controls.Add(this.checkBoxAtDateChecked);
            this.Controls.Add(this.label103);
            this.Controls.Add(this.label91);
            this.Controls.Add(this.dateTimePickerAtDate);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimpleSearchClaimsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация претензионно-исковых работ";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private vButton vButtonCancel;
        private vButton vButtonSearch;
        private DateTimePicker dateTimePickerAtDate;
        private Label label103;
        private Label label91;
        private ComboBox comboBoxState;
        private CheckBox checkBoxAtDateChecked;
        private CheckBox checkBoxStateEnable;
        private CheckBox checkBoxAccountChecked;
        private TextBox textBoxAccount;
        private DateTimePicker dateTimePickerDateStartStateTo;
        private ComboBox comboBoxDateStartStateExpr;
        private CheckBox checkBoxDateStartStateEnable;
        private DateTimePicker dateTimePickerDateStartStateFrom;
        private TextBox textBoxSRN;
        private CheckBox checkBoxSRNEnable;
        private Label label6;
        private GroupBox groupBox1;
        private CheckBox checkBoxLastState;
        private Label label2;
        private Label label3;
    }
}