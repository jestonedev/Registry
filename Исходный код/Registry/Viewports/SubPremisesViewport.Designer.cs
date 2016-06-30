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
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubPremisesViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_sub_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_premises = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sub_premises_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.living_area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_state = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.state_date = new CustomControls.DataGridViewDateTimeColumn();
            this.cadastral_num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cadastral_cost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.balance_cost = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.id_sub_premises,
            this.id_premises,
            this.sub_premises_num,
            this.total_area,
            this.living_area,
            this.description,
            this.id_state,
            this.state_date,
            this.cadastral_num,
            this.account,
            this.cadastral_cost,
            this.balance_cost});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(1002, 333);
            this.dataGridView.TabIndex = 0;
            // 
            // id_sub_premises
            // 
            this.id_sub_premises.HeaderText = "Внутренний номер комнаты";
            this.id_sub_premises.Name = "id_sub_premises";
            this.id_sub_premises.Visible = false;
            // 
            // id_premises
            // 
            this.id_premises.HeaderText = "Внутренний номер помещения";
            this.id_premises.Name = "id_premises";
            this.id_premises.Visible = false;
            // 
            // sub_premises_num
            // 
            this.sub_premises_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.sub_premises_num.HeaderText = "Номер комнаты";
            this.sub_premises_num.MinimumWidth = 100;
            this.sub_premises_num.Name = "sub_premises_num";
            this.sub_premises_num.Width = 115;
            // 
            // total_area
            // 
            this.total_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Format = "#0.0## м²";
            this.total_area.DefaultCellStyle = dataGridViewCellStyle2;
            this.total_area.HeaderText = "Общая площадь";
            this.total_area.MinimumWidth = 100;
            this.total_area.Name = "total_area";
            this.total_area.Width = 114;
            // 
            // living_area
            // 
            this.living_area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Format = "#0.0## м²";
            this.living_area.DefaultCellStyle = dataGridViewCellStyle3;
            this.living_area.HeaderText = "Жилая площадь";
            this.living_area.MinimumWidth = 100;
            this.living_area.Name = "living_area";
            this.living_area.Width = 114;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Примечание";
            this.description.MinimumWidth = 250;
            this.description.Name = "description";
            // 
            // id_state
            // 
            this.id_state.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.id_state.HeaderText = "Текущее состояние";
            this.id_state.MinimumWidth = 150;
            this.id_state.Name = "id_state";
            this.id_state.Width = 150;
            // 
            // state_date
            // 
            this.state_date.HeaderText = "Дата установки состояния";
            this.state_date.MinimumWidth = 170;
            this.state_date.Name = "state_date";
            this.state_date.Visible = false;
            this.state_date.Width = 170;
            // 
            // cadastral_num
            // 
            this.cadastral_num.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cadastral_num.HeaderText = "Кадастровый номер";
            this.cadastral_num.MaxInputLength = 20;
            this.cadastral_num.MinimumWidth = 100;
            this.cadastral_num.Name = "cadastral_num";
            this.cadastral_num.Width = 138;
            // 
            // account
            // 
            this.account.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.account.HeaderText = "Лицевой счет ФКР";
            this.account.MaxInputLength = 255;
            this.account.MinimumWidth = 100;
            this.account.Name = "account";
            this.account.Width = 105;
            // 
            // cadastral_cost
            // 
            this.cadastral_cost.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Format = "#0.0# руб.";
            this.cadastral_cost.DefaultCellStyle = dataGridViewCellStyle4;
            this.cadastral_cost.HeaderText = "Кадастровая стоимость";
            this.cadastral_cost.MinimumWidth = 100;
            this.cadastral_cost.Name = "cadastral_cost";
            this.cadastral_cost.Width = 159;
            // 
            // balance_cost
            // 
            this.balance_cost.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.Format = "#0.0# руб.";
            this.balance_cost.DefaultCellStyle = dataGridViewCellStyle5;
            this.balance_cost.HeaderText = "Балансовая стоимость";
            this.balance_cost.MinimumWidth = 100;
            this.balance_cost.Name = "balance_cost";
            this.balance_cost.Width = 153;
            // 
            // SubPremisesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 339);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SubPremisesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень комнат";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewTextBoxColumn id_sub_premises;
        private DataGridViewTextBoxColumn id_premises;
        private DataGridViewTextBoxColumn sub_premises_num;
        private DataGridViewTextBoxColumn total_area;
        private DataGridViewTextBoxColumn living_area;
        private DataGridViewTextBoxColumn description;
        private DataGridViewComboBoxColumn id_state;
        private DataGridViewDateTimeColumn state_date;
        private DataGridViewTextBoxColumn cadastral_num;
        private DataGridViewTextBoxColumn account;
        private DataGridViewTextBoxColumn cadastral_cost;
        private DataGridViewTextBoxColumn balance_cost;
    }
}
