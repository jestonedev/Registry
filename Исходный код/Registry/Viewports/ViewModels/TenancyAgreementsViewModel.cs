using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            {"rent_types", new ViewModelItem(DataModel.GetInstance<RentTypesDataModel>())}
        })
        {
            this["executors"].BindingSource.Filter = "is_inactive = 0";
        }
    }
}
