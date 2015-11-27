using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ClaimsDataModel: DataModel
    {
        private static ClaimsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM claims c WHERE deleted = 0";
        private const string TableName = "claims";

        private ClaimsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;      
        }

        public static ClaimsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ClaimsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_claim"] };
            Table.Columns["amount_of_debt_rent"].DefaultValue = 0;
            Table.Columns["amount_of_debt_fine"].DefaultValue = 0;
            Table.Columns["amount_of_rent"].DefaultValue = 0;
            Table.Columns["amount_of_fine"].DefaultValue = 0;
            Table.Columns["amount_of_rent_recover"].DefaultValue = 0;
            Table.Columns["amount_of_fine_recover"].DefaultValue = 0;
        }

        protected override void ConfigureRelations()
        {
            AddRelation("claims", "id_claim", "claim_states", "id_claim");
            AddRelation("payments_accounts", "id_account", "claims", "id_account"); 
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE claims SET deleted = 1 WHERE id_claim = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_claim", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE claims SET id_account = ?,
                            date_of_transfer = ?, amount_of_debt_rent = ?, amount_of_debt_fine = ?, at_date = ?,
                            amount_of_rent = ?, amount_of_fine = ?, amount_of_rent_recover = ?, 
                            amount_of_fine_recover = ?, start_dept_period = ?, end_dept_period = ?, description = ? WHERE id_claim = ?";
            var claim = (Claim)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_account", claim.IdAccount));
            command.Parameters.Add(DBConnection.CreateParameter("date_of_transfer", claim.DateOfTransfer));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_debt_rent", claim.AmountOfDebtRent));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_debt_fine", claim.AmountOfDebtFine));
            command.Parameters.Add(DBConnection.CreateParameter("at_date", claim.AtDate));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_rent", claim.AmountOfRent));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_fine", claim.AmountOfFine));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_rent_recover", claim.AmountOfRentRecover));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_fine_recover", claim.AmountOfFineRecover));
            command.Parameters.Add(DBConnection.CreateParameter("start_dept_period", claim.StartDeptPeriod));
            command.Parameters.Add(DBConnection.CreateParameter("end_dept_period", claim.EndDeptPeriod));
            command.Parameters.Add(DBConnection.CreateParameter("description", claim.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_claim", claim.IdClaim));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO claims
                            (id_account, date_of_transfer, amount_of_debt_rent, 
                            amount_of_debt_fine, at_date, amount_of_rent,
                            amount_of_fine, amount_of_rent_recover, amount_of_fine_recover, start_dept_period, end_dept_period, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            var claim = (Claim) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_account", claim.IdAccount));
            command.Parameters.Add(DBConnection.CreateParameter("date_of_transfer", claim.DateOfTransfer));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_debt_rent", claim.AmountOfDebtRent));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_debt_fine", claim.AmountOfDebtFine));
            command.Parameters.Add(DBConnection.CreateParameter("at_date", claim.AtDate));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_rent", claim.AmountOfRent));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_fine", claim.AmountOfFine));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_rent_recover", claim.AmountOfRentRecover));
            command.Parameters.Add(DBConnection.CreateParameter("amount_of_fine_recover", claim.AmountOfFineRecover));
            command.Parameters.Add(DBConnection.CreateParameter("start_dept_period", claim.StartDeptPeriod));
            command.Parameters.Add(DBConnection.CreateParameter("end_dept_period", claim.EndDeptPeriod));
            command.Parameters.Add(DBConnection.CreateParameter("description", claim.Description));
        }
    }
}
