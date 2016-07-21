using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyProcessConverter
    {
        public static TenancyProcess FromRow(DataRow row)
        {
            var tenancy = new TenancyProcess
            {
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdRentType = ViewportHelper.ValueOrNull<int>(row, "id_rent_type"),
                IdWarrant = ViewportHelper.ValueOrNull<int>(row, "id_warrant"),
                IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor"),
                RegistrationNum = ViewportHelper.ValueOrNull(row, "registration_num"),
                RegistrationDate = ViewportHelper.ValueOrNull<DateTime>(row, "registration_date"),
                IssueDate = ViewportHelper.ValueOrNull<DateTime>(row, "issue_date"),
                BeginDate = ViewportHelper.ValueOrNull<DateTime>(row, "begin_date"),
                EndDate = ViewportHelper.ValueOrNull<DateTime>(row, "end_date"),
                UntilDismissal = ViewportHelper.ValueOrNull<bool>(row, "until_dismissal"),
                ResidenceWarrantNum = ViewportHelper.ValueOrNull(row, "residence_warrant_num"),
                ResidenceWarrantDate = ViewportHelper.ValueOrNull<DateTime>(row, "residence_warrant_date"),
                ProtocolNum = ViewportHelper.ValueOrNull(row, "protocol_num"),
                ProtocolDate = ViewportHelper.ValueOrNull<DateTime>(row, "protocol_date"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
            return tenancy;
        }

        public static TenancyProcess FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }
    }
}
