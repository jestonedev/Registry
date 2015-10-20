namespace Registry.Entities
{
    public sealed class StructureType : Entity
    {
        public int? IdStructureType { get; set; }
        public string StructureTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as StructureType));
        }

        public bool Equals(StructureType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(StructureType first, StructureType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdStructureType == second.IdStructureType &&
                   first.StructureTypeName == second.StructureTypeName;
        }

        public static bool operator !=(StructureType first, StructureType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
