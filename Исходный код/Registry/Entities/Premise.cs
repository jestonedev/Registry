using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Premise
    {
        public int? IdPremises { get; set; }
        public int? IdBuilding { get; set; }
        public int? IdState { get; set; }
        public string PremisesNum { get; set; }
        public double? TotalArea { get; set; }
        public double? LivingArea { get; set; }
        public double? Height { get; set; }
        public short? NumRooms { get; set; }
        public short? NumBeds { get; set; }
        public int? IdPremisesType { get; set; }
        public int? IdPremisesKind { get; set; }
        public short? Floor { get; set; }
        public string CadastralNum { get; set; }
        public decimal? CadastralCost { get; set; }
        public decimal? BalanceCost { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Premise));
        }

        public bool Equals(Premise other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Premise first, Premise second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdPremises == second.IdPremises &&
                first.IdBuilding == second.IdBuilding &&
                first.PremisesNum == second.PremisesNum &&
                first.TotalArea == second.TotalArea &&
                first.LivingArea == second.LivingArea &&
                first.Height == second.Height &&
                first.NumRooms == second.NumRooms &&
                first.NumBeds == second.NumBeds &&
                first.IdPremisesType == second.IdPremisesType &&
                first.IdPremisesKind == second.IdPremisesKind &&
                first.Floor == second.Floor &&
                first.Description == second.Description &&
                first.CadastralNum == second.CadastralNum &&
                first.CadastralCost == second.CadastralCost &&
                first.BalanceCost == second.BalanceCost &&
                first.IdState == second.IdState;
        }

        public static bool operator !=(Premise first, Premise second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
