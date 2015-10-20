namespace Registry.Entities
{
    public sealed class OwnershipRightType : Entity
    {
        public int? IdOwnershipRightType { get; set; }
        public string OwnershipRightTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as OwnershipRightType));
        }

        public bool Equals(OwnershipRightType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(OwnershipRightType first, OwnershipRightType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdOwnershipRightType == second.IdOwnershipRightType &&
                   first.OwnershipRightTypeName == second.OwnershipRightTypeName;
        }

        public static bool operator !=(OwnershipRightType first, OwnershipRightType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
