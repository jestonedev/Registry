using System;

namespace Registry.Entities
{
    public sealed class ResettleProcess : Entity
    {
        public int? IdProcess { get; set; }
        public int? IdDocumentResidence { get; set; }
        public DateTime? ResettleDate { get; set; }
        public decimal? Debts { get; set; }
        public string Description { get; set; }
        public string DocNumber { get; set; }
    }
}
