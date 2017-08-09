using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [DataTable(Name = "district_committees")]
    public sealed class DistrictCommittee : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdCommittee { get; set; }
        public string NameNominative { get; set; }
    }
}
