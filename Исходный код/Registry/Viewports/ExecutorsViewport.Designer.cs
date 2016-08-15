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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExecutorsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_executor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.executor_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.executor_login = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.is_inactive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
            this.id_executor,
            this.executor_name,
            this.executor_login,
            this.phone,
            this.is_inactive});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(648, 281);
            this.dataGridView.TabIndex = 8;
            this.dataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValidated);
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            this.dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_CurrentCellDirtyStateChanged);
            // 
            // id_executor
            // 
            this.id_executor.Frozen = true;
            this.id_executor.HeaderText = "Идентификатор исполнителя";
            this.id_executor.Name = "id_executor";
            this.id_executor.ReadOnly = true;
            this.id_executor.Visible = false;
            // 
            // executor_name
            // 
            this.executor_name.HeaderText = "ФИО исполнителя";
            this.executor_name.MaxInputLength = 255;
            this.executor_name.MinimumWidth = 150;
            this.executor_name.Name = "executor_name";
            // 
            // executor_login
            // 
            this.executor_login.HeaderText = "Логин исполнителя";
            this.executor_login.MaxInputLength = 255;
            this.executor_login.MinimumWidth = 150;
            this.executor_login.Name = "executor_login";
            // 
            // phone
            // 
            this.phone.HeaderText = "Телефон";
            this.phone.MaxInputLength = 255;
            this.phone.MinimumWidth = 150;
            this.phone.Name = "phone";
            // 
            // is_inactive
            // 
            this.is_inactive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.is_inactive.HeaderText = "Неактивный";
            this.is_inactive.Name = "is_inactive";
            this.is_inactive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.is_inactive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ExecutorsViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(654, 287);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExecutorsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исполнители";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
