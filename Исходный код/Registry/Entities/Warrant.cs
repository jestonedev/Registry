using System;

namespace Registry.Entities
{
    public sealed class Warrant : Entity
    {
        public int? IdWarrant { get; set; }
        public int? IdWarrantDocType { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string OnBehalfOf { get; set; }
        public string Notary { get; set; }
        public string NotaryDistrict { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Warrant));
        }

        public bool Equals(Warrant other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(Warrant first, Warrant second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdWarrant == second.IdWarrant &&
                   first.IdWarrantDocType == second.IdWarrantDocType &&
                   first.Notary == second.Notary &&
                   first.NotaryDistrict == second.NotaryDistrict &&
                   first.OnBehalfOf == second.OnBehalfOf &&
                   first.RegistrationDate == second.RegistrationDate &&
                   first.RegistrationNum == second.RegistrationNum &&
                   first.Description == second.Description;
        }

        public static bool operator !=(Warrant first, Warrant second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
