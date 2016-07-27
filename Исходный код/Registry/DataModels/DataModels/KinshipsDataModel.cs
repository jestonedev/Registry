using System;

namespace Registry.DataModels.DataModels
{
    internal sealed class KinshipsDataModel : DataModel
    {
        private static KinshipsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM kinships";
        private const string TableName = "kinships";

        private KinshipsDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static KinshipsDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new KinshipsDataModel(afterLoadHandler));
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
