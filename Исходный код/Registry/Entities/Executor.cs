namespace Registry.Entities
{
    public sealed class Executor : Entity
    {
        public int? IdExecutor { get; set; }
        public string ExecutorName { get; set; }
        public string ExecutorLogin { get; set; }
        public string Phone { get; set; }
        public bool? IsInactive { get; set; }
    }
}
