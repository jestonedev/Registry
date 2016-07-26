using System;

namespace Registry.Entities
{
    public sealed class Claim : Entity
    {
        public int? IdClaim { get; set; }
        public int? IdAccount { get; set; }
        public DateTime? AtDate { get; set; }
        public DateTime? StartDeptPeriod { get; set; }
        public DateTime? EndDeptPeriod { get; set; }
        public string Description { get; set; }
        public decimal? AmountTenancy { get; set; }
        public decimal? AmountDgi { get; set; }
        public decimal? AmountPenalties { get; set; }
    }
}
