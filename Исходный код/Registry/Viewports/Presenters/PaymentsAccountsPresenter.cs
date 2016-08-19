using System.Collections.Generic;
using System.Linq;
using Registry.DataModels.Services;
using Registry.Entities.Infrastructure;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class PaymentsAccountsPresenter: Presenter
    {
        public PaymentsAccountsPresenter()
            : base(
                new PaymentsAccountsViewModel(), new ExtendedSearchPaymentAccounts(),
                new ExtendedSearchPaymentAccounts())
        {
            
        }

        internal string GetStaticFilter(string filter)
        {
            if (ParentRow == null) return "";
            IEnumerable<int> ids = new List<int>();
            switch (ParentType)
            {
                case ParentTypeEnum.Premises:
                    ids = PaymentService.GetAccountIdsByPremiseFilter(filter);
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = PaymentService.GetAccountIdsBySubPremiseFilter(filter);
                    break;
                case ParentTypeEnum.Claim:
                    ids = new List<int> { (int)ParentRow["id_account"] };
                    break;
            }
            if (ids.Any())
                return "id_account IN (" + ids.Select(id => id.ToString()).Aggregate((acc, v) => acc + "," + v) + ")";
            return "id_account IN (0)";
        }
    }
}
