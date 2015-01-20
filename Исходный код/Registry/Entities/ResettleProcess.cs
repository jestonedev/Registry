using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ResettleProcess
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
            return this.Equals((object)other);
        }

        public static bool operator ==(ResettleProcess first, ResettleProcess second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
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
