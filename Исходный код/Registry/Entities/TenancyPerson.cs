using System;
using Registry.Entities.Infrastructure;

namespace Registry.Entities
{
    [Relation(MasterTableName = "tenancy_processes", MasterFieldName = "id_process")]
    [Relation(MasterTableName = "kinships", MasterFieldName = "id_kinship")]
    [Relation(MasterTableName = "document_types", MasterFieldName = "id_document_type")]
    [Relation(MasterTableName = "documents_issued_by", MasterFieldName = "id_document_issued_by")]
    [DataTable(Name = "tenancy_persons", HasDeletedMark = true)]
    public sealed class TenancyPerson : Entity
    {
        [DataColumn(IsPrimaryKey = true)]
        public int? IdPerson { get; set; }
        public int? IdProcess { get; set; }
        public int? IdKinship { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime? DateOfBirth { get; set; }
        
        [DataColumn(DefaultValue = 255)]
        public int? IdDocumentType { get; set; }
        public DateTime? DateOfDocumentIssue { get; set; }
        public string DocumentNum { get; set; }
        public string DocumentSeria { get; set; }
        public int? IdDocumentIssuedBy { get; set; }
        public string Snils { get; set; }
        public string RegistrationIdStreet { get; set; }
        public string RegistrationHouse { get; set; }
        public string RegistrationFlat { get; set; }
        public string RegistrationRoom { get; set; }
        public string ResidenceIdStreet { get; set; }
        public string ResidenceHouse { get; set; }
        public string ResidenceFlat { get; set; }
        public string ResidenceRoom { get; set; }
        public string PersonalAccount { get; set; }
        public DateTime? IncludeDate { get; set; }
        public DateTime? ExcludeDate { get; set; }

        [DataColumn(IncludeIntoUpdate = false)]
        public DateTime? RegistrationDate { get; set; }     
    }
}
