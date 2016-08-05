using System.Collections.Generic;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class FundsHistoryViewModel: ViewModel
    {
        public FundsHistoryViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<FundHistory>.GetInstance())},
            {"fund_types", new ViewModelItem(DataModel.GetInstance<FundTypesDataModel>())}
        })
        {           
        }
    }
}
