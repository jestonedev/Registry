using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "tenancy_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [DataTable(Name = "tenancy_buildings_assoc", HasDeletedMark = true)]
    public sealed class TenancyBuildingAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public int? IdBuilding { get; set; }
        public int? IdProcess { get; set; }
        public double? RentTotalArea { get; set; }
        public double? RentLivingArea { get; set; }
    }
}
