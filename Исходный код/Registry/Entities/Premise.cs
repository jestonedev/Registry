using System;

namespace Registry.Entities
{
    public sealed class Premise : Entity
    {
        public int? IdPremises { get; set; }
        public int? IdBuilding { get; set; }
        public int? IdState { get; set; }
        public string PremisesNum { get; set; }
        public double? TotalArea { get; set; }
        public double? LivingArea { get; set; }
        public double? Height { get; set; }
        public short? NumRooms { get; set; }
        public short? NumBeds { get; set; }
        public int? IdPremisesType { get; set; }
        public int? IdPremisesKind { get; set; }
        public short? Floor { get; set; }
        public string CadastralNum { get; set; }
        public decimal? CadastralCost { get; set; }
        public decimal? BalanceCost { get; set; }
        public string Description { get; set; }
        public DateTime? RegDate { get; set; }
        public bool? IsMemorial { get; set; }
        public string Account { get; set; }
        public DateTime? StateDate { get; set; }
    }
}
