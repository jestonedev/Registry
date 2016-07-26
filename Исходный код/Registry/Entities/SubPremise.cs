using System;

namespace Registry.Entities
{
    public sealed class SubPremise : Entity
    {
        public int? IdSubPremises { get; set; }
        public int? IdPremises { get; set; }
        public int? IdState { get; set; }
        public string SubPremisesNum { get; set; }
        public string Description { get; set; }
        public double? TotalArea { get; set; }
        public double? LivingArea { get; set; }
        public DateTime? StateDate { get; set; }
        public string CadastralNum { get; set; }
        public decimal? CadastralCost { get; set; }
        public decimal? BalanceCost { get; set; }
        public string Account { get; set; }
    }
}
