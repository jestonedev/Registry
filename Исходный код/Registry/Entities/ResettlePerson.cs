namespace Registry.Entities
{
    [Relation(MasterTableName = "resettle_processes", MasterFieldName = "id_process")]
    [DataTable(Name = "resettle_persons", HasDeletedMark = true)]
    public sealed class ResettlePerson : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
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
