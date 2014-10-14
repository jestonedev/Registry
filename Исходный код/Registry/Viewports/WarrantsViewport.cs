using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using CustomControls;

namespace Registry.Viewport
{
    internal sealed class WarrantsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_warrant = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_registraction_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DateGridViewDateTimeColumn field_registraction_date = new CustomControls.DateGridViewDateTimeColumn();
        private DataGridViewComboBoxColumn field_id_warrant_doc_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
        private DataGridViewTextBoxColumn field_notary = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_on_behalf_of = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        public WarrantsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageWarrants";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Реестр доверенностей";
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
            this.field_id_warrant,
            this.field_registraction_num,
            this.field_registraction_date,
            this.field_notary,
            this.field_on_behalf_of,
            this.field_description});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 6;
            this.dataGridView.MultiSelect = false;
            // 
            // field_id_warrant
            // 
            this.field_id_warrant.Frozen = true;
            this.field_id_warrant.HeaderText = "Идентификатор доверенности";
            this.field_id_warrant.Name = "id_warrant";
            this.field_id_warrant.ReadOnly = true;
            this.field_id_warrant.Visible = false;
            // 
            // field_registraction_num
            // 
            this.field_registraction_num.HeaderText = "Регистрационный №";
            this.field_registraction_num.Name = "registraction_num";
            // 
            // field_registraction_date
            // 
            this.field_registraction_date.HeaderText = "Дата";
            this.field_registraction_date.Name = "registraction_date";
            //
            // field_id_warrant_doc_type
            //
            this.field_id_warrant_doc_type.HeaderText = "Тип документа";
            this.field_id_warrant_doc_type.Name = "id_warrant_doc_type";
            // 
            // field_notary
            // 
            this.field_notary.HeaderText = "Нотариус";
            this.field_notary.Name = "notary";
            // 
            // field_on_behalf_of
            // 
            this.field_on_behalf_of.HeaderText = "В лице кого";
            this.field_on_behalf_of.Name = "on_behalf_of";
            // 
            // field_description
            // 
            this.field_description.HeaderText = "Примечание";
            this.field_description.Name = "description";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
