using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class StructureTypeListPresenter: Presenter
    {
        public StructureTypeListPresenter()
            : base(new StructureTypeListViewModel(), null, null)
        {
        }

        public bool ValidateStructureTypesInSnapshot()
        {
            var structureTypes = EntitiesListFromSnapshot();
            foreach (var entity in structureTypes)
            {
                var structureType = (StructureType)entity;
                if (structureType.StructureTypeName == null)
                {
                    MessageBox.Show(@"Не заполнено наименование структуры здания", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (structureType.StructureTypeName == null || structureType.StructureTypeName.Length <= 255)
                    continue;
                MessageBox.Show(@"Длина названия структуры здания не может превышать 255 символов",
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
                list.Add(EntityConverter<StructureType>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<StructureType>.FromRow((DataRowView)row));
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
                var structureType = (StructureType)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_structure_type"];
                    if ((id == DBNull.Value) || ((int)id != structureType.IdStructureType)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (structureType.IdStructureType != null &&
                    ViewModel["general"].Model.Delete(structureType.IdStructureType.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(structureType.IdStructureType).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var structureType = (StructureType)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(structureType.IdStructureType);
                if (row == null) continue;
                if (EntityConverter<StructureType>.FromRow(row) == structureType)
                    continue;
                if (ViewModel["general"].Model.Update(structureType) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<StructureType>.FillRow(structureType, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var structureType = (StructureType)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(structureType.IdStructureType);
                if (row != null) continue;
                var idOwnershipRightType = ViewModel["general"].Model.Insert(structureType);
                if (idOwnershipRightType == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_structure_type"] = idOwnershipRightType;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<StructureType>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_structure_type", row["id_structure_type"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_structure_type", row["id_structure_type"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<StructureType>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["structure_type"] = row["structure_type"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_structure_type", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
