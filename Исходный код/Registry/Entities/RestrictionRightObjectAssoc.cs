namespace Registry.Entities
{
    public sealed class RestrictionRightObjectAssoc : Entity
    {
        public int? IdObject { get; set; }
        public int? IdRestriction { get; set; }

        public RestrictionRightObjectAssoc(int? idObject, int? idRestriction)
        {
            IdRestriction = idRestriction;
            IdObject = idObject;
        }
    }
}
