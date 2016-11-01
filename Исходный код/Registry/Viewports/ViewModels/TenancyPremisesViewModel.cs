using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Registry.DataModels.CalcDataModels;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Registry.Viewport.EntityConverters;

namespace Registry.Viewport.ViewModels
{
    internal sealed class TenancyPremisesViewModel : SnapshotedViewModel
    {
        public TenancyPremisesViewModel()
            : base(new Dictionary<string, ViewModelItem>
        {
            {"general", new ViewModelItem(EntityDataModel<Premise>.GetInstance())},
            {"buildings", new ViewModelItem(EntityDataModel<Building>.GetInstance())},
            {"tenancy_premises_assoc", new ViewModelItem(EntityDataModel<TenancyPremisesAssoc>.GetInstance())},
            {"premises_types", new ViewModelItem(DataModel.GetInstance<PremisesTypesDataModel>())},
            {"kladr", new ViewModelItem(DataModel.GetInstance<KladrStreetsDataModel>())},
            {"object_states", new ViewModelItem(DataModel.GetInstance<ObjectStatesDataModel>())},
            {"fund_types", new ViewModelItem(DataModel.GetInstance<FundTypesDataModel>())},
            {"premises_current_funds", new ViewModelItem(CalcDataModel.GetInstance<CalcDataModelPremisesCurrentFunds>())}
        })
        {
            
        }

        public override void InitializeSnapshot()
        {
            SnapshotDataSource = new DataTable("selected_premises") { Locale = CultureInfo.InvariantCulture };
            SnapshotDataSource.Columns.Add("id_assoc").DataType = typeof(int);
            SnapshotDataSource.Columns.Add("id_premises").DataType = typeof(int);
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
            var assoc = this["tenancy_premises_assoc"].BindingSource;
            foreach (var row in assoc)
                SnapshotDataSource.Rows.Add(TenancyPremiseConverter.ToArray((DataRowView)row));
        }
    }
}
