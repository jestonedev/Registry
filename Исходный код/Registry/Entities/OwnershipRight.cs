using System;

namespace Registry.Entities
{
    public class OwnershipRight : Entity
    {
        public int? IdOwnershipRight { get; set; }
        public int? IdOwnershipRightType { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
    }
}
