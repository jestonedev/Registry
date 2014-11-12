using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ClaimStateTypeRelation
    {
        public int? IdRelation { get; set; }
        public int? IdStateFrom { get; set; }
        public int? IdStateTo { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ClaimStateTypeRelation));
        }

        public bool Equals(ClaimStateTypeRelation other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ClaimStateTypeRelation first, ClaimStateTypeRelation second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdRelation == second.IdRelation &&
                first.IdStateFrom == second.IdStateFrom &&
                first.IdStateTo == second.IdStateTo;
        }

        public static bool operator !=(ClaimStateTypeRelation first, ClaimStateTypeRelation second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
