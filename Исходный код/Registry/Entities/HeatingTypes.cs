using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "buildings")]
    [DataTable(Name = "heating_type", HasDeletedMark = true)]
    public sealed class HeatingType : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdHeatingType { get; set; }
        public string HeatingTypeName { get; set; }
    }
}
