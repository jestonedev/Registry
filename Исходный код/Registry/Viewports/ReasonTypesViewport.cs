using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport
{
    internal sealed class ReasonTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_reason_type = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_name = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_reason_template = new DataGridViewTextBoxColumn();
        #endregion Components

        public ReasonTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageReasonTypes";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Виды оснований";
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
            this.field_id_reason_type,
            this.field_reason_name,
            this.field_reason_template});          
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 7;
            this.dataGridView.MultiSelect = false;
            // 
            // field_id_reason_type
            // 
            this.field_id_reason_type.Frozen = true;
            this.field_id_reason_type.HeaderText = "Идентификатор вида основания";
            this.field_id_reason_type.Name = "id_reason_type";
            this.field_id_reason_type.ReadOnly = true;
            this.field_id_reason_type.Visible = false;
            // 
            // field_reason_name
            // 
            this.field_reason_name.FillWeight = 200F;
            this.field_reason_name.HeaderText = "Имя вида основания";
            this.field_reason_name.MinimumWidth = 100;
            this.field_reason_name.Name = "reason_name";
            // 
            // field_reason_template
            // 
            this.field_reason_template.FillWeight = 500F;
            this.field_reason_template.HeaderText = "Шаблон вида основания";
            this.field_reason_template.MinimumWidth = 100;
            this.field_reason_template.Name = "reason_template";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
