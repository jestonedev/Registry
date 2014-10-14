using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal sealed class TenancyListViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_contract = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_registraction_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_residence_warrant_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_kumi_order_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_address = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_tenant = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        public TenancyListViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageBuildings";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Найм жилья";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font =
                new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_contract,
            this.field_registraction_num,
            this.field_residence_warrant_num,
            this.field_kumi_order_num,
            this.field_address,
            this.field_tenant});
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.TabIndex = 0;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.MultiSelect = false;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ReadOnly = true;
            this.dataGridView.VirtualMode = true; 

            // 
            // field_id_contract
            // 
            this.field_id_contract.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_id_contract.HeaderText = "№ записи";
            this.field_id_contract.MinimumWidth = 100;
            this.field_id_contract.Name = "id_contract";
            // 
            // field_registraction_num
            // 
            this.field_registraction_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_registraction_num.HeaderText = "№ договора";
            this.field_registraction_num.MinimumWidth = 120;
            this.field_registraction_num.Name = "registraction_num";
            // 
            // field_residence_warrant_num
            // 
            this.field_residence_warrant_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_residence_warrant_num.HeaderText = "№ ордера";
            this.field_residence_warrant_num.MinimumWidth = 120;
            this.field_residence_warrant_num.Name = "residence_warrant_num";
            // 
            // field_kumi_order_num
            // 
            this.field_kumi_order_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_kumi_order_num.HeaderText = "№ распоряжения";
            this.field_kumi_order_num.MinimumWidth = 120;
            this.field_kumi_order_num.Name = "kumi_order_num";
            // 
            // field_address
            // 
            this.field_address.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_address.HeaderText = "Нанимаемое жилье";
            this.field_address.MinimumWidth = 300;
            this.field_address.Name = "address";
            // 
            // field_tenant
            // 
            this.field_tenant.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.field_tenant.HeaderText = "Наниматель";
            this.field_tenant.MinimumWidth = 200;
            this.field_tenant.Name = "tenant";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
