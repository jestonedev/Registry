using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "claims", MasterFieldName = "id_claim")]
    [DataTable(HasDeletedMark = true, Name = "claim_persons")]
    public sealed class ClaimPerson : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdPerson { get; set; }
        public int? IdClaim { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string WorkPlace { get; set; }
        public bool IsClaimer { get; set; }
    }
}
