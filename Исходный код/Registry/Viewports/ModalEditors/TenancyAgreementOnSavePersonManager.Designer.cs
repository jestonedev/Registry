namespace Registry.Viewport.ModalEditors
{
    partial class TenancyAgreementOnSavePersonManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyAgreementOnSavePersonManager));
            this.vButtonSave = new VIBlend.WinForms.Controls.vButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.dateTimePickerDate = new System.Windows.Forms.DateTimePicker();
            this.labelDate = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id_person = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date_of_birth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kinship = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_kinship = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // vButtonSave
            // 
            this.vButtonSave.AllowAnimations = true;
            this.vButtonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonSave.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSave.Location = new System.Drawing.Point(470, 10);
            this.vButtonSave.Name = "vButtonSave";
            this.vButtonSave.RoundedCornersMask = ((byte)(15));
            this.vButtonSave.Size = new System.Drawing.Size(117, 35);
            this.vButtonSave.TabIndex = 1;
            this.vButtonSave.Text = "Сохранить";
            this.vButtonSave.UseVisualStyleBackColor = false;
            this.vButtonSave.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSave.Click += new System.EventHandler(this.vButtonSave_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.vButtonCancel);
            this.panel1.Controls.Add(this.dateTimePickerDate);
            this.panel1.Controls.Add(this.labelDate);
            this.panel1.Controls.Add(this.vButtonSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 266);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(722, 54);
            this.panel1.TabIndex = 9;
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(593, 10);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 2;
            this.vButtonCancel.Text = "Не сохранять";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // dateTimePickerDate
            // 
            this.dateTimePickerDate.Location = new System.Drawing.Point(12, 22);
            this.dateTimePickerDate.Name = "dateTimePickerDate";
            this.dateTimePickerDate.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerDate.TabIndex = 0;
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Location = new System.Drawing.Point(9, 6);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(91, 13);
            this.labelDate.TabIndex = 75;
            this.labelDate.Text = "Дата включения";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.is_checked,
            this.id_person,
            this.id_process,
            this.surname,
            this.name,
            this.patronymic,
            this.date_of_birth,
            this.kinship,
            this.id_kinship});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(722, 320);
            this.dataGridView.TabIndex = 0;
            // 
            // is_checked
            // 
            this.is_checked.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.is_checked.HeaderText = "";
            this.is_checked.MinimumWidth = 30;
            this.is_checked.Name = "is_checked";
            this.is_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.is_checked.Width = 30;
            // 
            // id_person
            // 
            this.id_person.HeaderText = "id_person";
            this.id_person.Name = "id_person";
            this.id_person.ReadOnly = true;
            this.id_person.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id_person.Visible = false;
            // 
            // id_process
            // 
            this.id_process.HeaderText = "id_process";
            this.id_process.Name = "id_process";
            this.id_process.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id_process.Visible = false;
            // 
            // surname
            // 
            this.surname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.surname.HeaderText = "Фамилия";
            this.surname.MinimumWidth = 120;
            this.surname.Name = "surname";
            this.surname.ReadOnly = true;
            this.surname.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.surname.Width = 120;
            // 
            // name
            // 
            this.name.HeaderText = "Имя";
            this.name.MinimumWidth = 120;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.name.Width = 120;
            // 
            // patronymic
            // 
            this.patronymic.HeaderText = "Отчество";
            this.patronymic.MinimumWidth = 120;
            this.patronymic.Name = "patronymic";
            this.patronymic.ReadOnly = true;
            this.patronymic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.patronymic.Width = 120;
            // 
            // date_of_birth
            // 
            dataGridViewCellStyle2.Format = "d";
            dataGridViewCellStyle2.NullValue = null;
            this.date_of_birth.DefaultCellStyle = dataGridViewCellStyle2;
            this.date_of_birth.HeaderText = "Дата рождения";
            this.date_of_birth.MinimumWidth = 130;
            this.date_of_birth.Name = "date_of_birth";
            this.date_of_birth.ReadOnly = true;
            this.date_of_birth.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.date_of_birth.Width = 130;
            // 
            // kinship
            // 
            this.kinship.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.kinship.HeaderText = "Родственное отношение";
            this.kinship.MinimumWidth = 200;
            this.kinship.Name = "kinship";
            this.kinship.ReadOnly = true;
            this.kinship.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.kinship.Width = 200;
            // 
            // id_kinship
            // 
            this.id_kinship.HeaderText = "id_kinship";
            this.id_kinship.Name = "id_kinship";
            this.id_kinship.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id_kinship.Visible = false;
            // 
            // TenancyAgreementOnSavePersonManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(722, 320);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyAgreementOnSavePersonManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Включить участников найма";
            this.Shown += new System.EventHandler(this.TenancyAgreementOnSavePersonManager_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker dateTimePickerDate;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.DataGridView dataGridView;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn is_checked;
        private System.Windows.Forms.DataGridViewTextBoxColumn id_person;
        private System.Windows.Forms.DataGridViewTextBoxColumn id_process;
        private System.Windows.Forms.DataGridViewTextBoxColumn surname;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn patronymic;
        private System.Windows.Forms.DataGridViewTextBoxColumn date_of_birth;
        private System.Windows.Forms.DataGridViewTextBoxColumn kinship;
        private System.Windows.Forms.DataGridViewTextBoxColumn id_kinship;

    }
}