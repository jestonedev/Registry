using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ClaimStatesDataModel: DataModel
    {
        private static ClaimStatesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM claim_states c WHERE deleted = 0";
        private const string TableName = "claim_states";

        private ClaimStatesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;      
        }

        public static ClaimStatesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ClaimStatesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_state"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("claims", "id_claim", TableName, "id_claim");
            AddRelation("claim_state_types", "id_state_type", TableName, "id_state_type");      
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE claim_states SET deleted = 1 WHERE id_state = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE claim_states SET id_claim = ?,
                            id_state_type = ?, date_start_state = ?, description = ?,
                            transfert_to_legal_department_date = ?, transfer_to_legal_department_who = ?,
                            accepted_by_legal_department_date = ?, accepted_by_legal_department_who = ?,
                            claim_direction_date = ?, claim_direction_description = ?,
                            court_order_date = ?, court_order_num = ?,
                            obtaining_court_order_date = ?, obtaining_court_order_description = ?,
                            direction_court_order_bailiffs_date = ?, direction_court_order_bailiffs_description = ?,
                            enforcement_proceeding_start_date = ?, enforcement_proceeding_start_description = ?,
                            enforcement_proceeding_end_date = ?, enforcement_proceeding_end_description = ?,
                            enforcement_proceeding_terminate_date = ?, enforcement_proceeding_terminate_description = ?,
                            repeated_direction_court_order_bailiffs_date = ?, repeated_direction_court_order_bailiffs_description = ?,
                            repeated_enforcement_proceeding_start_date = ?, repeated_enforcement_proceeding_start_description = ?,
                            repeated_enforcement_proceeding_end_date = ? ,repeated_enforcement_proceeding_end_description = ?,
                            court_order_cancel_date = ?, court_order_cancel_description = ?,
                            claim_complete_date = ?, claim_complete_description = ?, claim_complete_reason = ?
                            WHERE id_state = ?";
            var claimState = (ClaimState)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_claim", claimState.IdClaim));
            command.Parameters.Add(DBConnection.CreateParameter("id_state_type", claimState.IdStateType));
            command.Parameters.Add(DBConnection.CreateParameter("date_start_state", claimState.DateStartState));
            command.Parameters.Add(DBConnection.CreateParameter("description", claimState.Description));

            command.Parameters.Add(DBConnection.CreateParameter("transfert_to_legal_department_date", claimState.TransfertToLegalDepartmentDate));
            command.Parameters.Add(DBConnection.CreateParameter("transfer_to_legal_department_who", claimState.TransferToLegalDepartmentWho));
            command.Parameters.Add(DBConnection.CreateParameter("accepted_by_legal_department_date", claimState.AcceptedByLegalDepartmentDate));
            command.Parameters.Add(DBConnection.CreateParameter("accepted_by_legal_department_who", claimState.AcceptedByLegalDepartmentWho));

            command.Parameters.Add(DBConnection.CreateParameter("claim_direction_date", claimState.ClaimDirectionDate));
            command.Parameters.Add(DBConnection.CreateParameter("claim_direction_description", claimState.ClaimDirectionDescription));
            command.Parameters.Add(DBConnection.CreateParameter("court_order_date", claimState.CourtOrderDate));
            command.Parameters.Add(DBConnection.CreateParameter("court_order_num", claimState.CourtOrderNum));
            command.Parameters.Add(DBConnection.CreateParameter("obtaining_court_order_date", claimState.ObtainingCourtOrderDate));
            command.Parameters.Add(DBConnection.CreateParameter("obtaining_court_order_description", claimState.ObtainingCourtOrderDescription));

            command.Parameters.Add(DBConnection.CreateParameter("direction_court_order_bailiffs_date", claimState.DirectionCourtOrderBailiffsDate));
            command.Parameters.Add(DBConnection.CreateParameter("direction_court_order_bailiffs_description", claimState.DirectionCourtOrderBailiffsDescription));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_start_date", claimState.EnforcementProceedingStartDate));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_start_description", claimState.EnforcementProceedingStartDescription));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_end_date", claimState.EnforcementProceedingEndDate));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_end_description", claimState.EnforcementProceedingEndDescription));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_terminate_date", claimState.EnforcementProceedingTerminateDate));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_terminate_description", claimState.EnforcementProceedingTerminateDescription));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_direction_court_order_bailiffs_date", claimState.RepeatedDirectionCourtOrderBailiffsDate));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_direction_court_order_bailiffs_description", claimState.RepeatedDirectionCourtOrderBailiffsDescription));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_start_date", claimState.RepeatedEnforcementProceedingStartDate));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_start_description", claimState.RepeatedEnforcementProceedingStartDescription));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_end_date", claimState.RepeatedEnforcementProceedingEndDate));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_end_description", claimState.RepeatedEnforcementProceedingEndDescription));

            command.Parameters.Add(DBConnection.CreateParameter("court_order_cancel_date", claimState.CourtOrderCancelDate));
            command.Parameters.Add(DBConnection.CreateParameter("court_order_cancel_description", claimState.CourtOrderCancelDescription));
            command.Parameters.Add(DBConnection.CreateParameter("claim_complete_date", claimState.ClaimCompleteDate));
            command.Parameters.Add(DBConnection.CreateParameter("claim_complete_description", claimState.ClaimCompleteDescription));
            command.Parameters.Add(DBConnection.CreateParameter("claim_complete_reason", claimState.ClaimCompleteReason));

            command.Parameters.Add(DBConnection.CreateParameter("id_state", claimState.IdState));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO claim_states
                            (id_claim, id_state_type, date_start_state, description,
                            transfert_to_legal_department_date, transfer_to_legal_department_who,
                            accepted_by_legal_department_date, accepted_by_legal_department_who,
                            claim_direction_date, claim_direction_description,
                            court_order_date, court_order_num,
                            obtaining_court_order_date, obtaining_court_order_description,
                            direction_court_order_bailiffs_date, direction_court_order_bailiffs_description,
                            enforcement_proceeding_start_date, enforcement_proceeding_start_description,
                            enforcement_proceeding_end_date, enforcement_proceeding_end_description,
                            enforcement_proceeding_terminate_date, enforcement_proceeding_terminate_description,
                            repeated_direction_court_order_bailiffs_date, repeated_direction_court_order_bailiffs_description,
                            repeated_enforcement_proceeding_start_date, repeated_enforcement_proceeding_start_description,
                            repeated_enforcement_proceeding_end_date ,repeated_enforcement_proceeding_end_description,
                            court_order_cancel_date, court_order_cancel_description,
                            claim_complete_date, claim_complete_description, claim_complete_reason)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            var claimState = (ClaimState) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_claim", claimState.IdClaim));
            command.Parameters.Add(DBConnection.CreateParameter("id_state_type", claimState.IdStateType));
            command.Parameters.Add(DBConnection.CreateParameter("date_start_state", claimState.DateStartState));
            command.Parameters.Add(DBConnection.CreateParameter("description", claimState.Description));

            command.Parameters.Add(DBConnection.CreateParameter("transfert_to_legal_department_date", claimState.TransfertToLegalDepartmentDate));
            command.Parameters.Add(DBConnection.CreateParameter("transfer_to_legal_department_who", claimState.TransferToLegalDepartmentWho));
            command.Parameters.Add(DBConnection.CreateParameter("accepted_by_legal_department_date", claimState.AcceptedByLegalDepartmentDate));
            command.Parameters.Add(DBConnection.CreateParameter("accepted_by_legal_department_who", claimState.AcceptedByLegalDepartmentWho));

            command.Parameters.Add(DBConnection.CreateParameter("claim_direction_date", claimState.ClaimDirectionDate));
            command.Parameters.Add(DBConnection.CreateParameter("claim_direction_description", claimState.ClaimDirectionDescription));
            command.Parameters.Add(DBConnection.CreateParameter("court_order_date", claimState.CourtOrderDate));
            command.Parameters.Add(DBConnection.CreateParameter("court_order_num", claimState.CourtOrderNum));
            command.Parameters.Add(DBConnection.CreateParameter("obtaining_court_order_date", claimState.ObtainingCourtOrderDate));
            command.Parameters.Add(DBConnection.CreateParameter("obtaining_court_order_description", claimState.ObtainingCourtOrderDescription));

            command.Parameters.Add(DBConnection.CreateParameter("direction_court_order_bailiffs_date", claimState.DirectionCourtOrderBailiffsDate));
            command.Parameters.Add(DBConnection.CreateParameter("direction_court_order_bailiffs_description", claimState.DirectionCourtOrderBailiffsDescription));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_start_date", claimState.EnforcementProceedingStartDate));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_start_description", claimState.EnforcementProceedingStartDescription));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_end_date", claimState.EnforcementProceedingEndDate));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_end_description", claimState.EnforcementProceedingEndDescription));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_terminate_date", claimState.EnforcementProceedingTerminateDate));
            command.Parameters.Add(DBConnection.CreateParameter("enforcement_proceeding_terminate_description", claimState.EnforcementProceedingTerminateDescription));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_direction_court_order_bailiffs_date", claimState.RepeatedDirectionCourtOrderBailiffsDate));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_direction_court_order_bailiffs_description", claimState.RepeatedDirectionCourtOrderBailiffsDescription));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_start_date", claimState.RepeatedEnforcementProceedingStartDate));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_start_description", claimState.RepeatedEnforcementProceedingStartDescription));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_end_date", claimState.RepeatedEnforcementProceedingEndDate));
            command.Parameters.Add(DBConnection.CreateParameter("repeated_enforcement_proceeding_end_description", claimState.RepeatedEnforcementProceedingEndDescription));

            command.Parameters.Add(DBConnection.CreateParameter("court_order_cancel_date", claimState.CourtOrderCancelDate));
            command.Parameters.Add(DBConnection.CreateParameter("court_order_cancel_description", claimState.CourtOrderCancelDescription));
            command.Parameters.Add(DBConnection.CreateParameter("claim_complete_date", claimState.ClaimCompleteDate));
            command.Parameters.Add(DBConnection.CreateParameter("claim_complete_description", claimState.ClaimCompleteDescription));
            command.Parameters.Add(DBConnection.CreateParameter("claim_complete_reason", claimState.ClaimCompleteReason));
        }
    }
}
