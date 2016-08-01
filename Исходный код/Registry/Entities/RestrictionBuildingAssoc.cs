using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "restrictions", MasterFieldName = "id_restriction")]
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [DataTable(Name = "restrictions_buildings_assoc", HasDeletedMark = true)]
    public sealed class RestrictionBuildingAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true, IncludeIntoInsert = true)]
        public int? IdRestriction { get; set; }
        public int? IdBuilding { get; set; }

        [DataColumn(IncludeIntoInsert = false, IncludeIntoUpdate = false)]
        public DateTime? Date { get; set; }

        public RestrictionBuildingAssoc(int? idBuilding, int? idRestriction, DateTime? date)
        {
            IdBuilding = idBuilding;
            IdRestriction = idRestriction;
            Date = date;
        }
    }
}
