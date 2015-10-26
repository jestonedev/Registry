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
    internal partial class RestrictionListViewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_restriction;
        private DataGridViewTextBoxColumn number;
        private DataGridViewDateTimeColumn date;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_restriction_type;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(RestrictionListViewport));
            dataGridView = new DataGridView();
            id_restriction = new DataGridViewTextBoxColumn();
            number = new DataGridViewTextBoxColumn();
            date = new DataGridViewDateTimeColumn();
            description = new DataGridViewTextBoxColumn();
            id_restriction_type = new DataGridViewComboBoxColumn();
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
            dataGridView.Columns.AddRange(id_restriction, number, date, description, id_restriction_type);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(867, 385);
            dataGridView.TabIndex = 1;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            // 
            // id_restriction
            // 
            id_restriction.HeaderText = "Идентификатор реквизита";
            id_restriction.Name = "id_restriction";
            id_restriction.Visible = false;
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
            // id_restriction_type
            // 
            id_restriction_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_restriction_type.FillWeight = 200F;
            id_restriction_type.HeaderText = "Тип реквизита";
            id_restriction_type.MinimumWidth = 200;
            id_restriction_type.Name = "id_restriction_type";
            id_restriction_type.Width = 200;
            // 
            // RestrictionListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(873, 391);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "RestrictionListViewport";
            Padding = new Padding(3);
            Text = "Реквизиты";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
