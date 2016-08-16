using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;
using Security;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyBuildingsPresenter: Presenter
    {
        public TenancyBuildingsPresenter() : base(new TenancyBuildingsViewModel(), new SimpleSearchBuildingForm(), new ExtendedSearchBuildingForm())
        {
        }

        public string GetDefaultDynamicFilter()
        {
            var filter = "";
            var assoc = ViewModel["tenancy_buildings_assoc"].BindingSource;
            if (assoc.Count <= 0) return filter;
            filter = "id_building IN (0";
            foreach (var row in assoc)
                filter += "," + ((DataRowView)row)["id_building"];
            filter += ")";
            return filter;
        }

        public bool SnapshotHasChanges()
        {
            var listFromView = TenancyBuildingsFromView();
            var listFromViewport = TenancyBuildingsFromSnapshot();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            for (var i = 0; i < listFromView.Count; i++)
            {
                var founded = false;
                for (var j = 0; j < listFromViewport.Count; j++)
                    if (listFromView[i] == listFromViewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        public List<TenancyBuildingAssoc> TenancyBuildingsFromSnapshot()
        {
            var list = new List<TenancyBuildingAssoc>();
            for (var i = 0; i < ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Count; i++)
            {
                var row = ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new TenancyBuildingAssoc
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                    RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area"),
                    RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area")
                };
                list.Add(to);
            }
            return list;
        }

        public List<TenancyBuildingAssoc> TenancyBuildingsFromView()
        {
            var list = new List<TenancyBuildingAssoc>();
            for (var i = 0; i < ViewModel["tenancy_buildings_assoc"].BindingSource.Count; i++)
            {
                var row = (DataRowView) ViewModel["tenancy_buildings_assoc"].BindingSource[i];
                list.Add(TenancyBuildingConverter.FromRow(row));
            }
            return list;
        }

        public bool ValidateTenancyBuildings()
        {
            var tenancyBuildings = TenancyBuildingsFromSnapshot();
            foreach (var building in tenancyBuildings)
            {
                return building.IdBuilding == null ||
                    OtherService.BuildingFundAndRentMatch(building.IdBuilding.Value, (int)ParentRow["id_rent_type"]) ||
                    MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемого здания. Все равно продолжить сохранение?",
                        @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
            }
            return true;
        }

        public bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (OtherService.HasMunicipal((int)row[columnName], EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых зданий и зданий, в которых присутствуют муниципальные помещения",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (OtherService.HasNotMunicipal((int)row[columnName], EntityType.Building)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых зданий и зданий, в которых присутствуют немуниципальные помещения",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return ViewModel["general"].Delete((int)row[columnName]);
        }

        public void DeleteRowByIdFromSnapshot(int idSubPremise)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_building", idSubPremise);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }

        public void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_building", row["id_building"]);
            if (rowIndex == -1 && ViewModel["tenancy_buildings_assoc"].BindingSource.Find("id_assoc", row["id_assoc"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(TenancyBuildingConverter.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["rent_total_area"] = row["rent_total_area"];
                    rowSnapshot["rent_living_area"] = row["rent_living_area"];
                }
        }

        public bool SaveRecords()
        {
            var list = TenancyBuildingsFromSnapshot();
            if (!SaveInsertedRecords(list)) return false;
            if (!SaveUpdatedRecords(list)) return false;
            list = TenancyBuildingsFromView();
            return SaveDeletedRecords(list);
        }

        private bool SaveDeletedRecords(IEnumerable<TenancyBuildingAssoc> list)
        {
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var entity in list)
            {
                var rowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                {
                    var row = (DataRowView)snapshot[j];
                    var id = row["id_assoc"];
                    if ((id != DBNull.Value) && !string.IsNullOrEmpty(id.ToString()) &&
                        ((int)id == entity.IdAssoc) && Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture))
                        rowIndex = j;
                }
                if (rowIndex != -1) continue;
                if (entity.IdAssoc != null && EntityDataModel<TenancyBuildingAssoc>.GetInstance().Delete(entity.IdAssoc.Value) == -1)
                {
                    return false;
                }
                var snapshotRowIndex = -1;
                for (var j = 0; j < snapshot.Count; j++)
                    if (((DataRowView)snapshot[j])["id_assoc"] != DBNull.Value &&
                        Convert.ToInt32(((DataRowView)snapshot[j])["id_assoc"], CultureInfo.InvariantCulture) == entity.IdAssoc)
                        snapshotRowIndex = j;
                if (snapshotRowIndex != -1)
                {
                    ((DataRowView)snapshot[snapshotRowIndex]).Delete();
                }
                ViewModel["tenancy_buildings_assoc"].DataSource.Rows.Find(entity.IdAssoc).Delete();              
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<TenancyBuildingAssoc> list)
        {
            foreach (var entity in list)
            {
                DataRow row = null;
                if (entity.IdAssoc != null)
                    row = ViewModel["tenancy_buildings_assoc"].DataSource.Rows.Find(entity.IdAssoc);
                if (row == null) continue;
                if (TenancyBuildingConverter.FromRow(row) == entity)
                    continue;
                if (EntityDataModel<TenancyBuildingAssoc>.GetInstance().Update(entity) == -1)
                {
                    return false;
                }
                row["rent_total_area"] = entity.RentTotalArea == null ? DBNull.Value : (object)entity.RentTotalArea;
                row["rent_living_area"] = entity.RentLivingArea == null ? DBNull.Value : (object)entity.RentLivingArea;
            }
            return true;
        }

        private bool SaveInsertedRecords(IEnumerable<TenancyBuildingAssoc> list)
        {
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var entity in list)
            {
                DataRow row = null;
                if (entity.IdAssoc != null)
                    row = ViewModel["tenancy_buildings_assoc"].DataSource.Rows.Find(entity.IdAssoc);
                if (row != null) continue;
                var idAssoc = EntityDataModel<TenancyBuildingAssoc>.GetInstance().Insert(entity);
                if (idAssoc == -1)
                {
                    return false;
                }
                ((DataRowView)snapshot[snapshot.Find("id_building", entity.IdBuilding)])["id_assoc"] = idAssoc;
                ViewModel["tenancy_buildings_assoc"].DataSource.Rows.Add(idAssoc, 
                    entity.IdBuilding, entity.IdProcess, 
                    entity.RentTotalArea, entity.RentLivingArea, 0);
            }
            return true;
        }
    }
}
