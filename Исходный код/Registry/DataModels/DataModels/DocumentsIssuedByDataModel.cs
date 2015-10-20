using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class DocumentsIssuedByDataModel : DataModel
    {
        private static DocumentsIssuedByDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM documents_issued_by WHERE deleted <> 1";
        private const string TableName = "documents_issued_by";

        private DocumentsIssuedByDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static DocumentsIssuedByDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new DocumentsIssuedByDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_document_issued_by"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_document_issued_by", "tenancy_persons", "id_document_issued_by");  
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE documents_issued_by SET deleted = 1 WHERE id_document_issued_by = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_issued_by", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE documents_issued_by SET document_issued_by = ? WHERE id_document_issued_by = ?";
            var documentIssuedBy = (DocumentIssuedBy) entity;
            command.Parameters.Add(DBConnection.CreateParameter("document_issued_by", documentIssuedBy.DocumentIssuedByName));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_issued_by", documentIssuedBy.IdDocumentIssuedBy));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO documents_issued_by (document_issued_by) VALUES (?)";
            var documentIssuedBy = (DocumentIssuedBy)entity;
            command.Parameters.Add(DBConnection.CreateParameter("document_issued_by", documentIssuedBy.DocumentIssuedByName));
        }
    }
}
