using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.EntityConverters;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;
using Security;

namespace Registry.Viewport.Presenters
{
    internal sealed class ResettlePremisesPresenter: Presenter
    {
        public ResettlePremisesPresenter(): base(new ResettlePremisesViewModel(), new SimpleSearchPremiseForm(), new ExtendedSearchPremisesForm())
        {
            
        }

        public bool SnapshotHasChanges()
        {
            var listFromView = ResettlePremisesFromView();
            var listFromViewport = ResettlePremisesFromSnapshot();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            foreach (var rowFromView in listFromView)
            {
                var founded = false;
                foreach (var rowFromViewport in listFromViewport)
                    if (rowFromView == rowFromViewport)
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        public List<ResettlePremisesFromAssoc> ResettlePremisesFromSnapshot()
        {
            var list = new List<ResettlePremisesFromAssoc>();
            for (var i = 0; i < ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Count; i++)
            {
                var row = ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows[i];
                if (Convert.ToBoolean(row["is_checked"], CultureInfo.InvariantCulture) == false)
                    continue;
                var to = new ResettlePremisesFromAssoc
                {
                    IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                    IdProcess = ViewportHelper.ValueOrNull<int>(ParentRow, "id_process"),
                    IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises")
                };
                list.Add(to);
            }
            return list;
        }

        public List<ResettlePremisesFromAssoc> ResettlePremisesFromView()
        {
            var list = new List<ResettlePremisesFromAssoc>();
            foreach (DataRowView row in ViewModel["assoc"].BindingSource)
            {
                list.Add(ResettlePremiseConverter.FromRow(row));
            }
            return list;
        }

        public bool ValidateResettlePremises()
        {
            return true;
        }

        public string GetDefaultDynamicFilter()
        {
            var filter = "";
            if (ViewModel["assoc"].BindingSource.Count > 0)
            {
                filter = "id_premises IN (0";
                foreach (DataRowView row in ViewModel["assoc"].BindingSource)
                    filter += "," + row["id_premises"];
                filter += ")";
            }
            return filter;
        }

        internal bool DeleteRecord()
        {
            var row = ViewModel["general"].CurrentRow;
            var columnName = ViewModel["general"].PrimaryKeyFirst;
            if (OtherService.HasMunicipal((int)row[columnName], EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (OtherService.HasNotMunicipal((int)row[columnName], EntityType.Premise)
                && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return ViewModel["general"].Delete((int)row[columnName]);
        }

        public bool SaveRecords()
        {
            var list = ResettlePremisesFromSnapshot();
            if (!SaveInsertedRecords(list)) return false;
            if (!SaveUpdatedRecords(list)) return false;
            list = ResettlePremisesFromView();
            return SaveDeletedRecords(list);
        }

        private bool SaveDeletedRecords(IEnumerable<ResettlePremisesFromAssoc> list)
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
                if (entity.IdAssoc != null && ViewModel["assoc"].Model.Delete(entity.IdAssoc.Value) == -1)
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
                ViewModel["assoc"].DataSource.Rows.Find(entity.IdAssoc).Delete();
            }
            return true;
        }

        private bool SaveUpdatedRecords(IEnumerable<ResettlePremisesFromAssoc> list)
        {
            foreach (var entity in list)
            {
                DataRow row = null;
                if (entity.IdAssoc != null)
                    row = ViewModel["assoc"].DataSource.Rows.Find(entity.IdAssoc);
                if (row == null) continue;
                if (ResettlePremiseConverter.FromRow(row) == entity)
                    continue;
                if (ViewModel["assoc"].Model.Update(
                    ((ResettlePremisesViewModel)ViewModel).Way == ResettleEstateObjectWay.From ?
                    entity : (Entity)ResettlePremiseConverter.CastFromToAssoc(entity)) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        private bool SaveInsertedRecords(IEnumerable<ResettlePremisesFromAssoc> list)
        {
            var snapshot = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource;
            foreach (var entity in list)
            {
                DataRow row = null;
                if (entity.IdAssoc != null)
                    row = ViewModel["assoc"].DataSource.Rows.Find(entity.IdAssoc);
                if (row != null) continue;
                var idAssoc = ViewModel["assoc"].Model.Insert(
                    ((ResettlePremisesViewModel)ViewModel).Way == ResettleEstateObjectWay.From ?
                    entity : (Entity)ResettlePremiseConverter.CastFromToAssoc(entity));
                if (idAssoc == -1)
                {
                    return false;
                }
                ((DataRowView)snapshot[snapshot.Find("id_premises", entity.IdPremises)])["id_assoc"] = idAssoc;
                ViewModel["assoc"].DataSource.Rows.Add(idAssoc, entity.IdPremises, entity.IdProcess, 0);
            }
            return true;
        }

        internal void DeleteRowByIdFromSnapshot(int idPremises)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_premises", idPremises);
            if (rowIndex != -1)
                ((DataRowView)((SnapshotedViewModel)ViewModel).SnapshotBindingSource[rowIndex]).Delete();
        }

        internal void InsertOrUpdateRowIntoSnapshot(DataRow row)
        {
            var rowIndex = ((SnapshotedViewModel)ViewModel).SnapshotBindingSource.Find("id_premises", row["id_premises"]);
            if (rowIndex == -1 && ViewModel["assoc"].BindingSource.Find("id_assoc", row["id_assoc"]) != -1)
            {
                ((SnapshotedViewModel)ViewModel).SnapshotDataSource.Rows.Add(ResettlePremiseConverter.ToArray(row));
            }
        }
    }
}
