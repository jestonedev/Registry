using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "claims", MasterFieldName = "id_claim")]
    [DataTable(HasDeletedMark = true, Name = "claim_court_orders")]
    public sealed class ClaimCourtOrder : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdOrder { get; set; }
        public int? IdClaim { get; set; }
        public int? IdExecutor { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? IdSigner { get; set; }
        public int? IdJudge { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? OpenAccountDate { get; set; }
    }
}
