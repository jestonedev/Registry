using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [DataTable(Name = "tenancy_prolong_reason_types", HasDeletedMark = true)]
    public sealed class TenancyProlongReasonType : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdReasonType { get; set; }
        public string ReasonName { get; set; }
        public string ReasonTemplateGenetive { get; set; }
    }
}
