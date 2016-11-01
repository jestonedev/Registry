using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class TenancyAgreementsViewModel: ViewModel
    {
        public TenancyAgreementsViewModel(): base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<TenancyAgreement>.GetInstance())},
            {"kinships", new ViewModelItem(DataModel.GetInstance<KinshipsDataModel>())},
            {"executors", new ViewModelItem(EntityDataModel<Executor>.GetInstance())},
            {"warrants", new ViewModelItem(EntityDataModel<Warrant>.GetInstance())},
            {"rent_types", new ViewModelItem(DataModel.GetInstance<RentTypesDataModel>())},
            {"tenancy_premises_info", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelTenancyPremisesInfo>())}
        })
        {
            this["executors"].BindingSource.Filter = "is_inactive = 0";
        }
    }
}
