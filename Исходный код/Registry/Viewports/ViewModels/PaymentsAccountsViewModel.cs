using System.Collections.Generic;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal class PaymentsAccountsViewModel: ViewModel
    {
        public PaymentsAccountsViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(DataModel.GetInstance<PaymentsAccountsDataModel>())},
            {"claims", new ViewModelItem(EntityDataModel<Claim>.GetInstance())},
            {"claim_states", new ViewModelItem(EntityDataModel<ClaimState>.GetInstance())}
        })
        {
            
        }
    }
}
