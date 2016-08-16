using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport.ViewModels
{
    internal sealed class TenancyBuildingsViewModel: SnapshotedViewModel
    {
        public TenancyBuildingsViewModel() : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<Building>.GetInstance())},
            {"tenancy_buildings_assoc", new ViewModelItem(EntityDataModel<TenancyBuildingAssoc>.GetInstance())},
            {"kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())}
        })
        {
            
        }

        public override void InitializeSnapshot()
        {
            SnapshotDataSource = new DataTable("selected_buildings") { Locale = CultureInfo.InvariantCulture };
            SnapshotDataSource.Columns.Add("id_assoc").DataType = typeof(int);
            SnapshotDataSource.Columns.Add("id_building").DataType = typeof(int);
            SnapshotDataSource.Columns.Add("is_checked").DataType = typeof(bool);
            SnapshotDataSource.Columns.Add("rent_total_area").DataType = typeof(double);
            SnapshotDataSource.Columns.Add("rent_living_area").DataType = typeof(double);
            SnapshotBindingSource = new BindingSource
            {
                DataSource = SnapshotDataSource
            };
            LoadSnapshot();
        }

        public override void LoadSnapshot()
        {
            SnapshotDataSource.Clear();
            var assoc = this["tenancy_buildings_assoc"].BindingSource;
            foreach (var row in assoc)
                SnapshotDataSource.Rows.Add(TenancyBuildingConverter.ToArray((DataRowView)row));
        }
    }
}
