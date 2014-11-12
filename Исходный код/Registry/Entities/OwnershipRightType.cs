using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class OwnershipRightType
    {
        public int? IdOwnershipRightType { get; set; }
        public string OwnershipRightTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as OwnershipRightType));
        }

        public bool Equals(OwnershipRightType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(OwnershipRightType first, OwnershipRightType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdOwnershipRightType == second.IdOwnershipRightType &&
                first.OwnershipRightTypeName == second.OwnershipRightTypeName;
        }

        public static bool operator !=(OwnershipRightType first, OwnershipRightType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
