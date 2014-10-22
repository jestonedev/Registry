using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class DocumentIssuedBy
    {
        public int? id_document_issued_by { get; set; }
        public string document_issued_by { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DocumentIssuedBy))
                return false;
            DocumentIssuedBy obj_document = (DocumentIssuedBy)obj;
            if (this == obj_document)
                return true;
            else
                return false;
        }

        public bool Equals(DocumentIssuedBy other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(DocumentIssuedBy first, DocumentIssuedBy second)
        {
            return first.id_document_issued_by == second.id_document_issued_by &&
                first.document_issued_by == second.document_issued_by;
        }

        public static bool operator !=(DocumentIssuedBy first, DocumentIssuedBy second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
