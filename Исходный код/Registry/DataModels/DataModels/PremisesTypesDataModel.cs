using System;

namespace Registry.DataModels.DataModels
{
    public sealed class PremisesTypesDataModel : DataModel
    {
        private static PremisesTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM premises_types";
        private const string TableName = "premises_types";

        private PremisesTypesDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {          
        }

        public static PremisesTypesDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new PremisesTypesDataModel(afterLoadHandler));
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
