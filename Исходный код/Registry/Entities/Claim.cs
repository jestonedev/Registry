﻿using System;

namespace Registry.Entities
{
    public sealed class Claim : Entity
    {
        public int? IdClaim { get; set; }
        public int? IdAccount { get; set; }
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
