using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class KladrRegionsDataModel : DataModel
    {
        private static KladrRegionsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM v_kladr_regions";
        private const string TableName = "kladr_regions";

        private KladrRegionsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static KladrRegionsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new KladrRegionsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_region"] };
        }
    }
}
