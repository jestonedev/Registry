using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyReasonTypeConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_reason_type"], 
                row["reason_name"],
                row["reason_template"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static ReasonType FromRow(DataRow row)
        {
            return new ReasonType
            {
                IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type"),
                ReasonName = ViewportHelper.ValueOrNull(row, "reason_name"),
                ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template")
            };
        }

        public static ReasonType FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static ReasonType FromRow(DataGridViewRow row)
        {
            return new ReasonType
            {
                IdReasonType = ViewportHelper.ValueOrNull<int>(row, "id_reason_type"),
                ReasonName = ViewportHelper.ValueOrNull(row, "reason_name"),
                ReasonTemplate = ViewportHelper.ValueOrNull(row, "reason_template")
            };
        }

        public static void FillRow(ReasonType reasonType, DataRow row)
        {
            row.BeginEdit();
            row["id_reason_type"] = ViewportHelper.ValueOrDbNull(reasonType.IdReasonType);
            row["reason_name"] = ViewportHelper.ValueOrDbNull(reasonType.ReasonName);
            row["reason_template"] = ViewportHelper.ValueOrDbNull(reasonType.ReasonTemplate);
            row.EndEdit();
        }
    }
}
