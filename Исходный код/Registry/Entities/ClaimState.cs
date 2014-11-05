using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ClaimState
    {
        public int? id_state { get; set; }
        public int? id_claim { get; set; }
        public int? id_state_type { get; set; }
        public DateTime? date_start_state { get; set; }
        public DateTime? date_end_state { get; set; }
        public string document_num { get; set; }
        public DateTime? document_date { get; set; }
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ClaimState))
                return false;
            ClaimState obj_claim_state = (ClaimState)obj;
            if (this == obj_claim_state)
                return true;
            else
                return false;
        }

        public bool Equals(ClaimState other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ClaimState first, ClaimState second)
        {
            return first.id_state == second.id_state &&
                first.id_claim == second.id_claim &&
                first.id_state_type == second.id_state_type &&
                first.date_start_state == second.date_start_state &&
                first.date_end_state == second.date_end_state &&
                first.document_num == second.document_num &&
                first.document_date == second.document_date &&
                first.description == second.description;
        }

        public static bool operator !=(ClaimState first, ClaimState second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
