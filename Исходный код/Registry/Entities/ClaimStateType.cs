namespace Registry.Entities
{
    public sealed class ClaimStateType : Entity
    {
        public int? IdStateType { get; set; }
        public string StateType { get; set; }
        public bool? IsStartStateType { get; set; }
    }
}
