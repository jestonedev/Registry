namespace Registry.Entities
{
    public sealed class DocumentResidence : Entity
    {
        public int? IdDocumentResidence { get; set; }
        public string DocumentResidenceName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as DocumentResidence));
        }

        public bool Equals(DocumentResidence other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(DocumentResidence first, DocumentResidence second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
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
