using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Security;

namespace Registry.Viewport.ViewModels
{
    internal sealed class PremisesListViewModel: ViewModel
    {

        public PremisesListViewModel()
            : base(new Dictionary<string, ViewModelItem>
            {
                {"general", new ViewModelItem(EntityDataModel<Premise>.GetInstance())},
                {"sub_premises", new ViewModelItem(EntityDataModel<SubPremise>.GetInstance())},
                {"buildings", new ViewModelItem(EntityDataModel<Building>.GetInstance())},
                {"ownership_rights", new ViewModelItem(EntityDataModel<OwnershipRight>.GetInstance())},
                {"ownership_buildings_assoc", new ViewModelItem(EntityDataModel<OwnershipRightBuildingAssoc>.GetInstance())},
                {"ownership_premises_assoc", new ViewModelItem(EntityDataModel<OwnershipRightPremisesAssoc>.GetInstance())},
                {"kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())},
                {"object_states", new ViewModelItem(DataModel.GetInstance<ObjectStatesDataModel>())},
                {"premises_types", new ViewModelItem(DataModel.GetInstance<PremisesTypesDataModel>())},
                {"fund_types", new ViewModelItem(DataModel.GetInstance<FundTypesDataModel>())},
                {"premises_current_funds", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelPremisesCurrentFunds>())}
            })
        {
            if (AccessControl.HasPrivelege(Priveleges.TenancyRead))
            {
                AddViewModeItem("premises_tenancies_info", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelPremisesTenanciesInfo>()));
                AddViewModeItem("tenancy_payments_info", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelTenancyPaymentsInfo>()));
            }
        }
    }
}
