using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Agreement
    {
        public int? id_agreement { get; set; }
        public int? id_contract { get; set; }
        public DateTime? agreement_date { get; set; }
        public string agreement_content { get; set; }
        public int? id_executor { get; set; }
        public int? id_warrant { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Agreement))
                return false;
            Agreement obj_agreement = (Agreement)obj;
            if (this == obj_agreement)
                return true;
            else
                return false;
        }

        public bool Equals(Agreement other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Agreement first, Agreement second)
        {
            return first.id_agreement == second.id_agreement &&
                first.id_contract == second.id_contract &&
                first.id_executor == second.id_executor &&
                first.id_warrant == second.id_warrant &&
                first.agreement_date == second.agreement_date &&
                first.agreement_content == second.agreement_content;
        }

        public static bool operator !=(Agreement first, Agreement second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
