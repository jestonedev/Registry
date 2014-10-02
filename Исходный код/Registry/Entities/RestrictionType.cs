using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class RestrictionType
    {
        public int? id_restriction_type { get; set; }
        public string restriction_type { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is RestrictionType))
                return false;
            RestrictionType obj_restriction_type = (RestrictionType)obj;
            if (this == obj_restriction_type)
                return true;
            else
                return false;
        }

        public bool Equals(RestrictionType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(RestrictionType first, RestrictionType second)
        {
            return first.id_restriction_type == second.id_restriction_type &&
                first.restriction_type == second.restriction_type;
        }

        public static bool operator !=(RestrictionType first, RestrictionType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
