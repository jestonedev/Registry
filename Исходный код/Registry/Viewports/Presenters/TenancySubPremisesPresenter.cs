using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancySubPremisesPresenter: Presenter
    {
        public TenancySubPremisesPresenter(): base(new TenancySubPremisesViewModel())
        {
        }

        public bool SnapshotHasChanges()
        {
            var listFromView = TenancySubPremisesFromView();
            var listFromViewport = TenancySubPremiesesFromSnapshot();
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

        public List<TenancySubPremisesAssoc> TenancySubPremiesesFromSnapshot()
        {
            var list = new List<TenancySubPremisesAssoc>();
            for (var i = 0; i < ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Count; i++)
            {
                var row = ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new TenancySubPremisesAssoc
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises"),
                    RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area")
                };
                list.Add(to);
            }
            return list;
        }

        public List<TenancySubPremisesAssoc> TenancySubPremisesFromView()
        {
            var list = new List<TenancySubPremisesAssoc>();
            for (var i = 0; i < ViewModel["tenancy_sub_premises_assoc"].BindingSource.Count; i++)
            {
                var row = (DataRowView)ViewModel["tenancy_sub_premises_assoc"].BindingSource[i];
                list.Add(TenancySubPremiseConverter.FromRow(row));
            }
            return list;
        }

        public bool ValidateTenancySubPremises()
        {
            var tenancySubPremises = TenancySubPremiesesFromSnapshot();
            foreach (var subPremises in tenancySubPremises)
            {
                if (subPremises.IdSubPremises != null && OtherService.SubPremiseFundAndRentMatch(subPremises.IdSubPremises.Value,
                    (int)ParentRow["id_rent_type"])) continue;
                var idPremises = (int)EntityDataModel<SubPremise>.GetInstance().Select().Rows.Find(subPremises.IdSubPremises)["id_premises"];
                if (OtherService.PremiseFundAndRentMatch(idPremises, (int)ParentRow["id_rent_type"])) continue;
                var idBuilding = (int)EntityDataModel<Premise>.GetInstance().Select().Rows.Find(idPremises)["id_building"];
                return OtherService.BuildingFundAndRentMatch(idBuilding, (int)ParentRow["id_rent_type"]) ||
                    MessageBox.Show(@"Выбранный вид найма не соответствует фонду сдаваемой комнаты. Все равно продолжить сохранение?",
                        @"Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
            }
            return true;
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
            if (rowIndex == -1 && ViewModel["tenancy_sub_premises_assoc"].BindingSource.Find("id_assoc", row["id_assoc"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(TenancySubPremiseConverter.ToArray(row));
            }
            else
                if (rowIndex != -1)
                {
                    var rowSnapshot = (DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex];
                    rowSnapshot["rent_total_area"] = row["rent_total_area"];
                }
        }

        public bool SaveRecords()
        {
            var list = TenancySubPremiesesFromSnapshot();
            if (!SaveInsertedRecords(list)) return false;
            if (!SaveUpdatedRecords(list)) return false;
            list = TenancySubPremisesFromView();
            return SaveDeletedRecords(list);
        }

        private bool SaveDeletedRecords(IEnumerable<TenancySubPremisesAssoc> list)
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
                if (entity.IdAssoc != null && EntityDataModel<TenancySubPremisesAssoc>.GetInstance().Delete(entity.IdAssoc.Value) == -1)
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
                ViewModel["tenancy_sub_premises_assoc"].DataSource.Rows.Find(entity.IdAssoc).Delete();              
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<TenancySubPremisesAssoc> list)
        {
            foreach (var entity in list)
            {
                DataRow row = null;
                if (entity.IdAssoc != null)
                    row = ViewModel["tenancy_sub_premises_assoc"].DataSource.Rows.Find(entity.IdAssoc);
                if (row == null) continue;
                if (TenancySubPremiseConverter.FromRow(row) == entity)
                    continue;
                if (EntityDataModel<TenancySubPremisesAssoc>.GetInstance().Update(entity) == -1)
                {
                    return false;
                }
                row["rent_total_area"] = entity.RentTotalArea == null ? DBNull.Value : (object)entity.RentTotalArea;
            }
            return true;
        }

        private bool SaveInsertedRecords(IEnumerable<TenancySubPremisesAssoc> list)
        {
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var entity in list)
            {
                DataRow row = null;
                if (entity.IdAssoc != null)
                    row = ViewModel["tenancy_sub_premises_assoc"].DataSource.Rows.Find(entity.IdAssoc);
                if (row != null) continue;
                var idAssoc = EntityDataModel<TenancySubPremisesAssoc>.GetInstance().Insert(entity);
                if (idAssoc == -1)
                {
                    return false;
                }
                ((DataRowView)snapshot[snapshot.Find("id_sub_premises", entity.IdSubPremises)])["id_assoc"] = idAssoc;
                ViewModel["tenancy_sub_premises_assoc"].DataSource.Rows.Add(idAssoc,
                    entity.IdSubPremises, entity.IdProcess, 
                    entity.RentTotalArea, 0);
            }
            return true;
        }
    }
}
