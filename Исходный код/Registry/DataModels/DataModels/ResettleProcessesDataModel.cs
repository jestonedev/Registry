using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal class ResettleProcessesDataModel : DataModel
    {
        private static ResettleProcessesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM resettle_processes WHERE deleted = 0";
        private const string TableName = "resettle_processes";

        private ResettleProcessesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static ResettleProcessesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ResettleProcessesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_process"] };
            Table.Columns["debts"].DefaultValue = 0;
        }

        protected override void ConfigureRelations()
        {
            AddRelation("documents_residence", "id_document_residence", TableName, "id_document_residence");
            AddRelation(TableName, "id_process", "resettle_buildings_from_assoc", "id_process");
            AddRelation(TableName, "id_process", "resettle_buildings_to_assoc", "id_process");
            AddRelation(TableName, "id_process", "resettle_premises_from_assoc", "id_process");
            AddRelation(TableName, "id_process", "resettle_premises_to_assoc", "id_process");
            AddRelation(TableName, "id_process", "resettle_sub_premises_from_assoc", "id_process");
            AddRelation(TableName, "id_process", "resettle_sub_premises_to_assoc", "id_process");
            AddRelation(TableName, "id_process", "resettle_persons", "id_process");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE resettle_processes SET deleted = 1 WHERE id_process = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO resettle_processes
                            (resettle_date, id_document_residence, debts, description)
                            VALUES (?, ?, ?, ?)";
            var resettle = (ResettleProcess)entity;
            command.Parameters.Add(DBConnection.CreateParameter("resettle_date", resettle.ResettleDate));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_residence", resettle.IdDocumentResidence));
            command.Parameters.Add(DBConnection.CreateParameter("debts", resettle.Debts));
            command.Parameters.Add(DBConnection.CreateParameter("description", resettle.Description));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE resettle_processes SET resettle_date = ?, id_document_residence = ?, debts = ?, 
                            description = ? WHERE id_process = ?";
            var resettle = (ResettleProcess) entity;
            command.Parameters.Add(DBConnection.CreateParameter("resettle_date", resettle.ResettleDate));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_residence", resettle.IdDocumentResidence));
            command.Parameters.Add(DBConnection.CreateParameter("debts", resettle.Debts));
            command.Parameters.Add(DBConnection.CreateParameter("description", resettle.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", resettle.IdProcess));
        }
    }
}
