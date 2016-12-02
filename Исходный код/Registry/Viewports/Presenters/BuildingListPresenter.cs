using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;
using Security;

namespace Registry.Viewport.Presenters
{
    internal sealed class BuildingListPresenter: Presenter
    {
        public BuildingListPresenter()
            : base(new BuildingListViewModel(), new SimpleSearchBuildingForm(), new ExtendedSearchBuildingForm())
        {
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
    }
}
