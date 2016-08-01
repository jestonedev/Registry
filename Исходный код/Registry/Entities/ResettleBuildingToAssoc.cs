using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "resettle_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [DataTable(Name = "resettle_buildings_to_assoc", HasDeletedMark = true)]
    public sealed class ResettleBuildingToAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public int? IdBuilding { get; set; }
        public int? IdProcess { get; set; }
    }
}
