using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Claim
    {
        public int? id_claim { get; set; }
        public int? id_contract { get; set; }
        public DateTime? date_of_transfer { get; set; }
        public decimal? amount_of_debt_rent { get; set; }
        public decimal? amount_of_debt_fine { get; set; }
        public DateTime? at_date { get; set; }
        public decimal? amount_of_rent { get; set; }
        public decimal? amount_of_fine { get; set; }
        public decimal? amount_of_rent_recover { get; set; }
        public decimal? amount_of_fine_recover { get; set; }
        public DateTime? start_dept_period { get; set; }
        public DateTime? end_dept_period { get; set; }
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Claim))
                return false;
            Claim obj_claim = (Claim)obj;
            if (this == obj_claim)
                return true;
            else
                return false;
        }

        public bool Equals(Claim other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Claim first, Claim second)
        {
            return first.id_claim == second.id_claim &&
                first.id_contract == second.id_contract &&
                first.date_of_transfer == second.date_of_transfer &&
                first.amount_of_debt_rent == second.amount_of_debt_rent &&
                first.amount_of_debt_fine == second.amount_of_debt_fine &&
                first.at_date == second.at_date &&
                first.amount_of_fine == second.amount_of_fine &&
                first.amount_of_rent == second.amount_of_rent &&
                first.amount_of_fine_recover == second.amount_of_fine_recover &&
                first.amount_of_rent_recover == second.amount_of_rent_recover &&
                first.description == second.description &&
                first.start_dept_period == second.start_dept_period &&
                first.end_dept_period == second.end_dept_period;
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
