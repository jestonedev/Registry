using System;

namespace Registry.Entities
{
    public sealed class ClaimState : Entity
    {
        public int? IdState { get; set; }
        public int? IdClaim { get; set; }
        public int? IdStateType { get; set; }
        public DateTime? DateStartState { get; set; }
        public string Description { get; set; }
        public DateTime? TransfertToLegalDepartmentDate { get; set; }
        public string TransferToLegalDepartmentWho { get; set; }
        public DateTime? AcceptedByLegalDepartmentDate { get; set; }
        public string AcceptedByLegalDepartmentWho { get; set; }
        public DateTime? ClaimDirectionDate { get; set; }
        public string ClaimDirectionDescription { get; set; }
        public DateTime? CourtOrderDate { get; set; }
        public string CourtOrderNum { get; set; }
        public DateTime? ObtainingCourtOrderDate { get; set; }
        public string ObtainingCourtOrderDescription { get; set; }
        public DateTime? DirectionCourtOrderBailiffsDate { get; set; }
        public string DirectionCourtOrderBailiffsDescription { get; set; }
        public DateTime? EnforcementProceedingStartDate { get; set; }
        public string EnforcementProceedingStartDescription { get; set; }
        public DateTime? EnforcementProceedingEndDate { get; set; }
        public string EnforcementProceedingEndDescription { get; set; }
        public DateTime? EnforcementProceedingTerminateDate { get; set; }
        public string EnforcementProceedingTerminateDescription { get; set; }
        public DateTime? RepeatedDirectionCourtOrderBailiffsDate { get; set; }
        public string RepeatedDirectionCourtOrderBailiffsDescription { get; set; }
        public DateTime? RepeatedEnforcementProceedingStartDate { get; set; }
        public string RepeatedEnforcementProceedingStartDescription { get; set; }
        public DateTime? RepeatedEnforcementProceedingEndDate { get; set; }
        public string RepeatedEnforcementProceedingEndDescription { get; set; }
        public DateTime? CourtOrderCancelDate { get; set; }
        public string CourtOrderCancelDescription { get; set; }
        public DateTime? ClaimCompleteDate { get; set; }
        public string ClaimCompleteDescription { get; set; }
        public string ClaimCompleteReason { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ClaimState));
        }

        public bool Equals(ClaimState other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(ClaimState first, ClaimState second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdState == second.IdState &&
                   first.IdClaim == second.IdClaim &&
                   first.IdStateType == second.IdStateType &&
                   first.DateStartState == second.DateStartState &&
                   first.Description == second.Description &&

                   first.TransfertToLegalDepartmentDate == second.TransfertToLegalDepartmentDate &&
                   first.TransferToLegalDepartmentWho == second.TransferToLegalDepartmentWho &&
                   first.AcceptedByLegalDepartmentDate == second.AcceptedByLegalDepartmentDate &&
                   first.AcceptedByLegalDepartmentWho == second.AcceptedByLegalDepartmentWho &&

                   first.ClaimDirectionDate == second.ClaimDirectionDate &&
                   first.ClaimDirectionDescription == second.ClaimDirectionDescription &&
                   first.CourtOrderDate == second.CourtOrderDate &&
                   first.CourtOrderNum == second.CourtOrderNum &&
                   first.ObtainingCourtOrderDate == second.ObtainingCourtOrderDate &&
                   first.ObtainingCourtOrderDescription == second.ObtainingCourtOrderDescription &&

                   first.DirectionCourtOrderBailiffsDate == second.DirectionCourtOrderBailiffsDate &&
                   first.DirectionCourtOrderBailiffsDescription == second.DirectionCourtOrderBailiffsDescription &&
                   first.EnforcementProceedingStartDate == second.EnforcementProceedingStartDate &&
                   first.EnforcementProceedingStartDescription == second.EnforcementProceedingStartDescription &&
                   first.EnforcementProceedingEndDate == second.EnforcementProceedingEndDate &&
                   first.EnforcementProceedingEndDescription == second.EnforcementProceedingEndDescription &&
                   first.EnforcementProceedingTerminateDate == second.EnforcementProceedingTerminateDate &&
                   first.EnforcementProceedingTerminateDescription == second.EnforcementProceedingTerminateDescription &&
                   first.RepeatedDirectionCourtOrderBailiffsDate == second.RepeatedDirectionCourtOrderBailiffsDate &&
                   first.RepeatedDirectionCourtOrderBailiffsDescription ==
                   second.RepeatedDirectionCourtOrderBailiffsDescription &&
                   first.RepeatedEnforcementProceedingStartDate == second.RepeatedEnforcementProceedingStartDate &&
                   first.RepeatedEnforcementProceedingStartDescription ==
                   second.RepeatedEnforcementProceedingStartDescription &&
                   first.RepeatedEnforcementProceedingEndDate == second.RepeatedEnforcementProceedingEndDate &&
                   first.RepeatedEnforcementProceedingEndDescription ==
                   second.RepeatedEnforcementProceedingEndDescription &&

                   first.CourtOrderCancelDate == second.CourtOrderCancelDate &&
                   first.CourtOrderCancelDescription == second.CourtOrderCancelDescription &&
                   first.ClaimCompleteDate == second.ClaimCompleteDate &&
                   first.ClaimCompleteDescription == second.ClaimCompleteDescription &&
                   first.ClaimCompleteReason == second.ClaimCompleteReason;
        }

        public static bool operator !=(ClaimState first, ClaimState second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
