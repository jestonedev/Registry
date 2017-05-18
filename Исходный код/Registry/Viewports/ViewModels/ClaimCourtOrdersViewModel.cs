using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
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
            {"claims", new ViewModelItem(EntityDataModel<Claim>.GetInstance())},
            {"selectable_signers", new ViewModelItem(SelectableSignersDataModel.GetInstance())},
            {"payments_account_premises_assoc", new ViewModelItem(DataModel.GetInstance<PaymentsAccountsDataModel>())},
            {"judges_buildings_assoc", new ViewModelItem(EntityDataModel<JudgeBuildingAssoc>.GetInstance())},
            {"judge_info", new ViewModelItem(CalcDataModelJudgeInfo.GetInstance())}
        })
        {
            this["selectable_signers"].BindingSource.Filter = "id_signer_group = 3";
        }
    }
}
