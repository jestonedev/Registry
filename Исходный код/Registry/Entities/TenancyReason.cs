using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyReason
    {
        public int? IdReason { get; set; }
        public int? IdProcess { get; set; }
        public int? IdReasonType { get; set; }
        public string ReasonNumber { get; set; }
        public DateTime? ReasonDate { get; set; }
        public string ReasonPrepared { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as TenancyReason));
        }

        public bool Equals(TenancyReason other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(TenancyReason first, TenancyReason second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdReason == second.IdReason &&
                first.IdProcess == second.IdProcess &&
                first.IdReasonType == second.IdReasonType &&
                first.ReasonNumber == second.ReasonNumber &&
                first.ReasonDate == second.ReasonDate &&
                first.ReasonPrepared == second.ReasonPrepared;
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
