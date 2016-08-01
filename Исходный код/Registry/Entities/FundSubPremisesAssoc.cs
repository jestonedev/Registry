namespace Registry.Entities
{
    [Relation(MasterTableName = "funds_history", MasterFieldName = "id_fund")]
    [Relation(MasterTableName = "sub_premises", MasterFieldName = "id_sub_premises")]
    [DataTable(Name = "funds_sub_premises_assoc", HasDeletedMark = true)]
    public sealed class FundSubPremisesAssoc : Entity
    {
        [DataColumn(IsPrimaryKey = true, IncludeIntoInsert = true)]
        public int? IdFund { get; set; }
        public int? IdSubPremises { get; set; }

        public FundSubPremisesAssoc(int? idIdSubPremises, int? idFund)
        {
            IdFund = idFund;
            IdSubPremises = idIdSubPremises;
        }
    }
}
