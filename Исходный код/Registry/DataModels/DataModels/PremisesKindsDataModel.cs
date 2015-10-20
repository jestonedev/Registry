using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class PremisesKindsDataModel : DataModel
    {
        private static PremisesKindsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM premises_kinds";
        private const string TableName = "premises_kinds";

        private PremisesKindsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static PremisesKindsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new PremisesKindsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_premises_kind"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_premises_kind", "premises", "id_premises_kind");
        }
    }
}
