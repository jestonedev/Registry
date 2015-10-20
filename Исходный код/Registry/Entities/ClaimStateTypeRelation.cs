namespace Registry.Entities
{
    public sealed class ClaimStateTypeRelation : Entity
    {
        public int? IdRelation { get; set; }
        public int? IdStateFrom { get; set; }
        public int? IdStateTo { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ClaimStateTypeRelation));
        }

        public bool Equals(ClaimStateTypeRelation other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(ClaimStateTypeRelation first, ClaimStateTypeRelation second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdRelation == second.IdRelation &&
                   first.IdStateFrom == second.IdStateFrom &&
                   first.IdStateTo == second.IdStateTo;
        }

        public static bool operator !=(ClaimStateTypeRelation first, ClaimStateTypeRelation second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
