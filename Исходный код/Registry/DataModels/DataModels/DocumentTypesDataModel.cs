using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public sealed class DocumentTypesDataModel: DataModel
    {
        private const string SelectQuery = "SELECT * FROM document_types";
        private const string TableName = "document_types";

        private DocumentTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_document_type"] };
        }
    }
}
