using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Building
    {
        public int? id_building { get; set; }
        public int? id_state { get; set; } 
        public int? id_structure_type {get; set;} 
        public string id_street {get; set;} 
        public string house {get; set;}
        public short? floors {get;set;} 
        public int? num_premises {get;set;} 
        public int? num_rooms {get;set;} 
        public int? num_apartments {get;set;}
        public int? num_shared_apartments {get;set;} 
        public double? living_area {get;set;} 
        public string cadastral_num {get;set;}
        public decimal? cadastral_cost {get;set;} 
        public decimal? balance_cost {get;set;} 
        public string description {get;set;} 
        public int? startup_year {get;set;}
        public bool? improvement {get;set;}
        public bool? elevator { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Building))
                return false;
            Building obj_building = (Building)obj;
            if (this == obj_building)
                return true;
            else
                return false;
        }

        public bool Equals(Building other)
        {
            return this.Equals((object)other);
        }

        public static bool operator==(Building first, Building second)
        {
            return first.id_building == second.id_building &&
                first.id_street == second.id_street &&
                first.id_structure_type == second.id_structure_type &&
                first.house == second.house &&
                first.floors == second.floors &&
                first.num_premises == second.num_premises &&
                first.num_rooms == second.num_rooms &&
                first.num_apartments == second.num_apartments &&
                first.num_shared_apartments == second.num_shared_apartments &&
                first.living_area == second.living_area &&
                first.cadastral_num == second.cadastral_num &&
                first.cadastral_cost == second.cadastral_cost &&
                first.balance_cost == second.balance_cost &&
                first.description == second.description &&
                first.startup_year == second.startup_year &&
                first.improvement == second.improvement &&
                first.elevator == second.elevator &&
                first.id_state == second.id_state;
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
