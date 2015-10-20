namespace Registry.Entities
{
    public sealed class ResettlePerson : Entity
    {
        public int? IdPerson { get; set; }
        public int? IdProcess { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }

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
                   first.Patronymic == second.Patronymic;
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
