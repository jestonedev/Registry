using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels;
using System.Data;
using Registry.Entities;
using Security;

namespace Registry.Viewport
{
    internal sealed class DocumentIssuedByViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn id_document_issued_by;
        private DataGridViewTextBoxColumn document_issued_by;
        #endregion Components

        #region Models
        DocumentsIssuedByDataModel documents_issued_by = null;
        DataTable snapshot_documents_issued_by = new DataTable("snapshot_documents_issued_by");
        #endregion Models

        #region Views
        BindingSource v_documents_issued_by = null;
        BindingSource v_snapshot_documents_issued_by = null;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private DocumentIssuedByViewport()
            : this(null)
        {
        }

        public DocumentIssuedByViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public DocumentIssuedByViewport(DocumentIssuedByViewport documentIssuedByViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = documentIssuedByViewport.DynamicFilter;
            this.StaticFilter = documentIssuedByViewport.StaticFilter;
            this.ParentRow = documentIssuedByViewport.ParentRow;
            this.ParentType = documentIssuedByViewport.ParentType;
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

        private object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["id_document_issued_by"], 
                dataRowView["document_issued_by"]
            };
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
                if (documentIssuedBy.document_issued_by != null && documentIssuedBy.document_issued_by.Length > 255)
                {
                    MessageBox.Show("Длина наименования органа, выдающего документы, удостоверяющие личность, не может превышать 255 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private DocumentIssuedBy RowToDocumentIssuedBy(DataRow row)
        {
            DocumentIssuedBy documentIssuedBy = new DocumentIssuedBy();
            documentIssuedBy.id_document_issued_by = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by");
            documentIssuedBy.document_issued_by = ViewportHelper.ValueOrNull(row, "document_issued_by");
            return documentIssuedBy;
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
                    dib.id_document_issued_by = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by");
                    dib.document_issued_by = ViewportHelper.ValueOrNull(row, "document_issued_by");
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
                dib.id_document_issued_by = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by");
                dib.document_issued_by = ViewportHelper.ValueOrNull(row, "document_issued_by");
                list.Add(dib);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_documents_issued_by.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
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
            id_document_issued_by.DataPropertyName = "id_document_issued_by";
            document_issued_by.DataPropertyName = "document_issued_by";

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

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshot_documents_issued_by.AddNew();
            row.EndEdit();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
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
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            documents_issued_by.Select().RowChanged -= new DataRowChangeEventHandler(DocumentIssuedByViewport_RowChanged);
            documents_issued_by.Select().RowDeleting -= new DataRowChangeEventHandler(DocumentIssuedByViewport_RowDeleting);
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_documents_issued_by.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
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
            menuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
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
                    int id_document_issued_by = DocumentsIssuedByDataModel.Insert(list[i]);
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
                    if (DocumentsIssuedByDataModel.Update(list[i]) == -1)
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
                    if (DocumentsIssuedByDataModel.Delete(list[i].id_document_issued_by.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    documents_issued_by.Select().Rows.Find(((DocumentIssuedBy)list[i]).id_document_issued_by).Delete();
                }
            }
            sync_views = true;
            menuCallback.EditingStateUpdate();
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

        void v_snapshot_documents_issued_by_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
                menuCallback.NavigationStateUpdate();
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "document_issued_by":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования органа, выдающего документы, удостоверяющие личность, не может превышать 255 символов";
                    else
                        if (cell.Value.ToString().Trim() == "")
                            cell.ErrorText = "Наименование органа, выдающего документы, удостоверяющие личность, не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            menuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentIssuedByViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.id_document_issued_by = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.document_issued_by = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
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
            this.id_document_issued_by,
            this.document_issued_by});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(654, 345);
            this.dataGridView.TabIndex = 8;
            // 
            // id_document_issued_by
            // 
            this.id_document_issued_by.Frozen = true;
            this.id_document_issued_by.HeaderText = "Идентификатор органа";
            this.id_document_issued_by.Name = "id_document_issued_by";
            this.id_document_issued_by.ReadOnly = true;
            this.id_document_issued_by.Visible = false;
            // 
            // document_issued_by
            // 
            this.document_issued_by.HeaderText = "Наименование";
            this.document_issued_by.MinimumWidth = 100;
            this.document_issued_by.Name = "document_issued_by";
            // 
            // DocumentIssuedByViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(660, 351);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DocumentIssuedByViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Органы, выдающие документы";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
