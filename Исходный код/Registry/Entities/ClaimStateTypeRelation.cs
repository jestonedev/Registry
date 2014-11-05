using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ClaimStateTypeRelation
    {
        public int? id_relation { get; set; }
        public int? id_state_from { get; set; }
        public int? id_state_to { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ClaimStateTypeRelation))
                return false;
            ClaimStateTypeRelation obj_claim_state_type_relation = (ClaimStateTypeRelation)obj;
            if (this == obj_claim_state_type_relation)
                return true;
            else
                return false;
        }

        public bool Equals(ClaimStateTypeRelation other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ClaimStateTypeRelation first, ClaimStateTypeRelation second)
        {
            return first.id_relation == second.id_relation &&
                first.id_state_from == second.id_state_from &&
                first.id_state_to == second.id_state_to;
        }

        public static bool operator !=(ClaimStateTypeRelation first, ClaimStateTypeRelation second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
