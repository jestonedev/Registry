using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomControls;

namespace Registry.Viewport
{
    internal partial class OwnershipListViewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_ownership_right;
        private DataGridViewTextBoxColumn number;
        private DataGridViewDateTimeColumn date;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_ownership_right_type;
        #endregion Components

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(OwnershipListViewport));
            dataGridView = new DataGridView();
            id_ownership_right = new DataGridViewTextBoxColumn();
            number = new DataGridViewTextBoxColumn();
            date = new DataGridViewDateTimeColumn();
            description = new DataGridViewTextBoxColumn();
            id_ownership_right_type = new DataGridViewComboBoxColumn();
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
            dataGridView.Columns.AddRange(id_ownership_right, number, date, description, id_ownership_right_type);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(819, 328);
            dataGridView.TabIndex = 2;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            // 
            // id_ownership_right
            // 
            id_ownership_right.HeaderText = "Идентификатор основания";
            id_ownership_right.Name = "id_ownership_right";
            id_ownership_right.Visible = false;
            // 
            // number
            // 
            number.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            number.HeaderText = "Номер";
            number.MinimumWidth = 150;
            number.Name = "number";
            number.Width = 150;
            // 
            // date
            // 
            date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            date.HeaderText = "Дата";
            date.MinimumWidth = 150;
            date.Name = "date";
            date.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = "Наименование";
            description.MinimumWidth = 300;
            description.Name = "description";
            // 
            // id_ownership_right_type
            // 
            id_ownership_right_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_ownership_right_type.HeaderText = "Тип ограничения";
            id_ownership_right_type.MinimumWidth = 150;
            id_ownership_right_type.Name = "id_ownership_right_type";
            id_ownership_right_type.Width = 150;
            // 
            // OwnershipListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(825, 334);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "OwnershipListViewport";
            Padding = new Padding(3);
            Text = "Ограничения";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
