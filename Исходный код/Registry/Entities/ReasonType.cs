using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "tenancy_reasons")]
    [DataTable(Name = "tenancy_reason_types", HasDeletedMark = true)]
    public sealed class ReasonType : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdReasonType { get; set; }
        public string ReasonName { get; set; }
        public string ReasonTemplate { get; set; }
        public int Order { get; set; }
    }
}
