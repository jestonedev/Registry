using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "ownership_rights", MasterFieldName = "id_ownership_right")]
    [Relation(MasterTableName = "premises", MasterFieldName = "id_premises")]
    [DataTable(Name = "ownership_premises_assoc", HasDeletedMark = true)]
    public sealed class OwnershipRightPremisesAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true, IncludeIntoInsert = true)]
        public int? IdOwnershipRight { get; set; }
        public int? IdPremises { get; set; }

        public OwnershipRightPremisesAssoc(int? idPremises, int? idOwnershipRight)
        {
            IdOwnershipRight = idOwnershipRight;
            IdPremises = idPremises;
        }
    }
}
