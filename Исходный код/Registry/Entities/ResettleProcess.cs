using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "documents_residence", MasterFieldName = "id_document_residence")]
    [Relation(SlaveTableName = "resettle_buildings_from_assoc")]
    [Relation(SlaveTableName = "resettle_buildings_to_assoc")]
    [Relation(SlaveTableName = "resettle_premises_from_assoc")]
    [Relation(SlaveTableName = "resettle_premises_to_assoc")]
    [Relation(SlaveTableName = "resettle_sub_premises_from_assoc")]
    [Relation(SlaveTableName = "resettle_sub_premises_to_assoc")]
    [Relation(SlaveTableName = "resettle_persons")]
    [DataTable(Name = "resettle_processes", HasDeletedMark = true)]
    public sealed class ResettleProcess : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdProcess { get; set; }
        public int? IdDocumentResidence { get; set; }
        public DateTime? ResettleDate { get; set; }
        [DataColumn(DefaultValue = 0)]
        public decimal? Debts { get; set; }
        public string Description { get; set; }
        public string DocNumber { get; set; }
    }
}
