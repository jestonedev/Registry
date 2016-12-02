using System.Windows.Forms;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;
using Security;

namespace Registry.Viewport.Presenters
{
    internal sealed class PremisesListPresenter : Presenter
    {
        public PremisesListPresenter()
            : base(new PremisesListViewModel(), new SimpleSearchPremiseForm(), new ExtendedSearchPremisesForm())
        {
            
        }

        public bool DeleteRecord()
        {
            var id = (int)ViewModel["general"].CurrentRow[ViewModel["general"].PrimaryKeyFirst];
            if (OtherService.HasMunicipal(id, EntityType.Premise) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление муниципальных жилых помещений и помещений, в которых присутствуют муниципальные комнаты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            if (OtherService.HasNotMunicipal(id, EntityType.Premise) && !AccessControl.HasPrivelege(Priveleges.RegistryWriteNotMunicipal))
            {
                MessageBox.Show(@"У вас нет прав на удаление немуниципальных жилых помещений и помещений, в которых присутствуют немуниципальные комнаты",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            return ViewModel["general"].Delete(id);
        }

        public bool HasResettles()
        {
            var row = ViewModel["general"].CurrentRow;
            return row != null && PremisesService.HasResettles((int)row["id_premises"]);
        }

        public bool HasTenancies()
        {
            var row = ViewModel["general"].CurrentRow;
            return row != null && PremisesService.HasTenancies((int)row["id_premises"]);
        }
    }
}
