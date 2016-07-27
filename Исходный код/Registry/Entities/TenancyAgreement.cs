using System;

namespace Registry.Entities
{
    [Relation(MasterTableName = "tenancy_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "executors", MasterFieldName = "id_executor")]
    [Relation(MasterTableName = "warrants", MasterFieldName = "id_warrant")]
    [DataTable(Name = "tenancy_agreements", HasDeletedMark = true)]
    public sealed class TenancyAgreement : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAgreement { get; set; }
        public int? IdProcess { get; set; }
        public DateTime? AgreementDate { get; set; }
        public string AgreementContent { get; set; }
        public int? IdExecutor { get; set; }
        public int? IdWarrant { get; set; }
    }
}
