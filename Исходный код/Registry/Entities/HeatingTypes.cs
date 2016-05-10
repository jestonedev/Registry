namespace Registry.Entities
{
    public sealed class HeatingType : Entity
    {
        public int? IdHeatingType { get; set; }
        public string HeatingTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as HeatingType));
        }

        public bool Equals(HeatingType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(HeatingType first, HeatingType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdHeatingType == second.IdHeatingType &&
                   first.HeatingTypeName == second.HeatingTypeName;
        }

        public static bool operator !=(HeatingType first, HeatingType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
