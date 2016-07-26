namespace Registry.Entities
{
    public sealed class ReasonType : Entity
    {
        public int? IdReasonType { get; set; }
        public string ReasonName { get; set; }
        public string ReasonTemplate { get; set; }
    }
}
