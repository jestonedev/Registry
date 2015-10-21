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
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn registration_num;
        private DataGridViewTextBoxColumn residence_warrant_num;
        private DataGridViewTextBoxColumn tenant;
        private DataGridViewTextBoxColumn rent_type;
        private DataGridViewTextBoxColumn address;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyListViewport));
            dataGridView = new DataGridView();
            id_process = new DataGridViewTextBoxColumn();
            registration_num = new DataGridViewTextBoxColumn();
            residence_warrant_num = new DataGridViewTextBoxColumn();
            tenant = new DataGridViewTextBoxColumn();
            rent_type = new DataGridViewTextBoxColumn();
            address = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_process, registration_num, residence_warrant_num, tenant, rent_type, address);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(1166, 255);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CellDoubleClick += dataGridView_CellDoubleClick;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            dataGridView.Resize += dataGridView_Resize;
            // 
            // id_process
            // 
            id_process.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            id_process.HeaderText = "№";
            id_process.MinimumWidth = 100;
            id_process.Name = "id_process";
            id_process.ReadOnly = true;
            // 
            // registration_num
            // 
            registration_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            registration_num.HeaderText = "№ договора";
            registration_num.MinimumWidth = 130;
            registration_num.Name = "registration_num";
            registration_num.ReadOnly = true;
            registration_num.Width = 130;
            // 
            // residence_warrant_num
            // 
            residence_warrant_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            residence_warrant_num.HeaderText = "№ ордера";
            residence_warrant_num.MinimumWidth = 130;
            residence_warrant_num.Name = "residence_warrant_num";
            residence_warrant_num.ReadOnly = true;
            residence_warrant_num.Width = 130;
            // 
            // tenant
            // 
            tenant.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tenant.HeaderText = "Наниматель";
            tenant.MinimumWidth = 250;
            tenant.Name = "tenant";
            tenant.ReadOnly = true;
            tenant.SortMode = DataGridViewColumnSortMode.NotSortable;
            tenant.Width = 250;
            // 
            // rent_type
            // 
            rent_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            rent_type.HeaderText = "Тип найма";
            rent_type.MinimumWidth = 150;
            rent_type.Name = "rent_type";
            rent_type.ReadOnly = true;
            rent_type.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_type.Width = 150;
            // 
            // address
            // 
            address.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            address.HeaderText = "Нанимаемое жилье";
            address.MinimumWidth = 500;
            address.Name = "address";
            address.ReadOnly = true;
            address.SortMode = DataGridViewColumnSortMode.NotSortable;
            address.Width = 500;
            // 
            // TenancyListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(1172, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyListViewport";
            Padding = new Padding(3);
            Text = "Процессы найма жилья";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
