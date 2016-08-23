using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class DocumentsResidencePresenter: Presenter
    {
        public DocumentsResidencePresenter()
            : base(new DocumentsResidenceViewModel(), null, null)
        {
            
        }

        public bool ValidateDocumentsResidenceSnapshot()
        {
            var documentsResidence = EntitiesListFromSnapshot();
            foreach (var entity in documentsResidence)
            {
                var documentResidence = (DocumentResidence)entity;
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

        public List<Entity> EntitiesListFromSnapshot()
        {
            var list = new List<Entity>();
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var row in snapshot)
            {
                list.Add(EntityConverter<DocumentResidence>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<DocumentResidence>.FromRow((DataRowView)row));
            }
            return list;
        }

        public void InsertRecordIntoSnapshot()
        {
            var row = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource.AddNew();
            if (row != null)
                row.EndEdit();
        }

        public void DeleteCurrentRecordFromSnapshot()
        {
            var bindingSource = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            ((DataRowView)bindingSource[bindingSource.Position]).Row.Delete();
        }

        public bool SaveRecords()
        {
            var list = EntitiesListFromSnapshot();
            if (!SaveInsertedRecords(list)) return false;
            if (!SaveUpdatedRecords(list)) return false;
            list = EntitiesListFromView();
            return SaveDeletedRecords(list);
        }

        private bool SaveDeletedRecords(IEnumerable<Entity> list)
        {
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var entity in list)
            {
                var documentResidence = (DocumentResidence)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_document_residence"];
                    if ((id == DBNull.Value) || ((int)id != documentResidence.IdDocumentResidence)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (documentResidence.IdDocumentResidence != null &&
                    ViewModel["general"].Model.Delete(documentResidence.IdDocumentResidence.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(documentResidence.IdDocumentResidence).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var documentResidence = (DocumentResidence)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(documentResidence.IdDocumentResidence);
                if (row == null) continue;
                if (EntityConverter<DocumentResidence>.FromRow(row) == documentResidence)
                    continue;
                if (ViewModel["general"].Model.Update(documentResidence) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<DocumentResidence>.FillRow(documentResidence, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var documentResidence = (DocumentResidence)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(documentResidence.IdDocumentResidence);
                if (row != null) continue;
                var idOwnershipRightType = ViewModel["general"].Model.Insert(documentResidence);
                if (idOwnershipRightType == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_document_residence"] = idOwnershipRightType;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<DocumentResidence>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_document_residence", row["id_document_residence"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_document_residence", row["id_document_residence"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<DocumentResidence>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["document_residence"] = row["document_residence"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_document_residence", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
