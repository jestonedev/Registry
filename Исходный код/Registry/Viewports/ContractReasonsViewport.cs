using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using CustomControls;

namespace Registry.Viewport
{
    internal sealed class ContractReasonsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_contract_reason = new DataGridViewTextBoxColumn();
        private DataGridViewComboBoxColumn field_id_reason_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_reason_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_reason_date = new CustomControls.DateGridViewDateTimeColumn();
        #endregion Components

        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        public ContractReasonsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageContractReasons";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Основания найма №{0}";
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
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_contract_reason,
            this.field_id_reason_type,
            this.field_reason_number,
            this.field_reason_date});         
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 5;
            // 
            // field_id_contract_reason
            // 
            this.field_id_contract_reason.HeaderText = "Идентификатор основания";
            this.field_id_contract_reason.Name = "id_contract_reason";
            this.field_id_contract_reason.ReadOnly = true;
            this.field_id_contract_reason.Visible = false;
            // 
            // field_id_reason_type
            // 
            this.field_id_reason_type.HeaderText = "Тип основания";
            this.field_id_reason_type.Name = "id_reason_type";
            // 
            // field_reason_number
            // 
            this.field_reason_number.HeaderText = "Номер основания";
            this.field_reason_number.Name = "reason_number";
            // 
            // field_reason_date
            // 
            this.field_reason_date.HeaderText = "Дата основания";
            this.field_reason_date.Name = "reason_date";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
