using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class DocumentsResidenceViewport: EditableDataGridViewport
    {  
        private DocumentsResidenceViewport()
            : this(null)
        {
        }

        public DocumentsResidenceViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_documents_residence")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        public DocumentsResidenceViewport(DocumentsResidenceViewport documentResidenceViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = documentResidenceViewport.DynamicFilter;
            StaticFilter = documentResidenceViewport.StaticFilter;
            ParentRow = documentResidenceViewport.ParentRow;
            ParentType = documentResidenceViewport.ParentType;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_document_residence"], 
                dataRowView["document_residence"]
            };
        }

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var documentResidence = (DocumentResidence) entity;
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

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
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

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var document in GeneralBindingSource)
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

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = DockAreas.Document;
            GeneralDataModel = DataModel.GetInstance(DataModelType.DocumentsResidenceDataModel);

            //Ожидаем дозагрузки данных, если это необходимо
            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource
            {
                DataMember = "documents_residence",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(
                    GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var documentResidence in GeneralBindingSource)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)documentResidence)));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
            GeneralSnapshotBindingSource.CurrentItemChanged += v_snapshot_documents_issued_by_CurrentItemChanged;

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_document_residence.DataPropertyName = "id_document_residence";
            document_residence.DataPropertyName = "document_residence";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            GeneralDataModel.Select().RowChanged += DocumentResidenceViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting += DocumentResidenceViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted += DocumentResidenceViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.ResettleDirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)GeneralSnapshotBindingSource.AddNew();
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
            GeneralDataModel.Select().RowChanged -= DocumentResidenceViewport_RowChanged;
            GeneralDataModel.Select().RowDeleting -= DocumentResidenceViewport_RowDeleting;
            GeneralDataModel.Select().RowDeleted -= DocumentResidenceViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (GeneralSnapshotBindingSource.Position != -1) && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)GeneralSnapshotBindingSource[GeneralSnapshotBindingSource.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            GeneralSnapshot.Clear();
            foreach (var document in GeneralBindingSource)
                GeneralSnapshot.Rows.Add(DataRowViewToArray(((DataRowView)document)));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            GeneralDataModel.EditingNewRecord = true;
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var document = (DocumentResidence)list[i];
                var row = GeneralDataModel.Select().Rows.Find(document.IdDocumentResidence);
                if (row == null)
                {
                    var idDocumentResidence = GeneralDataModel.Insert(list[i]);
                    if (idDocumentResidence == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_document_residence"] = idDocumentResidence;
                    GeneralDataModel.Select().Rows.Add(DataRowViewToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {

                    if (RowToDocumentResidence(row) == document)
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["document_residence"] = document.DocumentResidenceName == null ? DBNull.Value : (object)document.DocumentResidenceName;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var document = (DocumentResidence)entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_document_residence"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_document_residence"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_document_residence"].Value == document.IdDocumentResidence))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (document.IdDocumentResidence != null && GeneralDataModel.Delete(document.IdDocumentResidence.Value) == -1)
                    {
                        sync_views = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    GeneralDataModel.Select().Rows.Find(document.IdDocumentResidence).Delete();
                }
            }
            sync_views = true;
            GeneralDataModel.EditingNewRecord = false;
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
                var rowIndex = GeneralSnapshotBindingSource.Find("id_document_residence", e.Row["id_document_residence"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
            }
        }

        void DocumentResidenceViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_document_residence", e.Row["id_document_residence"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_document_residence", e.Row["id_document_residence"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_document_residence"], e.Row["document_residence"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
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
    }
}
