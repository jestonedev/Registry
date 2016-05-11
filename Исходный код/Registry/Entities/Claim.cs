using System;

namespace Registry.Entities
{
    public sealed class Claim : Entity
    {
        public int? IdClaim { get; set; }
        public int? IdAccount { get; set; }
        public DateTime? AtDate { get; set; }
        public DateTime? StartDeptPeriod { get; set; }
        public DateTime? EndDeptPeriod { get; set; }
        public string Description { get; set; }
        public decimal? AmountTenancy { get; set; }
        public decimal? AmountDgi { get; set; }
        public decimal? AmountPenalties { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Claim));
        }

        public bool Equals(Claim other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(Claim first, Claim second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdClaim == second.IdClaim &&
                   first.IdAccount == second.IdAccount &&
                   first.AtDate == second.AtDate &&
                   first.Description == second.Description &&
                   first.StartDeptPeriod == second.StartDeptPeriod &&
                   first.EndDeptPeriod == second.EndDeptPeriod &&
                   first.AmountTenancy == second.AmountTenancy &&
                   first.AmountDgi == second.AmountDgi &&
                   first.AmountPenalties == second.AmountPenalties;
        }

        public static bool operator !=(Claim first, Claim second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
