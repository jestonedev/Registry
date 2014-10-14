using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal sealed class ExecutorsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_executor = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_executor_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_executor_login = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        public ExecutorsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageExecutors";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исполнители";
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
            this.field_id_executor,
            this.field_executor_name,
            this.field_executor_login});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 8;
            this.dataGridView.MultiSelect = false;
            // 
            // field_id_executor
            // 
            this.field_id_executor.Frozen = true;
            this.field_id_executor.HeaderText = "Идентификатор исполнителя";
            this.field_id_executor.Name = "id_executor";
            this.field_id_executor.ReadOnly = true;
            this.field_id_executor.Visible = false;
            // 
            // field_executor_name
            // 
            this.field_executor_name.HeaderText = "ФИО исполнителя";
            this.field_executor_name.MinimumWidth = 100;
            this.field_executor_name.Name = "executor_name";
            // 
            // field_executor_login
            // 
            this.field_executor_login.HeaderText = "Логин исполнителя";
            this.field_executor_login.MinimumWidth = 100;
            this.field_executor_login.Name = "executor_login";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
