using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyPersonConverter
    {
        internal static TenancyPerson FromRow(DataRow row)
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
                ResidenceIdStreet = ViewportHelper.ValueOrNull(row, "residence_id_street"),
                ResidenceHouse = ViewportHelper.ValueOrNull(row, "residence_house"),
                ResidenceFlat = ViewportHelper.ValueOrNull(row, "residence_flat"),
                ResidenceRoom = ViewportHelper.ValueOrNull(row, "residence_room"),
                PersonalAccount = ViewportHelper.ValueOrNull(row, "personal_account"),
                IncludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "include_date"),
                ExcludeDate = ViewportHelper.ValueOrNull<DateTime>(row, "exclude_date")
            };
        }
    }
}
