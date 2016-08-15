using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class DocumentIssuedByPresenter: Presenter
    {
        public DocumentIssuedByPresenter() : base(new DocumentIssuedByViewModel(), null, null)
        {
            
        }

        public bool ValidateDocumentIssuedInSnapshot()
        {
            var documentsIssuedBy = EntitiesListFromSnapshot();
            foreach (var entity in documentsIssuedBy)
            {
                var documentIssuedBy = (DocumentIssuedBy)entity;
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

        public List<Entity> EntitiesListFromSnapshot()
        {
            var list = new List<Entity>();
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var row in snapshot)
            {
                list.Add(EntityConverter<DocumentIssuedBy>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<DocumentIssuedBy>.FromRow((DataRowView)row));
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
                var documentIssuedBy = (DocumentIssuedBy)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_document_issued_by"];
                    if ((id == DBNull.Value) || ((int)id != documentIssuedBy.IdDocumentIssuedBy)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (documentIssuedBy.IdDocumentIssuedBy != null &&
                    ViewModel["general"].Model.Delete(documentIssuedBy.IdDocumentIssuedBy.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(documentIssuedBy.IdDocumentIssuedBy).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var documentIssuedBy = (DocumentIssuedBy)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(documentIssuedBy.IdDocumentIssuedBy);
                if (row == null) continue;
                if (EntityConverter<DocumentIssuedBy>.FromRow(row) == documentIssuedBy)
                    continue;
                if (ViewModel["general"].Model.Update(documentIssuedBy) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<DocumentIssuedBy>.FillRow(documentIssuedBy, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var documentIssuedBy = (DocumentIssuedBy)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(documentIssuedBy.IdDocumentIssuedBy);
                if (row != null) continue;
                var idOwnershipRightType = ViewModel["general"].Model.Insert(documentIssuedBy);
                if (idOwnershipRightType == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_document_issued_by"] = idOwnershipRightType;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<DocumentIssuedBy>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_document_issued_by", row["id_document_issued_by"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_document_issued_by", row["id_document_issued_by"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<DocumentIssuedBy>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["document_issued_by"] = row["document_issued_by"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_document_issued_by", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
