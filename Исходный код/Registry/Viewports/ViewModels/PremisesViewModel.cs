using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class PremisesViewModel: ViewModel
    {
        public PremisesViewModel(): base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<Premise>.GetInstance())},
            {"buildings", new ViewModelItem(EntityDataModel<Building>.GetInstance())},
            {"sub_premises", new ViewModelItem(EntityDataModel<SubPremise>.GetInstance())},
            {"restrictions", new ViewModelItem(EntityDataModel<Restriction>.GetInstance())},
            {"restriction_types", new ViewModelItem(EntityDataModel<RestrictionType>.GetInstance())},
            {"restrictions_buildings_assoc", new ViewModelItem(EntityDataModel<RestrictionBuildingAssoc>.GetInstance())},
            {"restrictions_premises_assoc", new ViewModelItem(EntityDataModel<RestrictionPremisesAssoc>.GetInstance())},
            {"ownership_rights", new ViewModelItem(EntityDataModel<OwnershipRight>.GetInstance())},
            {"ownership_right_types", new ViewModelItem(EntityDataModel<OwnershipRightType>.GetInstance())},
            {"ownership_buildings_assoc", new ViewModelItem(EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance())},
            {"ownership_premises_assoc", new ViewModelItem(EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance())},
            {"kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())},
            {"premises_types", new ViewModelItem(DataModel.GetInstance<PremisesTypesDataModel>())},
            {"premises_kinds", new ViewModelItem(DataModel.GetInstance<PremisesKindsDataModel>())},
            {"fund_types", new ViewModelItem(DataModel.GetInstance<FundTypesDataModel>())},
            {"object_states", new ViewModelItem(DataModel.GetInstance<ObjectStatesDataModel>())},
            {"sub_premises_object_states", new ViewModelItem(DataModel.GetInstance<ObjectStatesDataModel>())},
            {"current_funds", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelPremisesCurrentFunds>())},
            {"sub_premises_sum_area", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelPremiseSubPremisesSumArea>())},
            {"sub_premises_current_funds", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelSubPremisesCurrentFunds>())},
            {"municipal_premises", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelMunicipalPremises>())}
        })
        {
            AddViewModeItem("premises_restrictions_premises_assoc",
                new ViewModelItem(EntityDataModel<RestrictionPremisesAssoc>.GetInstance(), this["general"].BindingSource,
                    "premises_restrictions_premises_assoc"));
            AddViewModeItem("premises_ownership_premises_assoc",
                new ViewModelItem(EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance(),
                    this["general"].BindingSource, "premises_ownership_premises_assoc"));
            AddViewModeItem("kladr_buildings",
                new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>(),
                    this["kladr"].BindingSource, "kladr_buildings"));
            AddViewModeItem("premises_sub_premises",
                new ViewModelItem(EntityDataModel<SubPremise>.GetInstance(),
                    this["general"].BindingSource, "premises_sub_premises"));
        }
    }
}
