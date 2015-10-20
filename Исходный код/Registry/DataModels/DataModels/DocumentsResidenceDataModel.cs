using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class DocumentsResidenceDataModel : DataModel
    {
        private static DocumentsResidenceDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM documents_residence WHERE deleted <> 1";
        private const string TableName = "documents_residence";

        private DocumentsResidenceDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static DocumentsResidenceDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new DocumentsResidenceDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_document_residence"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_document_residence", "resettle_processes", "id_document_residence"); 
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE documents_residence SET deleted = 1 WHERE id_document_residence = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_residence", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE documents_residence SET document_residence = ? WHERE id_document_residence = ?";
            var documentResidence = (DocumentResidence) entity;
            command.Parameters.Add(DBConnection.CreateParameter("document_residence", documentResidence.DocumentResidenceName));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_residence", documentResidence.IdDocumentResidence));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO documents_residence (document_residence) VALUES (?)";
            var documentResidence = (DocumentResidence)entity;
            command.Parameters.Add(DBConnection.CreateParameter("document_residence", documentResidence.DocumentResidenceName));
        }
    }
}
