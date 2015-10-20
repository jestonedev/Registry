namespace Registry.Entities
{
    public sealed class DocumentIssuedBy : Entity
    {
        public int? IdDocumentIssuedBy { get; set; }
        public string DocumentIssuedByName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as DocumentIssuedBy));
        }

        public bool Equals(DocumentIssuedBy other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(DocumentIssuedBy first, DocumentIssuedBy second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdDocumentIssuedBy == second.IdDocumentIssuedBy &&
                   first.DocumentIssuedByName == second.DocumentIssuedByName;
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
