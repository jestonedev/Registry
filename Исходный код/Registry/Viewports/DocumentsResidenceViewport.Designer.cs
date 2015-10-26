using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport
{
    internal partial class DocumentsResidenceViewport
    {
        #region Components
        private DataGridViewTextBoxColumn id_document_residence;
        private DataGridViewTextBoxColumn document_residence;
        private DataGridView dataGridView;
        #endregion Components


        private void InitializeComponent()
        {
            var dataGridViewCellStyle3 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(DocumentsResidenceViewport));
            dataGridView = new DataGridView();
            id_document_residence = new DataGridViewTextBoxColumn();
            document_residence = new DataGridViewTextBoxColumn();
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
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(id_document_residence, document_residence);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(0, 0);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(671, 463);
            dataGridView.TabIndex = 9;
            // 
            // id_document_residence
            // 
            id_document_residence.Frozen = true;
            id_document_residence.HeaderText = @"Идентификатор органа";
            id_document_residence.Name = "id_document_residence";
            id_document_residence.ReadOnly = true;
            id_document_residence.Visible = false;
            // 
            // document_residence
            // 
            document_residence.HeaderText = @"Наименование";
            document_residence.MinimumWidth = 100;
            document_residence.Name = "document_residence";
            // 
            // DocumentsResidenceViewport
            // 
            ClientSize = new Size(671, 463);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "DocumentsResidenceViewport";
            Text = @"Виды документов на проживание";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
