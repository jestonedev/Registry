using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Tenancy
    {
        public int? id_contract { get; set; }
        public int? id_rent_type { get; set; }
        public int? id_warrant { get; set; }
        public int? id_executor { get; set; }
        public string registration_num { get; set; }
        public DateTime? registration_date { get; set; }
        public DateTime? issue_date { get; set; }
        public DateTime? begin_date { get; set; }
        public DateTime? end_date { get; set; }
        public string residence_warrant_num { get; set; }
        public DateTime? residence_warrant_date { get; set; }
        public string kumi_order_num { get; set; }
        public DateTime? kumi_order_date { get; set; }     
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Tenancy))
                return false;
            Tenancy obj_premise = (Tenancy)obj;
            if (this == obj_premise)
                return true;
            else
                return false;
        }

        public bool Equals(Tenancy other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Tenancy first, Tenancy second)
        {
            return first.id_contract == second.id_contract &&
                first.id_rent_type == second.id_rent_type &&
                first.id_warrant == second.id_warrant &&
                first.id_executor == second.id_executor &&
                first.registration_num == second.registration_num &&
                first.registration_date == second.registration_date &&
                first.issue_date == second.issue_date &&
                first.begin_date == second.begin_date &&
                first.end_date == second.end_date &&
                first.residence_warrant_num == second.residence_warrant_num &&
                first.residence_warrant_date == second.residence_warrant_date &&
                first.kumi_order_num == second.kumi_order_num &&
                first.kumi_order_date == second.kumi_order_date &&
                first.description == second.description;
        }

        public static bool operator !=(Tenancy first, Tenancy second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
