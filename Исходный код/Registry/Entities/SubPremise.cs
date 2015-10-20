using System;

namespace Registry.Entities
{
    public sealed class SubPremise : Entity
    {
        public int? IdSubPremises { get; set; }
        public int? IdPremises { get; set; }
        public int? IdState { get; set; }
        public string SubPremisesNum { get; set; }
        public string Description { get; set; }
        public double? TotalArea { get; set; }
        public double? LivingArea { get; set; }
        public DateTime? StateDate { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as SubPremise));
        }

        public bool Equals(SubPremise other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SubPremise first, SubPremise second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdSubPremises == second.IdSubPremises &&
                   first.IdPremises == second.IdPremises &&
                   first.SubPremisesNum == second.SubPremisesNum &&
                   first.TotalArea == second.TotalArea &&
                   first.LivingArea == second.LivingArea &&
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
