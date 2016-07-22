using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyAgreementConverter
    {
        public static TenancyAgreement FromRow(DataRow row)
        {
            return new TenancyAgreement
            {
                IdAgreement = ViewportHelper.ValueOrNull<int>(row, "id_agreement"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor"),
                IdWarrant = ViewportHelper.ValueOrNull<int>(row, "id_warrant"),
                AgreementDate = ViewportHelper.ValueOrNull<DateTime>(row, "agreement_date"),
                AgreementContent = ViewportHelper.ValueOrNull(row, "agreement_content")
            };
        }

        public static TenancyAgreement FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(TenancyAgreement tenancyAgreement, DataRowView row)
        {
            row.BeginEdit();
            row["id_agreement"] = ViewportHelper.ValueOrDbNull(tenancyAgreement.IdAgreement);
            row["id_process"] = ViewportHelper.ValueOrDbNull(tenancyAgreement.IdProcess);
            row["agreement_date"] = ViewportHelper.ValueOrDbNull(tenancyAgreement.AgreementDate);
            row["agreement_content"] = ViewportHelper.ValueOrDbNull(tenancyAgreement.AgreementContent);
            row["id_executor"] = ViewportHelper.ValueOrDbNull(tenancyAgreement.IdExecutor);
            row["id_warrant"] = ViewportHelper.ValueOrDbNull(tenancyAgreement.IdWarrant);
            row.EndEdit();
        }
    }
}
