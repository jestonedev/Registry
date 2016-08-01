using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "restrictions")]
    [DataTable(Name = "restriction_types", HasDeletedMark = true)]
    public sealed class RestrictionType : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdRestrictionType { get; set; }

        [DataColumn(Name = "restriction_type")]
        public string RestrictionTypeName { get; set; }
    }
}
