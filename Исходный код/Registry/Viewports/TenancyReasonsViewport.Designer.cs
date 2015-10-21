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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(TenancyReasonsViewport));
            dataGridView = new DataGridView();
            id_reason = new DataGridViewTextBoxColumn();
            id_process = new DataGridViewTextBoxColumn();
            id_reason_type = new DataGridViewComboBoxColumn();
            reason_number = new DataGridViewTextBoxColumn();
            reason_date = new DataGridViewDateTimeColumn();
            reason_prepared = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(id_reason, id_process, id_reason_type, reason_number, reason_date, reason_prepared);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(843, 255);
            dataGridView.TabIndex = 5;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            // 
            // id_reason
            // 
            id_reason.HeaderText = "Идентификатор основания";
            id_reason.Name = "id_reason";
            id_reason.ReadOnly = true;
            id_reason.Visible = false;
            // 
            // id_process
            // 
            id_process.HeaderText = "Идентификатор процесса найма";
            id_process.Name = "id_process";
            id_process.Visible = false;
            // 
            // id_reason_type
            // 
            id_reason_type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            id_reason_type.HeaderText = "Вид основания";
            id_reason_type.MinimumWidth = 150;
            id_reason_type.Name = "id_reason_type";
            id_reason_type.Width = 150;
            // 
            // reason_number
            // 
            reason_number.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            reason_number.HeaderText = "Номер основания";
            reason_number.MinimumWidth = 150;
            reason_number.Name = "reason_number";
            reason_number.Width = 150;
            // 
            // reason_date
            // 
            reason_date.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            reason_date.HeaderText = "Дата основания";
            reason_date.MinimumWidth = 150;
            reason_date.Name = "reason_date";
            reason_date.Width = 150;
            // 
            // reason_prepared
            // 
            reason_prepared.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.BackColor = Color.LightGray;
            reason_prepared.DefaultCellStyle = dataGridViewCellStyle2;
            reason_prepared.FillWeight = 59.64467F;
            reason_prepared.HeaderText = "Результирующее основание";
            reason_prepared.MinimumWidth = 300;
            reason_prepared.Name = "reason_prepared";
            reason_prepared.ReadOnly = true;
            // 
            // TenancyReasonsViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(849, 261);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "TenancyReasonsViewport";
            Padding = new Padding(3);
            Text = "Основания найма №{0}";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);
        }
    }
}
