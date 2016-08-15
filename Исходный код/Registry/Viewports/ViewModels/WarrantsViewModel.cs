using System.Collections.Generic;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class WarrantsViewModel: ViewModel
    {
        public WarrantsViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<Warrant>.GetInstance())},
            {"warrant_doc_types", new ViewModelItem(DataModel.GetInstance<WarrantDocTypesDataModel>())}
        })
        {
            this["general"].BindingSource.Sort = "registration_date DESC";
        }
    }
}
