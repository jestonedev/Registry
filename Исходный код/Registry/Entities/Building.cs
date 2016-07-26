using System;
using System.Linq;
using System.Reflection;

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
        public bool? Plumbing { get; set; }
        public bool? HotWaterSupply { get; set; }
        public bool? Canalization { get; set; }
        public bool? Electricity { get; set; }
        public bool? RadioNetwork { get; set; }
        public int? IdHeatingType { get; set; }
        public string RoomsBTI { get; set; }
        public string HousingCooperative { get; set; }     
    }
}
