using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class TenancyReasonTypesViewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_reason_type;
        private DataGridViewTextBoxColumn reason_name;
        private DataGridViewTextBoxColumn reason_template;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyReasonTypesViewport));
            dataGridView = new DataGridView();
            id_reason_type = new DataGridViewTextBoxColumn();
            reason_name = new DataGridViewTextBoxColumn();
            reason_template = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
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
            dataGridView.Columns.AddRange(id_reason_type, reason_name, reason_template);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(706, 255);
            dataGridView.TabIndex = 7;
            // 
            // id_reason_type
            // 
            id_reason_type.Frozen = true;
            id_reason_type.HeaderText = "Идентификатор вида основания";
            id_reason_type.Name = "id_reason_type";
            id_reason_type.ReadOnly = true;
            id_reason_type.Visible = false;
            // 
            // reason_name
            // 
            reason_name.FillWeight = 200F;
            reason_name.HeaderText = "Имя вида основания";
            reason_name.MinimumWidth = 100;
            reason_name.Name = "reason_name";
            // 
            // reason_template
            // 
            reason_template.FillWeight = 500F;
            reason_template.HeaderText = "Шаблон вида основания";
            reason_template.MinimumWidth = 100;
            reason_template.Name = "reason_template";
            // 
            // TenancyReasonTypesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(712, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyReasonTypesViewport";
            Padding = new Padding(3);
            Text = "Виды оснований";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
