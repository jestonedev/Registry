using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class BuildingListViewport
    {
        private DataGridView dataGridView;

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildingListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_building = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_street = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.house = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.floors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startup_year = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_structure_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.id_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BTI_rooms = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id_building,
            this.id_street,
            this.house,
            this.floors,
            this.living_area,
            this.cadastral_num,
            this.startup_year,
            this.id_structure_type,
            this.id_state});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1099, 723);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // id_building
            // 
            this.id_building.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_building.HeaderText = "№ по реестру";
            this.id_building.MinimumWidth = 60;
            this.id_building.Name = "id_building";
            this.id_building.ReadOnly = true;
            this.id_building.Width = 60;
            // 
            // id_street
            // 
            this.id_street.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_street.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_street.HeaderText = "Адрес";
            this.id_street.MinimumWidth = 250;
            this.id_street.Name = "id_street";
            this.id_street.ReadOnly = true;
            this.id_street.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.id_street.Width = 250;
            // 
            // house
            // 
            this.house.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.house.HeaderText = "Дом";
            this.house.MinimumWidth = 50;
            this.house.Name = "house";
            this.house.ReadOnly = true;
            this.house.Width = 50;
            // 
            // floors
            // 
            this.floors.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.floors.HeaderText = "Этажн.";
            this.floors.MinimumWidth = 60;
            this.floors.Name = "floors";
            this.floors.ReadOnly = true;
            this.floors.Width = 60;
            // 
            // living_area
            // 
            this.living_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.living_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.living_area.HeaderText = "Жил. площадь";
            this.living_area.MinimumWidth = 70;
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            this.living_area.Width = 70;
            // 
            // cadastral_num
            // 
            this.cadastral_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cadastral_num.HeaderText = "Кадастровый номер";
            this.cadastral_num.MinimumWidth = 120;
            this.cadastral_num.Name = "cadastral_num";
            this.cadastral_num.ReadOnly = true;
            this.cadastral_num.Width = 120;
            // 
            // startup_year
            // 
            this.startup_year.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.startup_year.HeaderText = "Год ввода в эксплуатацию";
            this.startup_year.MinimumWidth = 100;
            this.startup_year.Name = "startup_year";
            this.startup_year.ReadOnly = true;
            // 
            // id_structure_type
            // 
            this.id_structure_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_structure_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_structure_type.HeaderText = "Тип строения (материал)";
            this.id_structure_type.MinimumWidth = 100;
            this.id_structure_type.Name = "id_structure_type";
            this.id_structure_type.ReadOnly = true;
            // 
            // id_state
            // 
            this.id_state.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_state.HeaderText = "Текущее состояние";
            this.id_state.MinimumWidth = 200;
            this.id_state.Name = "id_state";
            this.id_state.ReadOnly = true;
            this.id_state.Width = 200;
            //
            //BTI_rooms
            //
            //this.BTI_rooms.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //this.BTI_rooms.HeaderText = "Приватиз. кв. по данным БТИ";
            //this.BTI_rooms.MinimumWidth = 200;
            //this.BTI_rooms.ReadOnly = true;
            //this.BTI_rooms.Name = "BTI_rooms";
            // 
            // BuildingListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1105, 729);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BuildingListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень зданий";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewTextBoxColumn id_building;
        private DataGridViewComboBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn floors;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn startup_year;
        private DataGridViewComboBoxColumn id_structure_type;
        private DataGridViewTextBoxColumn id_state;
        private DataGridViewTextBoxColumn BTI_rooms;
    }
}
