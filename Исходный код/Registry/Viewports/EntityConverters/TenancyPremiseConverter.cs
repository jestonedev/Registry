using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyPremiseConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_assoc"],
                row["id_premises"], 
                true, 
                row["rent_total_area"],
                row["rent_living_area"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static TenancyObject FromRow(DataRow row)
        {
            return new TenancyObject
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area"),
                RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area")
            };
        }

        public static TenancyObject FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }
    }
}
