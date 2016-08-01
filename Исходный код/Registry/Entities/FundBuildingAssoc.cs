using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "funds_history", MasterFieldName = "id_fund")]
    [Relation(MasterTableName = "buildings", MasterFieldName = "id_building")]
    [DataTable(Name = "funds_buildings_assoc", HasDeletedMark = true)]
    public sealed class FundBuildingAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true, IncludeIntoInsert = true)]
        public int? IdFund { get; set; }
        public int? IdBuilding { get; set; }

        public FundBuildingAssoc(int? idBuilding, int? idFund)
        {
            IdFund = idFund;
            IdBuilding = idBuilding;
        }
    }
}
