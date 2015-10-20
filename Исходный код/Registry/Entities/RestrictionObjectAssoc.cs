using System;

namespace Registry.Entities
{
    public sealed class RestrictionObjectAssoc : Entity
    {
        public int? IdObject { get; set; }
        public int? IdRestriction { get; set; }

        public DateTime? Date { get; set; }

        public RestrictionObjectAssoc(int? idObject, int? idRestriction, DateTime? date)
        {
            IdObject = idObject;
            IdRestriction = idRestriction;
            Date = date;
        }
    }
}
