using System.Collections.Generic;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class ResettleProcessListViewModel: ViewModel
    {
        public ResettleProcessListViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<ResettleProcess>.GetInstance())},
            {"documents_residence", new ViewModelItem(EntityDataModel<DocumentResidence>.GetInstance())},
            {"resettle_aggregate", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelResettleAggregated>())}
        })
        {
            
        }
    }
}
