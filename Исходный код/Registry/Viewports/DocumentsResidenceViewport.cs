using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed class DocumentsResidenceViewport: Viewport
    {  
        #region Components
        private DataGridViewTextBoxColumn id_document_residence;
        private DataGridViewTextBoxColumn document_residence;
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        DocumentsResidenceDataModel documents_residence;
        DataTable snapshot_documents_residence = new DataTable("snapshot_documents_residence");
        #endregion Models

        #region Views
        BindingSource v_documents_residence;
        BindingSource v_snapshot_documents_residence;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private DocumentsResidenceViewport()
            : this(null)
        {
        }

        public DocumentsResidenceViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshot_documents_residence.Locale = CultureInfo.InvariantCulture;
        }

        public DocumentsResidenceViewport(DocumentsResidenceViewport documentResidenceViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = documentResidenceViewport.DynamicFilter;
            StaticFilter = documentResidenceViewport.StaticFilter;
            ParentRow = documentResidenceViewport.ParentRow;
            ParentType = documentResidenceViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = DocumentsIssuedByFromView();
            var listFromViewport = DocumentsIssuedByFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            foreach (var documentView in listFromView)
            {
                var founded = false;
                foreach (var documentViewport in listFromViewport)
                    if (documentView == documentViewport)
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_document_residence"], 
                dataRowView["document_residence"]
            };
        }

        private static bool ValidateViewportData(List<DocumentResidence> list)
        {
            foreach (var documentResidence in list)
            {
                if (documentResidence.DocumentResidenceName == null)
                {
                    MessageBox.Show(@"Наименование документа-основания на проживание не может быть пустым",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (documentResidence.DocumentResidenceName == null ||
                    documentResidence.DocumentResidenceName.Length <= 255) continue;
                MessageBox.Show(@"Длина наименования документа-основания на проживание не может превышать 255 символов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        private static DocumentResidence RowToDocumentResidence(DataRow row)
        {
            var documentResidence = new DocumentResidence
            {
                IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence"),
                DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence")
            };
            return documentResidence;
        }

        private List<DocumentResidence> DocumentsIssuedByFromViewport()
        {
            var list = new List<DocumentResidence>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                var dib = new DocumentResidence
                {
                    IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence"),
                    DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence")
                };
                list.Add(dib);
            }
            return list;
        }

        private List<DocumentResidence> DocumentsIssuedByFromView()
        {
            var list = new List<DocumentResidence>();
            foreach (var document in v_documents_residence)
            {
                var row = ((DataRowView)document);
                var dib = new DocumentResidence
                {
                    IdDocumentResidence = ViewportHelper.ValueOrNull<int>(row, "id_document_residence"),
                    DocumentResidenceName = ViewportHelper.ValueOrNull(row, "document_residence")
                };
                list.Add(dib);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshot_documents_residence.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            documents_residence = DocumentsResidenceDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            documents_residence.Select();

            v_documents_residence = new BindingSource
            {
                DataMember = "documents_residence",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < documents_residence.Select().Columns.Count; i++)
                snapshot_documents_residence.Columns.Add(new DataColumn(
                    documents_residence.Select().Columns[i].ColumnName, documents_residence.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var documentResidence in v_documents_residence)
                snapshot_documents_residence.Rows.Add(DataRowViewToArray(((DataRowView)documentResidence)));
            v_snapshot_documents_residence = new BindingSource {DataSource = snapshot_documents_residence};
            v_snapshot_documents_residence.CurrentItemChanged += v_snapshot_documents_issued_by_CurrentItemChanged;

            dataGridView.DataSource = v_snapshot_documents_residence;
            id_document_residence.DataPropertyName = "id_document_residence";
            document_residence.DataPropertyName = "document_residence";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            documents_residence.Select().RowChanged += DocumentResidenceViewport_RowChanged;
            documents_residence.Select().RowDeleting += DocumentResidenceViewport_RowDeleting;
            documents_residence.Select().RowDeleted += DocumentResidenceViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            v_snapshot_documents_residence.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshot_documents_residence.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshot_documents_residence.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshot_documents_residence.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshot_documents_residence.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshot_documents_residence.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshot_documents_residence.Position > -1) && (v_snapshot_documents_residence.Position < (v_snapshot_documents_residence.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshot_documents_residence.Position > -1) && (v_snapshot_documents_residence.Position < (v_snapshot_documents_residence.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.ResettleDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshot_documents_residence.AddNew();
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
            documents_residence.Select().RowChanged -= DocumentResidenceViewport_RowChanged;
            documents_residence.Select().RowDeleting -= DocumentResidenceViewport_RowDeleting;
            documents_residence.Select().RowDeleted -= DocumentResidenceViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshot_documents_residence.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshot_documents_residence[v_snapshot_documents_residence.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshot_documents_residence.Clear();
            foreach (var document in v_documents_residence)
                snapshot_documents_residence.Rows.Add(DataRowViewToArray(((DataRowView)document)));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            documents_residence.EditingNewRecord = true;
            var list = DocumentsIssuedByFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                documents_residence.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = documents_residence.Select().Rows.Find(list[i].IdDocumentResidence);
                if (row == null)
                {
                    var idDocumentResidence = DocumentsResidenceDataModel.Insert(list[i]);
                    if (idDocumentResidence == -1)
                    {
                        sync_views = true;
                        documents_residence.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)v_snapshot_documents_residence[i])["id_document_residence"] = idDocumentResidence;
                    documents_residence.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshot_documents_residence[i]));
                }
                else
                {

                    if (RowToDocumentResidence(row) == list[i])
                        continue;
                    if (DocumentsResidenceDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        documents_residence.EditingNewRecord = false;
                        return;
                    }
                    row["document_residence"] = list[i].DocumentResidenceName == null ? DBNull.Value : (object)list[i].DocumentResidenceName;
                }
            }
            list = DocumentsIssuedByFromView();
            foreach (var document in list)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_document_residence"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_document_residence"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_document_residence"].Value == document.IdDocumentResidence))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (document.IdDocumentResidence != null && DocumentsResidenceDataModel.Delete(document.IdDocumentResidence.Value) == -1)
                    {
                        sync_views = true;
                        documents_residence.EditingNewRecord = false;
                        return;
                    }
                    documents_residence.Select().Rows.Find(document.IdDocumentResidence).Delete();
                }
            }
            sync_views = true;
            documents_residence.EditingNewRecord = false;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new DocumentsResidenceViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        void DocumentResidenceViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void DocumentResidenceViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = v_snapshot_documents_residence.Find("id_document_residence", e.Row["id_document_residence"]);
                if (rowIndex != -1)
                    ((DataRowView)v_snapshot_documents_residence[rowIndex]).Delete();
            }
        }

        void DocumentResidenceViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = v_snapshot_documents_residence.Find("id_document_residence", e.Row["id_document_residence"]);
            if (rowIndex == -1 && v_documents_residence.Find("id_document_residence", e.Row["id_document_residence"]) != -1)
            {
                snapshot_documents_residence.Rows.Add(e.Row["id_document_residence"], e.Row["document_residence"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)v_snapshot_documents_residence[rowIndex]);
                    row["document_residence"] = e.Row["document_residence"];
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
                case "document_residence":
                    if (cell.Value.ToString().Trim().Length > 255)
                        cell.ErrorText = "Длина наименования документа-основания на проживание не может превышать 255 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование документа-основания на проживание не может быть пустым";
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
