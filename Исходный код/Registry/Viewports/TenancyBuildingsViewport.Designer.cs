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
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyBuildingsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.is_checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.rent_total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rent_living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_building = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_street = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.house = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.floors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startup_year = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.is_checked,
            this.rent_total_area,
            this.rent_living_area,
            this.id_building,
            this.id_street,
            this.house,
            this.floors,
            this.living_area,
            this.cadastral_num,
            this.startup_year});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1410, 723);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValuePushed);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_CurrentCellDirtyStateChanged);
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // is_checked
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.NullValue = false;
            this.is_checked.DefaultCellStyle = dataGridViewCellStyle2;
            this.is_checked.HeaderText = "";
            this.is_checked.MinimumWidth = 30;
            this.is_checked.Name = "is_checked";
            this.is_checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.is_checked.Width = 30;
            // 
            // rent_total_area
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Format = "#0.0## м²";
            this.rent_total_area.DefaultCellStyle = dataGridViewCellStyle3;
            this.rent_total_area.HeaderText = "Площадь койко-места";
            this.rent_total_area.MinimumWidth = 160;
            this.rent_total_area.Name = "rent_total_area";
            this.rent_total_area.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rent_total_area.Width = 170;
            // 
            // rent_living_area
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Format = "#0.0## м²";
            this.rent_living_area.DefaultCellStyle = dataGridViewCellStyle4;
            this.rent_living_area.HeaderText = "Арендуемая S жил.";
            this.rent_living_area.MinimumWidth = 140;
            this.rent_living_area.Name = "rent_living_area";
            this.rent_living_area.Visible = false;
            this.rent_living_area.Width = 140;
            // 
            // id_building
            // 
            this.id_building.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.id_building.HeaderText = "№";
            this.id_building.MinimumWidth = 100;
            this.id_building.Name = "id_building";
            this.id_building.ReadOnly = true;
            // 
            // id_street
            // 
            this.id_street.HeaderText = "Адрес";
            this.id_street.MinimumWidth = 250;
            this.id_street.Name = "id_street";
            this.id_street.ReadOnly = true;
            this.id_street.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id_street.Width = 250;
            // 
            // house
            // 
            this.house.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.house.HeaderText = "Дом";
            this.house.MinimumWidth = 100;
            this.house.Name = "house";
            this.house.ReadOnly = true;
            // 
            // floors
            // 
            this.floors.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.floors.HeaderText = "Этажность";
            this.floors.MinimumWidth = 100;
            this.floors.Name = "floors";
            this.floors.ReadOnly = true;
            // 
            // living_area
            // 
            this.living_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Format = "#0.0## м²";
            this.living_area.DefaultCellStyle = dataGridViewCellStyle5;
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.MinimumWidth = 150;
            this.living_area.Name = "living_area";
            this.living_area.ReadOnly = true;
            this.living_area.Width = 150;
            // 
            // cadastral_num
            // 
            this.cadastral_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cadastral_num.HeaderText = "Кадастровый номер";
            this.cadastral_num.MinimumWidth = 170;
            this.cadastral_num.Name = "cadastral_num";
            this.cadastral_num.ReadOnly = true;
            this.cadastral_num.Width = 170;
            // 
            // startup_year
            // 
            this.startup_year.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.startup_year.HeaderText = "Год ввода в эксплуатацию";
            this.startup_year.MinimumWidth = 190;
            this.startup_year.Name = "startup_year";
            this.startup_year.ReadOnly = true;
            this.startup_year.Width = 190;
            // 
            // TenancyBuildingsViewport
            // 
            this.ClientSize = new System.Drawing.Size(1416, 729);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyBuildingsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Здания по процессу найма №{0}";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn rent_total_area;
        private DataGridViewTextBoxColumn rent_living_area;
        private DataGridViewTextBoxColumn id_building;
        private DataGridViewTextBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn floors;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn startup_year;
    }
}
