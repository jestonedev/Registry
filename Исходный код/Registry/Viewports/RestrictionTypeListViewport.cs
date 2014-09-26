using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal class RestrictionTypeListViewport: Viewport
    {
        private DataGridView dataGridView = new DataGridView();
        private DataGridViewTextBoxColumn field_id_restriction_type = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_number = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_description = new DataGridViewTextBoxColumn();

        public RestrictionTypeListViewport(IMenuCallback menuCallback): base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageRestrictionTypes";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Типы реквизитов";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        private void ConstructViewport()
        {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.Controls.Add(this.dataGridView);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
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
            this.field_id_restriction_type,
            this.field_number,
            this.field_description});
            this.dataGridView.Location = new System.Drawing.Point(6, 4);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(978, 529);
            this.dataGridView.TabIndex = 4;
            // 
            // field_id_restriction_type
            // 
            this.field_id_restriction_type.Frozen = true;
            this.field_id_restriction_type.HeaderText = "Идентификатор реквизита";
            this.field_id_restriction_type.Name = "id_restriction_type";
            this.field_id_restriction_type.ReadOnly = true;
            this.field_id_restriction_type.Visible = false;
            // 
            // field_number
            // 
            this.field_number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.field_number.HeaderText = "№";
            this.field_number.Name = "number";
            this.field_number.ReadOnly = true;
            // 
            // field_description
            // 
            this.field_description.HeaderText = "Наименование";
            this.field_description.Name = "description";
            this.field_description.ReadOnly = true;

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
