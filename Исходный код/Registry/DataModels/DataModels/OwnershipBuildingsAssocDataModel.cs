using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public sealed class OwnershipBuildingsAssocDataModel : DataModel
    {
        private const string SelectQuery = "SELECT * FROM ownership_buildings_assoc WHERE deleted = 0";
        private const string TableName = "ownership_buildings_assoc";

        private OwnershipBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_ownership_right"] };
        }
    }
}
