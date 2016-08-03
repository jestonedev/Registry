using System;

namespace Registry.DataModels.DataModels
{
    public sealed class TenancyNotifiesDataModel : DataModel
    {
        private static TenancyNotifiesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_notifies WHERE deleted <> 1";
        private const string TableName = "tenancy_notifies";

        private TenancyNotifiesDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static TenancyNotifiesDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new TenancyNotifiesDataModel(afterLoadHandler));
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
