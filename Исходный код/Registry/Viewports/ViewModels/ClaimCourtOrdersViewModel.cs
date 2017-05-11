using System.Collections.Generic;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal class ClaimCourtOrdersViewModel : ViewModel
    {
        public ClaimCourtOrdersViewModel()
            : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<ClaimCourtOrder>.GetInstance())},
            {"payments_accounts", new ViewModelItem(DataModel.GetInstance<PaymentsAccountsDataModel>())},
            {"claim_persons", new ViewModelItem(EntityDataModel<ClaimPerson>.GetInstance())},
            {"judges", new ViewModelItem(EntityDataModel<Judge>.GetInstance())},
            {"judges_buildings_assoc", new ViewModelItem(EntityDataModel<JudgeBuildingAssoc>.GetInstance())},
            {"executors", new ViewModelItem(EntityDataModel<Executor>.GetInstance())},
            {"payments_account_premises_assoc", new ViewModelItem(DataModel.GetInstance<PaymentsAccountsDataModel>())},
        })
        {
        }
    }
}
