using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "tenancy_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "sub_premises", MasterFieldName = "id_sub_premises")]
    [DataTable(Name = "tenancy_sub_premises_assoc", HasDeletedMark = true)]
    public sealed class TenancySubPremisesAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public int? IdSubPremises { get; set; }
        public int? IdProcess { get; set; }
        public double? RentTotalArea { get; set; }
    }
}
