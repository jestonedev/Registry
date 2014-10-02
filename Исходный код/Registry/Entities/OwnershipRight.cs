using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public class OwnershipRight
    {
        public int? id_ownership_right { get; set; }
        public int? id_ownership_right_type { get; set; }
        public string number { get; set; }
        public DateTime? date { get; set; }
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is OwnershipRight))
                return false;
            OwnershipRight obj_ownership_right = (OwnershipRight)obj;
            if (this == obj_ownership_right)
                return true;
            else
                return false;
        }

        public bool Equals(OwnershipRight other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(OwnershipRight first, OwnershipRight second)
        {
            return first.id_ownership_right == second.id_ownership_right &&
                first.id_ownership_right_type == second.id_ownership_right_type &&
                first.number == second.number &&
                first.date == second.date &&
                first.description == second.description;
        }

        public static bool operator !=(OwnershipRight first, OwnershipRight second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
