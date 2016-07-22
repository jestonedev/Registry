using System;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class RestrictionConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[]
            {
                row["id_restriction"],
                row["id_restriction_type"],
                row["number"],
                row["date"],
                row["description"]
            };
        }


        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static Restriction FromRow(DataRow row)
        {
            return new Restriction
            {
                IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction"),
                IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type"),
                Number = ViewportHelper.ValueOrNull(row, "number"),
                Date = ViewportHelper.ValueOrNull<DateTime>(row, "date"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static Restriction FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static Restriction FromRow(DataGridViewRow row)
        {
            return new Restriction
            {
                IdRestriction = ViewportHelper.ValueOrNull<int>(row, "id_restriction"),
                IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type"),
                Number = ViewportHelper.ValueOrNull(row, "number"),
                Date = ViewportHelper.ValueOrNull<DateTime>(row, "date"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static void FillRow(Restriction restriction, DataRow row)
        {
            row.BeginEdit();
            row["id_restriction"] = ViewportHelper.ValueOrDbNull(restriction.IdRestriction);
            row["id_restriction_type"] = ViewportHelper.ValueOrDbNull(restriction.IdRestrictionType);
            row["number"] = ViewportHelper.ValueOrDbNull(restriction.Number);
            row["date"] = ViewportHelper.ValueOrDbNull(restriction.Date);
            row["description"] = ViewportHelper.ValueOrDbNull(restriction.Description);
            row.EndEdit();
        }
    }
}
