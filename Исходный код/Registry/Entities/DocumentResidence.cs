using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class DocumentResidence
    {
        public int? IdDocumentResidence { get; set; }
        public string DocumentResidenceName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as DocumentResidence));
        }

        public bool Equals(DocumentResidence other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(DocumentResidence first, DocumentResidence second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
                    return first.IdDocumentResidence == second.IdDocumentResidence &&
                        first.DocumentResidenceName == second.DocumentResidenceName;
        }

        public static bool operator !=(DocumentResidence first, DocumentResidence second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
