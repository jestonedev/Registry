namespace Registry.Entities
{
    public class ResettleObject : Entity
    {
        public int? IdAssoc { get; set; }
        public virtual int? IdObject { get; set; }
        public int? IdProcess { get; set; }
    }
}
