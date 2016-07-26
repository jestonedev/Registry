using System;

namespace Registry.Entities
{
    public class Restriction : Entity
    {
        public int? IdRestriction { get; set; }
        public int? IdRestrictionType { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
    }
}
