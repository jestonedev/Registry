using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport.ViewModels
{
    internal sealed class ResettleBuildingsViewModel: SnapshotedViewModel
    {
        public ResettleEstateObjectWay Way { get; private set; }

        public ResettleBuildingsViewModel()
            : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<Building>.GetInstance())},
            {"kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())}
        })
        {
            
        }

        public void AddAssocViewModelItem(ResettleEstateObjectWay way)
        {
            switch (way)
            {
                case ResettleEstateObjectWay.From:
                    AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<ResettleBuildingFromAssoc>.GetInstance()));
                    break;
                case ResettleEstateObjectWay.To:
                    AddViewModeItem("assoc", new ViewModelItem(EntityDataModel<ResettleBuildingToAssoc>.GetInstance()));
                    break;
                default:
                    throw new ViewportException("Неподдерживаемый тип ResettleEstateObjectWay");
            }
            Way = way;
        }

        public override void InitializeSnapshot()
        {
            SnapshotDataSource = new DataTable("selected_buildings") { Locale = CultureInfo.InvariantCulture };
            SnapshotDataSource.Columns.Add("id_assoc").DataType = typeof(int);
            SnapshotDataSource.Columns.Add("id_building").DataType = typeof(int);
            SnapshotDataSource.Columns.Add("is_checked").DataType = typeof(bool);
            SnapshotBindingSource = new BindingSource
            {
                DataSource = SnapshotDataSource
            };
            LoadSnapshot();
        }

        public override void LoadSnapshot()
        {
            SnapshotDataSource.Clear();
            var assoc = this["assoc"].BindingSource;
            foreach (var row in assoc)
                SnapshotDataSource.Rows.Add(ResettleBuildingConverter.ToArray((DataRowView)row));
        }
    }
}
