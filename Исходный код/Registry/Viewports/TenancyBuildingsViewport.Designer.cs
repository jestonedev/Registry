using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class TenancyBuildingsViewport
    {        
        #region Components
        private DataGridView dataGridView;
        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn rent_total_area;
        private DataGridViewTextBoxColumn rent_living_area;
        private DataGridViewTextBoxColumn id_building;
        private DataGridViewComboBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn floors;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn startup_year;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle6 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var dataGridViewCellStyle4 = new DataGridViewCellStyle();
            var dataGridViewCellStyle5 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyBuildingsViewport));
            dataGridView = new DataGridView();
            is_checked = new DataGridViewCheckBoxColumn();
            rent_total_area = new DataGridViewTextBoxColumn();
            rent_living_area = new DataGridViewTextBoxColumn();
            id_building = new DataGridViewTextBoxColumn();
            id_street = new DataGridViewComboBoxColumn();
            house = new DataGridViewTextBoxColumn();
            floors = new DataGridViewTextBoxColumn();
            living_area = new DataGridViewTextBoxColumn();
            cadastral_num = new DataGridViewTextBoxColumn();
            startup_year = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(is_checked, rent_total_area, rent_living_area, id_building, id_street, house, floors, living_area, cadastral_num, startup_year);
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.FromArgb(224, 224, 224);
            dataGridViewCellStyle6.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle6.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dataGridView.DefaultCellStyle = dataGridViewCellStyle6;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(1410, 723);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.CurrentCellDirtyStateChanged += dataGridView_CurrentCellDirtyStateChanged;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            dataGridView.Resize += dataGridView_Resize;
            // 
            // is_checked
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.NullValue = false;
            is_checked.DefaultCellStyle = dataGridViewCellStyle2;
            is_checked.HeaderText = "";
            is_checked.MinimumWidth = 30;
            is_checked.Name = "is_checked";
            is_checked.Resizable = DataGridViewTriState.False;
            is_checked.Width = 30;
            // 
            // rent_total_area
            // 
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Format = "#0.0## м²";
            rent_total_area.DefaultCellStyle = dataGridViewCellStyle3;
            rent_total_area.HeaderText = "Площадь койко-места";
            rent_total_area.MinimumWidth = 160;
            rent_total_area.Name = "rent_total_area";
            rent_total_area.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_total_area.Width = 170;
            // 
            // rent_living_area
            // 
            dataGridViewCellStyle4.BackColor = Color.White;
            dataGridViewCellStyle4.Format = "#0.0## м²";
            rent_living_area.DefaultCellStyle = dataGridViewCellStyle4;
            rent_living_area.HeaderText = "Арендуемая S жил.";
            rent_living_area.MinimumWidth = 140;
            rent_living_area.Name = "rent_living_area";
            rent_living_area.Visible = false;
            rent_living_area.Width = 140;
            // 
            // id_building
            // 
            id_building.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            id_building.HeaderText = "№";
            id_building.MinimumWidth = 100;
            id_building.Name = "id_building";
            id_building.ReadOnly = true;
            // 
            // id_street
            // 
            id_street.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            id_street.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_street.HeaderText = "Адрес";
            id_street.MinimumWidth = 250;
            id_street.Name = "id_street";
            id_street.ReadOnly = true;
            id_street.SortMode = DataGridViewColumnSortMode.Automatic;
            id_street.Width = 250;
            // 
            // house
            // 
            house.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            house.HeaderText = "Дом";
            house.MinimumWidth = 100;
            house.Name = "house";
            house.ReadOnly = true;
            // 
            // floors
            // 
            floors.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            floors.HeaderText = "Этажность";
            floors.MinimumWidth = 100;
            floors.Name = "floors";
            floors.ReadOnly = true;
            // 
            // living_area
            // 
            living_area.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Format = "#0.0## м²";
            living_area.DefaultCellStyle = dataGridViewCellStyle5;
            living_area.HeaderText = "Жилая площадь";
            living_area.MinimumWidth = 150;
            living_area.Name = "living_area";
            living_area.ReadOnly = true;
            living_area.Width = 150;
            // 
            // cadastral_num
            // 
            cadastral_num.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cadastral_num.HeaderText = "Кадастровый номер";
            cadastral_num.MinimumWidth = 170;
            cadastral_num.Name = "cadastral_num";
            cadastral_num.ReadOnly = true;
            cadastral_num.Width = 170;
            // 
            // startup_year
            // 
            startup_year.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            startup_year.HeaderText = "Год ввода в эксплуатацию";
            startup_year.MinimumWidth = 190;
            startup_year.Name = "startup_year";
            startup_year.ReadOnly = true;
            startup_year.Width = 190;
            // 
            // TenancyBuildingsViewport
            // 
            ClientSize = new Size(1416, 729);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyBuildingsViewport";
            Padding = new Padding(3);
            Text = "Здания по процессу найма №{0}";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
