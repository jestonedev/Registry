using System;

namespace Registry.DataModels.DataModels
{
    internal sealed class PremisesKindsDataModel : DataModel
    {
        private static PremisesKindsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM premises_kinds";
        private const string TableName = "premises_kinds";

        private PremisesKindsDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static PremisesKindsDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new PremisesKindsDataModel(afterLoadHandler));
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
