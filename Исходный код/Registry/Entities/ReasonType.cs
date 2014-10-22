using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ReasonType
    {
        public int? id_reason_type { get; set; }
        public string reason_name { get; set; }
        public string reason_template { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ReasonType))
                return false;
            ReasonType obj_reason_type = (ReasonType)obj;
            if (this == obj_reason_type)
                return true;
            else
                return false;
        }

        public bool Equals(ReasonType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ReasonType first, ReasonType second)
        {
            return first.id_reason_type == second.id_reason_type &&
                first.reason_name == second.reason_name &&
                first.reason_template == second.reason_template;
        }

        public static bool operator !=(ReasonType first, ReasonType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
