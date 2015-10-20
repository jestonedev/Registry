using System;

namespace Registry.Entities
{
    public sealed class Building: Entity
    {
        public int? IdBuilding { get; set; }
        public int? IdState { get; set; } 
        public int? IdStructureType {get; set;} 
        public string IdStreet {get; set;} 
        public string House {get; set;}
        public short? Floors {get;set;} 
        public int? NumPremises {get;set;} 
        public int? NumRooms {get;set;} 
        public int? NumApartments {get;set;}
        public int? NumSharedApartments {get;set;}
        public double? LivingArea { get; set; }
        public double? TotalArea { get; set; } 
        public string CadastralNum {get;set;}
        public decimal? CadastralCost {get;set;} 
        public decimal? BalanceCost {get;set;} 
        public string Description {get;set;} 
        public int? StartupYear {get;set;}
        public bool? Improvement {get;set;}
        public bool? Elevator { get; set; }
        public bool? RubbishChute { get; set; }
        public double? Wear { get; set; }
        public DateTime? StateDate { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Building));
        }

        public bool Equals(Building other)
        {
            return Equals((object)other);
        }

        public static bool operator==(Building first, Building second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdBuilding == second.IdBuilding &&
                   first.IdStreet == second.IdStreet &&
                   first.IdStructureType == second.IdStructureType &&
                   first.House == second.House &&
                   first.Floors == second.Floors &&
                   first.NumPremises == second.NumPremises &&
                   first.NumRooms == second.NumRooms &&
                   first.NumApartments == second.NumApartments &&
                   first.NumSharedApartments == second.NumSharedApartments &&
                   first.TotalArea == second.TotalArea &&
                   first.LivingArea == second.LivingArea &&
                   first.CadastralNum == second.CadastralNum &&
                   first.CadastralCost == second.CadastralCost &&
                   first.BalanceCost == second.BalanceCost &&
                   first.Description == second.Description &&
                   first.StartupYear == second.StartupYear &&
                   first.Improvement == second.Improvement &&
                   first.Elevator == second.Elevator &&
                   first.RubbishChute == second.RubbishChute &&
                   first.IdState == second.IdState &&
                   first.Wear == second.Wear &&
                   first.StateDate == second.StateDate;
        }

        public static bool operator !=(Building first, Building second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
