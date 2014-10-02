using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public class Restriction
    {
        public int? id_restriction { get; set; }
        public int? id_restriction_type { get; set; }
        public string number { get; set; }
        public DateTime? date { get; set; }
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Restriction))
                return false;
            Restriction obj_restriction = (Restriction)obj;
            if (this == obj_restriction)
                return true;
            else
                return false;
        }

        public bool Equals(Restriction other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Restriction first, Restriction second)
        {
            return first.id_restriction == second.id_restriction &&
                first.id_restriction_type == second.id_restriction_type &&
                first.number == second.number &&
                first.date == second.date &&
                first.description == second.description;
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
