using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "tenancy_processes")]
    [Relation(SlaveTableName = "tenancy_agreements")]
    [DataTable(Name = "executors", HasDeletedMark = true)]
    public sealed class Executor : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdExecutor { get; set; }
        public string ExecutorName { get; set; }
        public string ExecutorLogin { get; set; }
        public string Phone { get; set; }

        [DataColumn(DefaultValue = false)]
        public bool? IsInactive { get; set; }
    }
}
