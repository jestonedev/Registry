using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ContractReason
    {
        public int? id_contract_reason { get; set; }
        public int? id_contract { get; set; }
        public int? id_reason_type { get; set; }
        public string reason_number { get; set; }
        public DateTime? reason_date { get; set; }
        public string contract_reason_prepared { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ContractReason))
                return false;
            ContractReason obj_contract_reason = (ContractReason)obj;
            if (this == obj_contract_reason)
                return true;
            else
                return false;
        }

        public bool Equals(ContractReason other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ContractReason first, ContractReason second)
        {
            return first.id_contract_reason == second.id_contract_reason &&
                first.id_contract == second.id_contract &&
                first.id_reason_type == second.id_reason_type &&
                first.reason_number == second.reason_number &&
                first.reason_date == second.reason_date &&
                first.contract_reason_prepared == second.contract_reason_prepared;
        }

        public static bool operator !=(ContractReason first, ContractReason second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
