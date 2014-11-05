using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyReason
    {
        public int? id_reason { get; set; }
        public int? id_process { get; set; }
        public int? id_reason_type { get; set; }
        public string reason_number { get; set; }
        public DateTime? reason_date { get; set; }
        public string reason_prepared { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TenancyReason))
                return false;
            TenancyReason obj_reason = (TenancyReason)obj;
            if (this == obj_reason)
                return true;
            else
                return false;
        }

        public bool Equals(TenancyReason other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(TenancyReason first, TenancyReason second)
        {
            return first.id_reason == second.id_reason &&
                first.id_process == second.id_process &&
                first.id_reason_type == second.id_reason_type &&
                first.reason_number == second.reason_number &&
                first.reason_date == second.reason_date &&
                first.reason_prepared == second.reason_prepared;
        }

        public static bool operator !=(TenancyReason first, TenancyReason second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
