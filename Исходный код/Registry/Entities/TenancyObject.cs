namespace Registry.Entities
{
    public sealed class TenancyObject : Entity
    {
        public int? IdAssoc { get; set; }
        public int? IdObject { get; set; }
        public int? IdProcess { get; set; }
        public double? RentTotalArea { get; set; }
        public double? RentLivingArea { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as TenancyObject));
        }

        public bool Equals(TenancyObject other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(TenancyObject first, TenancyObject second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdObject == second.IdObject &&
                   first.IdProcess == second.IdProcess &&
                   first.RentTotalArea == second.RentTotalArea &&
                   first.RentLivingArea == second.RentLivingArea;
        }

        public static bool operator !=(TenancyObject first, TenancyObject second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
