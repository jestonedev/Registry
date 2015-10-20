using System;

namespace Registry.Entities
{
    public sealed class FundHistory : Entity
    {
        public int? IdFund { get; set; }
        public int? IdFundType { get; set; }
        public string ProtocolNumber { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public string IncludeRestrictionNumber { get; set; }
        public DateTime? IncludeRestrictionDate { get; set; }
        public string IncludeRestrictionDescription { get; set; }
        public string ExcludeRestrictionNumber { get; set; }
        public DateTime? ExcludeRestrictionDate { get; set; }
        public string ExcludeRestrictionDescription { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as FundHistory));
        }

        public bool Equals(FundHistory other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(FundHistory first, FundHistory second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdFund == second.IdFund &&
                   first.IdFundType == second.IdFundType &&
                   first.ProtocolNumber == second.ProtocolNumber &&
                   first.ProtocolDate == second.ProtocolDate &&
                   first.IncludeRestrictionNumber == second.IncludeRestrictionNumber &&
                   first.IncludeRestrictionDate == second.IncludeRestrictionDate &&
                   first.IncludeRestrictionDescription == second.IncludeRestrictionDescription &&
                   first.ExcludeRestrictionNumber == second.ExcludeRestrictionNumber &&
                   first.ExcludeRestrictionDate == second.ExcludeRestrictionDate &&
                   first.ExcludeRestrictionDescription == second.ExcludeRestrictionDescription &&
                   first.Description == second.Description;
        }

        public static bool operator !=(FundHistory first, FundHistory second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
