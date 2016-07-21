using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class OwnershipRightConverter
    {
        public static object[] ToArray(DataRowView row)
        {
            return new[] { 
                row["id_ownership_right"], 
                row["id_ownership_right_type"], 
                row["number"], 
                row["date"], 
                row["description"]
            };
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
            row["id_ownership_right"] = ViewportHelper.ValueOrDBNull(ownershipRight.IdOwnershipRight);
            row["id_ownership_right_type"] = ViewportHelper.ValueOrDBNull(ownershipRight.IdOwnershipRightType);
            row["number"] = ViewportHelper.ValueOrDBNull(ownershipRight.Number);
            row["date"] = ViewportHelper.ValueOrDBNull(ownershipRight.Date);
            row["description"] = ViewportHelper.ValueOrDBNull(ownershipRight.Description);
        }
    }
}
