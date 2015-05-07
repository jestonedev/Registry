using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class SubPremise
    {
        public int? IdSubPremises { get; set; }
        public int? IdPremises { get; set; }
        public int? IdState { get; set; }
        public string SubPremisesNum { get; set; }
        public string Description { get; set; }
        public double? TotalArea { get; set; }
        public DateTime? StateDate { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as SubPremise));
        }

        public bool Equals(SubPremise other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(SubPremise first, SubPremise second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdSubPremises == second.IdSubPremises &&
                first.IdPremises == second.IdPremises &&
                first.SubPremisesNum == second.SubPremisesNum &&
                first.TotalArea == second.TotalArea &&
                first.Description == second.Description &&
                first.IdState == second.IdState &&
                first.StateDate == second.StateDate;
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
