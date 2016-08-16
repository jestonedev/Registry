using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class TenancyListViewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registration_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.residence_warrant_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
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
            this.id_process,
            this.registration_num,
            this.registration_date,
            this.end_date,
            this.residence_warrant_num,
            this.tenant,
            this.rent_type,
            this.address});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1166, 255);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // id_process
            // 
            this.id_process.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_process.HeaderText = "№";
            this.id_process.MinimumWidth = 60;
            this.id_process.Name = "id_process";
            this.id_process.ReadOnly = true;
            this.id_process.Width = 60;
            // 
            // registration_num
            // 
            this.registration_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.registration_num.HeaderText = "№ договора";
            this.registration_num.MinimumWidth = 90;
            this.registration_num.Name = "registration_num";
            this.registration_num.ReadOnly = true;
            this.registration_num.Width = 90;
            // 
            // registration_date
            // 
            this.registration_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.registration_date.HeaderText = "Дата регистрации договора";
            this.registration_date.MinimumWidth = 90;
            this.registration_date.Name = "registration_date";
            this.registration_date.ReadOnly = true;
            this.registration_date.Width = 90;
            // 
            // end_date
            // 
            this.end_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.end_date.HeaderText = "Дата окончания договора";
            this.end_date.MinimumWidth = 90;
            this.end_date.Name = "end_date";
            this.end_date.ReadOnly = true;
            this.end_date.Width = 90;
            // 
            // residence_warrant_num
            // 
            this.residence_warrant_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.residence_warrant_num.HeaderText = "№ ордера";
            this.residence_warrant_num.MinimumWidth = 90;
            this.residence_warrant_num.Name = "residence_warrant_num";
            this.residence_warrant_num.ReadOnly = true;
            this.residence_warrant_num.Width = 90;
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
            this.rent_type.MinimumWidth = 140;
            this.rent_type.Name = "rent_type";
            this.rent_type.ReadOnly = true;
            this.rent_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_type.Width = 140;
            // 
            // address
            // 
            this.address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.address.HeaderText = "Нанимаемое жилье";
            this.address.MinimumWidth = 400;
            this.address.Name = "address";
            this.address.ReadOnly = true;
            this.address.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.address.Width = 400;
            // 
            // TenancyListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1172, 261);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Процессы найма жилья";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewTextBoxColumn registration_date;
        private DataGridViewTextBoxColumn end_date;
        private DataGridViewTextBoxColumn residence_warrant_num;
        private DataGridViewTextBoxColumn tenant;
        private DataGridViewTextBoxColumn rent_type;
        private DataGridViewTextBoxColumn address;
    }
}
