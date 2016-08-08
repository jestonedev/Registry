using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class RestrictionTypeListPresenter: Presenter
    {
        public RestrictionTypeListPresenter()
            : base(new RestrictionTypeListViewModel(), null, null)
        {
        }

        public bool ValidateRestrictionTypesInSnapshot()
        {
            var restrictionTypes = EntitiesListFromSnapshot();
            foreach (var entity in restrictionTypes)
            {
                var restrictionType = (RestrictionType)entity;
                if (restrictionType.RestrictionTypeName == null)
                {
                    MessageBox.Show(@"Не заполнено наименование типа реквизита", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restrictionType.RestrictionTypeName == null || restrictionType.RestrictionTypeName.Length <= 255)
                    continue;
                MessageBox.Show(@"Длина названия типа реквизита не может превышать 255 символов", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                list.Add(EntityConverter<RestrictionType>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<RestrictionType>.FromRow((DataRowView)row));
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
                var restrictionType = (RestrictionType)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_restriction_type"];
                    if ((id == DBNull.Value) || ((int)id != restrictionType.IdRestrictionType)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (restrictionType.IdRestrictionType != null &&
                    ViewModel["general"].Model.Delete(restrictionType.IdRestrictionType.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(restrictionType.IdRestrictionType).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var restrictionType = (RestrictionType)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(restrictionType.IdRestrictionType);
                if (row == null) continue;
                if (EntityConverter<RestrictionType>.FromRow(row) == restrictionType)
                    continue;
                if (ViewModel["general"].Model.Update(restrictionType) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<RestrictionType>.FillRow(restrictionType, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var restrictionType = (RestrictionType)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(restrictionType.IdRestrictionType);
                if (row != null) continue;
                var idRestrictionType = ViewModel["general"].Model.Insert(restrictionType);
                if (idRestrictionType == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_restriction_type"] = idRestrictionType;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<RestrictionType>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_restriction_type", row["id_restriction_type"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_restriction_type", row["id_restriction_type"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<RestrictionType>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["restriction_type"] = row["restriction_type"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_restriction_type", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
