using System;

namespace Registry.Entities
{
    public sealed class TenancyReason : Entity
    {
        public int? IdReason { get; set; }
        public int? IdProcess { get; set; }
        public int? IdReasonType { get; set; }
        public string ReasonNumber { get; set; }
        public DateTime? ReasonDate { get; set; }
        public string ReasonPrepared { get; set; }
    }
}
