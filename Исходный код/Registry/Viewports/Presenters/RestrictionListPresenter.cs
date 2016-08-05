using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;
using Security;

namespace Registry.Viewport.Presenters
{
    internal sealed class RestrictionListPresenter: Presenter
    {
        public RestrictionListPresenter() : 
            base(new RestrictionListViewModel(), null, null)
        {
        }

        public void AddAssocViewModelItem()
        {
            switch (ParentType)
            {
                case ParentTypeEnum.Premises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<RestrictionPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_premises = " + ParentRow["id_premises"];
                    break;
                case ParentTypeEnum.Building:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<RestrictionBuildingAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_building = " + ParentRow["id_building"];
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
        }

        public void RebuildFilter()
        {
            var ownershipFilter = "id_restriction IN (0";
            foreach (var ownership in ViewModel["assoc"].BindingSource)
                ownershipFilter += ((DataRowView)ownership)["id_restriction"] + ",";
            ownershipFilter = ownershipFilter.TrimEnd(',');
            ownershipFilter += ")";
            ViewModel["general"].BindingSource.Filter = ownershipFilter;
        }

        public void DeleteCurrentRecordFromSnapshot()
        {
            var bindingSource = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            ((DataRowView)bindingSource[bindingSource.Position]).Row.Delete();
        }

        public void InsertRecordIntoSnapshot()
        {
            var row = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource.AddNew();
            if (row != null)
                row.EndEdit();
        }

        public bool ValidatePermissions()
        {
            var entity = EntityType.Unknown;
            string fieldName = null;
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    entity = EntityType.Building;
                    fieldName = "id_building";
                    break;
                case ParentTypeEnum.Premises:
                    entity = EntityType.Premise;
                    fieldName = "id_premises";
                    break;
            }
            if (fieldName == null)
                return false;
            if (OtherService.HasMunicipal((int)ParentRow[fieldName], entity)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (!OtherService.HasNotMunicipal((int)ParentRow[fieldName], entity) ||
                AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)) return true;
            MessageBox.Show(@"У вас нет прав на изменение информации о реквизитах немуниципальных объектов",
                @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            return false;
        }

        public bool ValidateRestrictionsInSnapshot()
        {
            if (ValidatePermissions() == false)
                return false;
            var restrictions = EntitiesListFromSnapshot();
            foreach (var entity in restrictions)
            {
                var restriction = (Restriction)entity;
                if (restriction.Number != null && restriction.Number.Length > 10)
                {
                    MessageBox.Show(@"Номер реквизита не может привышать 10 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.Date == null)
                {
                    MessageBox.Show(@"Не заполнена дата", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.Description != null && restriction.Description.Length > 255)
                {
                    MessageBox.Show(@"Длина наименования реквизита не может превышать 255 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (restriction.IdRestrictionType == null)
                {
                    MessageBox.Show(@"Не выбран тип реквизита", @"Ошибка",
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
                list.Add(EntityConverter<Restriction>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromSnapshot()
        {
            var list = new List<Entity>();
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var row in snapshot)
            {
                list.Add(EntityConverter<Restriction>.FromRow((DataRowView)row));
            }
            return list;
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
                var restriction = (Restriction)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_restriction"];
                    if ((id == DBNull.Value) || ((int)id != restriction.IdRestriction)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (restriction.IdRestriction != null &&
                    ViewModel["general"].Model.Delete(restriction.IdRestriction.Value) == -1)
                {
                    RebuildFilter();
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(restriction.IdRestriction).Delete();
                RebuildFilter();
            }
            RebuildFilter();
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var restriction = (Restriction)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(restriction.IdRestriction);
                if (row == null) continue;
                if (EntityConverter<Restriction>.FromRow(row) == restriction)
                    continue;
                if (ViewModel["general"].Model.Update(restriction) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    RebuildFilter();
                    return false;
                }
                EntityConverter<Restriction>.FillRow(restriction, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var restriction = (Restriction)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(restriction.IdRestriction);
                if (row != null) continue;
                var idParent = 
                    (ParentType == ParentTypeEnum.Premises) && ParentRow != null ? (int)ParentRow["id_premises"] :
                        (ParentType == ParentTypeEnum.Building) && ParentRow != null ? (int)ParentRow["id_building"] :
                            -1;
                if (idParent == -1)
                {
                    MessageBox.Show(@"Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору", 
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    ViewModel["general"].Model.EditingNewRecord = false;
                    RebuildFilter();
                    return false;
                }
                var idRestriction = ViewModel["general"].Model.Insert(restriction);
                if (idRestriction == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    RebuildFilter();
                    return false;
                }
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        ViewModel["assoc"].Model.Insert(new RestrictionBuildingAssoc(idParent, idRestriction, null));
                        break;
                    case ParentTypeEnum.Premises:
                        ViewModel["assoc"].Model.Insert(new RestrictionPremisesAssoc(idParent, idRestriction, null));
                        break;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_restriction"] = idRestriction;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<Restriction>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
                ViewModel["assoc"].DataSource.Rows.Add(idParent, idRestriction);
            }
            return true;
        }

        public void InsertOrUpdateAssocRowIntoSnapshot(DataRow row)
        {
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр
            RebuildFilter();
            //Если в модели есть запись, а в снапшоте нет, то добавляем в снапшот
            if (row["id_restriction"] == DBNull.Value)
                return;
            var rowIndex = ViewModel["general"].BindingSource.Find("id_restriction", row["id_restriction"]);
            if (rowIndex == -1)
                return;
            var snapshotRow = (DataRowView)ViewModel["general"].BindingSource[rowIndex];
            if (((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_restriction", row["id_restriction"]) == -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<Restriction>.ToArray(snapshotRow));
            }
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_restriction", row["id_restriction"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_restriction", row["id_restriction"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<Restriction>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["id_restriction_type"] = row["id_restriction_type"];
                    rowSnapshot["number"] = row["number"];
                    rowSnapshot["date"] = row["date"];
                    rowSnapshot["description"] = row["description"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_restriction", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
