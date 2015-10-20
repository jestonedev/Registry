using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal class ObjectStatesDataModel : DataModel
    {
        private static ObjectStatesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM object_states";
        private const string TableName = "object_states";

        private ObjectStatesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static ObjectStatesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ObjectStatesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_state"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_state", "premises", "id_state");
            AddRelation(TableName, "id_state", "sub_premises", "id_state");
            AddRelation(TableName, "id_state", "buildings", "id_state");
        }
    }
}
