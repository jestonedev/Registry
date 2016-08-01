using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "ownership_rights", MasterFieldName = "id_ownership_right")]
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [DataTable(Name = "ownership_buildings_assoc", HasDeletedMark = true)]
    public sealed class OwnershipRightBuildingAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true, IncludeIntoInsert = true)]
        public int? IdOwnershipRight { get; set; }
        public int? IdBuilding { get; set; }

        public OwnershipRightBuildingAssoc(int? idBuilding, int? idOwnershipRight)
        {
            IdOwnershipRight = idOwnershipRight;
            IdBuilding = idBuilding;
        }
    }
}
