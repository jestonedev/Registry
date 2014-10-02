using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class StructureType
    {
        public int? id_structure_type { get; set; }
        public string structure_type { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is StructureType))
                return false;
            StructureType obj_structure_type = (StructureType)obj;
            if (this == obj_structure_type)
                return true;
            else
                return false;
        }

        public bool Equals(StructureType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(StructureType first, StructureType second)
        {
            return first.id_structure_type == second.id_structure_type &&
                first.structure_type == second.structure_type;
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
