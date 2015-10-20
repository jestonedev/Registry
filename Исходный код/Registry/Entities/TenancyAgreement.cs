using System;

namespace Registry.Entities
{
    public sealed class TenancyAgreement : Entity
    {
        public int? IdAgreement { get; set; }
        public int? IdProcess { get; set; }
        public DateTime? AgreementDate { get; set; }
        public string AgreementContent { get; set; }
        public int? IdExecutor { get; set; }
        public int? IdWarrant { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as TenancyAgreement));
        }

        public bool Equals(TenancyAgreement other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(TenancyAgreement first, TenancyAgreement second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdAgreement == second.IdAgreement &&
                   first.IdProcess == second.IdProcess &&
                   first.IdExecutor == second.IdExecutor &&
                   first.IdWarrant == second.IdWarrant &&
                   first.AgreementDate == second.AgreementDate &&
                   first.AgreementContent == second.AgreementContent;
        }

        public static bool operator !=(TenancyAgreement first, TenancyAgreement second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
