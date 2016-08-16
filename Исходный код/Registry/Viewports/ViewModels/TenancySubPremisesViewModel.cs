using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport.ViewModels
{
    internal sealed class TenancySubPremisesViewModel: SnapshotedViewModel
    {
        public TenancySubPremisesViewModel()
            : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<SubPremise>.GetInstance())},
            {"tenancy_sub_premises_assoc", new ViewModelItem(EntityDataModel<TenancySubPremisesAssoc>.GetInstance())}
        })
        {
            
        }

        public override void InitializeSnapshot()
        {
            SnapshotDataSource = new DataTable("selected_sub_premises") { Locale = CultureInfo.InvariantCulture };
            SnapshotDataSource.Columns.Add("id_assoc").DataType = typeof(int);
            SnapshotDataSource.Columns.Add("id_sub_premises").DataType = typeof(int);
            SnapshotDataSource.Columns.Add("is_checked").DataType = typeof(bool);
            SnapshotDataSource.Columns.Add("rent_total_area").DataType = typeof(double);
            SnapshotBindingSource = new BindingSource
            {
                DataSource = SnapshotDataSource
            };
            LoadSnapshot();
        }

        public override void LoadSnapshot()
        {
            SnapshotDataSource.Clear();
            var assoc = this["tenancy_sub_premises_assoc"].BindingSource;
            foreach (var row in assoc)
                SnapshotDataSource.Rows.Add(TenancySubPremiseConverter.ToArray((DataRowView)row));
        }
    }
}
