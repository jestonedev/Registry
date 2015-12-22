using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class ClaimListViewport
    {
        #region Components
        TableLayoutPanel tableLayoutPanel15;
        TableLayoutPanel tableLayoutPanel16;
        Panel panel8;
        Panel panel9;
        GroupBox groupBox34;
        NumericUpDown numericUpDownAmountDGI;
        NumericUpDown numericUpDownAmountTenancy;
        Label label91;
        Label label94;
        Label label95;
        Label label96;
        Label label97;
        Label label98;
        Label label99;
        Label label103;
        TextBox textBoxDescription;
        DateTimePicker dateTimePickerStartDeptPeriod;
        DateTimePicker dateTimePickerEndDeptPeriod;
        DateTimePicker dateTimePickerAtDate;
        DataGridView dataGridViewClaims;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimListViewport));
            this.tableLayoutPanel15 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel16 = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.comboBoxAccount = new System.Windows.Forms.ComboBox();
            this.label103 = new System.Windows.Forms.Label();
            this.dateTimePickerAtDate = new System.Windows.Forms.DateTimePicker();
            this.label91 = new System.Windows.Forms.Label();
            this.dateTimePickerEndDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.label96 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.dateTimePickerStartDeptPeriod = new System.Windows.Forms.DateTimePicker();
            this.label97 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.numericUpDownAmountDGI = new System.Windows.Forms.NumericUpDown();
            this.label95 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.numericUpDownAmountTenancy = new System.Windows.Forms.NumericUpDown();
            this.label99 = new System.Windows.Forms.Label();
            this.label94 = new System.Windows.Forms.Label();
            this.dataGridViewClaims = new System.Windows.Forms.DataGridView();
            this.id_claim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.at_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.current_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.start_dept_period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_dept_period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_tenancy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amount_dgi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel15.SuspendLayout();
            this.groupBox34.SuspendLayout();
            this.tableLayoutPanel16.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountDGI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountTenancy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel15
            // 
            this.tableLayoutPanel15.ColumnCount = 1;
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Controls.Add(this.groupBox34, 0, 0);
            this.tableLayoutPanel15.Controls.Add(this.dataGridViewClaims, 0, 1);
            this.tableLayoutPanel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel15.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel15.Name = "tableLayoutPanel15";
            this.tableLayoutPanel15.RowCount = 2;
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Size = new System.Drawing.Size(680, 491);
            this.tableLayoutPanel15.TabIndex = 0;
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.tableLayoutPanel16);
            this.groupBox34.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox34.Location = new System.Drawing.Point(0, 0);
            this.groupBox34.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.Size = new System.Drawing.Size(680, 140);
            this.groupBox34.TabIndex = 1;
            this.groupBox34.TabStop = false;
            this.groupBox34.Text = "Общие сведения";
            // 
            // tableLayoutPanel16
            // 
            this.tableLayoutPanel16.ColumnCount = 2;
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel16.Controls.Add(this.panel8, 0, 0);
            this.tableLayoutPanel16.Controls.Add(this.panel9, 1, 0);
            this.tableLayoutPanel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel16.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel16.Name = "tableLayoutPanel16";
            this.tableLayoutPanel16.RowCount = 1;
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tableLayoutPanel16.Size = new System.Drawing.Size(674, 120);
            this.tableLayoutPanel16.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.comboBoxAccount);
            this.panel8.Controls.Add(this.label103);
            this.panel8.Controls.Add(this.dateTimePickerAtDate);
            this.panel8.Controls.Add(this.label91);
            this.panel8.Controls.Add(this.dateTimePickerEndDeptPeriod);
            this.panel8.Controls.Add(this.label96);
            this.panel8.Controls.Add(this.label98);
            this.panel8.Controls.Add(this.dateTimePickerStartDeptPeriod);
            this.panel8.Controls.Add(this.label97);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(337, 120);
            this.panel8.TabIndex = 0;
            // 
            // comboBoxAccount
            // 
            this.comboBoxAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAccount.Location = new System.Drawing.Point(170, 5);
            this.comboBoxAccount.Name = "comboBoxAccount";
            this.comboBoxAccount.Size = new System.Drawing.Size(156, 23);
            this.comboBoxAccount.TabIndex = 0;
            this.comboBoxAccount.DropDownClosed += new System.EventHandler(this.comboBoxAccount_DropDownClosed);
            this.comboBoxAccount.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxAccount_KeyUp);
            this.comboBoxAccount.Leave += new System.EventHandler(this.comboBoxAccount_Leave);
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(12, 7);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(139, 15);
            this.label103.TabIndex = 51;
            this.label103.Text = "Номер лицевого счета";
            // 
            // dateTimePickerAtDate
            // 
            this.dateTimePickerAtDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerAtDate.Location = new System.Drawing.Point(170, 35);
            this.dateTimePickerAtDate.Name = "dateTimePickerAtDate";
            this.dateTimePickerAtDate.ShowCheckBox = true;
            this.dateTimePickerAtDate.Size = new System.Drawing.Size(156, 21);
            this.dateTimePickerAtDate.TabIndex = 1;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(12, 37);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(130, 15);
            this.label91.TabIndex = 31;
            this.label91.Text = "Дата формирования";
            // 
            // dateTimePickerEndDeptPeriod
            // 
            this.dateTimePickerEndDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerEndDeptPeriod.Location = new System.Drawing.Point(170, 92);
            this.dateTimePickerEndDeptPeriod.Name = "dateTimePickerEndDeptPeriod";
            this.dateTimePickerEndDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerEndDeptPeriod.Size = new System.Drawing.Size(156, 21);
            this.dateTimePickerEndDeptPeriod.TabIndex = 3;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(12, 65);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(124, 15);
            this.label96.TabIndex = 44;
            this.label96.Text = "Предъявлен период";
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(143, 94);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(21, 15);
            this.label98.TabIndex = 48;
            this.label98.Text = "по";
            // 
            // dateTimePickerStartDeptPeriod
            // 
            this.dateTimePickerStartDeptPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerStartDeptPeriod.Location = new System.Drawing.Point(170, 63);
            this.dateTimePickerStartDeptPeriod.Name = "dateTimePickerStartDeptPeriod";
            this.dateTimePickerStartDeptPeriod.ShowCheckBox = true;
            this.dateTimePickerStartDeptPeriod.Size = new System.Drawing.Size(156, 21);
            this.dateTimePickerStartDeptPeriod.TabIndex = 2;
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(149, 65);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(13, 15);
            this.label97.TabIndex = 46;
            this.label97.Text = "с";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.numericUpDownAmountDGI);
            this.panel9.Controls.Add(this.label95);
            this.panel9.Controls.Add(this.textBoxDescription);
            this.panel9.Controls.Add(this.numericUpDownAmountTenancy);
            this.panel9.Controls.Add(this.label99);
            this.panel9.Controls.Add(this.label94);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(337, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(337, 120);
            this.panel9.TabIndex = 1;
            // 
            // numericUpDownAmountDGI
            // 
            this.numericUpDownAmountDGI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountDGI.DecimalPlaces = 2;
            this.numericUpDownAmountDGI.Location = new System.Drawing.Point(172, 35);
            this.numericUpDownAmountDGI.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountDGI.Name = "numericUpDownAmountDGI";
            this.numericUpDownAmountDGI.Size = new System.Drawing.Size(155, 21);
            this.numericUpDownAmountDGI.TabIndex = 1;
            this.numericUpDownAmountDGI.ThousandsSeparator = true;
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(14, 37);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(149, 15);
            this.label95.TabIndex = 39;
            this.label95.Text = "Сумма к взысканию ДГИ";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(172, 63);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(155, 50);
            this.textBoxDescription.TabIndex = 2;
            // 
            // numericUpDownAmountTenancy
            // 
            this.numericUpDownAmountTenancy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmountTenancy.DecimalPlaces = 2;
            this.numericUpDownAmountTenancy.Location = new System.Drawing.Point(172, 6);
            this.numericUpDownAmountTenancy.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownAmountTenancy.Name = "numericUpDownAmountTenancy";
            this.numericUpDownAmountTenancy.Size = new System.Drawing.Size(155, 21);
            this.numericUpDownAmountTenancy.TabIndex = 0;
            this.numericUpDownAmountTenancy.ThousandsSeparator = true;
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(14, 65);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(80, 15);
            this.label99.TabIndex = 49;
            this.label99.Text = "Примечание";
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(14, 8);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(154, 15);
            this.label94.TabIndex = 37;
            this.label94.Text = "Сумма к взысканию найм";
            // 
            // dataGridViewClaims
            // 
            this.dataGridViewClaims.AllowUserToAddRows = false;
            this.dataGridViewClaims.AllowUserToDeleteRows = false;
            this.dataGridViewClaims.AllowUserToResizeRows = false;
            this.dataGridViewClaims.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewClaims.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewClaims.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaims.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_claim,
            this.id_account,
            this.at_date,
            this.current_state,
            this.start_dept_period,
            this.end_dept_period,
            this.amount_tenancy,
            this.amount_dgi,
            this.description});
            this.dataGridViewClaims.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewClaims.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewClaims.Location = new System.Drawing.Point(3, 143);
            this.dataGridViewClaims.MultiSelect = false;
            this.dataGridViewClaims.Name = "dataGridViewClaims";
            this.dataGridViewClaims.ReadOnly = true;
            this.dataGridViewClaims.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewClaims.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewClaims.ShowCellToolTips = false;
            this.dataGridViewClaims.Size = new System.Drawing.Size(674, 345);
            this.dataGridViewClaims.TabIndex = 0;
            this.dataGridViewClaims.VirtualMode = true;
            this.dataGridViewClaims.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridViewClaims_CellValueNeeded);
            this.dataGridViewClaims.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewClaims_ColumnHeaderMouseClick);
            this.dataGridViewClaims.SelectionChanged += new System.EventHandler(this.dataGridViewClaims_SelectionChanged);
            // 
            // id_claim
            // 
            this.id_claim.HeaderText = "№";
            this.id_claim.MinimumWidth = 50;
            this.id_claim.Name = "id_claim";
            this.id_claim.ReadOnly = true;
            this.id_claim.Width = 50;
            // 
            // id_account
            // 
            this.id_account.HeaderText = "Лицевой счет";
            this.id_account.MinimumWidth = 150;
            this.id_account.Name = "id_account";
            this.id_account.ReadOnly = true;
            this.id_account.Width = 150;
            // 
            // at_date
            // 
            this.at_date.HeaderText = "Дата формирования";
            this.at_date.MinimumWidth = 170;
            this.at_date.Name = "at_date";
            this.at_date.ReadOnly = true;
            this.at_date.Width = 170;
            // 
            // current_state
            // 
            this.current_state.HeaderText = "Текущее состояние";
            this.current_state.MinimumWidth = 150;
            this.current_state.Name = "current_state";
            this.current_state.ReadOnly = true;
            this.current_state.Width = 150;
            // 
            // start_dept_period
            // 
            this.start_dept_period.HeaderText = "Период с";
            this.start_dept_period.MinimumWidth = 150;
            this.start_dept_period.Name = "start_dept_period";
            this.start_dept_period.ReadOnly = true;
            this.start_dept_period.Width = 150;
            // 
            // end_dept_period
            // 
            this.end_dept_period.HeaderText = "Период по";
            this.end_dept_period.MinimumWidth = 150;
            this.end_dept_period.Name = "end_dept_period";
            this.end_dept_period.ReadOnly = true;
            this.end_dept_period.Width = 150;
            // 
            // amount_tenancy
            // 
            dataGridViewCellStyle1.Format = "#0.0# руб.";
            this.amount_tenancy.DefaultCellStyle = dataGridViewCellStyle1;
            this.amount_tenancy.HeaderText = "Сумма долга найм";
            this.amount_tenancy.MinimumWidth = 200;
            this.amount_tenancy.Name = "amount_tenancy";
            this.amount_tenancy.ReadOnly = true;
            this.amount_tenancy.Width = 200;
            // 
            // amount_dgi
            // 
            dataGridViewCellStyle2.Format = "#0.0# руб.";
            this.amount_dgi.DefaultCellStyle = dataGridViewCellStyle2;
            this.amount_dgi.HeaderText = "Сумма долга ДГИ";
            this.amount_dgi.MinimumWidth = 200;
            this.amount_dgi.Name = "amount_dgi";
            this.amount_dgi.ReadOnly = true;
            this.amount_dgi.Width = 200;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 200;
            this.description.Name = "description";
            this.description.ReadOnly = true;
            // 
            // ClaimListViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(680, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(681, 497);
            this.Controls.Add(this.tableLayoutPanel15);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClaimListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исковая работа №{0}";
            this.tableLayoutPanel15.ResumeLayout(false);
            this.groupBox34.ResumeLayout(false);
            this.tableLayoutPanel16.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountDGI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmountTenancy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).EndInit();
            this.ResumeLayout(false);

        }
        private ComboBox comboBoxAccount;
        private DataGridViewTextBoxColumn id_claim;
        private DataGridViewTextBoxColumn id_account;
        private DataGridViewTextBoxColumn at_date;
        private DataGridViewTextBoxColumn current_state;
        private DataGridViewTextBoxColumn start_dept_period;
        private DataGridViewTextBoxColumn end_dept_period;
        private DataGridViewTextBoxColumn amount_tenancy;
        private DataGridViewTextBoxColumn amount_dgi;
        private DataGridViewTextBoxColumn description;
    }
}
