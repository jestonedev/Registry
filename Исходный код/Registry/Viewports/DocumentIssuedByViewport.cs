using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

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
        DocumentsIssuedByDataModel documents_issued_by;
        DataTable snapshot_documents_issued_by = new DataTable("snapshot_documents_issued_by");
        #endregion Models

        #region Views
        BindingSource v_documents_issued_by;
        BindingSource v_snapshot_documents_issued_by;
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
            snapshot_documents_issued_by.Locale = CultureInfo.InvariantCulture;
        }

        public DocumentIssuedByViewport(DocumentIssuedByViewport documentIssuedByViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = documentIssuedByViewport.DynamicFilter;
            StaticFilter = documentIssuedByViewport.StaticFilter;
            ParentRow = documentIssuedByViewport.ParentRow;
            ParentType = documentIssuedByViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = DocumentsIssuedByFromView();
            var listFromViewport = DocumentsIssuedByFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            foreach (var documentFromView in listFromView)
            {
                var founded = false;
                foreach (var documentFromViewport in listFromViewport)
                    if (documentFromView == documentFromViewport)
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_document_issued_by"], 
                dataRowView["document_issued_by"]
            };
        }

        private static bool ValidateViewportData(List<DocumentIssuedBy> list)
        {
            foreach (var documentIssuedBy in list)
            {
                if (documentIssuedBy.DocumentIssuedByName == null)
                {
                    MessageBox.Show(@"Наименование органа, выдающего документы, удостоверяющие личность, не может быть пустым",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (documentIssuedBy.DocumentIssuedByName != null && documentIssuedBy.DocumentIssuedByName.Length > 255)
                {
                    MessageBox.Show(@"Длина наименования органа, выдающего документы, удостоверяющие личность, не может превышать 255 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static DocumentIssuedBy RowToDocumentIssuedBy(DataRow row)
        {
            var documentIssuedBy = new DocumentIssuedBy
            {
                IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by"),
                DocumentIssuedByName = ViewportHelper.ValueOrNull(row, "document_issued_by")
            };
            return documentIssuedBy;
        }

        private List<DocumentIssuedBy> DocumentsIssuedByFromViewport()
        {
            var list = new List<DocumentIssuedBy>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var dib = new DocumentIssuedBy();
                var row = dataGridView.Rows[i];
                dib.IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by");
                dib.DocumentIssuedByName = ViewportHelper.ValueOrNull(row, "document_issued_by");
                list.Add(dib);
            }
            return list;
        }

        private List<DocumentIssuedBy> DocumentsIssuedByFromView()
        {
            var list = new List<DocumentIssuedBy>();
            foreach (var document in v_documents_issued_by)
            {
                var dib = new DocumentIssuedBy();
                var row = ((DataRowView)document);
                dib.IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by");
                dib.DocumentIssuedByName = ViewportHelper.ValueOrNull(row, "document_issued_by");
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
            DockAreas = DockAreas.Document;
            documents_issued_by = DocumentsIssuedByDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            documents_issued_by.Select();

            v_documents_issued_by = new BindingSource
            {
                DataMember = "documents_issued_by",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < documents_issued_by.Select().Columns.Count; i++)
                snapshot_documents_issued_by.Columns.Add(new DataColumn(
                    documents_issued_by.Select().Columns[i].ColumnName, documents_issued_by.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var document in v_documents_issued_by)
                snapshot_documents_issued_by.Rows.Add(DataRowViewToArray(((DataRowView)document)));
            v_snapshot_documents_issued_by = new BindingSource {DataSource = snapshot_documents_issued_by};
            v_snapshot_documents_issued_by.CurrentItemChanged += v_snapshot_documents_issued_by_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_documents_issued_by;
            id_document_issued_by.DataPropertyName = "id_document_issued_by";
            document_issued_by.DataPropertyName = "document_issued_by";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            documents_issued_by.Select().RowChanged += DocumentIssuedByViewport_RowChanged;
            documents_issued_by.Select().RowDeleting += DocumentIssuedByViewport_RowDeleting;
            documents_issued_by.Select().RowDeleted += DocumentIssuedByViewport_RowDeleted;
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
            var row = (DataRowView)v_snapshot_documents_issued_by.AddNew();
            if (row != null) row.EndEdit();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveRecord();
                        break;
                    case DialogResult.No:
                        CancelRecord();
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
            documents_issued_by.Select().RowChanged -= DocumentIssuedByViewport_RowChanged;
            documents_issued_by.Select().RowDeleting -= DocumentIssuedByViewport_RowDeleting;
            documents_issued_by.Select().RowDeleted -= DocumentIssuedByViewport_RowDeleted;
            base.OnClosing(e);
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
            foreach (var document in v_documents_issued_by)
                snapshot_documents_issued_by.Rows.Add(DataRowViewToArray(((DataRowView)document)));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            documents_issued_by.EditingNewRecord = true;
            var list = DocumentsIssuedByFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                documents_issued_by.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = documents_issued_by.Select().Rows.Find(list[i].IdDocumentIssuedBy);
                if (row == null)
                {
                    var idDocumentIssuedBy = DocumentsIssuedByDataModel.Insert(list[i]);
                    if (idDocumentIssuedBy == -1)
                    {
                        sync_views = true;
                        documents_issued_by.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_documents_issued_by[i])["id_document_issued_by"] = idDocumentIssuedBy;
                    documents_issued_by.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_documents_issued_by[i]));
                }
                else
                {

                    if (RowToDocumentIssuedBy(row) == list[i])
                        continue;
                    if (DocumentsIssuedByDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        documents_issued_by.EditingNewRecord = false;
                        return;
                    }
                    row["document_issued_by"] = list[i].DocumentIssuedByName == null ? DBNull.Value : (object)list[i].DocumentIssuedByName;
                }
            }
            list = DocumentsIssuedByFromView();
            foreach (DocumentIssuedBy document in list)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_document_issued_by"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_document_issued_by"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_document_issued_by"].Value == document.IdDocumentIssuedBy))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (document.IdDocumentIssuedBy != null && DocumentsIssuedByDataModel.Delete(document.IdDocumentIssuedBy.Value) == -1)
                    {
                        sync_views = true;
                        documents_issued_by.EditingNewRecord = false;
                        return;
                    }
                    documents_issued_by.Select().Rows.Find(document.IdDocumentIssuedBy).Delete();
                }
            }
            sync_views = true;
            documents_issued_by.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new DocumentIssuedByViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        void DocumentIssuedByViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void DocumentIssuedByViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = v_snapshot_documents_issued_by.Find("id_document_issued_by", e.Row["id_document_issued_by"]);
                if (rowIndex != -1)
                    ((DataRowView)v_snapshot_documents_issued_by[rowIndex]).Delete();
            }
        }

        void DocumentIssuedByViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = v_snapshot_documents_issued_by.Find("id_document_issued_by", e.Row["id_document_issued_by"]);
            if (rowIndex == -1 && v_documents_issued_by.Find("id_document_issued_by", e.Row["id_document_issued_by"]) != -1)
            {
                snapshot_documents_issued_by.Rows.Add(e.Row["id_document_issued_by"], e.Row["document_issued_by"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)v_snapshot_documents_issued_by[rowIndex]);
                    row["document_issued_by"] = e.Row["document_issued_by"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshot_documents_issued_by_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "document_issued_by":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования органа, выдающего документы, удостоверяющие личность, не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование органа, выдающего документы, удостоверяющие личность, не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new ComponentResourceManager(typeof(DocumentIssuedByViewport));
            dataGridView = new DataGridView();
            id_document_issued_by = new DataGridViewTextBoxColumn();
            document_issued_by = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(id_document_issued_by, document_issued_by);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(654, 345);
            dataGridView.TabIndex = 8;
            // 
            // id_document_issued_by
            // 
            id_document_issued_by.Frozen = true;
            id_document_issued_by.HeaderText = @"Идентификатор органа";
            id_document_issued_by.Name = "id_document_issued_by";
            id_document_issued_by.ReadOnly = true;
            id_document_issued_by.Visible = false;
            // 
            // document_issued_by
            // 
            document_issued_by.HeaderText = @"Наименование";
            document_issued_by.MinimumWidth = 100;
            document_issued_by.Name = "document_issued_by";
            // 
            // DocumentIssuedByViewport
            // 
            BackColor = Color.White;
            ClientSize = new Size(660, 351);
            Controls.Add(dataGridView);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "DocumentIssuedByViewport";
            Padding = new Padding(3);
            Text = @"Органы, выдающие документы";
            ((ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
