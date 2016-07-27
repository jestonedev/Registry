using System;

namespace Registry.Entities
{
    [Relation(MasterTableName = "ownership_right_types", MasterFieldName = "id_ownership_right_type")]
    [Relation(SlaveTableName = "ownership_premises_assoc")]
    [Relation(SlaveTableName = "ownership_buildings_assoc")]
    [DataTable(Name = "ownership_rights", HasDeletedMark = true)]
    public class OwnershipRight : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdOwnershipRight { get; set; }
        public int? IdOwnershipRightType { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
    }
}
