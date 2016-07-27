namespace Registry.Entities
{
    [Relation(SlaveTableName = "ownership_rights")]
    [DataTable(Name = "ownership_right_types", HasDeletedMark = true)]
    public sealed class OwnershipRightType : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdOwnershipRightType { get; set; }
        public string OwnershipRightTypeName { get; set; }
    }
}
