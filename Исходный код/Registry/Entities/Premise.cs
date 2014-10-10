using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Premise
    {
        public int? id_premises { get; set; }
        public int? id_building { get; set; }
        public int? id_state { get; set; }
        public string premises_num { get; set; }
        public double? total_area { get; set; }
        public double? living_area { get; set; }
        public short? num_beds { get; set; }
        public int? id_premises_type { get; set; }
        public int? id_premises_kind { get; set; }
        public short? floor { get; set; }
        public bool? for_orphans { get; set; }
        public bool? accepted_by_exchange { get; set; }
        public bool? accepted_by_donation { get; set; }
        public bool? accepted_by_other { get; set; }
        public string cadastral_num { get; set; }
        public decimal? cadastral_cost { get; set; }
        public decimal? balance_cost { get; set; }
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Premise))
                return false;
            Premise obj_premise = (Premise)obj;
            if (this == obj_premise)
                return true;
            else
                return false;
        }

        public bool Equals(Premise other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Premise first, Premise second)
        {
            return first.id_premises == second.id_premises &&
                first.id_building == second.id_building &&
                first.premises_num == second.premises_num &&
                first.total_area == second.total_area &&
                first.living_area == second.living_area &&
                first.num_beds == second.num_beds &&
                first.id_premises_type == second.id_premises_type &&
                first.id_premises_kind == second.id_premises_kind &&
                first.floor == second.floor &&
                first.for_orphans == second.for_orphans &&
                first.accepted_by_exchange == second.accepted_by_exchange &&
                first.accepted_by_donation == second.accepted_by_donation &&
                first.accepted_by_other == second.accepted_by_other &&
                first.description == second.description &&
                first.cadastral_num == second.cadastral_num &&
                first.cadastral_cost == second.cadastral_cost &&
                first.balance_cost == second.balance_cost &&
                first.id_state == second.id_state;
        }

        public static bool operator !=(Premise first, Premise second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
