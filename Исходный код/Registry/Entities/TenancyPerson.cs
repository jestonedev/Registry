using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyPerson
    {
        public int? id_person { get; set; }
        public int? id_process { get; set; }
        public int? id_kinship { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public DateTime? date_of_birth { get; set; }
        public int? id_document_type { get; set; }
        public DateTime? date_of_document_issue { get; set; }
        public string document_num { get; set; }
        public string document_seria { get; set; }
        public int? id_document_issued_by { get; set; }
        public string registration_id_street { get; set; }
        public string registration_house { get; set; }
        public string registration_flat { get; set; }
        public string registration_room { get; set; }
        public string residence_id_street { get; set; }
        public string residence_house { get; set; }
        public string residence_flat { get; set; }
        public string residence_room { get; set; }
        public string personal_account { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is TenancyPerson))
                return false;
            TenancyPerson obj_person = (TenancyPerson)obj;
            if (this == obj_person)
                return true;
            else
                return false;
        }

        public bool Equals(TenancyPerson other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(TenancyPerson first, TenancyPerson second)
        {
            return first.id_person == second.id_person &&
                first.id_process == second.id_process &&
                first.id_kinship == second.id_kinship &&
                first.surname == second.surname &&
                first.name == second.name &&
                first.patronymic == second.patronymic &&
                first.date_of_birth == second.date_of_birth &&
                first.id_document_type == second.id_document_type &&
                first.date_of_document_issue == second.date_of_document_issue &&
                first.document_num == second.document_num &&
                first.document_seria == second.document_seria &&
                first.id_document_issued_by == second.id_document_issued_by &&
                first.registration_id_street == second.registration_id_street &&
                first.registration_house == second.registration_house &&
                first.registration_flat == second.registration_flat &&
                first.registration_room == second.registration_room &&
                first.residence_id_street == second.residence_id_street &&
                first.residence_house == second.residence_house &&
                first.residence_flat == second.residence_flat &&
                first.residence_room == second.residence_room &&
                first.personal_account == second.personal_account;
        }

        public static bool operator !=(TenancyPerson first, TenancyPerson second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
