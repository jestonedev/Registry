namespace Registry.Entities
{
    [Relation(SlaveTableName = "tenancy_persons")]
    [DataTable(Name = "documents_issued_by", HasDeletedMark = true)]
    public sealed class DocumentIssuedBy : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdDocumentIssuedBy { get; set; }
        public string DocumentIssuedByName { get; set; }
    }
}
