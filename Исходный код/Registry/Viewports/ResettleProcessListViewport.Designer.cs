using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class ResettleProcessListViewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel6;
        private GroupBox groupBox15;
        private TextBox textBoxDescription;
        private GroupBox groupBox14;
        private NumericUpDown numericUpDownDebts;
        private DateTimePicker dateTimePickerResettleDate;
        private Label label37;
        private Label label36;
        private ComboBox comboBoxDocumentResidence;
        private Label label35;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn resettle_date;
        private DataGridViewTextBoxColumn resettle_persons;
        private DataGridViewTextBoxColumn address_from;
        private DataGridViewTextBoxColumn address_to;
        #endregion Components


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResettleProcessListViewport));
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resettle_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resettle_persons = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address_from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address_to = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.numericUpDownDebts = new System.Windows.Forms.NumericUpDown();
            this.dateTimePickerResettleDate = new System.Windows.Forms.DateTimePicker();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.comboBoxDocumentResidence = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBox15.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDebts)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.groupBox15, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.groupBox14, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(708, 336);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_process,
            this.resettle_date,
            this.resettle_persons,
            this.address_from,
            this.address_to});
            this.tableLayoutPanel6.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 119);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(702, 214);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // id_process
            // 
            this.id_process.HeaderText = "№";
            this.id_process.MinimumWidth = 100;
            this.id_process.Name = "id_process";
            this.id_process.ReadOnly = true;
            // 
            // resettle_date
            // 
            this.resettle_date.HeaderText = "Дата переселения";
            this.resettle_date.MinimumWidth = 150;
            this.resettle_date.Name = "resettle_date";
            this.resettle_date.ReadOnly = true;
            this.resettle_date.Width = 150;
            // 
            // resettle_persons
            // 
            this.resettle_persons.HeaderText = "Участники";
            this.resettle_persons.MinimumWidth = 250;
            this.resettle_persons.Name = "resettle_persons";
            this.resettle_persons.ReadOnly = true;
            this.resettle_persons.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.resettle_persons.Width = 250;
            // 
            // address_from
            // 
            this.address_from.HeaderText = "Адрес (откуда)";
            this.address_from.MinimumWidth = 500;
            this.address_from.Name = "address_from";
            this.address_from.ReadOnly = true;
            this.address_from.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address_from.Width = 500;
            // 
            // address_to
            // 
            this.address_to.HeaderText = "Адрес (куда)";
            this.address_to.MinimumWidth = 500;
            this.address_to.Name = "address_to";
            this.address_to.ReadOnly = true;
            this.address_to.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address_to.Width = 500;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.textBoxDescription);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(357, 3);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(348, 110);
            this.groupBox15.TabIndex = 2;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Дополнительные сведения";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(3, 17);
            this.textBoxDescription.MaxLength = 4000;
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(342, 90);
            this.textBoxDescription.TabIndex = 0;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.numericUpDownDebts);
            this.groupBox14.Controls.Add(this.dateTimePickerResettleDate);
            this.groupBox14.Controls.Add(this.label37);
            this.groupBox14.Controls.Add(this.label36);
            this.groupBox14.Controls.Add(this.comboBoxDocumentResidence);
            this.groupBox14.Controls.Add(this.label35);
            this.groupBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox14.Location = new System.Drawing.Point(3, 3);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(348, 110);
            this.groupBox14.TabIndex = 1;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Общие сведения";
            // 
            // numericUpDownDebts
            // 
            this.numericUpDownDebts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownDebts.DecimalPlaces = 2;
            this.numericUpDownDebts.Location = new System.Drawing.Point(160, 51);
            this.numericUpDownDebts.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownDebts.Name = "numericUpDownDebts";
            this.numericUpDownDebts.Size = new System.Drawing.Size(181, 21);
            this.numericUpDownDebts.TabIndex = 1;
            this.numericUpDownDebts.ThousandsSeparator = true;
            // 
            // dateTimePickerResettleDate
            // 
            this.dateTimePickerResettleDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerResettleDate.Location = new System.Drawing.Point(161, 80);
            this.dateTimePickerResettleDate.Name = "dateTimePickerResettleDate";
            this.dateTimePickerResettleDate.ShowCheckBox = true;
            this.dateTimePickerResettleDate.Size = new System.Drawing.Size(181, 21);
            this.dateTimePickerResettleDate.TabIndex = 2;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(14, 82);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(116, 15);
            this.label37.TabIndex = 3;
            this.label37.Text = "Дата переселения";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(14, 54);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(86, 15);
            this.label36.TabIndex = 4;
            this.label36.Text = "Задолжность";
            // 
            // comboBoxDocumentResidence
            // 
            this.comboBoxDocumentResidence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDocumentResidence.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDocumentResidence.FormattingEnabled = true;
            this.comboBoxDocumentResidence.Location = new System.Drawing.Point(161, 22);
            this.comboBoxDocumentResidence.Name = "comboBoxDocumentResidence";
            this.comboBoxDocumentResidence.Size = new System.Drawing.Size(181, 23);
            this.comboBoxDocumentResidence.TabIndex = 0;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(14, 25);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(146, 15);
            this.label35.TabIndex = 5;
            this.label35.Text = "Основание проживания";
            // 
            // ResettleProcessListViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(640, 320);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(714, 342);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResettleProcessListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процесс переселения №{0}";
            this.tableLayoutPanel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDebts)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
