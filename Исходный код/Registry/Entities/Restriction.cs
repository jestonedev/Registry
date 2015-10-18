using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public class Restriction : Entity
    {
        public int? IdRestriction { get; set; }
        public int? IdRestrictionType { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Restriction));
        }

        public bool Equals(Restriction other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(Restriction first, Restriction second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdRestriction == second.IdRestriction &&
                   first.IdRestrictionType == second.IdRestrictionType &&
                   first.Number == second.Number &&
                   first.Date == second.Date &&
                   first.Description == second.Description;
        }

        public static bool operator !=(Restriction first, Restriction second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
