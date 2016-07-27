using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyPersonConverter
    {
        public static TenancyPerson FromRow(DataRow row)
        {
            return new TenancyPerson
            {
                IdPerson = ViewportHelper.ValueOrNull<int>(row, "id_person"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdKinship = ViewportHelper.ValueOrNull<int>(row, "id_kinship"),
                IdDocumentType = ViewportHelper.ValueOrNull<int>(row, "id_document_type"),
                IdDocumentIssuedBy = ViewportHelper.ValueOrNull<int>(row, "id_document_issued_by"),
                Surname = ViewportHelper.ValueOrNull(row, "surname"),
                Name = ViewportHelper.ValueOrNull(row, "name"),
                Patronymic = ViewportHelper.ValueOrNull(row, "patronymic"),
                DateOfBirth = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_birth"),
                DateOfDocumentIssue = ViewportHelper.ValueOrNull<DateTime>(row, "date_of_document_issue"),
                DocumentNum = ViewportHelper.ValueOrNull(row, "document_num"),
                DocumentSeria = ViewportHelper.ValueOrNull(row, "document_seria"),
                RegistrationIdStreet = ViewportHelper.ValueOrNull(row, "registration_id_street"),
                RegistrationHouse = ViewportHelper.ValueOrNull(row, "registration_house"),
                RegistrationFlat = ViewportHelper.ValueOrNull(row, "registration_flat"),
                RegistrationRoom = ViewportHelper.ValueOrNull(row, "registration_room"),
                RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date"),
                ResidenceIdStreet = ViewportHelper.ValueOrNull(row, "residence_id_street"),
                ResidenceHouse = ViewportHelper.ValueOrNull(row, "residence_house"),
                ResidenceFlat = ViewportHelper.ValueOrNull(row, "residence_flat"),
                ResidenceRoom = ViewportHelper.ValueOrNull(row, "residence_room"),
                PersonalAccount = ViewportHelper.ValueOrNull(row, "personal_account"),
                IncludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "include_date"),
                ExcludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "exclude_date")
            };
        }

        public static TenancyPerson FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static object[] ToArray(TenancyPerson tenancyPerson)
        {
            return new object[]
            {
                tenancyPerson.IdPerson,
                tenancyPerson.IdProcess,
                tenancyPerson.IdKinship,
                tenancyPerson.Surname,
                tenancyPerson.Name,
                tenancyPerson.Patronymic,
                tenancyPerson.DateOfBirth,
                tenancyPerson.IdDocumentType,
                tenancyPerson.DateOfDocumentIssue,
                tenancyPerson.DocumentNum,
                tenancyPerson.DocumentSeria,
                tenancyPerson.IdDocumentIssuedBy,
                tenancyPerson.RegistrationIdStreet,
                tenancyPerson.RegistrationHouse,
                tenancyPerson.RegistrationFlat,
                tenancyPerson.RegistrationRoom,
                tenancyPerson.RegistrationDate,
                tenancyPerson.ResidenceIdStreet,
                tenancyPerson.ResidenceHouse,
                tenancyPerson.ResidenceFlat,
                tenancyPerson.ResidenceRoom,
                tenancyPerson.PersonalAccount,
                tenancyPerson.IncludeDate,
                tenancyPerson.ExcludeDate
            };
        }

        public static void FillRow(TenancyPerson tenancyPerson, DataRowView row)
        {
            row.BeginEdit();
            row["id_person"] = ViewportHelper.ValueOrDbNull(tenancyPerson.IdPerson);
            row["id_process"] = ViewportHelper.ValueOrDbNull(tenancyPerson.IdProcess);
            row["id_kinship"] = ViewportHelper.ValueOrDbNull(tenancyPerson.IdKinship);
            row["surname"] = ViewportHelper.ValueOrDbNull(tenancyPerson.Surname);
            row["name"] = ViewportHelper.ValueOrDbNull(tenancyPerson.Name);
            row["patronymic"] = ViewportHelper.ValueOrDbNull(tenancyPerson.Patronymic);
            row["date_of_birth"] = ViewportHelper.ValueOrDbNull(tenancyPerson.DateOfBirth);
            row["id_document_type"] = ViewportHelper.ValueOrDbNull(tenancyPerson.IdDocumentType);
            row["date_of_document_issue"] = ViewportHelper.ValueOrDbNull(tenancyPerson.DateOfDocumentIssue);
            row["document_num"] = ViewportHelper.ValueOrDbNull(tenancyPerson.DocumentNum);
            row["document_seria"] = ViewportHelper.ValueOrDbNull(tenancyPerson.DocumentSeria);
            row["id_document_issued_by"] = ViewportHelper.ValueOrDbNull(tenancyPerson.IdDocumentIssuedBy);
            row["registration_id_street"] = ViewportHelper.ValueOrDbNull(tenancyPerson.RegistrationIdStreet);
            row["registration_house"] = ViewportHelper.ValueOrDbNull(tenancyPerson.RegistrationHouse);
            row["registration_flat"] = ViewportHelper.ValueOrDbNull(tenancyPerson.RegistrationFlat);
            row["registration_room"] = ViewportHelper.ValueOrDbNull(tenancyPerson.RegistrationRoom);
            row["registration_date"] = ViewportHelper.ValueOrDbNull(tenancyPerson.RegistrationDate);
            row["residence_id_street"] = ViewportHelper.ValueOrDbNull(tenancyPerson.ResidenceIdStreet);
            row["residence_house"] = ViewportHelper.ValueOrDbNull(tenancyPerson.ResidenceHouse);
            row["residence_flat"] = ViewportHelper.ValueOrDbNull(tenancyPerson.ResidenceFlat);
            row["residence_room"] = ViewportHelper.ValueOrDbNull(tenancyPerson.ResidenceRoom);
            row["personal_account"] = ViewportHelper.ValueOrDbNull(tenancyPerson.PersonalAccount);
            row["include_date"] = ViewportHelper.ValueOrDbNull(tenancyPerson.IncludeDate);
            row["exclude_date"] = ViewportHelper.ValueOrDbNull(tenancyPerson.ExcludeDate);
            row.EndEdit();
        }
    }
}
