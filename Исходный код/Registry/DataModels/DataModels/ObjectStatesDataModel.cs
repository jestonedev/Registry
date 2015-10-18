using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public class ObjectStatesDataModel: DataModel
    {
        private const string SelectQuery = "SELECT * FROM object_states";
        private const string TableName = "object_states";

        private ObjectStatesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_state"] };
        }
    }
}
