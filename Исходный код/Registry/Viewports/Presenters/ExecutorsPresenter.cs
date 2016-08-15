using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;
using Settings;

namespace Registry.Viewport.Presenters
{
    internal sealed class ExecutorsPresenter: Presenter
    {
        public ExecutorsPresenter() : base(new ExecutorsViewModel(), null, null)
        {
            
        }

        public bool ValidateExecutorsInSnapshot()
        {
            var executors = EntitiesListFromSnapshot();
            foreach (var entity in executors)
            {
                var executor = (Executor)entity;
                if (executor.ExecutorName == null)
                {
                    MessageBox.Show(@"ФИО исполнителя не может быть пустым", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if ((executor.ExecutorLogin != null) && (RegistrySettings.UseLdap) &&
                    (UserDomain.GetUserDomain(executor.ExecutorLogin) == null))
                {
                    MessageBox.Show(@"Пользователя с указанным логином не существует", @"Ошибка",
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
                list.Add(EntityConverter<Executor>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<Executor>.FromRow((DataRowView)row));
            }
            return list;
        }

        public void InsertRecordIntoSnapshot()
        {
            var row = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource.AddNew();
            if (row == null) return;
            row["is_inactive"] = false;
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
                var executor = (Executor)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_executor"];
                    if ((id == DBNull.Value) || ((int)id != executor.IdExecutor)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (executor.IdExecutor != null &&
                    ViewModel["general"].Model.Delete(executor.IdExecutor.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(executor.IdExecutor).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var executor = (Executor)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(executor.IdExecutor);
                if (row == null) continue;
                if (EntityConverter<Executor>.FromRow(row) == executor)
                    continue;
                if (ViewModel["general"].Model.Update(executor) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<Executor>.FillRow(executor, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var executor = (Executor)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(executor.IdExecutor);
                if (row != null) continue;
                var idOwnershipRightType = ViewModel["general"].Model.Insert(executor);
                if (idOwnershipRightType == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_executor"] = idOwnershipRightType;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<Executor>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_executor", row["id_executor"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_executor", row["id_executor"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<Executor>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["executor_name"] = row["executor_name"];
                    rowSnapshot["executor_login"] = row["executor_login"];
                    rowSnapshot["phone"] = row["phone"];
                    rowSnapshot["is_inactive"] = row["is_inactive"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_executor", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
