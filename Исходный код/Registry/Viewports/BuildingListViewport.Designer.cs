using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal  partial class BuildingListViewport
    {
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_building;
        private DataGridViewComboBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn floors;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn startup_year;
        private DataGridViewTextBoxColumn id_state;

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(BuildingListViewport));
            dataGridView = new DataGridView();
            id_building = new DataGridViewTextBoxColumn();
            id_street = new DataGridViewComboBoxColumn();
            house = new DataGridViewTextBoxColumn();
            floors = new DataGridViewTextBoxColumn();
            living_area = new DataGridViewTextBoxColumn();
            cadastral_num = new DataGridViewTextBoxColumn();
            startup_year = new DataGridViewTextBoxColumn();
            id_state = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(id_building, id_street, house, floors, living_area, cadastral_num, startup_year, id_state);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(1099, 723);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CellDoubleClick += dataGridView_CellDoubleClick;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            dataGridView.Resize += dataGridView_Resize;
            // 
            // id_building
            // 
            id_building.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            id_building.HeaderText = @"№";
            id_building.MinimumWidth = 100;
            id_building.Name = "id_building";
            id_building.ReadOnly = true;
            // 
            // id_street
            // 
            id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            id_street.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_street.HeaderText = @"Адрес";
            id_street.MinimumWidth = 250;
            id_street.Name = "id_street";
            id_street.ReadOnly = true;
            id_street.SortMode = DataGridViewColumnSortMode.Automatic;
            id_street.Width = 250;
            // 
            // house
            // 
            house.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            house.HeaderText = @"Дом";
            house.MinimumWidth = 100;
            house.Name = "house";
            house.ReadOnly = true;
            // 
            // floors
            // 
            floors.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            floors.HeaderText = @"Этажность";
            floors.MinimumWidth = 100;
            floors.Name = "floors";
            floors.ReadOnly = true;
            // 
            // living_area
            // 
            living_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            living_area.DefaultCellStyle = dataGridViewCellStyle2;
            living_area.HeaderText = @"Жилая площадь";
            living_area.MinimumWidth = 150;
            living_area.Name = "living_area";
            living_area.ReadOnly = true;
            living_area.Width = 150;
            // 
            // cadastral_num
            // 
            cadastral_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cadastral_num.HeaderText = @"Кадастровый номер";
            cadastral_num.MinimumWidth = 170;
            cadastral_num.Name = "cadastral_num";
            cadastral_num.ReadOnly = true;
            cadastral_num.Width = 170;
            // 
            // startup_year
            // 
            startup_year.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            startup_year.HeaderText = @"Год ввода в эксплуатацию";
            startup_year.MinimumWidth = 190;
            startup_year.Name = "startup_year";
            startup_year.ReadOnly = true;
            startup_year.Width = 190;
            // 
            // id_state
            // 
            id_state.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            id_state.HeaderText = @"Текущее состояние";
            id_state.MinimumWidth = 170;
            id_state.Name = "id_state";
            id_state.ReadOnly = true;
            id_state.Width = 170;
            // 
            // BuildingListViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(1105, 729);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "BuildingListViewport";
            Padding = new Padding(3);
            Text = @"Перечень зданий";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
