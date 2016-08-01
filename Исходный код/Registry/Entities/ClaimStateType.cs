using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "claim_states")]
    [Relation(SlaveTableName = "claim_state_types_relations", SlaveFieldName = "id_state_from")]
    [Relation(SlaveTableName = "claim_state_types_relations", SlaveFieldName = "id_state_to")]
    [DataTable(HasDeletedMark = true, Name = "claim_state_types")]
    public sealed class ClaimStateType : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdStateType { get; set; }
        public string StateType { get; set; }
        public bool? IsStartStateType { get; set; }
    }
}
