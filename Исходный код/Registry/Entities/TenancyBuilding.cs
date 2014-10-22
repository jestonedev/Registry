using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyObject
    {
        public int? id_assoc { get; set; }
        public int? id_object { get; set; }
        public int? id_contract { get; set; }
        public string beds { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TenancyObject))
                return false;
            TenancyObject obj_tenancy = (TenancyObject)obj;
            if (this == obj_tenancy)
                return true;
            else
                return false;
        }

        public bool Equals(TenancyObject other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(TenancyObject first, TenancyObject second)
        {
            return first.id_object == second.id_object &&
                first.id_contract == second.id_contract &&
                first.beds == second.beds;
        }

        public static bool operator !=(TenancyObject first, TenancyObject second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
