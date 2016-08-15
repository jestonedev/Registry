using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;
using Settings;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyReasonTypesPresenter: Presenter
    {
        public TenancyReasonTypesPresenter() : base(new TenancyReasonTypesViewModel(), null, null)
        {
        }

        public bool ValidateReasonTypesInSnapshot()
        {
            var reasonTypes = EntitiesListFromSnapshot();
            foreach (var entity in reasonTypes)
            {
                var reasonType = (ReasonType)entity;
                if (reasonType.ReasonName == null)
                {
                    MessageBox.Show(@"Имя вида основания не может быть пустым", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonName != null && reasonType.ReasonName.Length > 150)
                {
                    MessageBox.Show(@"Длина имени типа основания не может превышать 150 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonTemplate == null)
                {
                    MessageBox.Show(@"Шаблон основания не может быть пустым", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (reasonType.ReasonTemplate != null && reasonType.ReasonTemplate.Length > 4000)
                {
                    MessageBox.Show(@"Длина шаблона вида основания не может превышать 4000 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (Regex.IsMatch(reasonType.ReasonTemplate, "@reason_number@") &&
                    Regex.IsMatch(reasonType.ReasonTemplate, "@reason_date@")) continue;
                MessageBox.Show(@"Шаблон основания имеет неверный формат. В шаблоне должны быть указаны номер (в виде шаблона @reason_number@) и" +
                                @" дата (в виде шаблона @reason_date@) основания", @"Ошибка", 
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
                list.Add(EntityConverter<ReasonType>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<ReasonType>.FromRow((DataRowView)row));
            }
            return list;
        }

        public void InsertRecordIntoSnapshot()
        {
            var row = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource.AddNew();
            if (row == null) return;
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
                var reasonType = (ReasonType)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_reason_type"];
                    if ((id == DBNull.Value) || ((int)id != reasonType.IdReasonType)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (reasonType.IdReasonType != null &&
                    ViewModel["general"].Model.Delete(reasonType.IdReasonType.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(reasonType.IdReasonType).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var reasonType = (ReasonType)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(reasonType.IdReasonType);
                if (row == null) continue;
                if (EntityConverter<ReasonType>.FromRow(row) == reasonType)
                    continue;
                if (ViewModel["general"].Model.Update(reasonType) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<ReasonType>.FillRow(reasonType, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var reasonType = (ReasonType)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(reasonType.IdReasonType);
                if (row != null) continue;
                var idOwnershipRightType = ViewModel["general"].Model.Insert(reasonType);
                if (idOwnershipRightType == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_reason_type"] = idOwnershipRightType;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<ReasonType>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_reason_type", row["id_reason_type"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_reason_type", row["id_reason_type"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<ReasonType>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    // TODO:
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_reason_type", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
