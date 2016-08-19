using System.Collections.Generic;
using Registry.DataModels.DataModels;

namespace Registry.Viewport.ViewModels
{
    internal class PaymentsAccountsViewModel: ViewModel
    {
        public PaymentsAccountsViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(DataModel.GetInstance<PaymentsAccountsDataModel>())}
        })
        {
            
        }
    }
}
