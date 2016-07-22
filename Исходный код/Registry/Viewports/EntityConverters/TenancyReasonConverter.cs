using System;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyReasonConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_reason"], 
                row["id_process"], 
                row["id_reason_type"], 
                row["reason_number"], 
                row["reason_date"], 
                row["reason_prepared"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static TenancyReason FromRow(DataRow row)
        {
            return new TenancyReason
            {
                IdReason = ViewportHelper.ValueOrNull<int>(row, "id_reason"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type"),
                ReasonNumber = ViewportHelper.ValueOrNull(row, "reason_number"),
                ReasonDate = ViewportHelper.ValueOrNull<DateTime>(row, "reason_date"),
                ReasonPrepared = ViewportHelper.ValueOrNull(row, "reason_prepared")
            };
        }

        public static TenancyReason FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static TenancyReason FromRow(DataGridViewRow row)
        {
            return new TenancyReason
            {
                IdReason = ViewportHelper.ValueOrNull<int>(row, "id_reason"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type"),
                ReasonNumber = ViewportHelper.ValueOrNull(row, "reason_number"),
                ReasonDate = ViewportHelper.ValueOrNull<DateTime>(row, "reason_date"),
                ReasonPrepared = ViewportHelper.ValueOrNull(row, "reason_prepared")
            };
        }

        public static void FillRow(TenancyReason tenancyReason, DataRow row)
        {
            row.BeginEdit();
            row["id_reason"] = ViewportHelper.ValueOrDbNull(tenancyReason.IdReason);
            row["id_process"] = ViewportHelper.ValueOrDbNull(tenancyReason.IdProcess);
            row["id_reason_type"] = ViewportHelper.ValueOrDbNull(tenancyReason.IdReasonType);
            row["reason_number"] = ViewportHelper.ValueOrDbNull(tenancyReason.ReasonNumber);
            row["reason_date"] = ViewportHelper.ValueOrDbNull(tenancyReason.ReasonDate);
            row["reason_prepared"] = ViewportHelper.ValueOrDbNull(tenancyReason.ReasonPrepared);
            row.EndEdit();
        }
    }
}
