using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [DataTable(Name = "district_committees_pre_conctract_preambles")]
    public sealed class DistrictCommitteePreContractPreamble : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdPreamble { get; set; }
        public string Name { get; set; }
    }
}
