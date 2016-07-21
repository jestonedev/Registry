using System;
using System.Data;
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
            row["id_claim"] = ViewportHelper.ValueOrDBNull(claim.IdClaim);
            row["id_account"] = ViewportHelper.ValueOrDBNull(claim.IdAccount);
            row["at_date"] = ViewportHelper.ValueOrDBNull(claim.AtDate);
            row["start_dept_period"] = ViewportHelper.ValueOrDBNull(claim.StartDeptPeriod);
            row["end_dept_period"] = ViewportHelper.ValueOrDBNull(claim.EndDeptPeriod);
            row["amount_tenancy"] = ViewportHelper.ValueOrDBNull(claim.AmountTenancy);
            row["amount_dgi"] = ViewportHelper.ValueOrDBNull(claim.AmountDgi);
            row["amount_penalties"] = ViewportHelper.ValueOrDBNull(claim.AmountPenalties);
            row["description"] = ViewportHelper.ValueOrDBNull(claim.Description);
            row.EndEdit();
        }
    }
}
