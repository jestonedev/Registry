namespace Registry.Reporting.SettingForms
{
    partial class TenancyNotifiesSettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyNotifiesSettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxProlongContracts = new System.Windows.Forms.CheckBox();
            this.checkBoxWithoutRegNum = new System.Windows.Forms.CheckBox();
            this.comboBoxExecutor = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.vButtonExport = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxExpired = new System.Windows.Forms.CheckBox();
            this.checkBoxExpiring = new System.Windows.Forms.CheckBox();
            this.vButtonNotify = new VIBlend.WinForms.Controls.vButton();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.begin_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notify_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxCheckAll = new System.Windows.Forms.CheckBox();
            this.contextMenuStripNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.повторноеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.повторноеToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ответНаОбращениеПоПродлениюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenuStripNotify.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxProlongContracts);
            this.panel1.Controls.Add(this.checkBoxWithoutRegNum);
            this.panel1.Controls.Add(this.comboBoxExecutor);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.vButtonExport);
            this.panel1.Controls.Add(this.checkBoxExpired);
            this.panel1.Controls.Add(this.checkBoxExpiring);
            this.panel1.Controls.Add(this.vButtonNotify);
            this.panel1.Controls.Add(this.vButtonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 606);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(946, 95);
            this.panel1.TabIndex = 0;
            // 
            // checkBoxProlongContracts
            // 
            this.checkBoxProlongContracts.AutoSize = true;
            this.checkBoxProlongContracts.Location = new System.Drawing.Point(12, 71);
            this.checkBoxProlongContracts.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxProlongContracts.Name = "checkBoxProlongContracts";
            this.checkBoxProlongContracts.Size = new System.Drawing.Size(187, 17);
            this.checkBoxProlongContracts.TabIndex = 3;
            this.checkBoxProlongContracts.Text = "Процессы найма с продлением";
            this.checkBoxProlongContracts.UseVisualStyleBackColor = true;
            this.checkBoxProlongContracts.CheckedChanged += new System.EventHandler(this.checkBoxProlongContracts_CheckedChanged);
            // 
            // checkBoxWithoutRegNum
            // 
            this.checkBoxWithoutRegNum.AutoSize = true;
            this.checkBoxWithoutRegNum.Location = new System.Drawing.Point(12, 50);
            this.checkBoxWithoutRegNum.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxWithoutRegNum.Name = "checkBoxWithoutRegNum";
            this.checkBoxWithoutRegNum.Size = new System.Drawing.Size(190, 17);
            this.checkBoxWithoutRegNum.TabIndex = 2;
            this.checkBoxWithoutRegNum.Text = "Процессы найма без договоров";
            this.checkBoxWithoutRegNum.UseVisualStyleBackColor = true;
            this.checkBoxWithoutRegNum.CheckedChanged += new System.EventHandler(this.checkBoxWithoutRegNum_CheckedChanged);
            // 
            // comboBoxExecutor
            // 
            this.comboBoxExecutor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExecutor.FormattingEnabled = true;
            this.comboBoxExecutor.Location = new System.Drawing.Point(298, 45);
            this.comboBoxExecutor.Name = "comboBoxExecutor";
            this.comboBoxExecutor.Size = new System.Drawing.Size(249, 21);
            this.comboBoxExecutor.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(295, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 75;
            this.label4.Text = "Исполнитель";
            // 
            // vButtonExport
            // 
            this.vButtonExport.AllowAnimations = true;
            this.vButtonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonExport.BackColor = System.Drawing.Color.Transparent;
            this.vButtonExport.Location = new System.Drawing.Point(568, 38);
            this.vButtonExport.Name = "vButtonExport";
            this.vButtonExport.RoundedCornersMask = ((byte)(15));
            this.vButtonExport.Size = new System.Drawing.Size(117, 35);
            this.vButtonExport.TabIndex = 5;
            this.vButtonExport.Text = "Экспорт";
            this.vButtonExport.UseVisualStyleBackColor = false;
            this.vButtonExport.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonExport.Click += new System.EventHandler(this.vButtonExport_Click);
            // 
            // checkBoxExpired
            // 
            this.checkBoxExpired.AutoSize = true;
            this.checkBoxExpired.Location = new System.Drawing.Point(12, 29);
            this.checkBoxExpired.Name = "checkBoxExpired";
            this.checkBoxExpired.Size = new System.Drawing.Size(262, 17);
            this.checkBoxExpired.TabIndex = 1;
            this.checkBoxExpired.Text = "Договоры с закончившимся сроком действия";
            this.checkBoxExpired.UseVisualStyleBackColor = true;
            this.checkBoxExpired.CheckedChanged += new System.EventHandler(this.checkBoxExpired_CheckedChanged);
            // 
            // checkBoxExpiring
            // 
            this.checkBoxExpiring.AutoSize = true;
            this.checkBoxExpiring.Checked = true;
            this.checkBoxExpiring.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExpiring.Location = new System.Drawing.Point(12, 8);
            this.checkBoxExpiring.Name = "checkBoxExpiring";
            this.checkBoxExpiring.Size = new System.Drawing.Size(277, 17);
            this.checkBoxExpiring.TabIndex = 0;
            this.checkBoxExpiring.Text = "Договоры с заканчивающимся сроком действия";
            this.checkBoxExpiring.UseVisualStyleBackColor = true;
            this.checkBoxExpiring.CheckedChanged += new System.EventHandler(this.checkBoxExpiring_CheckedChanged);
            // 
            // vButtonNotify
            // 
            this.vButtonNotify.AllowAnimations = true;
            this.vButtonNotify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonNotify.BackColor = System.Drawing.Color.Transparent;
            this.vButtonNotify.Location = new System.Drawing.Point(692, 38);
            this.vButtonNotify.Name = "vButtonNotify";
            this.vButtonNotify.RoundedCornersMask = ((byte)(15));
            this.vButtonNotify.Size = new System.Drawing.Size(117, 35);
            this.vButtonNotify.TabIndex = 6;
            this.vButtonNotify.Text = "Уведомление";
            this.vButtonNotify.UseVisualStyleBackColor = false;
            this.vButtonNotify.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonNotify.Click += new System.EventHandler(this.vButtonNotify_Click);
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(816, 38);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 5;
            this.vButtonCancel.Text = "Отменить";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
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
            this.id_process,
            this.registration_num,
            this.registration_date,
            this.begin_date,
            this.end_date,
            this.notify_date,
            this.tenant,
            this.rent_type,
            this.address});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(946, 606);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValuePushed);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
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
            // id_process
            // 
            this.id_process.HeaderText = "id_process";
            this.id_process.Name = "id_process";
            this.id_process.ReadOnly = true;
            this.id_process.Visible = false;
            // 
            // registration_num
            // 
            this.registration_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.registration_num.HeaderText = "№ договора";
            this.registration_num.MinimumWidth = 120;
            this.registration_num.Name = "registration_num";
            this.registration_num.ReadOnly = true;
            this.registration_num.Width = 120;
            // 
            // registration_date
            // 
            this.registration_date.HeaderText = "Дата регистрации";
            this.registration_date.MinimumWidth = 140;
            this.registration_date.Name = "registration_date";
            this.registration_date.ReadOnly = true;
            this.registration_date.Width = 140;
            // 
            // begin_date
            // 
            this.begin_date.HeaderText = "Срок действия с";
            this.begin_date.MinimumWidth = 135;
            this.begin_date.Name = "begin_date";
            this.begin_date.ReadOnly = true;
            this.begin_date.Width = 135;
            // 
            // end_date
            // 
            this.end_date.HeaderText = "Срок действия по";
            this.end_date.MinimumWidth = 135;
            this.end_date.Name = "end_date";
            this.end_date.ReadOnly = true;
            this.end_date.Width = 135;
            // 
            // notify_date
            // 
            this.notify_date.HeaderText = "Дата уведомления";
            this.notify_date.MinimumWidth = 140;
            this.notify_date.Name = "notify_date";
            this.notify_date.ReadOnly = true;
            this.notify_date.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.notify_date.Width = 140;
            // 
            // tenant
            // 
            this.tenant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.tenant.HeaderText = "Наниматель";
            this.tenant.MinimumWidth = 250;
            this.tenant.Name = "tenant";
            this.tenant.ReadOnly = true;
            this.tenant.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.tenant.Width = 250;
            // 
            // rent_type
            // 
            this.rent_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.rent_type.HeaderText = "Тип найма";
            this.rent_type.MinimumWidth = 150;
            this.rent_type.Name = "id_rent_type";
            this.rent_type.ReadOnly = true;
            // this.rent_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_type.Width = 150;
            // 
            // address
            // 
            this.address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.address.HeaderText = "Нанимаемое жилье";
            this.address.MinimumWidth = 500;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            this.address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address.Width = 500;
            // 
            // checkBoxCheckAll
            // 
            this.checkBoxCheckAll.AutoSize = true;
            this.checkBoxCheckAll.BackColor = System.Drawing.Color.White;
            this.checkBoxCheckAll.Location = new System.Drawing.Point(9, 7);
            this.checkBoxCheckAll.Name = "checkBoxCheckAll";
            this.checkBoxCheckAll.Size = new System.Drawing.Size(15, 14);
            this.checkBoxCheckAll.TabIndex = 8;
            this.checkBoxCheckAll.UseVisualStyleBackColor = false;
            this.checkBoxCheckAll.CheckedChanged += new System.EventHandler(this.checkBoxCheckAll_CheckedChanged);
            // 
            // contextMenuStripNotify
            // 
            this.contextMenuStripNotify.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.повторноеToolStripMenuItem,
            this.повторноеToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.ответНаОбращениеПоПродлениюToolStripMenuItem});
            this.contextMenuStripNotify.Name = "contextMenuStripNotify";
            this.contextMenuStripNotify.Size = new System.Drawing.Size(273, 76);
            // 
            // повторноеToolStripMenuItem
            // 
            this.повторноеToolStripMenuItem.Name = "повторноеToolStripMenuItem";
            this.повторноеToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.повторноеToolStripMenuItem.Text = "Первичное";
            this.повторноеToolStripMenuItem.Click += new System.EventHandler(this.повторноеToolStripMenuItem_Click);
            // 
            // повторноеToolStripMenuItem1
            // 
            this.повторноеToolStripMenuItem1.Name = "повторноеToolStripMenuItem1";
            this.повторноеToolStripMenuItem1.Size = new System.Drawing.Size(272, 22);
            this.повторноеToolStripMenuItem1.Text = "Повторное";
            this.повторноеToolStripMenuItem1.Click += new System.EventHandler(this.повторноеToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(269, 6);
            // 
            // ответНаОбращениеПоПродлениюToolStripMenuItem
            // 
            this.ответНаОбращениеПоПродлениюToolStripMenuItem.Name = "ответНаОбращениеПоПродлениюToolStripMenuItem";
            this.ответНаОбращениеПоПродлениюToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.ответНаОбращениеПоПродлениюToolStripMenuItem.Text = "Ответ на обращение по продлению";
            this.ответНаОбращениеПоПродлениюToolStripMenuItem.Click += new System.EventHandler(this.ответНаОбращениеПоПродлениюToolStripMenuItem_Click);
            // 
            // TenancyNotifiesSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(946, 701);
            this.Controls.Add(this.checkBoxCheckAll);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(962, 298);
            this.Name = "TenancyNotifiesSettingsForm";
            this.Text = "Печать уведомлений";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStripNotify.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private VIBlend.WinForms.Controls.vButton vButtonNotify;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.CheckBox checkBoxCheckAll;
        private System.Windows.Forms.CheckBox checkBoxExpired;
        private System.Windows.Forms.CheckBox checkBoxExpiring;
        private VIBlend.WinForms.Controls.vButton vButtonExport;
        private System.Windows.Forms.ComboBox comboBoxExecutor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn is_checked;
        private System.Windows.Forms.DataGridViewTextBoxColumn id_process;
        private System.Windows.Forms.DataGridViewTextBoxColumn registration_num;
        private System.Windows.Forms.DataGridViewTextBoxColumn registration_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn begin_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn end_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn notify_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn tenant;
        private System.Windows.Forms.DataGridViewTextBoxColumn rent_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn address;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotify;
        private System.Windows.Forms.ToolStripMenuItem повторноеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem повторноеToolStripMenuItem1;
        private System.Windows.Forms.CheckBox checkBoxWithoutRegNum;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ответНаОбращениеПоПродлениюToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxProlongContracts;


    }
}