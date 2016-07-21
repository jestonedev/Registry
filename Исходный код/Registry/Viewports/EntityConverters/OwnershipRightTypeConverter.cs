using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class OwnershipRightTypeConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_ownership_right_type"], 
                row["ownership_right_type"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static OwnershipRightType FromRow(DataRow row)
        {
            return new OwnershipRightType
            {
                IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type"),
                OwnershipRightTypeName = ViewportHelper.ValueOrNull(row, "ownership_right_type")
            };
        }

        public static OwnershipRightType FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(OwnershipRightType ownershipRightType, DataRow row)
        {
            row.BeginEdit();
            row["id_ownership_right_type"] = ViewportHelper.ValueOrDBNull(ownershipRightType.IdOwnershipRightType);
            row["ownership_right_type"] = ViewportHelper.ValueOrDBNull(ownershipRightType.OwnershipRightTypeName);
            row.EndEdit();
        }
    }
}
