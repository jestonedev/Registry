using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;

namespace Registry.Viewport
{
    internal partial class TenancyPremisesViewport
    {        
        #region Components
        private DataGridViewWithDetails dataGridView;
        private DataGridViewImageColumn image;
        private DataGridViewCheckBoxColumn is_checked;
        private DataGridViewTextBoxColumn rent_total_area;
        private DataGridViewTextBoxColumn rent_living_area;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn premises_num;
        private DataGridViewComboBoxColumn id_premises_type;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn id_state;
        private DataGridViewTextBoxColumn current_fund;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle14 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyPremisesViewport));
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var dataGridViewCellStyle4 = new DataGridViewCellStyle();
            var dataGridViewCellStyle5 = new DataGridViewCellStyle();
            var dataGridViewCellStyle6 = new DataGridViewCellStyle();
            var dataGridViewCellStyle7 = new DataGridViewCellStyle();
            var dataGridViewCellStyle8 = new DataGridViewCellStyle();
            var dataGridViewCellStyle9 = new DataGridViewCellStyle();
            var dataGridViewCellStyle10 = new DataGridViewCellStyle();
            var dataGridViewCellStyle11 = new DataGridViewCellStyle();
            var dataGridViewCellStyle12 = new DataGridViewCellStyle();
            var dataGridViewCellStyle13 = new DataGridViewCellStyle();
            dataGridView = new DataGridViewWithDetails();
            image = new DataGridViewImageColumn();
            is_checked = new DataGridViewCheckBoxColumn();
            rent_total_area = new DataGridViewTextBoxColumn();
            rent_living_area = new DataGridViewTextBoxColumn();
            id_premises = new DataGridViewTextBoxColumn();
            id_street = new DataGridViewTextBoxColumn();
            house = new DataGridViewTextBoxColumn();
            premises_num = new DataGridViewTextBoxColumn();
            id_premises_type = new DataGridViewComboBoxColumn();
            total_area = new DataGridViewTextBoxColumn();
            id_state = new DataGridViewTextBoxColumn();
            current_fund = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(image, is_checked, rent_total_area, rent_living_area, id_premises, id_street, house, premises_num, id_premises_type, total_area, id_state, current_fund);
            dataGridViewCellStyle14.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = Color.FromArgb(224, 224, 224);
            dataGridViewCellStyle14.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle14.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = DataGridViewTriState.False;
            dataGridView.DefaultCellStyle = dataGridViewCellStyle14;
            dataGridView.DetailsBackColor = SystemColors.Window;
            dataGridView.DetailsCollapsedImage = null;
            dataGridView.DetailsExpandedImage = null;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new Size(1364, 298);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.BeforeExpandDetails += dataGridView_BeforeExpandDetails;
            dataGridView.BeforeCollapseDetails += dataGridView_BeforeCollapseDetails;
            dataGridView.CellContentClick += dataGridView_CellContentClick;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.CellValuePushed += dataGridView_CellValuePushed;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.CurrentCellDirtyStateChanged += dataGridView_CurrentCellDirtyStateChanged;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            dataGridView.Resize += dataGridView_Resize;
            // 
            // image
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle2.BackColor = Color.LightGray;
            dataGridViewCellStyle2.NullValue = resources.GetObject("dataGridViewCellStyle2.NullValue");
            image.DefaultCellStyle = dataGridViewCellStyle2;
            image.HeaderText = "";
            image.MinimumWidth = 23;
            image.Name = "image";
            image.ReadOnly = true;
            image.Resizable = DataGridViewTriState.False;
            image.Width = 23;
            // 
            // is_checked
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.NullValue = false;
            dataGridViewCellStyle3.Padding = new Padding(0, 2, 0, 0);
            is_checked.DefaultCellStyle = dataGridViewCellStyle3;
            is_checked.HeaderText = "";
            is_checked.MinimumWidth = 30;
            is_checked.Name = "is_checked";
            is_checked.Resizable = DataGridViewTriState.False;
            is_checked.Width = 30;
            // 
            // rent_total_area
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.BackColor = Color.White;
            dataGridViewCellStyle4.Format = "#0.0## м²";
            rent_total_area.DefaultCellStyle = dataGridViewCellStyle4;
            rent_total_area.HeaderText = "Площадь койко-места";
            rent_total_area.MinimumWidth = 160;
            rent_total_area.Name = "rent_total_area";
            rent_total_area.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_total_area.Width = 160;
            // 
            // rent_living_area
            // 
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Format = "#0.0## м²";
            rent_living_area.DefaultCellStyle = dataGridViewCellStyle5;
            rent_living_area.HeaderText = "Арендуемая S жил.";
            rent_living_area.MinimumWidth = 130;
            rent_living_area.Name = "rent_living_area";
            rent_living_area.SortMode = DataGridViewColumnSortMode.NotSortable;
            rent_living_area.Visible = false;
            rent_living_area.Width = 130;
            // 
            // id_premises
            // 
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.TopLeft;
            id_premises.DefaultCellStyle = dataGridViewCellStyle6;
            id_premises.HeaderText = "№";
            id_premises.MinimumWidth = 100;
            id_premises.Name = "id_premises";
            id_premises.ReadOnly = true;
            // 
            // id_street
            // 
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.TopLeft;
            id_street.DefaultCellStyle = dataGridViewCellStyle7;
            id_street.HeaderText = "Адрес";
            id_street.MinimumWidth = 300;
            id_street.Name = "id_street";
            id_street.ReadOnly = true;
            id_street.SortMode = DataGridViewColumnSortMode.NotSortable;
            id_street.Width = 300;
            // 
            // house
            // 
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.TopLeft;
            house.DefaultCellStyle = dataGridViewCellStyle8;
            house.HeaderText = "Дом";
            house.MinimumWidth = 100;
            house.Name = "house";
            house.ReadOnly = true;
            house.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // premises_num
            // 
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.TopLeft;
            premises_num.DefaultCellStyle = dataGridViewCellStyle9;
            premises_num.HeaderText = "Помещение";
            premises_num.MinimumWidth = 100;
            premises_num.Name = "premises_num";
            premises_num.ReadOnly = true;
            // 
            // id_premises_type
            // 
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.TopLeft;
            id_premises_type.DefaultCellStyle = dataGridViewCellStyle10;
            id_premises_type.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            id_premises_type.HeaderText = "Тип помещения";
            id_premises_type.MinimumWidth = 150;
            id_premises_type.Name = "id_premises_type";
            id_premises_type.ReadOnly = true;
            id_premises_type.Width = 150;
            // 
            // total_area
            // 
            dataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle11.Format = "#0.0## м²";
            total_area.DefaultCellStyle = dataGridViewCellStyle11;
            total_area.HeaderText = "Общая площадь";
            total_area.MinimumWidth = 130;
            total_area.Name = "total_area";
            total_area.ReadOnly = true;
            total_area.Width = 130;
            // 
            // id_state
            // 
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.TopLeft;
            id_state.DefaultCellStyle = dataGridViewCellStyle12;
            id_state.HeaderText = "Текущее состояние";
            id_state.MinimumWidth = 170;
            id_state.Name = "id_state";
            id_state.ReadOnly = true;
            id_state.Width = 170;
            // 
            // current_fund
            // 
            dataGridViewCellStyle13.Alignment = DataGridViewContentAlignment.TopLeft;
            current_fund.DefaultCellStyle = dataGridViewCellStyle13;
            current_fund.HeaderText = "Текущий фонд";
            current_fund.MinimumWidth = 170;
            current_fund.Name = "current_fund";
            current_fund.ReadOnly = true;
            current_fund.SortMode = DataGridViewColumnSortMode.NotSortable;
            current_fund.Width = 170;
            // 
            // TenancyPremisesViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(1370, 304);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyPremisesViewport";
            Padding = new Padding(3);
            Text = "Перечень помещений";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
