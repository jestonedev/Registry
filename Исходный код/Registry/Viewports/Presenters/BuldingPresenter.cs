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
    internal sealed class BuildingPresenter : Presenter
    {
        public BuildingPresenter()
            : base(new BuildingViewModel(), new SimpleSearchBuildingForm(), new ExtendedSearchBuildingForm())
        {
            ViewModel["restrictions"].BindingSource.Sort = "date";
            ViewModel["ownership_rights"].BindingSource.Sort = "date";
        }

        public void RestrictionsFilterRebuild()
        {
            var assocBindingSource = ViewModel["buildings_restrictions_buildings_assoc"].BindingSource;
            var bindingSource = ViewModel["restrictions"].BindingSource;
            var columnName = ViewModel["buildings_restrictions_buildings_assoc"].PrimaryKeyFirst;
            var restrictionsFilter = columnName+" IN (0";
            for (var i = 0; i < assocBindingSource.Count; i++)
                restrictionsFilter += ((DataRowView)assocBindingSource[i])[columnName] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";
            bindingSource.Filter = restrictionsFilter;
        }

        public void OwnershipsFilterRebuild()
        {
            var assocBindingSource = ViewModel["buildings_ownership_buildings_assoc"].BindingSource;
            var bindingSource = ViewModel["ownership_rights"].BindingSource;
            var columnName = ViewModel["buildings_ownership_buildings_assoc"].PrimaryKeyFirst;
            var restrictionsFilter = columnName + " IN (0";
            for (var i = 0; i < assocBindingSource.Count; i++)
                restrictionsFilter += ((DataRowView)assocBindingSource[i])[columnName] + ",";
            restrictionsFilter = restrictionsFilter.TrimEnd(',');
            restrictionsFilter += ")";
            bindingSource.Filter = restrictionsFilter;
        }

        public bool InsertRecord(Building building)
        {
            var id = ViewModel["general"].Model.Insert(building);
            if (id == -1)
            {
                return false;
            }
            building.IdBuilding = id;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, building.IdBuilding);
            var row = ViewModel["general"].CurrentRow ?? (DataRowView)ViewModel["general"].BindingSource.AddNew();
            EntityConverter<Building>.FillRow(building, row);
            ViewModel["general"].Model.EditingNewRecord = false;
            return true;
        }

        public bool UpdateRecord(Building building)
        {
            if (building.IdBuilding == null)
            {
                MessageBox.Show(@"Вы пытаетесь изменить здание без внутренного номера. " +
                    @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (ViewModel["general"].Model.Update(building) == -1)
                return false;
            RebuildFilterAfterSave(ViewModel["general"].BindingSource, building.IdBuilding);
            var row = ViewModel["general"].CurrentRow;
            EntityConverter<Building>.FillRow(building, row);
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

        public bool HasResettles()
        {
            var row = ViewModel["general"].CurrentRow;
            return row != null && BuildingService.HasResettles((int)row["id_building"]);
        }

        public bool HasTenancies()
        {
            var row = ViewModel["general"].CurrentRow;
            return row != null && BuildingService.HasTenancies((int)row["id_building"]);
        }

        private static void RebuildFilterAfterSave(IBindingListView bindingSource, int? idBuilding)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(bindingSource.Filter))
                filter += " OR ";
            else
                filter += "(1 = 1) OR ";
            filter += string.Format(CultureInfo.CurrentCulture, "(id_building = {0})", idBuilding);
            bindingSource.Filter += filter;
        }
    }
}
