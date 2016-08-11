using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyReasonsPresenter: Presenter
    {
        public TenancyReasonsPresenter(): base(new TenancyReasonsViewModel(), null, null)
        {       
        }

        public bool ValidateTenancyReasonsInSnapshot()
        {
            var tenancyReasons = EntitiesListFromSnapshot();
            foreach (var entity in tenancyReasons)
            {
                var tenancyReason = (TenancyReason)entity;
                if (tenancyReason.IdReasonType == null)
                {
                    MessageBox.Show(@"Не выбран вид основания", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (tenancyReason.ReasonNumber != null && tenancyReason.ReasonNumber.Length > 50)
                {
                    MessageBox.Show(@"Длина номера основания не может превышать 50 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (tenancyReason.ReasonDate == null)
                {
                    MessageBox.Show(@"Не заполнена дата основания", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }


        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<TenancyReason>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromSnapshot()
        {
            var list = new List<Entity>();
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var row in snapshot)
            {
                list.Add(EntityConverter<TenancyReason>.FromRow((DataRowView)row));
            }
            return list;
        }

        internal void InsertRecordIntoSnapshot()
        {
            var row = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource.AddNew();
            if (row != null)
            {
                row["id_process"] = ParentRow["id_process"];
                row["reason_date"] = DateTime.Now.Date;
                row.EndEdit();
            }
        }

        internal void DeleteCurrentRecordFromSnapshot()
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
                var tenancyReason = (TenancyReason)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_reason"];
                    if ((id == DBNull.Value) || ((int)id != tenancyReason.IdReason)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (tenancyReason.IdReason != null &&
                    ViewModel["general"].Model.Delete(tenancyReason.IdReason.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(tenancyReason.IdReason).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var tenancyReason = (TenancyReason)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(tenancyReason.IdReason);
                if (row == null) continue;
                if (EntityConverter<TenancyReason>.FromRow(row) == tenancyReason)
                    continue;
                if (ViewModel["general"].Model.Update(tenancyReason) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<TenancyReason>.FillRow(tenancyReason, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var tenancyReason = (TenancyReason)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(tenancyReason.IdReason);
                if (row != null) continue;

                var idReason = ViewModel["general"].Model.Insert(tenancyReason);
                if (idReason == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_reason"] = idReason;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<TenancyReason>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        internal void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_reason", row["id_reason"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_reason", row["id_reason"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<TenancyReason>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["id_process"] = row["id_process"];
                    rowSnapshot["id_reason_type"] = row["id_reason_type"];
                    rowSnapshot["reason_number"] = row["reason_number"];
                    rowSnapshot["reason_date"] = row["reason_date"];
                    rowSnapshot["reason_prepared"] = row["reason_prepared"];
                }
        }

        internal void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_reason", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
