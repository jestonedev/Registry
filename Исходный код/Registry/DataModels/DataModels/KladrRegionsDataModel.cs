using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public sealed class KladrRegionsDataModel : DataModel
    {
        private const string SelectQuery = "SELECT * FROM v_kladr_regions";
        private const string TableName = "kladr_regions";

        private KladrRegionsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_region"] };
        }
    }
}
