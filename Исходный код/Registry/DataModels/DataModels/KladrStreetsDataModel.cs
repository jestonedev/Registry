using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public sealed class KladrStreetsDataModel : DataModel
    {
        private const string SelectQuery = "SELECT * FROM v_kladr_streets";
        private const string TableName = "kladr";

        private KladrStreetsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_street"] };
        }
    }
}
