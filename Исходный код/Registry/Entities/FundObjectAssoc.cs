namespace Registry.Entities
{
    [DataTable(HasDeletedMark = true)]
    public sealed class FundObjectAssoc : Entity
    {
        public int? IdObject { get; set; }
        public int? IdFund { get; set; }

        public FundObjectAssoc(int? idObject, int? idFund)
        {
            IdFund = idFund;
            IdObject = idObject;
        }
    }
}
