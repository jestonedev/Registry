using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class RestrictionObjectAssoc
    {
        public int? IdObject { get; set; }
        public int? IdRestriction { get; set; }
        public DateTime? date { get; set; }

        public RestrictionObjectAssoc(int? idObject, int? idRestriction, DateTime? date)
        {
            this.IdObject = idObject;
            this.IdRestriction = idRestriction;
            this.date = date;
        }
    }
}
