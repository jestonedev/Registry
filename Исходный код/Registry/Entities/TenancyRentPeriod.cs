using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "tenancy_processes", MasterFieldName = "id_process")]
    [DataTable(Name = "tenancy_rent_periods_history", HasDeletedMark = true)]
    public sealed class TenancyRentPeriod : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdRentPeriod { get; set; }
        public int? IdProcess { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        [DataColumn(DefaultValue = false)]
        public bool? UntilDismissal { get; set; }
    }
}
