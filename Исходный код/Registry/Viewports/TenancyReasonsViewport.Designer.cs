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
    internal partial class TenancyReasonsViewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_reason;
        private DataGridViewTextBoxColumn id_process;
        private DataGridViewComboBoxColumn id_reason_type;
        private DataGridViewTextBoxColumn reason_number;
        private DataGridViewDateTimeColumn reason_date;
        private DataGridViewTextBoxColumn reason_prepared;
        #endregion Components


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TenancyReasonsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_reason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_reason_type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.reason_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reason_date = new CustomControls.DataGridViewDateTimeColumn();
            this.reason_prepared = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.id_reason,
            this.id_process,
            this.id_reason_type,
            this.reason_number,
            this.reason_date,
            this.reason_prepared});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(843, 255);
            this.dataGridView.TabIndex = 5;
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
            // 
            // id_reason
            // 
            this.id_reason.HeaderText = "Идентификатор основания";
            this.id_reason.Name = "id_reason";
            this.id_reason.ReadOnly = true;
            this.id_reason.Visible = false;
            // 
            // id_process
            // 
            this.id_process.HeaderText = "Идентификатор процесса найма";
            this.id_process.Name = "id_process";
            this.id_process.Visible = false;
            // 
            // id_reason_type
            // 
            this.id_reason_type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.id_reason_type.HeaderText = "Вид основания";
            this.id_reason_type.MinimumWidth = 150;
            this.id_reason_type.Name = "id_reason_type";
            this.id_reason_type.Width = 150;
            // 
            // reason_number
            // 
            this.reason_number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.reason_number.HeaderText = "Номер основания";
            this.reason_number.MinimumWidth = 150;
            this.reason_number.Name = "reason_number";
            this.reason_number.Width = 150;
            // 
            // reason_date
            // 
            this.reason_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.reason_date.HeaderText = "Дата основания";
            this.reason_date.MinimumWidth = 150;
            this.reason_date.Name = "reason_date";
            this.reason_date.Width = 150;
            // 
            // reason_prepared
            // 
            this.reason_prepared.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            this.reason_prepared.DefaultCellStyle = dataGridViewCellStyle2;
            this.reason_prepared.FillWeight = 59.64467F;
            this.reason_prepared.HeaderText = "Результирующее основание";
            this.reason_prepared.MinimumWidth = 300;
            this.reason_prepared.Name = "reason_prepared";
            this.reason_prepared.ReadOnly = true;
            // 
            // TenancyReasonsViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(849, 261);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TenancyReasonsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Основания найма №{0}";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
