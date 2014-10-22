using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class Warrant
    {
        public int? id_warrant { get; set; }
        public int? id_warrant_doc_type { get; set; }
        public string registration_num { get; set; }
        public DateTime? registration_date { get; set; }
        public string on_behalf_of { get; set; }
        public string notary { get; set; }
        public string notary_district { get; set; }
        public string description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Warrant))
                return false;
            Warrant obj_warrant = (Warrant)obj;
            if (this == obj_warrant)
                return true;
            else
                return false;
        }

        public bool Equals(Warrant other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(Warrant first, Warrant second)
        {
            return first.id_warrant == second.id_warrant &&
                first.id_warrant_doc_type == second.id_warrant_doc_type &&
                first.notary == second.notary &&
                first.notary_district == second.notary_district &&
                first.on_behalf_of == second.on_behalf_of &&
                first.registration_date == second.registration_date &&
                first.registration_num == second.registration_num &&
                first.description == second.description;
        }

        public static bool operator !=(Warrant first, Warrant second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
