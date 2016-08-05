using System;
using System.Data;
using Registry.Entities.Infrastructure;
using Registry.Viewport.SearchForms;
using Registry.Viewport.ViewModels;

namespace Registry.Viewport.Presenters
{
    public class Presenter
    {
        public ViewModel ViewModel { get; private set; }
        public SearchForm SimpleSearchForm { get; private set; }
        public SearchForm ExtendedSearchForm { get; private set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        public Presenter(ViewModel viewModel, SearchForm simpleSearchForm = null, SearchForm extendedSearchForm = null)
        {
            ViewModel = viewModel;
            SimpleSearchForm = simpleSearchForm;
            ExtendedSearchForm = extendedSearchForm;
            if (ViewModel["general"] == null)
            {
                throw new ViewportException("Не задана обязательная ViewModel с именем general");
            }
        }

        public void SetGeneralBindingSourceFilter(string staticFilter, string dynamicFilter)
        {
            ViewModel["general"].BindingSource.Filter = staticFilter;
            if (!string.IsNullOrEmpty(staticFilter) && !string.IsNullOrEmpty(dynamicFilter))
                ViewModel["general"].BindingSource.Filter += " AND ";
            ViewModel["general"].BindingSource.Filter += dynamicFilter;
        }

        public void LocateEntityBy(string fieldName, object value)
        {
            var position = ViewModel["general"].BindingSource.Find(fieldName, value);
            if (position > 0)
                ViewModel["general"].BindingSource.Position = position;
        }

        public int GetCurrentId()
        {
            var row = ViewModel["general"].CurrentRow;
            if (row == null)
            {
                return -1;
            }
            if (row[ViewModel["general"].PrimaryKeyFirst] != DBNull.Value)
                return (int)row[ViewModel["general"].PrimaryKeyFirst];
            return -1;
        }
    }
}
