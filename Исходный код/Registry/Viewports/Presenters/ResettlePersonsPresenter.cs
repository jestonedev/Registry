using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class ResettlePersonsPresenter: Presenter
    {
        public ResettlePersonsPresenter()
            : base(new ResettlePersonsViewModel(), null, null)
        {       
        }

        public bool ValidateResettlePersonInSnapshot()
        {
            var resettlePersons = EntitiesListFromSnapshot();
            foreach (var entity in resettlePersons)
            {
                var resettlePerson = (ResettlePerson)entity;
                if (resettlePerson.Surname == null)
                {
                    MessageBox.Show(@"Фамилия и имя являются обязательными для заполнения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Name == null)
                {
                    MessageBox.Show(@"Фамилия и имя являются обязательными для заполнения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Surname != null && resettlePerson.Surname.Length > 50)
                {
                    MessageBox.Show(@"Длина фамилии не может превышать 50 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Name != null && resettlePerson.Name.Length > 50)
                {
                    MessageBox.Show(@"Длина имени не может превышать 50 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (resettlePerson.Patronymic != null && resettlePerson.Patronymic.Length > 255)
                {
                    MessageBox.Show(@"Длина отчества не может превышать 255 символов", @"Ошибка",
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
                list.Add(EntityConverter<ResettlePerson>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromSnapshot()
        {
            var list = new List<Entity>();
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var row in snapshot)
            {
                list.Add(EntityConverter<ResettlePerson>.FromRow((DataRowView)row));
            }
            return list;
        }

        internal void InsertRecordIntoSnapshot()
        {
            var row = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource.AddNew();
            if (row != null)
            {
                row["id_process"] = ParentRow["id_process"];
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
                var resettlePerson = (ResettlePerson)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_person"];
                    if ((id == DBNull.Value) || ((int)id != resettlePerson.IdPerson)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (resettlePerson.IdPerson != null &&
                    ViewModel["general"].Model.Delete(resettlePerson.IdPerson.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(resettlePerson.IdPerson).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var resettlePerson = (ResettlePerson)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(resettlePerson.IdPerson);
                if (row == null) continue;
                if (EntityConverter<ResettlePerson>.FromRow(row) == resettlePerson)
                    continue;
                if (ViewModel["general"].Model.Update(resettlePerson) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                EntityConverter<ResettlePerson>.FillRow(resettlePerson, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var resettlePerson = (ResettlePerson)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(resettlePerson.IdPerson);
                if (row != null) continue;

                var idPerson = ViewModel["general"].Model.Insert(resettlePerson);
                if (idPerson == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_person"] = idPerson;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<ResettlePerson>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        internal void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_person", row["id_person"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_person", row["id_person"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<ResettlePerson>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["id_process"] = row["id_process"];
                    rowSnapshot["surname"] = row["surname"];
                    rowSnapshot["name"] = row["name"];
                    rowSnapshot["patronymic"] = row["patronymic"];
                    rowSnapshot["document_num"] = row["document_num"];
                    rowSnapshot["document_seria"] = row["document_seria"];
                    rowSnapshot["founding_doc"] = row["founding_doc"];
                }
        }

        internal void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_person", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
