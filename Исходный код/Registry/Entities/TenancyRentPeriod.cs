using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyRentPeriod : Entity
    {
        public int? IdRentPeriod { get; set; }
        public int? IdProcess { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? UntilDismissal { get; set; }

        public override bool Equals(object obj)
        {
            return (this == obj as TenancyRentPeriod);
        }

        public bool Equals(TenancyRentPeriod other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(TenancyRentPeriod first, TenancyRentPeriod second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdRentPeriod == second.IdRentPeriod &&
                   first.IdProcess == second.IdProcess &&
                   first.BeginDate == second.BeginDate &&
                   first.EndDate == second.EndDate;
        }

        public static bool operator !=(TenancyRentPeriod first, TenancyRentPeriod second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
