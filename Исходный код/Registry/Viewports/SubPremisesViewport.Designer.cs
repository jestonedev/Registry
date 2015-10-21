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
    internal partial class SubPremisesViewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_sub_premises;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_state;
        private DataGridViewDateTimeColumn state_date;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(SubPremisesViewport));
            dataGridView = new DataGridView();
            id_sub_premises = new DataGridViewTextBoxColumn();
            id_premises = new DataGridViewTextBoxColumn();
            sub_premises_num = new DataGridViewTextBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            living_area = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            id_state = new DataGridViewComboBoxColumn();
            state_date = new DataGridViewDateTimeColumn();
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
            dataGridView.Columns.AddRange(id_sub_premises, id_premises, sub_premises_num, total_area, living_area, description, id_state, state_date);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(966, 333);
            dataGridView.TabIndex = 0;
            // 
            // id_sub_premises
            // 
            id_sub_premises.HeaderText = "Внутренний номер комнаты";
            id_sub_premises.Name = "id_sub_premises";
            id_sub_premises.Visible = false;
            // 
            // id_premises
            // 
            id_premises.HeaderText = "Внутренний номер помещения";
            id_premises.Name = "id_premises";
            id_premises.Visible = false;
            // 
            // sub_premises_num
            // 
            sub_premises_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            sub_premises_num.HeaderText = "Номер комнаты";
            sub_premises_num.MinimumWidth = 150;
            sub_premises_num.Name = "sub_premises_num";
            sub_premises_num.Width = 150;
            // 
            // total_area
            // 
            total_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle2;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 150;
            total_area.Name = "total_area";
            total_area.Width = 150;
            // 
            // living_area
            // 
            living_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            living_area.DefaultCellStyle = dataGridViewCellStyle2;
            living_area.HeaderText = "Жилая площадь";
            living_area.MinimumWidth = 150;
            living_area.Name = "living_area";
            living_area.Width = 150;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.HeaderText = "Примечание";
            description.MinimumWidth = 300;
            description.Name = "description";
            // 
            // id_state
            // 
            id_state.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_state.HeaderText = "Текущее состояние";
            id_state.MinimumWidth = 150;
            id_state.Name = "id_state";
            id_state.Width = 150;
            // 
            // state_date
            // 
            state_date.HeaderText = "Дата установки состояния";
            state_date.MinimumWidth = 170;
            state_date.Name = "state_date";
            state_date.Width = 170;
            // 
            // SubPremisesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(972, 339);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "SubPremisesViewport";
            Padding = new Padding(3);
            Text = "Перечень комнат";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
