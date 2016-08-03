using System;
using System.Data;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;

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

    }
}
