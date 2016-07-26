using System;

namespace Registry.Entities
{
    public sealed class TenancyAgreement : Entity
    {
        public int? IdAgreement { get; set; }
        public int? IdProcess { get; set; }
        public DateTime? AgreementDate { get; set; }
        public string AgreementContent { get; set; }
        public int? IdExecutor { get; set; }
        public int? IdWarrant { get; set; }
    }
}
