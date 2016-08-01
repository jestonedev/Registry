namespace Registry.Entities
{
    [Relation(MasterTableName = "funds_history", MasterFieldName = "id_fund")]
    [Relation(MasterTableName = "premises", MasterFieldName = "id_premises")]
    [DataTable(Name = "funds_premises_assoc", HasDeletedMark = true)]
    public sealed class FundPremisesAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true, IncludeIntoInsert = true)]
        public int? IdFund { get; set; }
        public int? IdPremises { get; set; }

        public FundPremisesAssoc(int? idPremises, int? idFund)
        {
            IdFund = idFund;
            IdPremises = idPremises;
        }
    }
}
