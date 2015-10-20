namespace Registry.Entities
{
    public sealed class AggregatedAddress
    {
        public int IdProcess { get; set; }
        public string Address { get; set; }

        public AggregatedAddress(int idProcess, string address)
        {
            IdProcess = idProcess;
            Address = address;
        }
    }
}
