using System;

namespace Registry.Entities
{
    [Relation(MasterTableName = "fund_types", MasterFieldName = "id_fund_type")]
    [Relation(SlaveTableName = "funds_buildings_assoc")]
    [Relation(SlaveTableName = "funds_premises_assoc")]
    [Relation(SlaveTableName = "funds_sub_premises_assoc")]
    [DataTable(Name = "funds_history", HasDeletedMark = true)]
    public sealed class FundHistory : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdFund { get; set; }
        public int? IdFundType { get; set; }
        public string ProtocolNumber { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public string IncludeRestrictionNumber { get; set; }
        public DateTime? IncludeRestrictionDate { get; set; }
        public string IncludeRestrictionDescription { get; set; }
        public string ExcludeRestrictionNumber { get; set; }
        public DateTime? ExcludeRestrictionDate { get; set; }
        public string ExcludeRestrictionDescription { get; set; }
        public string Description { get; set; }
    }
}
