using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class ExecutorsViewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_executor;
        private DataGridViewTextBoxColumn executor_name;
        private DataGridViewTextBoxColumn executor_login;
        private DataGridViewTextBoxColumn phone;
        private DataGridViewCheckBoxColumn is_inactive;
        #endregion Components

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(ExecutorsViewport));
            dataGridView = new DataGridView();
            id_executor = new DataGridViewTextBoxColumn();
            executor_name = new DataGridViewTextBoxColumn();
            executor_login = new DataGridViewTextBoxColumn();
            phone = new DataGridViewTextBoxColumn();
            is_inactive = new DataGridViewCheckBoxColumn();
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
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
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
            dataGridView.Columns.AddRange(id_executor, executor_name, executor_login, phone, is_inactive);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(648, 281);
            dataGridView.TabIndex = 8;
            dataGridView.CurrentCellDirtyStateChanged += dataGridView_CurrentCellDirtyStateChanged;
            // 
            // id_executor
            // 
            id_executor.Frozen = true;
            id_executor.HeaderText = @"Идентификатор исполнителя";
            id_executor.Name = "id_executor";
            id_executor.ReadOnly = true;
            id_executor.Visible = false;
            // 
            // executor_name
            // 
            executor_name.HeaderText = @"ФИО исполнителя";
            executor_name.MaxInputLength = 255;
            executor_name.MinimumWidth = 150;
            executor_name.Name = "executor_name";
            // 
            // executor_login
            // 
            executor_login.HeaderText = @"Логин исполнителя";
            executor_login.MaxInputLength = 255;
            executor_login.MinimumWidth = 150;
            executor_login.Name = "executor_login";
            // 
            // phone
            // 
            phone.HeaderText = @"Телефон";
            phone.MaxInputLength = 255;
            phone.MinimumWidth = 150;
            phone.Name = "phone";
            // 
            // is_inactive
            // 
            is_inactive.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            is_inactive.HeaderText = @"Неактивный";
            is_inactive.Name = "is_inactive";
            is_inactive.Resizable = DataGridViewTriState.True;
            is_inactive.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // ExecutorsViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(654, 287);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "ExecutorsViewport";
            Padding = new Padding(3);
            Text = @"Исполнители";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
