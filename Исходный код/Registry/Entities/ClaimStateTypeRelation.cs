namespace Registry.Entities
{
    [Relation(MasterTableName = "claim_state_types", MasterFieldName = "id_state_type", SlaveFieldName = "id_state_from")]
    [Relation(MasterTableName = "claim_state_types", MasterFieldName = "id_state_type", SlaveFieldName = "id_state_to")]
    [DataTable(Name = "claim_state_types_relations", HasDeletedMark = true)]
    public sealed class ClaimStateTypeRelation : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdRelation { get; set; }
        public int? IdStateFrom { get; set; }
        public int? IdStateTo { get; set; }
    }
}
