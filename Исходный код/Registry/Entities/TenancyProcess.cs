using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "tenancy_buildings_assoc")]
    [Relation(SlaveTableName = "tenancy_premises_assoc")]
    [Relation(SlaveTableName = "tenancy_sub_premises_assoc")]
    [Relation(SlaveTableName = "tenancy_reasons")]
    [Relation(SlaveTableName = "tenancy_notifies")]
    [Relation(SlaveTableName = "tenancy_agreements")]
    [Relation(SlaveTableName = "tenancy_persons")]
    [Relation(SlaveTableName = "tenancy_rent_periods_history")]
    [Relation(MasterTableName = "rent_types", MasterFieldName = "id_rent_type")]
    [Relation(MasterTableName = "executors", MasterFieldName = "id_executor")]
    [Relation(MasterTableName = "warrants", MasterFieldName = "id_warrant")]
    [DataTable(Name = "tenancy_processes", HasDeletedMark = true)]
    public sealed class TenancyProcess : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdProcess { get; set; }
        public int? IdRentType { get; set; }
        public int? IdWarrant { get; set; }
        public int? IdExecutor { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        [DataColumn(DefaultValue = false)]
        public bool? UntilDismissal { get; set; }
        public string ResidenceWarrantNum { get; set; }
        public DateTime? ResidenceWarrantDate { get; set; }
        public string ProtocolNum { get; set; }
        public DateTime? ProtocolDate { get; set; }     
        public string Description { get; set; }
        public DateTime? SubTenancyDate { get; set; }
        public string SubTenancyNum { get; set; }
    }
}
