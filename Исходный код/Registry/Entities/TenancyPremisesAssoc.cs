using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "tenancy_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "premises", MasterFieldName = "id_premises")]
    [DataTable(Name = "tenancy_premises_assoc", HasDeletedMark = true)]
    public sealed class TenancyPremisesAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdAssoc { get; set; }
        public int? IdPremises { get; set; }
        public int? IdProcess { get; set; }
        public double? RentTotalArea { get; set; }
        public double? RentLivingArea { get; set; }
    }
}
