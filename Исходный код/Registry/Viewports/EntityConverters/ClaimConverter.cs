using System;
using System.Data;
using System.Reflection;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ClaimConverter
    {
        public static Claim FromRow(DataRow row)
        {
            return new Claim
            {
                IdClaim = ViewportHelper.ValueOrNull<int>(row, "id_claim"),
                IdAccount = ViewportHelper.ValueOrNull<int>(row, "id_account"),
                AmountTenancy = ViewportHelper.ValueOrNull<decimal>(row, "amount_tenancy"),
                AmountDgi = ViewportHelper.ValueOrNull<decimal>(row, "amount_dgi"),
                AmountPenalties = ViewportHelper.ValueOrNull<decimal>(row, "amount_penalties"),
                AtDate = ViewportHelper.ValueOrNull<DateTime>(row, "at_date"),
                StartDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "start_dept_period"),
                EndDeptPeriod = ViewportHelper.ValueOrNull<DateTime>(row, "end_dept_period"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static Claim FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(Claim claim, DataRowView row)
        {
            row.BeginEdit();
            row["id_claim"] = ViewportHelper.ValueOrDbNull(claim.IdClaim);
            row["id_account"] = ViewportHelper.ValueOrDbNull(claim.IdAccount);
            row["at_date"] = ViewportHelper.ValueOrDbNull(claim.AtDate);
            row["start_dept_period"] = ViewportHelper.ValueOrDbNull(claim.StartDeptPeriod);
            row["end_dept_period"] = ViewportHelper.ValueOrDbNull(claim.EndDeptPeriod);
            row["amount_tenancy"] = ViewportHelper.ValueOrDbNull(claim.AmountTenancy);
            row["amount_dgi"] = ViewportHelper.ValueOrDbNull(claim.AmountDgi);
            row["amount_penalties"] = ViewportHelper.ValueOrDbNull(claim.AmountPenalties);
            row["description"] = ViewportHelper.ValueOrDbNull(claim.Description);
            row.EndEdit();
        }
    }
}
