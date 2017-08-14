using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [DataTable(Name = "tenancy_files", CustomSelectQuery = "SELECT * FROM tenancy_files WHERE id_process = {0}")]
    public class TenancyFile : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int IdFile { get; set; }
        public int IdProcess { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get; set; }
    }
}
