using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Registry.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;
using Security;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;

namespace Registry.Viewport.Presenters
{
    internal sealed class SubPremisesPresenter: Presenter
    {
        public SubPremisesPresenter():base(new SubPremisesViewModel())
        {
            
        }

        public void DeleteCurrentRecordFromSnapshot()
        {
            var bindingSource = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            ((DataRowView)bindingSource[bindingSource.Position]).Row.Delete();
        }

        public void DeleteRowByIdFromSnapshot(int idSubPremise)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_sub_premises", idSubPremise);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_sub_premises", row["id_sub_premises"]);
            if (rowIndex == -1 && ViewModel["general"].BindingSource.Find("id_sub_premises", row["id_sub_premises"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(EntityConverter<SubPremise>.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["id_premises"] = row["id_premises"];
                    rowSnapshot["id_state"] = row["id_state"];
                    rowSnapshot["sub_premises_num"] = row["sub_premises_num"];
                    rowSnapshot["total_area"] = row["total_area"];
                    rowSnapshot["living_area"] = row["living_area"];
                    rowSnapshot["description"] = row["description"];
                    rowSnapshot["state_date"] = row["state_date"];
                    rowSnapshot["cadastral_num"] = row["cadastral_num"];
                    rowSnapshot["cadastral_cost"] = row["cadastral_cost"];
                    rowSnapshot["balance_cost"] = row["balance_cost"];
                    rowSnapshot["account"] = row["account"];
                }
        }

        public void InsertRecordIntoSnapshot(int idPremise)
        {
            var row = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource.AddNew();
            if (row == null) return;
            row["id_premises"] = idPremise;
            row["total_area"] = 0;
            row["living_area"] = 0;
            row["cadastral_cost"] = 0;
            row["balance_cost"] = 0;
            row.EndEdit();
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
                var subPremise = (SubPremise)entity;
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var id = ((DataRowView)snapshot[j])["id_sub_premises"];
                    if ((id == DBNull.Value) || ((int)id != subPremise.IdSubPremises)) continue;
                    rowIndex = j;
                    break;
                }
                if (rowIndex != -1) continue;
                if (subPremise.IdSubPremises != null)
                {
                    var hasResettles = SubPremisesService.HasResettles(subPremise.IdSubPremises.Value);
                    var hasTenancies = SubPremisesService.HasTenancies(subPremise.IdSubPremises.Value);
                    if (hasResettles || hasTenancies)
                    {
                        if (MessageBox.Show(
                            string.Format(@"К комнате {0} привязаны процессы", subPremise.SubPremisesNum) +
                            (hasTenancies ? " найма" : "") +
                            (hasTenancies && hasResettles ? " и" : "") +
                            (hasResettles ? " переселения" : "") +
                            @". Вы действительно хотите удалить эту комнату?", @"Внимание",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) !=
                            DialogResult.Yes)
                            return false;
                    }
                }
                if (subPremise.IdSubPremises != null && OtherService.HasMunicipal(subPremise.IdSubPremises.Value, EntityType.SubPremise) &&
                    !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"Вы не можете удалить муниципальную комнату, т.к. не имеете на это прав",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.IdSubPremises != null && OtherService.HasNotMunicipal(subPremise.IdSubPremises.Value, EntityType.SubPremise) &&
                    !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"Вы не можете удалить немуниципальную комнату, т.к. не имеете на это прав",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.IdSubPremises != null && ViewModel["general"].Model.Delete(subPremise.IdSubPremises.Value) == -1)
                {
                    return false;
                }
                ViewModel["general"].DataSource.Rows.Find(subPremise.IdSubPremises).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<Entity> list)
        {
            foreach (var entity in list)
            {
                var subPremise = (SubPremise)entity;
                var row = ViewModel["general"].DataSource.Rows.Find(subPremise.IdSubPremises);
                if (row == null) continue;
                var subPremiseFromView = EntityConverter<SubPremise>.FromRow(row);
                if (subPremiseFromView == subPremise)
                    continue;
                if (subPremiseFromView.IdSubPremises != null && OtherService.HasMunicipal(subPremiseFromView.IdSubPremises.Value, EntityType.SubPremise) &&
                    !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"Вы не можете изменить информацию по данной комнате, т.к. она является муниципальной",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremiseFromView.IdSubPremises != null && OtherService.HasNotMunicipal(subPremiseFromView.IdSubPremises.Value, EntityType.SubPremise) &&
                    !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"Вы не можете изменить информацию по данной комнате, т.к. она является немуниципальной",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (ViewModel["general"].Model.Update(subPremise) == -1)
                {
                    return false;
                }
                EntityConverter<SubPremise>.FillRow(subPremise, row);
            }
            return true;
        }

        private bool SaveInsertedRecords(IList<Entity> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var subPremise = (SubPremise)list[i];
                var row = ViewModel["general"].DataSource.Rows.Find(subPremise.IdSubPremises);
                if (row != null) continue;
                if (subPremise.IdState != null && DataModelHelper.MunicipalObjectStates().Contains(subPremise.IdState.Value) &&
                        !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на добавление в базу муниципальных жилых помещений", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.IdState != null && DataModelHelper.NonMunicipalAndUnknownObjectStates().Contains(subPremise.IdState.Value) &&
                    !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
                {
                    MessageBox.Show(@"У вас нет прав на добавление в базу немуниципальных жилых помещений", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                var idSubPremises = ViewModel["general"].Model.Insert(subPremise);
                if (idSubPremises == -1)
                {
                    return false;
                }
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i])["id_sub_premises"] = idSubPremises;
                ViewModel["general"].DataSource.Rows.Add(
                    EntityConverter<SubPremise>.ToArray((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[i]));
            }
            return true;
        }

        public List<Entity> EntitiesListFromView()
        {
            var list = new List<Entity>();
            foreach (var row in ViewModel["general"].BindingSource)
            {
                list.Add(EntityConverter<SubPremise>.FromRow((DataRowView)row));
            }
            return list;
        }

        public List<Entity> EntitiesListFromSnapshot()
        {
            var list = new List<Entity>();
            var snapshot = ((SnapshotedViewModel) ViewModel).SnapshotBindingSource;
            foreach (var row in snapshot)
            {
                list.Add(EntityConverter<SubPremise>.FromRow((DataRowView)row));
            }
            return list;
        }

        public bool ValidateSubPremisesInSnapshot()
        {
            var subPremises = EntitiesListFromSnapshot();
            foreach (var entity in subPremises)
            {
                var subPremise = (SubPremise)entity;
                if (subPremise.IdState == null)
                {
                    MessageBox.Show(@"Необходимо выбрать состояние помещения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.SubPremisesNum != null && subPremise.SubPremisesNum.Length > 20)
                {
                    MessageBox.Show(@"Длина номера комнаты не может превышать 20 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.SubPremisesNum != null && !Regex.IsMatch(subPremise.SubPremisesNum, "^([0-9]+[а-я]{0,1}|[а-я])$"))
                {
                    MessageBox.Show(@"Некорректно задан номер комнаты. Можно использовать только цифры и не более одной строчной буквы кириллицы",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (subPremise.Description != null && subPremise.Description.Length > 65535)
                {
                    MessageBox.Show(@"Длина примечания комнаты не может превышать 65535 символов", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }
    }
}
