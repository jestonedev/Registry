using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public sealed class KinshipsDataModel: DataModel
    {
        private const string SelectQuery = "SELECT * FROM kinships";
        private const string TableName = "kinships";

        private KinshipsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_kinship"] };
        }
    }
}
