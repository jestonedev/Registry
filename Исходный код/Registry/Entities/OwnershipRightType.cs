using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class OwnershipRightType
    {
        public int? id_ownership_right_type { get; set; }
        public string ownership_right_type { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is OwnershipRightType))
                return false;
            OwnershipRightType obj_ownership_right_type = (OwnershipRightType)obj;
            if (this == obj_ownership_right_type)
                return true;
            else
                return false;
        }

        public bool Equals(OwnershipRightType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(OwnershipRightType first, OwnershipRightType second)
        {
            return first.id_ownership_right_type == second.id_ownership_right_type &&
                first.ownership_right_type == second.ownership_right_type;
        }

        public static bool operator !=(OwnershipRightType first, OwnershipRightType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
