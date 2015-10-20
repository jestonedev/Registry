using System;

namespace Registry.Entities
{
    public sealed class ResettleProcess : Entity
    {
        public int? IdProcess { get; set; }
        public int? IdDocumentResidence { get; set; }
        public DateTime? ResettleDate { get; set; }
        public decimal? Debts { get; set; }
        public string Description { get; set; }



        public override bool Equals(object obj)
        {
            return (this == (obj as ResettleProcess));
        }

        public bool Equals(ResettleProcess other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(ResettleProcess first, ResettleProcess second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdProcess == second.IdProcess &&
                   first.ResettleDate == second.ResettleDate &&
                   first.IdDocumentResidence == second.IdDocumentResidence &&
                   first.Debts == second.Debts &&
                   first.Description == second.Description;
        }

        public static bool operator !=(ResettleProcess first, ResettleProcess second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
