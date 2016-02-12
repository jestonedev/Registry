namespace Registry.Entities
{
    public sealed class ResettlePerson : Entity
    {
        public int? IdPerson { get; set; }
        public int? IdProcess { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string DocumentNum { get; set; }
        public string DocumentSeria { get; set; }
        public string FoundingDoc { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as ResettlePerson));
        }

        public bool Equals(ResettlePerson other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(ResettlePerson first, ResettlePerson second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdPerson == second.IdPerson &&
                   first.IdProcess == second.IdProcess &&
                   first.Surname == second.Surname &&
                   first.Name == second.Name &&
                   first.Patronymic == second.Patronymic &&
                   first.DocumentSeria == second.DocumentSeria &&
                   first.DocumentNum == second.DocumentNum &&
                   first.FoundingDoc == second.FoundingDoc;
        }

        public static bool operator !=(ResettlePerson first, ResettlePerson second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
