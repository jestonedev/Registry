using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal class ClaimListViewModel: ViewModel
    {
        public ClaimListViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<Claim>.GetInstance())},
            {"payments_accounts", new ViewModelItem(DataModel.GetInstance<PaymentsAccountsDataModel>())},
            {"claim_states", new ViewModelItem(EntityDataModel<ClaimState>.GetInstance())},
            {"last_claim_states", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelLastClaimStates>())}
        })
        {
            
        }
    }
}
