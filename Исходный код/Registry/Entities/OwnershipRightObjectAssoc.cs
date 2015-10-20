namespace Registry.Entities
{
    public sealed class OwnershipRightObjectAssoc : Entity
    {
        public int? IdObject { get; set; }
        public int? IdOwnershipRight { get; set; }

        public OwnershipRightObjectAssoc(int? idObject, int? idOwnershipRight)
        {
            IdOwnershipRight = idOwnershipRight;
            IdObject = idObject;
        }
    }
}
