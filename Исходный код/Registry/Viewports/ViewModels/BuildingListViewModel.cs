using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class BuildingListViewModel : ViewModel
    {

        public BuildingListViewModel()
            : base(new Dictionary
                <string, ViewModelItem>
            {
                {"general", new ViewModelItem(EntityDataModel<Building>.GetInstance())},
                {"kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())},
                {"object_states", new ViewModelItem(DataModel.GetInstance<ObjectStatesDataModel>())},
                {"structure_types", new ViewModelItem(EntityDataModel<StructureType>.GetInstance())},
                {"heating_types", new ViewModelItem(EntityDataModel<HeatingType>.GetInstance())},
                {"municipal_premises", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelMunicipalPremises>())}
            })
        {
        }
    }
}