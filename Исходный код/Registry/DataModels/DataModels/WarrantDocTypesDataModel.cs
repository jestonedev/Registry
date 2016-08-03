using System;

namespace Registry.DataModels.DataModels
{
    public sealed class WarrantDocTypesDataModel : DataModel
    {
        private static WarrantDocTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM warrant_doc_types";
        private const string TableName = "warrant_doc_types";

        private WarrantDocTypesDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static WarrantDocTypesDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new WarrantDocTypesDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_warrant_doc_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_warrant_doc_type", "warrants", "id_warrant_doc_type");
        }
    }
}
