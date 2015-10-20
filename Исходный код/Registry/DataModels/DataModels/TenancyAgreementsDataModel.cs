using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyAgreementsDataModel : DataModel
    {
        private static TenancyAgreementsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_agreements WHERE deleted = 0";
        private const string TableName = "tenancy_agreements";

        private TenancyAgreementsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyAgreementsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyAgreementsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_agreement"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("tenancy_processes", "id_process", TableName, "id_process");
            AddRelation("executors", "id_executor", TableName, "id_executor");
            AddRelation("warrants", "id_warrant", TableName, "id_warrant");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_agreements SET deleted = 1 WHERE id_agreement = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_agreement", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_agreements (id_process, agreement_date, agreement_content, id_executor, id_warrant)
                            VALUES (?, ?, ?, ?, ?)";
            var tenancyAgreement = (TenancyAgreement) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyAgreement.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("agreement_date", tenancyAgreement.AgreementDate));
            command.Parameters.Add(DBConnection.CreateParameter("agreement_content", tenancyAgreement.AgreementContent));
            command.Parameters.Add(DBConnection.CreateParameter("id_executor", tenancyAgreement.IdExecutor));
            command.Parameters.Add(DBConnection.CreateParameter("id_warrant", tenancyAgreement.IdWarrant));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_agreements SET id_process = ?, agreement_date = ?, agreement_content = ?, 
                            id_executor = ?, id_warrant = ? WHERE id_agreement = ?";
            var tenancyAgreement = (TenancyAgreement)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyAgreement.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("agreement_date", tenancyAgreement.AgreementDate));
            command.Parameters.Add(DBConnection.CreateParameter("agreement_content", tenancyAgreement.AgreementContent));
            command.Parameters.Add(DBConnection.CreateParameter("id_executor", tenancyAgreement.IdExecutor));
            command.Parameters.Add(DBConnection.CreateParameter("id_warrant", tenancyAgreement.IdWarrant));
            command.Parameters.Add(DBConnection.CreateParameter("id_agreement", tenancyAgreement.IdAgreement));
        }
    }
}
