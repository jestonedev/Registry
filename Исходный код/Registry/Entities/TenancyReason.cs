using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "tenancy_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "tenancy_reason_types", MasterFieldName = "id_reason_type")]
    [DataTable(Name = "tenancy_reasons", HasDeletedMark = true)]
    public sealed class TenancyReason : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdReason { get; set; }
        public int? IdProcess { get; set; }
        public int? IdReasonType { get; set; }
        public string ReasonNumber { get; set; }
        public DateTime? ReasonDate { get; set; }
        public string ReasonPrepared { get; set; }
    }
}
