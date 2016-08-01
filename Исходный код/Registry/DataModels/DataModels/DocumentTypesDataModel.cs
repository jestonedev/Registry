using System;

namespace Registry.DataModels.DataModels
{
    internal sealed class DocumentTypesDataModel : DataModel
    {
        private static DocumentTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM document_types";
        private const string TableName = "document_types";

        private DocumentTypesDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static DocumentTypesDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new DocumentTypesDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_document_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_document_type", "tenancy_persons", "id_document_type");
        }
    }
}
