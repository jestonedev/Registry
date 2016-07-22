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
            row["id_state"] = ViewportHelper.ValueOrDbNull(claimState.IdState);
            row["id_claim"] = ViewportHelper.ValueOrDbNull(claimState.IdClaim);
            row["id_state_type"] = ViewportHelper.ValueOrDbNull(claimState.IdStateType);
            row["date_start_state"] = ViewportHelper.ValueOrDbNull(claimState.DateStartState);
            row["description"] = ViewportHelper.ValueOrDbNull(claimState.Description);

            row["transfert_to_legal_department_date"] = ViewportHelper.ValueOrDbNull(claimState.TransfertToLegalDepartmentDate);
            row["transfer_to_legal_department_who"] = ViewportHelper.ValueOrDbNull(claimState.TransferToLegalDepartmentWho);
            row["accepted_by_legal_department_date"] = ViewportHelper.ValueOrDbNull(claimState.AcceptedByLegalDepartmentDate);
            row["accepted_by_legal_department_who"] = ViewportHelper.ValueOrDbNull(claimState.AcceptedByLegalDepartmentWho);

            row["claim_direction_date"] = ViewportHelper.ValueOrDbNull(claimState.ClaimDirectionDate);
            row["claim_direction_description"] = ViewportHelper.ValueOrDbNull(claimState.ClaimDirectionDescription);
            row["court_order_date"] = ViewportHelper.ValueOrDbNull(claimState.CourtOrderDate);
            row["court_order_num"] = ViewportHelper.ValueOrDbNull(claimState.CourtOrderNum);
            row["obtaining_court_order_date"] = ViewportHelper.ValueOrDbNull(claimState.ObtainingCourtOrderDate);
            row["obtaining_court_order_description"] = ViewportHelper.ValueOrDbNull(claimState.ObtainingCourtOrderDescription);

            row["direction_court_order_bailiffs_date"] = ViewportHelper.ValueOrDbNull(claimState.DirectionCourtOrderBailiffsDate);
            row["direction_court_order_bailiffs_description"] = ViewportHelper.ValueOrDbNull(claimState.DirectionCourtOrderBailiffsDescription);
            row["enforcement_proceeding_start_date"] = ViewportHelper.ValueOrDbNull(claimState.EnforcementProceedingStartDate);
            row["enforcement_proceeding_start_description"] = ViewportHelper.ValueOrDbNull(claimState.EnforcementProceedingStartDescription);
            row["enforcement_proceeding_end_date"] = ViewportHelper.ValueOrDbNull(claimState.EnforcementProceedingEndDate);
            row["enforcement_proceeding_end_description"] = ViewportHelper.ValueOrDbNull(claimState.EnforcementProceedingEndDescription);
            row["enforcement_proceeding_terminate_date"] = ViewportHelper.ValueOrDbNull(claimState.EnforcementProceedingTerminateDate);
            row["enforcement_proceeding_terminate_description"] = ViewportHelper.ValueOrDbNull(claimState.EnforcementProceedingTerminateDescription);
            row["repeated_direction_court_order_bailiffs_date"] = ViewportHelper.ValueOrDbNull(claimState.RepeatedDirectionCourtOrderBailiffsDate);
            row["repeated_direction_court_order_bailiffs_description"] = ViewportHelper.ValueOrDbNull(claimState.RepeatedDirectionCourtOrderBailiffsDescription);
            row["repeated_enforcement_proceeding_start_date"] = ViewportHelper.ValueOrDbNull(claimState.RepeatedEnforcementProceedingStartDate);
            row["repeated_enforcement_proceeding_start_description"] = ViewportHelper.ValueOrDbNull(claimState.RepeatedEnforcementProceedingStartDescription);
            row["repeated_enforcement_proceeding_end_date"] = ViewportHelper.ValueOrDbNull(claimState.RepeatedEnforcementProceedingEndDate);
            row["repeated_enforcement_proceeding_end_description"] = ViewportHelper.ValueOrDbNull(claimState.RepeatedEnforcementProceedingEndDescription);

            row["court_order_cancel_date"] = ViewportHelper.ValueOrDbNull(claimState.CourtOrderCancelDate);
            row["court_order_cancel_description"] = ViewportHelper.ValueOrDbNull(claimState.CourtOrderCancelDescription);
            row["claim_complete_date"] = ViewportHelper.ValueOrDbNull(claimState.ClaimCompleteDate);
            row["claim_complete_description"] = ViewportHelper.ValueOrDbNull(claimState.ClaimCompleteDescription);
            row["claim_complete_reason"] = ViewportHelper.ValueOrDbNull(claimState.ClaimCompleteReason);
            row.EndEdit();
        }
    }
}
