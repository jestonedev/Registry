using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class WarrantConverter
    {
        public static Warrant FromRow(DataRowView row)
        {
            return new Warrant
            {
                IdWarrant = ViewportHelper.ValueOrNull<int>(row, "id_warrant"),
                IdWarrantDocType = ViewportHelper.ValueOrNull<int>(row, "id_warrant_doc_type"),
                RegistrationNum = ViewportHelper.ValueOrNull(row, "registration_num"),
                RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date"),
                OnBehalfOf = ViewportHelper.ValueOrNull(row, "on_behalf_of"),
                Notary = ViewportHelper.ValueOrNull(row, "notary"),
                NotaryDistrict = ViewportHelper.ValueOrNull(row, "notary_district"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static void FillRow(Warrant warrant, DataRowView row)
        {
            row.BeginEdit();
            row["id_warrant"] = ViewportHelper.ValueOrDbNull(warrant.IdWarrant);
            row["id_warrant_doc_type"] = ViewportHelper.ValueOrDbNull(warrant.IdWarrantDocType);
            row["registration_num"] = ViewportHelper.ValueOrDbNull(warrant.RegistrationNum);
            row["registration_date"] = ViewportHelper.ValueOrDbNull(warrant.RegistrationDate);
            row["on_behalf_of"] = ViewportHelper.ValueOrDbNull(warrant.OnBehalfOf);
            row["notary"] = ViewportHelper.ValueOrDbNull(warrant.Notary);
            row["notary_district"] = ViewportHelper.ValueOrDbNull(warrant.NotaryDistrict);
            row["description"] = ViewportHelper.ValueOrDbNull(warrant.Description);
            row.EndEdit();
        }
    }
}
