namespace Registry.DataModels.Services.ServiceModels
{
    public class ClaimPaymentAccountInfo
    {
        public int IdClaim { get; set; }

        public int IdAccount { get; set; }

        public string RawAddress { get; set; }

        public string ParsedAddress { get; set; }

        public string Account { get; set; }

        public string Tenant { get; set; }

        public string StateType { get; set; }
    }
}
