using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class KladrStreetsDataModel : DataModel
    {
        private static KladrStreetsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM v_kladr_streets";
        private const string TableName = "kladr";

        private KladrStreetsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static KladrStreetsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new KladrStreetsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_street"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_street", "buildings", "id_street");
        }
    }
}
