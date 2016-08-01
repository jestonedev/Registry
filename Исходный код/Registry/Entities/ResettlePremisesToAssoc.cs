using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "resettle_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "premises", MasterFieldName = "id_premises")]
    [DataTable(Name = "resettle_premises_to_assoc", HasDeletedMark = true)]
    public sealed class ResettlePremisesToAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public int? IdPremises { get; set; }
        public int? IdProcess { get; set; }
    }
}
