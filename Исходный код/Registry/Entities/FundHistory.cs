using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class FundHistory
    {
        public int? id_fund { get; set; }
        public int? id_fund_type { get; set; }
        public string protocol_number { get; set; }
        public DateTime? protocol_date { get; set; }
        public string include_restriction_number { get; set; }
        public DateTime? include_restriction_date { get; set; }
        public string include_restriction_description { get; set; }
        public string exclude_restriction_number { get; set; }
        public DateTime? exclude_restriction_date { get; set; }
        public string exclude_restriction_description { get; set; }
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is FundHistory))
                return false;
            FundHistory obj_fund_history = (FundHistory)obj;
            if (this == obj_fund_history)
                return true;
            else
                return false;
        }

        public bool Equals(FundHistory other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(FundHistory first, FundHistory second)
        {
            return first.id_fund == second.id_fund &&
                first.id_fund_type == second.id_fund_type &&
                first.protocol_number == second.protocol_number &&
                first.protocol_date == second.protocol_date &&
                first.include_restriction_number == second.include_restriction_number &&
                first.include_restriction_date == second.include_restriction_date &&
                first.include_restriction_description == second.include_restriction_description &&
                first.exclude_restriction_number == second.exclude_restriction_number &&
                first.exclude_restriction_date == second.exclude_restriction_date &&
                first.exclude_restriction_description == second.exclude_restriction_description &&
                first.description == second.description;
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
