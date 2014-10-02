using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class SubPremise
    {
        public int? id_sub_premises { get; set; }
        public int? id_premises { get; set; }
        public string sub_premises_num { get; set; }
        public double? total_area { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is SubPremise))
                return false;
            SubPremise obj_sub_premise = (SubPremise)obj;
            if (this == obj_sub_premise)
                return true;
            else
                return false;
        }

        public bool Equals(SubPremise other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(SubPremise first, SubPremise second)
        {
            return first.id_sub_premises == second.id_sub_premises &&
                first.id_premises == second.id_premises &&
                first.sub_premises_num == second.sub_premises_num &&
                first.total_area == second.total_area;
        }

        public static bool operator !=(SubPremise first, SubPremise second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
