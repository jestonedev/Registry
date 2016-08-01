using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [DataTable(Name = "claim_files", CustomSelectQuery = "SELECT * FROM claim_files WHERE id_claim = {0}")]
    public class ClaimFile: Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int IdFile { get; set; }
        public int IdClaim { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get; set; }
    }
}
