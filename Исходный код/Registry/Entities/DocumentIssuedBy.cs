using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "tenancy_persons")]
    [DataTable(Name = "documents_issued_by", HasDeletedMark = true)]
    public sealed class DocumentIssuedBy : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdDocumentIssuedBy { get; set; }

        [DataColumn(Name = "document_issued_by")]
        public string DocumentIssuedByName { get; set; }
    }
}
