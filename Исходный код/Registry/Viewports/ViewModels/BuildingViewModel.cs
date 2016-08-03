using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class BuildingViewModel : ViewModel
    {
        public BuildingViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<Building>.GetInstance())},
            {"premises", new ViewModelItem(EntityDataModel<Premise>.GetInstance())},
            {"restrictions", new ViewModelItem(EntityDataModel<Restriction>.GetInstance())},
            {"restriction_types", new ViewModelItem(EntityDataModel<RestrictionType>.GetInstance())},
            {"restrictions_buildings_assoc", new ViewModelItem(EntityDataModel<RestrictionBuildingAssoc>.GetInstance())},
            {"ownership_rights", new ViewModelItem(EntityDataModel<OwnershipRight>.GetInstance())},
            {"ownership_right_types", new ViewModelItem(EntityDataModel<OwnershipRightType>.GetInstance())},
            {"ownership_buildings_assoc", new ViewModelItem(EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance())},
            {"structure_types", new ViewModelItem(EntityDataModel<StructureType>.GetInstance())},
            {"heating_types", new ViewModelItem(EntityDataModel<HeatingType>.GetInstance())},
            {"kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())},
            {"fund_types", new ViewModelItem(DataModel.GetInstance<FundTypesDataModel>())},
            {"object_states", new ViewModelItem(DataModel.GetInstance<ObjectStatesDataModel>())},
            {"buildings_premises_funds", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelBuildingsPremisesFunds>())},
            {"buildings_current_funds", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelBuildingsCurrentFunds>())},
            {"buildings_premises_sum_area", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelBuildingsPremisesSumArea>())},
            {"municipal_premises", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelMunicipalPremises>())},
        })
        {
            AddViewModeItem("buildings_restrictions_buildings_assoc",
                new ViewModelItem(EntityDataModel<RestrictionBuildingAssoc>.GetInstance(), this["general"].BindingSource,
                    "buildings_restrictions_buildings_assoc"));
            AddViewModeItem("buildings_ownership_buildings_assoc",
                new ViewModelItem(EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance(),
                    this["general"].BindingSource, "buildings_ownership_buildings_assoc"));
        }
    }
}
