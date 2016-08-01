using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "resettle_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "sub_premises", MasterFieldName = "id_sub_premises")]
    [DataTable(Name = "resettle_sub_premises_from_assoc", HasDeletedMark = true)]
    public sealed class ResettleSubPremisesFromAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public int? IdSubPremises { get; set; }
        public int? IdProcess { get; set; }
    }
}
