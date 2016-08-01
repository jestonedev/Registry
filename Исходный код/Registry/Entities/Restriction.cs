using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "restriction_types", MasterFieldName = "id_restriction_type")]
    [Relation(SlaveTableName = "restrictions_buildings_assoc")]
    [Relation(SlaveTableName = "restrictions_premises_assoc")]
    [DataTable(Name = "restrictions", HasDeletedMark = true)]
    public class Restriction : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdRestriction { get; set; }
        public int? IdRestrictionType { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
    }
}
