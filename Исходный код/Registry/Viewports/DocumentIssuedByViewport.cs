using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport
{
    internal class DocumentIssuedByViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView = new System.Windows.Forms.DataGridView();
        private DataGridViewTextBoxColumn field_id_document_issued_by = new System.Windows.Forms.DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn field_document_issued_by = new System.Windows.Forms.DataGridViewTextBoxColumn();
        #endregion Components

        //Modeles
        DocumentsIssuedByDataModel documents_issued_by = null;
        DataTable snapshot_documents_issued_by = new DataTable("snapshot_documents_issued_by");

        //Views
        BindingSource v_documents_issued_by = null;
        BindingSource v_snapshot_documents_issued_by = null;

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        public DocumentIssuedByViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            this.SuspendLayout();
            ConstructViewport();
            this.Name = "tabPageExecutors";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Органы, выдающие документы";
            this.UseVisualStyleBackColor = true;
            this.ResumeLayout(false);
        }

        public DocumentIssuedByViewport(DocumentIssuedByViewport documentIssuedByViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = documentIssuedByViewport.DynamicFilter;
            this.StaticFilter = documentIssuedByViewport.StaticFilter;
            this.ParentRow = documentIssuedByViewport.ParentRow;
            this.ParentType = documentIssuedByViewport.ParentType;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            documents_issued_by = DocumentsIssuedByDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            documents_issued_by.Select();

            v_documents_issued_by = new BindingSource();
            v_documents_issued_by.DataMember = "documents_issued_by";
            v_documents_issued_by.DataSource = DataSetManager.GetDataSet();

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < documents_issued_by.Select().Columns.Count; i++)
                snapshot_documents_issued_by.Columns.Add(new DataColumn(
                    documents_issued_by.Select().Columns[i].ColumnName, documents_issued_by.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_documents_issued_by.Count; i++)
                snapshot_documents_issued_by.Rows.Add(DataRowViewToArray(((DataRowView)v_documents_issued_by[i])));
            v_snapshot_documents_issued_by = new BindingSource();
            v_snapshot_documents_issued_by.DataSource = snapshot_documents_issued_by;
            v_snapshot_documents_issued_by.CurrentItemChanged += new EventHandler(v_snapshot_documents_issued_by_CurrentItemChanged);

            dataGridView.DataSource = v_snapshot_documents_issued_by;
            field_id_document_issued_by.DataPropertyName = "id_document_issued_by";
            field_document_issued_by.DataPropertyName = "document_issued_by";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            documents_issued_by.Select().RowChanged += new DataRowChangeEventHandler(DocumentIssuedByViewport_RowChanged);
            documents_issued_by.Select().RowDeleting += new DataRowChangeEventHandler(DocumentIssuedByViewport_RowDeleting);
        }

        public override void MoveFirst()
        {
            v_snapshot_documents_issued_by.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_documents_issued_by.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_documents_issued_by.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_documents_issued_by.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_documents_issued_by.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_documents_issued_by.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_documents_issued_by.Position > -1) && (v_snapshot_documents_issued_by.Position < (v_snapshot_documents_issued_by.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_documents_issued_by.Position > -1) && (v_snapshot_documents_issued_by.Position < (v_snapshot_documents_issued_by.Count - 1));
        }

        void DocumentIssuedByViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshot_documents_issued_by.Find("id_document_issued_by", e.Row["id_document_issued_by"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshot_documents_issued_by[row_index]).Delete();
            }
        }

        void DocumentIssuedByViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if ((e.Action == DataRowAction.Change) || (e.Action == DataRowAction.ChangeCurrentAndOriginal) || e.Action == DataRowAction.ChangeOriginal)
            {
                int row_index = v_snapshot_documents_issued_by.Find("id_document_issued_by", e.Row["id_document_issued_by"]);
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshot_documents_issued_by[row_index]);
                    row["document_issued_by"] = e.Row["document_issued_by"];
                }
            }
            else
                if (e.Action == DataRowAction.Add)
                {
                    snapshot_documents_issued_by.Rows.Add(new object[] { 
                        e.Row["id_document_issued_by"], 
                        e.Row["document_issued_by"]
                    });
                }
        }

        private bool SnapshotHasChanges()
        {
            List<DocumentIssuedBy> list_from_view = DocumentsIssuedByFromView();
            List<DocumentIssuedBy> list_from_viewport = DocumentsIssuedByFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private List<DocumentIssuedBy> DocumentsIssuedByFromViewport()
        {
            List<DocumentIssuedBy> list = new List<DocumentIssuedBy>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    DocumentIssuedBy dib = new DocumentIssuedBy();
                    DataGridViewRow row = dataGridView.Rows[i];
                    dib.id_document_issued_by = row.Cells["id_document_issued_by"].Value == DBNull.Value ? null :
                        (int?)Convert.ToInt32(row.Cells["id_document_issued_by"].Value);
                    dib.document_issued_by = row.Cells["document_issued_by"].Value == DBNull.Value ? null : row.Cells["document_issued_by"].Value.ToString();
                    list.Add(dib);
                }
            }
            return list;
        }

        private List<DocumentIssuedBy> DocumentsIssuedByFromView()
        {
            List<DocumentIssuedBy> list = new List<DocumentIssuedBy>();
            for (int i = 0; i < v_documents_issued_by.Count; i++)
            {
                DocumentIssuedBy dib = new DocumentIssuedBy();
                DataRowView row = ((DataRowView)v_documents_issued_by[i]);
                dib.id_document_issued_by = row["id_document_issued_by"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["id_document_issued_by"]);
                dib.document_issued_by = row["document_issued_by"] == DBNull.Value ? null : row["document_issued_by"].ToString();
                list.Add(dib);
            }
            return list;
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            menuCallback.EditingStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return true;
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_documents_issued_by.AddNew();
            row.EndEdit();
        }

        public override void Close()
        {
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения о виде основания в базу данных?", "Внимание",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                        return;
            }
            documents_issued_by.Select().RowChanged -= new DataRowChangeEventHandler(DocumentIssuedByViewport_RowChanged);
            documents_issued_by.Select().RowDeleting -= new DataRowChangeEventHandler(DocumentIssuedByViewport_RowDeleting);
            base.Close();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_documents_issued_by.Position != -1);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_documents_issued_by[v_snapshot_documents_issued_by.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_documents_issued_by.Clear();
            for (int i = 0; i < v_documents_issued_by.Count; i++)
                snapshot_documents_issued_by.Rows.Add(DataRowViewToArray(((DataRowView)v_documents_issued_by[i])));
        }

        private bool ValidateViewportData(List<DocumentIssuedBy> list)
        {
            foreach (DocumentIssuedBy documentIssuedBy in list)
            {
                if (documentIssuedBy.document_issued_by == null)
                {
                    MessageBox.Show("Наименование органа, выдающего документы, удостоверяющие личность, не может быть пустым", 
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<DocumentIssuedBy> list = DocumentsIssuedByFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = documents_issued_by.Select().Rows.Find(((DocumentIssuedBy)list[i]).id_document_issued_by);
                if (row == null)
                {
                    int id_document_issued_by = documents_issued_by.Insert(list[i]);
                    if (id_document_issued_by == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshot_documents_issued_by[i])["id_document_issued_by"] = id_document_issued_by;
                    documents_issued_by.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_documents_issued_by[i]));
                }
                else
                {

                    if (RowToDocumentIssuedBy(row) == list[i])
                        continue;
                    if (documents_issued_by.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["document_issued_by"] = list[i].document_issued_by == null ? DBNull.Value : (object)list[i].document_issued_by;
                }
            }
            list = DocumentsIssuedByFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_document_issued_by"].Value != null) &&
                        (dataGridView.Rows[j].Cells["id_document_issued_by"].Value.ToString() != "") &&
                        ((int)dataGridView.Rows[j].Cells["id_document_issued_by"].Value == list[i].id_document_issued_by))
                        row_index = j;
                if (row_index == -1)
                {
                    if (documents_issued_by.Delete(list[i].id_document_issued_by.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    documents_issued_by.Select().Rows.Find(((DocumentIssuedBy)list[i]).id_document_issued_by).Delete();
                }
            }
            sync_views = true;
        }

        private DocumentIssuedBy RowToDocumentIssuedBy(DataRow row)
        {
            DocumentIssuedBy documentIssuedBy = new DocumentIssuedBy();
            documentIssuedBy.id_document_issued_by = row["id_document_issued_by"] == DBNull.Value ? null : 
                (int?)Convert.ToInt32(row["id_document_issued_by"]);
            documentIssuedBy.document_issued_by = row["document_issued_by"] == DBNull.Value ? null : row["document_issued_by"].ToString();
            return documentIssuedBy;
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges();
        }

        void v_snapshot_documents_issued_by_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.OwningColumn.Name == "document_issued_by")
            {
                if (cell.Value.ToString().Length > 255)
                {
                    MessageBox.Show("Наименование органа, выдающего документы, удостоверяющие личность не может превышать 255 символов", 
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cell.Value = cell.Value.ToString().Substring(0, 255);
                }
                if (cell.Value.ToString().Trim().Length == 0)
                {
                    cell.ErrorText = "Наименование органа, выдающего документы, удостоверяющие личность не может быть пустым";
                }
                else
                    cell.ErrorText = "";
            }
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            DocumentIssuedByViewport viewport = new DocumentIssuedByViewport(this, menuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_documents_issued_by.Count;
        }

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_document_issued_by"], 
                dataRowView["document_issued_by"]
            };
        }

        private void ConstructViewport()
        {
            this.Controls.Add(dataGridView);
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.field_id_document_issued_by,
            this.field_document_issued_by});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(990, 537);
            this.dataGridView.TabIndex = 8;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.AutoGenerateColumns = false;
            // 
            // field_id_document_issued_by
            // 
            this.field_id_document_issued_by.Frozen = true;
            this.field_id_document_issued_by.HeaderText = "Идентификатор органа";
            this.field_id_document_issued_by.Name = "id_document_issued_by";
            this.field_id_document_issued_by.ReadOnly = true;
            this.field_id_document_issued_by.Visible = false;
            // 
            // field_document_issued_by
            // 
            this.field_document_issued_by.HeaderText = "Наименование";
            this.field_document_issued_by.Name = "document_issued_by";
            this.field_document_issued_by.MinimumWidth = 100;

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
        }
    }
}
