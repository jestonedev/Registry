using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ClaimStateConverter
    {
        public static ClaimState FromRow(DataRow row)
        {
            return new ClaimState
            {
                IdState = ViewportHelper.ValueOrNull<int>(row, "id_state"),
                IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type"),
                IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim"),
                Description = ViewportHelper.ValueOrNull(row, "description"),
                DateStartState = ViewportHelper.ValueOrNull<DateTime>(row, "date_start_state"),

                TransfertToLegalDepartmentDate = ViewportHelper.ValueOrNull<DateTime>(row, "transfert_to_legal_department_date"),
                TransferToLegalDepartmentWho = ViewportHelper.ValueOrNull(row, "transfer_to_legal_department_who"),
                AcceptedByLegalDepartmentDate = ViewportHelper.ValueOrNull<DateTime>(row, "accepted_by_legal_department_date"),
                AcceptedByLegalDepartmentWho = ViewportHelper.ValueOrNull(row, "accepted_by_legal_department_who"),

                ClaimDirectionDate = ViewportHelper.ValueOrNull<DateTime>(row, "claim_direction_date"),
                ClaimDirectionDescription = ViewportHelper.ValueOrNull(row, "claim_direction_description"),
                CourtOrderDate = ViewportHelper.ValueOrNull<DateTime>(row, "court_order_date"),
                CourtOrderNum = ViewportHelper.ValueOrNull(row, "court_order_num"),
                ObtainingCourtOrderDate = ViewportHelper.ValueOrNull<DateTime>(row, "obtaining_court_order_date"),
                ObtainingCourtOrderDescription = ViewportHelper.ValueOrNull(row, "obtaining_court_order_description"),

                DirectionCourtOrderBailiffsDate = ViewportHelper.ValueOrNull<DateTime>(row, "direction_court_order_bailiffs_date"),
                DirectionCourtOrderBailiffsDescription = ViewportHelper.ValueOrNull(row, "direction_court_order_bailiffs_description"),
                EnforcementProceedingStartDate = ViewportHelper.ValueOrNull<DateTime>(row, "enforcement_proceeding_start_date"),
                EnforcementProceedingStartDescription = ViewportHelper.ValueOrNull(row, "enforcement_proceeding_start_description"),
                EnforcementProceedingEndDate = ViewportHelper.ValueOrNull<DateTime>(row, "enforcement_proceeding_end_date"),
                EnforcementProceedingEndDescription = ViewportHelper.ValueOrNull(row, "enforcement_proceeding_end_description"),
                EnforcementProceedingTerminateDate = ViewportHelper.ValueOrNull<DateTime>(row, "enforcement_proceeding_terminate_date"),
                EnforcementProceedingTerminateDescription = ViewportHelper.ValueOrNull(row, "enforcement_proceeding_terminate_description"),
                RepeatedDirectionCourtOrderBailiffsDate = ViewportHelper.ValueOrNull<DateTime>(row, "repeated_direction_court_order_bailiffs_date"),
                RepeatedDirectionCourtOrderBailiffsDescription = ViewportHelper.ValueOrNull(row, "repeated_direction_court_order_bailiffs_description"),
                RepeatedEnforcementProceedingStartDate = ViewportHelper.ValueOrNull<DateTime>(row, "repeated_enforcement_proceeding_start_date"),
                RepeatedEnforcementProceedingStartDescription = ViewportHelper.ValueOrNull(row, "repeated_enforcement_proceeding_start_description"),
                RepeatedEnforcementProceedingEndDate = ViewportHelper.ValueOrNull<DateTime>(row, "repeated_enforcement_proceeding_end_date"),
                RepeatedEnforcementProceedingEndDescription = ViewportHelper.ValueOrNull(row, "repeated_enforcement_proceeding_end_description"),

                CourtOrderCancelDate = ViewportHelper.ValueOrNull<DateTime>(row, "court_order_cancel_date"),
                CourtOrderCancelDescription = ViewportHelper.ValueOrNull(row, "court_order_cancel_description"),
                ClaimCompleteDate = ViewportHelper.ValueOrNull<DateTime>(row, "claim_complete_date"),
                ClaimCompleteDescription = ViewportHelper.ValueOrNull(row, "claim_complete_description"),
                ClaimCompleteReason = ViewportHelper.ValueOrNull(row, "claim_complete_reason"),
            };
        }

        public static ClaimState FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(ClaimState claimState, DataRowView row)
        {
            row.BeginEdit();
            row["id_state"] = ViewportHelper.ValueOrDBNull(claimState.IdState);
            row["id_claim"] = ViewportHelper.ValueOrDBNull(claimState.IdClaim);
            row["id_state_type"] = ViewportHelper.ValueOrDBNull(claimState.IdStateType);
            row["date_start_state"] = ViewportHelper.ValueOrDBNull(claimState.DateStartState);
            row["description"] = ViewportHelper.ValueOrDBNull(claimState.Description);

            row["transfert_to_legal_department_date"] = ViewportHelper.ValueOrDBNull(claimState.TransfertToLegalDepartmentDate);
            row["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.TransferToLegalDepartmentWho);
            row["accepted_by_legal_department_date"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentDate);
            row["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDBNull(claimState.AcceptedByLegalDepartmentWho);

            row["claim_direction_date"] = ViewportHelper.ValueOrDBNull(claimState.ClaimDirectionDate);
            row["claim_direction_description"] = ViewportHelper.ValueOrDBNull(claimState.ClaimDirectionDescription);
            row["court_order_date"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderDate);
            row["court_order_num"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderNum);
            row["obtaining_court_order_date"] = ViewportHelper.ValueOrDBNull(claimState.ObtainingCourtOrderDate);
            row["obtaining_court_order_description"] = ViewportHelper.ValueOrDBNull(claimState.ObtainingCourtOrderDescription);

            row["direction_court_order_bailiffs_date"] = ViewportHelper.ValueOrDBNull(claimState.DirectionCourtOrderBailiffsDate);
            row["direction_court_order_bailiffs_description"] = ViewportHelper.ValueOrDBNull(claimState.DirectionCourtOrderBailiffsDescription);
            row["enforcement_proceeding_start_date"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingStartDate);
            row["enforcement_proceeding_start_description"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingStartDescription);
            row["enforcement_proceeding_end_date"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingEndDate);
            row["enforcement_proceeding_end_description"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingEndDescription);
            row["enforcement_proceeding_terminate_date"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingTerminateDate);
            row["enforcement_proceeding_terminate_description"] = ViewportHelper.ValueOrDBNull(claimState.EnforcementProceedingTerminateDescription);
            row["repeated_direction_court_order_bailiffs_date"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedDirectionCourtOrderBailiffsDate);
            row["repeated_direction_court_order_bailiffs_description"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedDirectionCourtOrderBailiffsDescription);
            row["repeated_enforcement_proceeding_start_date"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingStartDate);
            row["repeated_enforcement_proceeding_start_description"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingStartDescription);
            row["repeated_enforcement_proceeding_end_date"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingEndDate);
            row["repeated_enforcement_proceeding_end_description"] = ViewportHelper.ValueOrDBNull(claimState.RepeatedEnforcementProceedingEndDescription);

            row["court_order_cancel_date"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderCancelDate);
            row["court_order_cancel_description"] = ViewportHelper.ValueOrDBNull(claimState.CourtOrderCancelDescription);
            row["claim_complete_date"] = ViewportHelper.ValueOrDBNull(claimState.ClaimCompleteDate);
            row["claim_complete_description"] = ViewportHelper.ValueOrDBNull(claimState.ClaimCompleteDescription);
            row["claim_complete_reason"] = ViewportHelper.ValueOrDBNull(claimState.ClaimCompleteReason);
            row.EndEdit();
        }
    }
}
