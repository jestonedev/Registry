using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class OwnershipTypeListPresenter: Presenter
    {
        public OwnershipTypeListPresenter() : base(new OwnershipTypeListViewModel(), null, null)
        {
        }

        public bool ValidateOwnershipTypesInSnapshot()
        {
            var ownershipTypes = EntitiesListFromSnapshot();
            foreach (var entity in ownershipTypes)
            {
                var ownershipRightType = (OwnershipRightType)entity;
                if (ownershipRightType.OwnershipRightTypeName == null)
                {
                    MessageBox.Show(@"Не заполнено наименование типа ограничения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRightType.OwnershipRightTypeName != null && ownershipRightType.OwnershipRightTypeName.Length > 255)
                {
                    MessageBox.Show(@"Длина названия типа ограничения не может превышать 255 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                list.Add(EntityConverter<OwnershipRightType>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<OwnershipRightType>.FromRow((DataRowView)row));
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
                var ownershipRightType = (OwnershipRightType)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_ownership_right_type"];
                    if ((id == DBNull.Value) || ((int)id != ownershipRightType.IdOwnershipRightType)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (ownershipRightType.IdOwnershipRightType != null &&
                    ViewModel["general"].Model.Delete(ownershipRightType.IdOwnershipRightType.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(ownershipRightType.IdOwnershipRightType).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var ownershipRightType = (OwnershipRightType)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(ownershipRightType.IdOwnershipRightType);
                if (row == null) continue;
                if (EntityConverter<OwnershipRightType>.FromRow(row) == ownershipRightType)
                    continue;
                if (ViewModel["general"].Model.Update(ownershipRightType) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<OwnershipRightType>.FillRow(ownershipRightType, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var ownershipRightType = (OwnershipRightType)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(ownershipRightType.IdOwnershipRightType);
                if (row != null) continue;
                var idOwnershipRightType = ViewModel["general"].Model.Insert(ownershipRightType);
                if (idOwnershipRightType == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_ownership_right_type"] = idOwnershipRightType;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<OwnershipRightType>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_ownership_right_type", row["id_ownership_right_type"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_ownership_right_type", row["id_ownership_right_type"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<OwnershipRightType>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["ownership_right_type"] = row["ownership_right_type"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_ownership_right_type", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
