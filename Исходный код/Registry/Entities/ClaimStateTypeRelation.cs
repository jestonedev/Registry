namespace Registry.Entities
{
    public sealed class ClaimStateTypeRelation : Entity
    {
        public int? IdRelation { get; set; }
        public int? IdStateFrom { get; set; }
        public int? IdStateTo { get; set; }
    }
}
