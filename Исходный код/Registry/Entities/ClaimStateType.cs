namespace Registry.Entities
{
    public sealed class ClaimStateType : Entity
    {
        public int? IdStateType { get; set; }
        public string StateType { get; set; }
        public bool? IsStartStateType { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ClaimStateType));
        }

        public bool Equals(ClaimStateType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(ClaimStateType first, ClaimStateType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdStateType == second.IdStateType &&
                   first.IsStartStateType == second.IsStartStateType &&
                   first.StateType == second.StateType;
        }

        public static bool operator !=(ClaimStateType first, ClaimStateType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
