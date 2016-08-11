using System.Collections.Generic;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.Viewport.ViewModels
{
    internal sealed class TenancyPersonsViewModel: ViewModel
    {
        public TenancyPersonsViewModel(): base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<TenancyPerson>.GetInstance())},
            {"kinships", new ViewModelItem(DataModel.GetInstance<KinshipsDataModel>())},
            {"document_types", new ViewModelItem(DataModel.GetInstance<DocumentTypesDataModel>())},
            {"documents_issued_by", new ViewModelItem(EntityDataModel<DocumentIssuedBy>.GetInstance())},
            {"registration_kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())},
            {"residence_kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())},
        })
        {
            this["documents_issued_by"].BindingSource.Sort = "document_issued_by";
        }
    }
}
