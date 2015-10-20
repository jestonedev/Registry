using System;
using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyReasonsDataModel : DataModel
    {
        private static TenancyReasonsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_reasons WHERE deleted = 0";
        private const string TableName = "tenancy_reasons";

        private TenancyReasonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyReasonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyReasonsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_reason"] };
            Table.Columns["reason_date"].DefaultValue = DateTime.Now.Date;
        }

        protected override void ConfigureRelations()
        {
            AddRelation("tenancy_processes", "id_process", TableName, "id_process");
            AddRelation("tenancy_reason_types", "id_reason_type", TableName, "id_reason_type");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_reasons SET deleted = 1 WHERE id_reason = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_reason", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_reasons SET id_process = ?, id_reason_type = ?, reason_number = ?, 
                            reason_date = ?, reason_prepared = ? WHERE id_reason = ?";
            var tenancyReason = (TenancyReason) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyReason.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("id_reason_type", tenancyReason.IdReasonType));
            command.Parameters.Add(DBConnection.CreateParameter("reason_number", tenancyReason.ReasonNumber));
            command.Parameters.Add(DBConnection.CreateParameter("reason_date", tenancyReason.ReasonDate));
            command.Parameters.Add(DBConnection.CreateParameter("reason_prepared", tenancyReason.ReasonPrepared));
            command.Parameters.Add(DBConnection.CreateParameter("id_reason", tenancyReason.IdReason));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_reasons
                            (id_process, id_reason_type, reason_number, reason_date, reason_prepared)
                            VALUES (?, ?, ?, ?, ?)";
            var tenancyReason = (TenancyReason)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyReason.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("id_reason_type", tenancyReason.IdReasonType));
            command.Parameters.Add(DBConnection.CreateParameter("reason_number", tenancyReason.ReasonNumber));
            command.Parameters.Add(DBConnection.CreateParameter("reason_date", tenancyReason.ReasonDate));
            command.Parameters.Add(DBConnection.CreateParameter("reason_prepared", tenancyReason.ReasonPrepared));
        }
    }
}
