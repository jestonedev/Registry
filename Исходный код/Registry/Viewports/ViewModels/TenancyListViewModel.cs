using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class TenancyListViewModel: ViewModel
    {
        public TenancyListViewModel(): base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<TenancyProcess>.GetInstance())},
            {"rent_types", new ViewModelItem(DataModel.GetInstance<RentTypesDataModel>())},
            {"tenancy_aggregated", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelTenancyAggregated>())},
            {"tenancy_payments_info", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelTenancyPaymentsInfo>())}
        })
        {
        }
    }
}
