using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class ClaimStateType
    {
        public int? IdStateType { get; set; }
        public string StateType { get; set; }
        public bool? IsStartStateType { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ClaimStateType));
        }

        public bool Equals(ClaimStateType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(ClaimStateType first, ClaimStateType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdStateType == second.IdStateType &&
                first.IsStartStateType == second.IsStartStateType &&
                first.StateType == second.StateType;
        }

        public static bool operator !=(ClaimStateType first, ClaimStateType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
