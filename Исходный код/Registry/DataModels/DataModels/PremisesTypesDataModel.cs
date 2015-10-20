using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class PremisesTypesDataModel : DataModel
    {
        private static PremisesTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM premises_types";
        private const string TableName = "premises_types";

        private PremisesTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {          
        }

        public static PremisesTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new PremisesTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_premises_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_premises_type", "premises", "id_premises_type");
        }
    }
}
