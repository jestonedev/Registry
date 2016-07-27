using System;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class OwnershipRightConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_ownership_right"], 
                row["id_ownership_right_type"], 
                row["number"], 
                row["date"], 
                row["description"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static OwnershipRight FromRow(DataRow row)
        {
            return new OwnershipRight
            {
                IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right"),
                IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type"),
                Number = ViewportHelper.ValueOrNull(row, "number"),
                Date = ViewportHelper.ValueOrNull<DateTime>(row, "date"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static OwnershipRight FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static OwnershipRight FromRow(DataGridViewRow row)
        {
            return new OwnershipRight
            {
                IdOwnershipRight = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right"),
                IdOwnershipRightType = ViewportHelper.ValueOrNull<int>(row, "id_ownership_right_type"),
                Number = ViewportHelper.ValueOrNull(row, "number"),
                Date = ViewportHelper.ValueOrNull<DateTime>(row, "date"),
                Description = ViewportHelper.ValueOrNull(row, "description")
            };
        }

        public static void FillRow(OwnershipRight ownershipRight, DataRow row)
        {
            row["id_ownership_right"] = ViewportHelper.ValueOrDbNull(ownershipRight.IdOwnershipRight);
            row["id_ownership_right_type"] = ViewportHelper.ValueOrDbNull(ownershipRight.IdOwnershipRightType);
            row["number"] = ViewportHelper.ValueOrDbNull(ownershipRight.Number);
            row["date"] = ViewportHelper.ValueOrDbNull(ownershipRight.Date);
            row["description"] = ViewportHelper.ValueOrDbNull(ownershipRight.Description);
        }
    }
}
