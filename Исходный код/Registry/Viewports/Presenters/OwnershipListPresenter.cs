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
    internal sealed class OwnershipListPresenter: Presenter
    {
        public OwnershipListPresenter() : 
            base(new OwnershipListViewModel(), null, null)
        {
        }

        public void AddAssocViewModelItem()
        {
            switch (ParentType)
            {
                case ParentTypeEnum.Premises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_premises = " + ParentRow["id_premises"];
                    break;
                case ParentTypeEnum.Building:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_building = " + ParentRow["id_building"];
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
        }

        public void RebuildFilter()
        {
            var ownershipFilter = "id_ownership_right IN (0";
            foreach (var ownership in ViewModel["assoc"].BindingSource)
                ownershipFilter += ((DataRowView)ownership)["id_ownership_right"] + ",";
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
                MessageBox.Show(@"У вас нет прав на изменение информации об ограничениях муниципальных объектов",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (!OtherService.HasNotMunicipal((int)ParentRow[fieldName], entity) ||
                AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal)) return true;
            MessageBox.Show(@"У вас нет прав на изменение информации об ограничениях немуниципальных объектов",
                @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            return false;
        }

        public bool ValidateOwnershipsInSnapshot()
        {
            if (ValidatePermissions() == false)
                return false;
            var ownerships = EntitiesListFromSnapshot();
            foreach (var entity in ownerships)
            {
                var ownershipRight = (OwnershipRight)entity;
                if (ownershipRight.Number != null && ownershipRight.Number.Length > 20)
                {
                    MessageBox.Show(@"Длина номера основания не может превышать 20 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Date == null)
                {
                    MessageBox.Show(@"Не заполнена дата начала действия ограничения, установленного в отношении муниципальной собственности",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.Description != null && ownershipRight.Description.Length > 255)
                {
                    MessageBox.Show(@"Длина наименования ограничения не может превышать 255 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ownershipRight.IdOwnershipRightType == null)
                {
                    MessageBox.Show(@"Не выбран тип ограничения, установленного в отношении муниципальной собственности", @"Ошибка",
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
                list.Add(EntityConverter<OwnershipRight>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromSnapshot()
        {
            var list = new List<Entity>();
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var row in snapshot)
            {
                list.Add(EntityConverter<OwnershipRight>.FromRow((DataRowView)row));
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
                var ownershipRight = (OwnershipRight)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_ownership_right"];
                    if ((id == DBNull.Value) || ((int)id != ownershipRight.IdOwnershipRight)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (ownershipRight.IdOwnershipRight != null &&
                    ViewModel["general"].Model.Delete(ownershipRight.IdOwnershipRight.Value) == -1)
                {
                    RebuildFilter();
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(ownershipRight.IdOwnershipRight).Delete();
                RebuildFilter();
            }
            RebuildFilter();
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var ownershipRight = (OwnershipRight)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(ownershipRight.IdOwnershipRight);
                if (row == null) continue;
                if (EntityConverter<OwnershipRight>.FromRow(row) == ownershipRight)
                    continue;
                if (ViewModel["general"].Model.Update(ownershipRight) == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    RebuildFilter();
                    return false;
                }
                EntityConverter<OwnershipRight>.FillRow(ownershipRight, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var ownershipRight = (OwnershipRight) list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(ownershipRight.IdOwnershipRight);
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
                var idOwnershipRight = ViewModel["general"].Model.Insert(ownershipRight);
                if (idOwnershipRight == -1)
                {
                    ViewModel["general"].Model.EditingNewRecord = false;
                    RebuildFilter();
                    return false;
                }
                switch (ParentType)
                {
                    case ParentTypeEnum.Building:
                        EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance().Insert(
                            new OwnershipRightBuildingAssoc(idParent, idOwnershipRight));
                        break;
                    case ParentTypeEnum.Premises:
                        EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance().Insert(
                            new OwnershipRightPremisesAssoc(idParent, idOwnershipRight));
                        break;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_ownership_right"] = idOwnershipRight;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<OwnershipRight>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
                ViewModel["assoc"].DataSource.Rows.Add(idParent, idOwnershipRight);
            }
            return true;
        }

        public void InsertOrUpdateAssocRowIntoSnapshot(DataRow row)
        {
            //Если добавлена новая ассоциативная связь, то перестраиваем фильтр v_ownerships_rights.Filter
            RebuildFilter();
            //Если в модели есть запись, а в снапшоте нет, то добавляем в снапшот
            if (row["id_ownership_right"] == DBNull.Value)
                return;
            var rowIndex = ViewModel["general"].BindingSource.Find("id_ownership_right", row["id_ownership_right"]);
            if (rowIndex == -1)
                return;
            var snapshotRow = (DataRowView)ViewModel["general"].BindingSource[rowIndex];
            if (((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_ownership_right", row["id_ownership_right"]) == -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<OwnershipRight>.ToArray(snapshotRow));
            }
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_ownership_right", row["id_ownership_right"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_ownership_right", row["id_ownership_right"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<OwnershipRight>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["id_ownership_right_type"] = row["id_ownership_right_type"];
                    rowSnapshot["number"] = row["number"];
                    rowSnapshot["date"] = row["date"];
                    rowSnapshot["description"] = row["description"];
                }
        }

        public void DeleteRowByIdFromSnapshot(int id)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_ownership_right", id);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }
    }
}
