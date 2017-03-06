using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class PremisesListViewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PremisesListViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_street = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.house = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_premises_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rooms_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.current_fund = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.id_premises,
            this.id_street,
            this.house,
            this.premises_num,
            this.id_premises_type,
            this.cadastral_num,
            this.total_area,
            this.rooms_area,
            this.id_state,
            this.current_fund});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1125, 704);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // id_premises
            // 
            this.id_premises.Frozen = true;
            this.id_premises.HeaderText = "№ по реестру";
            this.id_premises.MinimumWidth = 60;
            this.id_premises.Name = "id_premises";
            this.id_premises.ReadOnly = true;
            this.id_premises.Width = 60;
            // 
            // id_street
            // 
            this.id_street.Frozen = true;
            this.id_street.HeaderText = "Адрес";
            this.id_street.MinimumWidth = 250;
            this.id_street.Name = "id_street";
            this.id_street.ReadOnly = true;
            this.id_street.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id_street.Width = 250;
            // 
            // house
            // 
            this.house.Frozen = true;
            this.house.HeaderText = "Дом";
            this.house.MinimumWidth = 50;
            this.house.Name = "house";
            this.house.ReadOnly = true;
            this.house.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.house.Width = 50;
            // 
            // premises_num
            // 
            this.premises_num.Frozen = true;
            this.premises_num.HeaderText = "Пом.";
            this.premises_num.MinimumWidth = 50;
            this.premises_num.Name = "premises_num";
            this.premises_num.ReadOnly = true;
            this.premises_num.Width = 50;
            // 
            // id_premises_type
            // 
            this.id_premises_type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.id_premises_type.HeaderText = "Тип помещения";
            this.id_premises_type.MinimumWidth = 90;
            this.id_premises_type.Name = "id_premises_type";
            this.id_premises_type.ReadOnly = true;
            this.id_premises_type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.id_premises_type.Width = 90;
            // 
            // cadastral_num
            // 
            this.cadastral_num.HeaderText = "Кадастровый номер";
            this.cadastral_num.MinimumWidth = 110;
            this.cadastral_num.Name = "cadastral_num";
            this.cadastral_num.ReadOnly = true;
            this.cadastral_num.Width = 110;
            // 
            // total_area
            // 
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 70;
            this.total_area.Name = "total_area";
            this.total_area.ReadOnly = true;
            this.total_area.Width = 70;
            // 
            // rooms_area
            // 
            this.rooms_area.HeaderText = "Площадь мун. комнат";
            this.rooms_area.MinimumWidth = 100;
            this.rooms_area.Name = "rooms_area";
            this.rooms_area.ReadOnly = true;
            this.rooms_area.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // id_state
            // 
            this.id_state.HeaderText = "Текущее состояние";
            this.id_state.MinimumWidth = 150;
            this.id_state.Name = "id_state";
            this.id_state.ReadOnly = true;
            this.id_state.Width = 150;
            // 
            // current_fund
            // 
            this.current_fund.HeaderText = "Текущий фонд";
            this.current_fund.MinimumWidth = 140;
            this.current_fund.Name = "current_fund";
            this.current_fund.ReadOnly = true;
            this.current_fund.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.current_fund.Width = 140;
            // 
            // PremisesListViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1131, 710);
            this.Controls.Add(this.dataGridView);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PremisesListViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень помещений";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn id_street;
        private DataGridViewTextBoxColumn house;
        private DataGridViewTextBoxColumn premises_num;
        private DataGridViewComboBoxColumn id_premises_type;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn rooms_area;
        private DataGridViewTextBoxColumn id_state;
        private DataGridViewTextBoxColumn current_fund;
    }
}
