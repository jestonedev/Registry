using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    public sealed class FundsHistoryDataModel : DataModel
    {
        private const string SelectQuery = "SELECT * FROM funds_history WHERE deleted = 0";
        private const string TableName = "funds_history";

        private FundsHistoryDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_fund"] };
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE funds_history SET deleted = 1 WHERE id_fund = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_fund", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE funds_history SET id_fund_type = ?,
                            protocol_number = ?, protocol_date = ?, include_restriction_number = ?, include_restriction_date = ?,
                            include_restriction_description = ?, exclude_restriction_number = ?, exclude_restriction_date = ?, 
                            exclude_restriction_description = ?, description = ? WHERE id_fund = ?";
            var fundHistory = (FundHistory) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_fund_type", fundHistory.IdFundType));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_number", fundHistory.ProtocolNumber));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_date", fundHistory.ProtocolDate));
            command.Parameters.Add(DBConnection.CreateParameter("include_restriction_number", fundHistory.IncludeRestrictionNumber));
            command.Parameters.Add(DBConnection.CreateParameter("include_restriction_date", fundHistory.IncludeRestrictionDate));
            command.Parameters.Add(DBConnection.CreateParameter("include_restriction_description", fundHistory.IncludeRestrictionDescription));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_restriction_number", fundHistory.ExcludeRestrictionNumber));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_restriction_date", fundHistory.ExcludeRestrictionDate));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_restriction_description", fundHistory.ExcludeRestrictionDescription));
            command.Parameters.Add(DBConnection.CreateParameter("description", fundHistory.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_fund", fundHistory.IdFund));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO funds_history
                            (id_fund_type, protocol_number, protocol_date, 
                            include_restriction_number, include_restriction_date, include_restriction_description,
                            exclude_restriction_number, exclude_restriction_date, exclude_restriction_description, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            var fundHistory = (FundHistory)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_fund_type", fundHistory.IdFundType));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_number", fundHistory.ProtocolNumber));
            command.Parameters.Add(DBConnection.CreateParameter("protocol_date", fundHistory.ProtocolDate));
            command.Parameters.Add(DBConnection.CreateParameter("include_restriction_number", fundHistory.IncludeRestrictionNumber));
            command.Parameters.Add(DBConnection.CreateParameter("include_restriction_date", fundHistory.IncludeRestrictionDate));
            command.Parameters.Add(DBConnection.CreateParameter("include_restriction_description", fundHistory.IncludeRestrictionDescription));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_restriction_number", fundHistory.ExcludeRestrictionNumber));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_restriction_date", fundHistory.ExcludeRestrictionDate));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_restriction_description", fundHistory.ExcludeRestrictionDescription));
            command.Parameters.Add(DBConnection.CreateParameter("description", fundHistory.Description));
        }
    }
}
