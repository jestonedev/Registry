using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "buildings")]
    [DataTable(Name = "structure_types", HasDeletedMark = true)]
    public sealed class StructureType : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdStructureType { get; set; }
        [DataColumn(Name = "structure_type")]
        public string StructureTypeName { get; set; }
    }
}
