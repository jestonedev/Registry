using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(SlaveTableName = "judges_buildings_assoc")]
    [DataTable(HasDeletedMark = true, Name = "judges")]
    public sealed class Judge : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdJudge { get; set; }
        public int? NumDistrict { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string AddrDistrict { get; set; }
        public string PhoneDistrict { get; set; }
        public string IsActive { get; set; }
    }
}
