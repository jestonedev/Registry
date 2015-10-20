using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyNotifiesDataModel : DataModel
    {
        private static TenancyNotifiesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_notifies WHERE deleted <> 1";
        private const string TableName = "tenancy_notifies";

        private TenancyNotifiesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyNotifiesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyNotifiesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_notify"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("tenancy_processes", "id_process", TableName, "id_process");
        }
    }
}
