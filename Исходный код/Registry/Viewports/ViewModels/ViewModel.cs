using System.Collections.Generic;
using Registry.DataModels.DataModels;

namespace Registry.Viewport.ViewModels
{
    public class ViewModel
    {
        private Dictionary<string, ViewModelItem> ViewModelCollection { get; set; }

        public ViewModel(Dictionary<string, ViewModelItem> viewModelCollection = null)
        {
            ViewModelCollection = viewModelCollection ?? new Dictionary<string, ViewModelItem>();
        }

        public ViewModelItem GetViewModel(DataModel model)
        {
            foreach (var viewModel in ViewModelCollection)
            {
                if (viewModel.Value.Model == model)
                {
                    return viewModel.Value;
                }
            }
            return null;
        }

        public ViewModelItem GetViewModel(string viewModelName)
        {
            foreach (var viewModel in ViewModelCollection)
            {
                if (viewModel.Key == viewModelName)
                {
                    return viewModel.Value;
                }
            }
            return null;
        }

        public void AddViewModeItem(string name, ViewModelItem item)
        {
            ViewModelCollection.Add(name, item);
        }

        public ViewModelItem this[string viewModelName] {
            get { return GetViewModel(viewModelName); }
        }

        public ViewModelItem this[DataModel model]
        {
            get { return GetViewModel(model); }
        }
    }
}
