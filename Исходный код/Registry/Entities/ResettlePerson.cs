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
    }
}
