using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyObject
    {
        public int? IdAssoc { get; set; }
        public int? IdObject { get; set; }
        public int? IdProcess { get; set; }
        public double? RentTotalArea { get; set; }
        public double? RentLivingArea { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as TenancyObject));
        }

        public bool Equals(TenancyObject other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(TenancyObject first, TenancyObject second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdObject == second.IdObject &&
                first.IdProcess == second.IdProcess &&
                first.RentTotalArea == second.RentTotalArea &&
                first.RentLivingArea == second.RentLivingArea;
        }

        public static bool operator !=(TenancyObject first, TenancyObject second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
