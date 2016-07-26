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
    }
}
