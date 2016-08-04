using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.Viewport.ViewModels
{
    public abstract class SnapshotedViewModel: ViewModel
    {
        public BindingSource SnapshotBindingSource { get; protected set; }
        public DataTable SnapshotDataSource { get; protected set; }

        protected SnapshotedViewModel(Dictionary<string, ViewModelItem> viewModelCollection = null) : base(viewModelCollection)
        {
        }

        public abstract void InitializeSnapshot();
        public abstract void LoadSnapshot();
    }
}
