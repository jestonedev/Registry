using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class RentTypesDataModel : DataModel
    {
        private static RentTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM rent_types rt ORDER BY rt.rent_type DESC";
        private const string TableName = "rent_types";

        private RentTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static RentTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new RentTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_rent_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_rent_type", "tenancy_processes", "id_rent_type");
        }
    }
}
