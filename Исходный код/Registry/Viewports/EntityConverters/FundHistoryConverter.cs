using System;
using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class FundHistoryConverter
    {
        public static FundHistory FromRow(DataRow row)
        {
            return new FundHistory
            {
                IdFund = ViewportHelper.ValueOrNull<int>(row, "id_fund"),
                IdFundType = ViewportHelper.ValueOrNull<int>(row, "id_fund_type"),
                ProtocolNumber = ViewportHelper.ValueOrNull(row, "protocol_number"),
                ProtocolDate = ViewportHelper.ValueOrNull<DateTime>(row, "protocol_date"),
                IncludeRestrictionNumber = ViewportHelper.ValueOrNull(row, "include_restriction_number"),
                IncludeRestrictionDate = ViewportHelper.ValueOrNull<DateTime>(row, "include_restriction_date"),
                IncludeRestrictionDescription = ViewportHelper.ValueOrNull(row, "include_restriction_description"),
                ExcludeRestrictionNumber = ViewportHelper.ValueOrNull(row, "exclude_restriction_number"),
                ExcludeRestrictionDate = ViewportHelper.ValueOrNull<DateTime>(row, "exclude_restriction_date"),
                ExcludeRestrictionDescription = ViewportHelper.ValueOrNull(row, "exclude_restriction_description"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static FundHistory FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(FundHistory fundHistory, DataRowView row)
        {
            row.BeginEdit();
            row["id_fund"] = ViewportHelper.ValueOrDbNull(fundHistory.IdFund);
            row["id_fund_type"] = ViewportHelper.ValueOrDbNull(fundHistory.IdFundType);
            row["protocol_number"] = ViewportHelper.ValueOrDbNull(fundHistory.ProtocolNumber);
            row["protocol_date"] = ViewportHelper.ValueOrDbNull(fundHistory.ProtocolDate);
            row["include_restriction_number"] = ViewportHelper.ValueOrDbNull(fundHistory.IncludeRestrictionNumber);
            row["include_restriction_date"] = ViewportHelper.ValueOrDbNull(fundHistory.IncludeRestrictionDate);
            row["include_restriction_description"] = ViewportHelper.ValueOrDbNull(fundHistory.IncludeRestrictionDescription);
            row["exclude_restriction_number"] = ViewportHelper.ValueOrDbNull(fundHistory.ExcludeRestrictionNumber);
            row["exclude_restriction_date"] = ViewportHelper.ValueOrDbNull(fundHistory.ExcludeRestrictionDate);
            row["exclude_restriction_description"] = ViewportHelper.ValueOrDbNull(fundHistory.ExcludeRestrictionDescription);
            row["description"] = ViewportHelper.ValueOrDbNull(fundHistory.Description);
            row.EndEdit();
        }
    }
}
