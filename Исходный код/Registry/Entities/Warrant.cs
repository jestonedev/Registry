using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "tenancy_agreements")]
    [Relation(SlaveTableName = "tenancy_processes")]
    [Relation(MasterTableName = "warrant_doc_types", MasterFieldName = "id_warrant_doc_type")]
    [DataTable(Name = "warrants", HasDeletedMark = true)]
    public sealed class Warrant : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdWarrant { get; set; }
        public int? IdWarrantDocType { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string OnBehalfOf { get; set; }
        public string Notary { get; set; }
        public string NotaryDistrict { get; set; }
        public string Description { get; set; }
    }
}
