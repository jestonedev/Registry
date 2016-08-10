using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class TenancyViewModel: ViewModel
    {
        public TenancyViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<TenancyProcess>.GetInstance())},
            {"executors", new ViewModelItem(EntityDataModel<Executor>.GetInstance())},
            {"rent_types", new ViewModelItem(DataModel.GetInstance<RentTypesDataModel>())},
            {"warrants", new ViewModelItem(EntityDataModel<Warrant>.GetInstance())},
            {"kinships", new ViewModelItem(DataModel.GetInstance<KinshipsDataModel>())},
            {"tenancy_premises_info", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelTenancyPremisesInfo>())}
        })
        {
            AddViewModeItem("tenancy_processes_tenancy_rent_periods_history",
                new ViewModelItem(EntityDataModel<TenancyRentPeriod>.GetInstance(), this["general"].BindingSource,
                    "tenancy_processes_tenancy_rent_periods_history"));
            AddViewModeItem("tenancy_processes_tenancy_reasons",
                new ViewModelItem(EntityDataModel<TenancyReason>.GetInstance(), this["general"].BindingSource,
                    "tenancy_processes_tenancy_reasons"));
            AddViewModeItem("tenancy_processes_tenancy_persons",
                new ViewModelItem(EntityDataModel<TenancyPerson>.GetInstance(), this["general"].BindingSource,
                    "tenancy_processes_tenancy_persons"));
            AddViewModeItem("tenancy_processes_tenancy_agreements",
                new ViewModelItem(EntityDataModel<TenancyAgreement>.GetInstance(), this["general"].BindingSource,
                    "tenancy_processes_tenancy_agreements"));
        }
    }
}
