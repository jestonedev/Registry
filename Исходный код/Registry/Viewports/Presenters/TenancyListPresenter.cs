using System;
using System.Collections.Generic;
using System.Globalization;
using Registry.DataModels.DataModels;
using Registry.DataModels.Services;
using Registry.Entities;
using Registry.Entities.Infrastructure;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    internal sealed class TenancyListPresenter: Presenter
    {
        public TenancyListPresenter()
            : base(new TenancyListViewModel(), new SimpleSearchTenancyForm(), new ExtendedSearchTenancyForm())
        {
            
        }

        public void AddAssocViewModelItem()
        {
            switch (ParentType)
            {
                case ParentTypeEnum.SubPremises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<TenancySubPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_sub_premises = " + ParentRow["id_sub_premises"];
                    break;
                case ParentTypeEnum.Premises:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<TenancyPremisesAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_premises = " + ParentRow["id_premises"];
                    break;
                case ParentTypeEnum.Building:
                    ViewModel.AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<TenancyBuildingAssoc>.GetInstance()));
                    ViewModel["assoc"].BindingSource.Filter = "id_building = " + ParentRow["id_building"];
                    break;
            }
        }

        public string GetStaticFilter()
        {
            IEnumerable<int> ids;
            if (ParentRow == null)
                return "";
            switch (ParentType)
            {
                case ParentTypeEnum.Building:
                    ids = TenancyService.TenancyProcessIDsByBuildingId(Convert.ToInt32(ParentRow["id_building"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.Premises:
                    ids = TenancyService.TenancyProcessIDsByPremisesId(Convert.ToInt32(ParentRow["id_premises"], CultureInfo.InvariantCulture));
                    break;
                case ParentTypeEnum.SubPremises:
                    ids = TenancyService.TenancyProcessIDsBySubPremisesId(Convert.ToInt32(ParentRow["id_sub_premises"], CultureInfo.InvariantCulture));
                    break;
                default:
                    throw new ViewportException("Неизвестный тип родительского объекта");
            }
            var filter = "";
            if (ids == null) return filter;
            filter = "id_process IN (0";
            foreach (var id in ids)
                filter += id.ToString(CultureInfo.InvariantCulture) + ",";
            filter = filter.TrimEnd(',') + ")";
            return filter;
        }

        internal bool DeleteRecord()
        {
            var id = (int)ViewModel["general"].CurrentRow[ViewModel["general"].PrimaryKeyFirst];
            return ViewModel["general"].Delete(id);
        }
    }
}
