namespace Registry.Entities
{
    public sealed class TenancyObject : Entity
    {
        public int? IdAssoc { get; set; }
        public int? IdObject { get; set; }
        public int? IdProcess { get; set; }
        public double? RentTotalArea { get; set; }
        public double? RentLivingArea { get; set; }
    }
}
