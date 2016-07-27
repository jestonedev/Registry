namespace Registry.Entities
{
    [Relation(SlaveTableName = "resettle_processes")]
    [DataTable(Name = "documents_residence", HasDeletedMark = true)]
    public sealed class DocumentResidence : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdDocumentResidence { get; set; }
        public string DocumentResidenceName { get; set; }
    }
}
