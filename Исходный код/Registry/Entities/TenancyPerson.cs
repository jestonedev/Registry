﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Entities
{
    public sealed class TenancyPerson
    {
        public int? IdPerson { get; set; }
        public int? IdProcess { get; set; }
        public int? IdKinship { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? IdDocumentType { get; set; }
        public DateTime? DateOfDocumentIssue { get; set; }
        public string DocumentNum { get; set; }
        public string DocumentSeria { get; set; }
        public int? IdDocumentIssuedBy { get; set; }
        public string RegistrationIdStreet { get; set; }
        public string RegistrationHouse { get; set; }
        public string RegistrationFlat { get; set; }
        public string RegistrationRoom { get; set; }
        public string ResidenceIdStreet { get; set; }
        public string ResidenceHouse { get; set; }
        public string ResidenceFlat { get; set; }
        public string ResidenceRoom { get; set; }
        public string PersonalAccount { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as TenancyPerson));
        }

        public bool Equals(TenancyPerson other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(TenancyPerson first, TenancyPerson second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdPerson == second.IdPerson &&
                first.IdProcess == second.IdProcess &&
                first.IdKinship == second.IdKinship &&
                first.Surname == second.Surname &&
                first.Name == second.Name &&
                first.Patronymic == second.Patronymic &&
                first.DateOfBirth == second.DateOfBirth &&
                first.IdDocumentType == second.IdDocumentType &&
                first.DateOfDocumentIssue == second.DateOfDocumentIssue &&
                first.DocumentNum == second.DocumentNum &&
                first.DocumentSeria == second.DocumentSeria &&
                first.IdDocumentIssuedBy == second.IdDocumentIssuedBy &&
                first.RegistrationIdStreet == second.RegistrationIdStreet &&
                first.RegistrationHouse == second.RegistrationHouse &&
                first.RegistrationFlat == second.RegistrationFlat &&
                first.RegistrationRoom == second.RegistrationRoom &&
                first.ResidenceIdStreet == second.ResidenceIdStreet &&
                first.ResidenceHouse == second.ResidenceHouse &&
                first.ResidenceFlat == second.ResidenceFlat &&
                first.ResidenceRoom == second.ResidenceRoom &&
                first.PersonalAccount == second.PersonalAccount;
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
