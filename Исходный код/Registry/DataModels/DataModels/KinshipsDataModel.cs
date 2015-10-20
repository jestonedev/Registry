using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class KinshipsDataModel : DataModel
    {
        private static KinshipsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM kinships";
        private const string TableName = "kinships";

        private KinshipsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static KinshipsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new KinshipsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_kinship"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_kinship", "tenancy_persons", "id_kinship");
        }
    }
}
