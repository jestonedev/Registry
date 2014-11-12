using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class StructureType
    {
        public int? IdStructureType { get; set; }
        public string StructureTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as StructureType));
        }

        public bool Equals(StructureType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(StructureType first, StructureType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdStructureType == second.IdStructureType &&
                first.StructureTypeName == second.StructureTypeName;
        }

        public static bool operator !=(StructureType first, StructureType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
