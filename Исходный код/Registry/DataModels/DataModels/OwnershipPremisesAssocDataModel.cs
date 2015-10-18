using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public sealed class OwnershipPremisesAssocDataModel : DataModel
    {
        private const string SelectQuery = "SELECT * FROM ownership_premises_assoc WHERE deleted = 0";
        private const string TableName = "ownership_premises_assoc";

        private OwnershipPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_ownership_right"] };
        }
    }
}
