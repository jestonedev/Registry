using System;
using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyProcessesDataModel : DataModel
    {
        private static TenancyProcessesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_processes WHERE deleted = 0";
        private const string TableName = "tenancy_processes";

        private TenancyProcessesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyProcessesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyProcessesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_process"] };
            Table.Columns["registration_date"].DefaultValue = DateTime.Now.Date;
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_process", "tenancy_buildings_assoc", "id_process");
            AddRelation(TableName, "id_process", "tenancy_premises_assoc", "id_process");
            AddRelation(TableName, "id_process", "tenancy_sub_premises_assoc", "id_process");
            AddRelation(TableName, "id_process", "tenancy_reasons", "id_process");
            AddRelation(TableName, "id_process", "tenancy_notifies", "id_process");
            AddRelation(TableName, "id_process", "tenancy_agreements", "id_process");
            AddRelation(TableName, "id_process", "tenancy_persons", "id_process");
            AddRelation("rent_types", "id_rent_type", TableName, "id_rent_type");
            AddRelation("executors", "id_executor", TableName, "id_executor");
            AddRelation("warrants", "id_warrant", TableName, "id_warrant");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_processes SET deleted = 1 WHERE id_process = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_processes
                            (id_rent_type, id_warrant, registration_num
                             , registration_date, issue_date, begin_date, end_date
                             , residence_warrant_num, residence_warrant_date
                             , protocol_num, protocol_date, id_executor, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            var tenancy = (TenancyProcess) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_rent_type", tenancy.IdRentType));
            command.Parameters.Add(DBConnection.CreateParameter("id_warrant", tenancy.IdWarrant));
            command.Parameters.Add(DBConnection.CreateParameter("registration_num", tenancy.RegistrationNum));
            command.Parameters.Add(DBConnection.CreateParameter("registration_date", tenancy.RegistrationDate));
            command.Parameters.Add(DBConnection.CreateParameter("issue_date", tenancy.IssueDate));
            command.Parameters.Add(DBConnection.CreateParameter("begin_date", tenancy.BeginDate));
            command.Parameters.Add(DBConnection.CreateParameter("end_date", tenancy.EndDate));
            command.Parameters.Add(DBConnection.CreateParameter("residence_warrant_num", tenancy.ResidenceWarrantNum));
            command.Parameters.Add(DBConnection.CreateParameter("residence_warrant_date", tenancy.ResidenceWarrantDate));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_num", tenancy.ProtocolNum));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_date", tenancy.ProtocolDate));
            command.Parameters.Add(DBConnection.CreateParameter("id_executor", tenancy.IdExecutor));
            command.Parameters.Add(DBConnection.CreateParameter("description", tenancy.Description));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_processes SET id_rent_type = ?, id_warrant = ?, registration_num = ?, 
                            registration_date = ?, issue_date = ?, begin_date = ?, end_date = ?,
                            residence_warrant_num = ?, residence_warrant_date = ?, protocol_num = ?, 
                            protocol_date = ?, id_executor = ?, description = ? WHERE id_process = ?";
            var tenancy = (TenancyProcess)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_rent_type", tenancy.IdRentType));
            command.Parameters.Add(DBConnection.CreateParameter("id_warrant", tenancy.IdWarrant));
            command.Parameters.Add(DBConnection.CreateParameter("registration_num", tenancy.RegistrationNum));
            command.Parameters.Add(DBConnection.CreateParameter("registration_date", tenancy.RegistrationDate));
            command.Parameters.Add(DBConnection.CreateParameter("issue_date", tenancy.IssueDate));
            command.Parameters.Add(DBConnection.CreateParameter("begin_date", tenancy.BeginDate));
            command.Parameters.Add(DBConnection.CreateParameter("end_date", tenancy.EndDate));
            command.Parameters.Add(DBConnection.CreateParameter("residence_warrant_num", tenancy.ResidenceWarrantNum));
            command.Parameters.Add(DBConnection.CreateParameter("residence_warrant_date", tenancy.ResidenceWarrantDate));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_num", tenancy.ProtocolNum));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_date", tenancy.ProtocolDate));
            command.Parameters.Add(DBConnection.CreateParameter("id_executor", tenancy.IdExecutor));
            command.Parameters.Add(DBConnection.CreateParameter("description", tenancy.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancy.IdProcess));
        }
    }
}
