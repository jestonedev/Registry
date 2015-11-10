using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class ResettlePersonsViewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_person;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewTextBoxColumn surname;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn patronymic;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResettlePersonsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_person = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.surname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patronymic = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.id_person,
            this.id_process,
            this.surname,
            this.name,
            this.patronymic});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(856, 415);
            this.dataGridView.TabIndex = 1;
            // 
            // id_person
            // 
            this.id_person.HeaderText = "Внутренний идентификатор участника";
            this.id_person.Name = "id_person";
            this.id_person.Visible = false;
            // 
            // id_process
            // 
            this.id_process.HeaderText = "Внутренний идентификатор процесса переселения";
            this.id_process.Name = "id_process";
            this.id_process.Visible = false;
            // 
            // surname
            // 
            this.surname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.surname.HeaderText = "Фамилия";
            this.surname.MinimumWidth = 150;
            this.surname.Name = "surname";
            // 
            // name
            // 
            this.name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.name.HeaderText = "Имя";
            this.name.MinimumWidth = 150;
            this.name.Name = "name";
            // 
            // patronymic
            // 
            this.patronymic.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.patronymic.HeaderText = "Отчество";
            this.patronymic.MinimumWidth = 150;
            this.patronymic.Name = "patronymic";
            // 
            // ResettlePersonsViewport
            // 
            this.ClientSize = new System.Drawing.Size(856, 415);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResettlePersonsViewport";
            this.Text = "Участники переселения №{0}";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
