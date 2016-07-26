﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Security;
using WeifenLuo.WinFormsUI.Docking;

namespace Registry.Viewport
{
    internal sealed partial class DocumentIssuedByViewport: EditableDataGridViewport
    {
        private DocumentIssuedByViewport()
            : this(null, null)
        {
        }

        public DocumentIssuedByViewport(Viewport viewport, IMenuCallback menuCallback)
            : base(viewport, menuCallback)
        {
            InitializeComponent();
            GeneralSnapshot = new DataTable("snapshot_documents_issued_by")
            {
                Locale = CultureInfo.InvariantCulture
            };
        }

        private static bool ValidateViewportData(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var documentIssuedBy = (DocumentIssuedBy) entity;
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

        protected override List<Entity> EntitiesListFromViewport()
        {
            var list = new List<Entity>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                list.Add(DocumentIssuedByConverter.FromRow(row));
            }
            return list;
        }

        protected override List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var document in GeneralBindingSource)
            {
                var row = (DataRowView)document;
                list.Add(DocumentIssuedByConverter.FromRow(row));
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
            GeneralDataModel = DataModel.GetInstance<DocumentsIssuedByDataModel>();

            GeneralDataModel.Select();

            GeneralBindingSource = new BindingSource
            {
                DataMember = "documents_issued_by",
                DataSource = DataModel.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < GeneralDataModel.Select().Columns.Count; i++)
                GeneralSnapshot.Columns.Add(new DataColumn(
                    GeneralDataModel.Select().Columns[i].ColumnName, GeneralDataModel.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            foreach (var document in GeneralBindingSource)
                GeneralSnapshot.Rows.Add(DocumentIssuedByConverter.ToArray((DataRowView)document));
            GeneralSnapshotBindingSource = new BindingSource { DataSource = GeneralSnapshot };
            AddEventHandler<EventArgs>(GeneralSnapshotBindingSource, "CurrentItemChanged", v_snapshot_documents_issued_by_CurrentItemChanged);

            dataGridView.DataSource = GeneralSnapshotBindingSource;
            id_document_issued_by.DataPropertyName = "id_document_issued_by";
            document_issued_by.DataPropertyName = "document_issued_by";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValidated", dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            AddEventHandler<DataGridViewCellEventArgs>(dataGridView, "CellValueChanged", dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowChanged", DocumentIssuedByViewport_RowChanged);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleting", DocumentIssuedByViewport_RowDeleting);
            AddEventHandler<DataRowChangeEventArgs>(GeneralDataModel.Select(), "RowDeleted", DocumentIssuedByViewport_RowDeleted);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
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
                GeneralSnapshot.Rows.Add(DocumentIssuedByConverter.ToArray(((DataRowView)document)));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.TenancyDirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            SyncViews = false;
            dataGridView.EndEdit();
            GeneralDataModel.EditingNewRecord = true;
            var list = EntitiesListFromViewport();
            if (!ValidateViewportData(list))
            {
                SyncViews = true;
                GeneralDataModel.EditingNewRecord = false;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var document = (DocumentIssuedBy)list[i];
                var row = GeneralDataModel.Select().Rows.Find(document.IdDocumentIssuedBy);
                if (row == null)
                {
                    var idDocumentIssuedBy = GeneralDataModel.Insert(list[i]);
                    if (idDocumentIssuedBy == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    ((DataRowView)GeneralSnapshotBindingSource[i])["id_document_issued_by"] = idDocumentIssuedBy;
                    GeneralDataModel.Select().Rows.Add(DocumentIssuedByConverter.ToArray((DataRowView)GeneralSnapshotBindingSource[i]));
                }
                else
                {

                    if (DocumentIssuedByConverter.FromRow(row) == document)
                        continue;
                    if (GeneralDataModel.Update(list[i]) == -1)
                    {
                        SyncViews = true;
                        GeneralDataModel.EditingNewRecord = false;
                        return;
                    }
                    row["document_issued_by"] = document.DocumentIssuedByName == null ? DBNull.Value : (object)document.DocumentIssuedByName;
                }
            }
            list = EntitiesListFromView();
            foreach (var entity in list)
            {
                var document = (DocumentIssuedBy)entity;
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["id_document_issued_by"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["id_document_issued_by"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["id_document_issued_by"].Value == document.IdDocumentIssuedBy))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (document.IdDocumentIssuedBy != null && GeneralDataModel.Delete(document.IdDocumentIssuedBy.Value) == -1)
                {
                    SyncViews = true;
                    GeneralDataModel.EditingNewRecord = false;
                    return;
                }
                GeneralDataModel.Select().Rows.Find(document.IdDocumentIssuedBy).Delete();
            }
            SyncViews = true;
            GeneralDataModel.EditingNewRecord = false;
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
            if (!SyncViews)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = GeneralSnapshotBindingSource.Find("id_document_issued_by", e.Row["id_document_issued_by"]);
                if (rowIndex != -1)
                    ((DataRowView)GeneralSnapshotBindingSource[rowIndex]).Delete();
            }
        }

        void DocumentIssuedByViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!SyncViews)
                return;
            var rowIndex = GeneralSnapshotBindingSource.Find("id_document_issued_by", e.Row["id_document_issued_by"]);
            if (rowIndex == -1 && GeneralBindingSource.Find("id_document_issued_by", e.Row["id_document_issued_by"]) != -1)
            {
                GeneralSnapshot.Rows.Add(e.Row["id_document_issued_by"], e.Row["document_issued_by"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)GeneralSnapshotBindingSource[rowIndex]);
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

        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }
    }
}
