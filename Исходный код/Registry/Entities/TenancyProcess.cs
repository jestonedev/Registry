using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyProcess : Entity
    {
        public int? IdProcess { get; set; }
        public int? IdRentType { get; set; }
        public int? IdWarrant { get; set; }
        public int? IdExecutor { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? UntilDismissal { get; set; }
        public string ResidenceWarrantNum { get; set; }
        public DateTime? ResidenceWarrantDate { get; set; }
        public string ProtocolNum { get; set; }
        public DateTime? ProtocolDate { get; set; }     
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == obj as TenancyProcess);
        }

        public bool Equals(TenancyProcess other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(TenancyProcess first, TenancyProcess second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdProcess == second.IdProcess &&
                   first.IdRentType == second.IdRentType &&
                   first.IdWarrant == second.IdWarrant &&
                   first.IdExecutor == second.IdExecutor &&
                   first.RegistrationNum == second.RegistrationNum &&
                   first.RegistrationDate == second.RegistrationDate &&
                   first.IssueDate == second.IssueDate &&
                   first.BeginDate == second.BeginDate &&
                   first.EndDate == second.EndDate &&
                   first.UntilDismissal == second.UntilDismissal &&
                   first.ResidenceWarrantNum == second.ResidenceWarrantNum &&
                   first.ResidenceWarrantDate == second.ResidenceWarrantDate &&
                   first.ProtocolNum == second.ProtocolNum &&
                   first.ProtocolDate == second.ProtocolDate &&
                   first.Description == second.Description;
        }

        public static bool operator !=(TenancyProcess first, TenancyProcess second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
