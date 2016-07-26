using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class RestrictionTypeConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_restriction_type"], 
                row["restriction_type"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static RestrictionType FromRow(DataRow row)
        {
            return new RestrictionType
            {
                IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type"),
                RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type")
            };
        }

        public static RestrictionType FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static RestrictionType FromRow(DataGridViewRow row)
        {
            return new RestrictionType
            {
                IdRestrictionType = ViewportHelper.ValueOrNull<int>(row, "id_restriction_type"),
                RestrictionTypeName = ViewportHelper.ValueOrNull(row, "restriction_type")
            };
        }

        public static void FillRow(RestrictionType restrictionType, DataRow row)
        {
            row["id_restriction_type"] = ViewportHelper.ValueOrDbNull(restrictionType.IdRestrictionType);
            row["restriction_type"] = ViewportHelper.ValueOrDbNull(restrictionType.RestrictionTypeName);
        }

    }
}
