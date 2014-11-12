using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Claim
    {
        public int? IdClaim { get; set; }
        public int? IdProcess { get; set; }
        public DateTime? DateOfTransfer { get; set; }
        public decimal? AmountOfDebtRent { get; set; }
        public decimal? AmountOfDebtFine { get; set; }
        public DateTime? AtDate { get; set; }
        public decimal? AmountOfRent { get; set; }
        public decimal? AmountOfFine { get; set; }
        public decimal? AmountOfRentRecover { get; set; }
        public decimal? AmountOfFineRecover { get; set; }
        public DateTime? StartDeptPeriod { get; set; }
        public DateTime? EndDeptPeriod { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Claim));
        }

        public bool Equals(Claim other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Claim first, Claim second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdClaim == second.IdClaim &&
                first.IdProcess == second.IdProcess &&
                first.DateOfTransfer == second.DateOfTransfer &&
                first.AmountOfDebtRent == second.AmountOfDebtRent &&
                first.AmountOfDebtFine == second.AmountOfDebtFine &&
                first.AtDate == second.AtDate &&
                first.AmountOfFine == second.AmountOfFine &&
                first.AmountOfRent == second.AmountOfRent &&
                first.AmountOfFineRecover == second.AmountOfFineRecover &&
                first.AmountOfRentRecover == second.AmountOfRentRecover &&
                first.Description == second.Description &&
                first.StartDeptPeriod == second.StartDeptPeriod &&
                first.EndDeptPeriod == second.EndDeptPeriod;
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
