using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ClaimStatesDataModel: DataModel
    {
        private const string SelectQuery = "SELECT * FROM claim_states c WHERE deleted = 0";
        private const string TableName = "claim_states";

        public ClaimStatesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_state"] };
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE claim_states SET deleted = 1 WHERE id_state = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE claim_states SET id_claim = ?,
                            id_state_type = ?, date_start_state = ?, date_end_state = ?, document_num = ?,
                            document_date = ?, description = ? WHERE id_state = ?";
            var claimState = (ClaimState)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_claim", claimState.IdClaim));
            command.Parameters.Add(DBConnection.CreateParameter("id_state_type", claimState.IdStateType));
            command.Parameters.Add(DBConnection.CreateParameter("date_start_state", claimState.DateStartState));
            command.Parameters.Add(DBConnection.CreateParameter("date_end_state", claimState.DateEndState));
            command.Parameters.Add(DBConnection.CreateParameter("document_num", claimState.DocumentNum));
            command.Parameters.Add(DBConnection.CreateParameter("document_date", claimState.DocumentDate));
            command.Parameters.Add(DBConnection.CreateParameter("description", claimState.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_state", claimState.IdState));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO claim_states
                            (id_claim, id_state_type, date_start_state, date_end_state, document_num,
                            document_date, description) VALUES (?, ?, ?, ?, ?, ?, ?)";
            var claimState = (ClaimState) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_claim", claimState.IdClaim));
            command.Parameters.Add(DBConnection.CreateParameter("id_state_type", claimState.IdStateType));
            command.Parameters.Add(DBConnection.CreateParameter("date_start_state", claimState.DateStartState));
            command.Parameters.Add(DBConnection.CreateParameter("date_end_state", claimState.DateEndState));
            command.Parameters.Add(DBConnection.CreateParameter("document_num", claimState.DocumentNum));
            command.Parameters.Add(DBConnection.CreateParameter("document_date", claimState.DocumentDate));
            command.Parameters.Add(DBConnection.CreateParameter("description", claimState.Description));
        }
    }
}
