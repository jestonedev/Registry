using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class RestrictionType : Entity
    {
        public int? IdRestrictionType { get; set; }
        public string RestrictionTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as RestrictionType));
        }

        public bool Equals(RestrictionType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(RestrictionType first, RestrictionType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdRestrictionType == second.IdRestrictionType &&
                   first.RestrictionTypeName == second.RestrictionTypeName;
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
