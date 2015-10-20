using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class DocumentTypesDataModel : DataModel
    {
        private static DocumentTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM document_types";
        private const string TableName = "document_types";

        private DocumentTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static DocumentTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new DocumentTypesDataModel(progressBar, incrementor));
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
