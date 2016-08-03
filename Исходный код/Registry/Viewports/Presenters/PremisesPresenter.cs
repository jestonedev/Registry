using System;
using System.ComponentModel;
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
    internal sealed class PremisesPresenter: Presenter
    {
        public PremisesPresenter(): base(new PremisesViewModel(), new SimpleSearchPremiseForm(), new ExtendedSearchPremisesForm())
        {
            ViewModel["restrictions"].BindingSource.Sort = "date";
            ViewModel["ownership_rights"].BindingSource.Sort = "date";
            ViewModel["buildings"].BindingSource.Sort = "id_building DESC";
        }

        public void RestrictionsFilterRebuild()
        {
            var row = ViewModel["general"].CurrentRow;

            var premisesAssoc = ViewModel["premises_restrictions_premises_assoc"].BindingSource;
            var premisesColumName = ViewModel["premises_restrictions_premises_assoc"].PrimaryKeyFirst;
            var restrictionsFilter = premisesColumName+" IN (0";
            for (var i = 0; i < premisesAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)premisesAssoc[i])[premisesColumName] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";

            var buildingAssoc = ViewModel["restrictions_buildings_assoc"].BindingSource;
            var buildingColumName = ViewModel["restrictions_buildings_assoc"].PrimaryKeyFirst;

            if (row != null && row["id_building"] != DBNull.Value)
            {
                buildingAssoc.Filter = "id_building = " + row["id_building"];
                restrictionsFilter += " OR " + buildingColumName + " IN (0";
                for (var i = 0; i < buildingAssoc.Count; i++)
                    restrictionsFilter += ((DataRowView)buildingAssoc[i])[buildingColumName] + ",";
                restrictionsFilter = restrictionsFilter.TrimEnd(',');
                restrictionsFilter += ")";
            }

            ViewModel["restrictions"].BindingSource.Filter = restrictionsFilter;
        }

        public void OwnershipsFilterRebuild()
        {
            var row = ViewModel["general"].CurrentRow;

            var premisesAssoc = ViewModel["premises_ownership_premises_assoc"].BindingSource;
            var premisesColumName = ViewModel["premises_ownership_premises_assoc"].PrimaryKeyFirst;
            var restrictionsFilter = premisesColumName + " IN (0";
            for (var i = 0; i < premisesAssoc.Count; i++)
                restrictionsFilter += ((DataRowView)premisesAssoc[i])[premisesColumName] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";

            var buildingAssoc = ViewModel["ownership_buildings_assoc"].BindingSource;
            var buildingColumName = ViewModel["ownership_buildings_assoc"].PrimaryKeyFirst;

            if (row != null && row["id_building"] != DBNull.Value)
            {
                buildingAssoc.Filter = "id_building = " + row["id_building"];
                restrictionsFilter += " OR " + buildingColumName + " IN (0";
                for (var i = 0; i < buildingAssoc.Count; i++)
                    restrictionsFilter += ((DataRowView)buildingAssoc[i])[buildingColumName] + ",";
                restrictionsFilter = restrictionsFilter.TrimEnd(',');
                restrictionsFilter += ")";
            }

            ViewModel["ownership_rights"].BindingSource.Filter = restrictionsFilter;
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

        public bool InsertRecord(Premise premise)
        {
            var id = ViewModel["general"].Model.Insert(premise);
            if (id == -1)
            {
                return false;
            }
            premise.IdPremises = id;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, premise.IdPremises);
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<Premise>.FillRow(premise, row);
            return true;
        }

        public bool UpdateRecord(Premise premise)
        {
            if (premise.IdPremises == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить помещение без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(premise) == -1)
                return false;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, premise.IdPremises);
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<Premise>.FillRow(premise, row);
            return true;
        }

        private void RebuildFilterAfterSave(IBindingListView bindingSource, int? idPremise)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_premises = {0})", idPremise);
            bindingSource.Filter += filter;
        }
    }
}
