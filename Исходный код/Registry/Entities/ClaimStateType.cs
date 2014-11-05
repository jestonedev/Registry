using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ClaimStateType
    {
        public int? id_state_type { get; set; }
        public string state_type { get; set; }
        public bool? is_start_state_type { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ClaimStateType))
                return false;
            ClaimStateType obj_claim_state_type = (ClaimStateType)obj;
            if (this == obj_claim_state_type)
                return true;
            else
                return false;
        }

        public bool Equals(ClaimStateType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ClaimStateType first, ClaimStateType second)
        {
            return first.id_state_type == second.id_state_type &&
                first.is_start_state_type == second.is_start_state_type &&
                first.state_type == second.state_type;
        }

        public static bool operator !=(ClaimStateType first, ClaimStateType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
