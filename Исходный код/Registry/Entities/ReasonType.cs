using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ReasonType : Entity
    {
        public int? IdReasonType { get; set; }
        public string ReasonName { get; set; }
        public string ReasonTemplate { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ReasonType));
        }

        public bool Equals(ReasonType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(ReasonType first, ReasonType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdReasonType == second.IdReasonType &&
                   first.ReasonName == second.ReasonName &&
                   first.ReasonTemplate == second.ReasonTemplate;
        }

        public static bool operator !=(ReasonType first, ReasonType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
