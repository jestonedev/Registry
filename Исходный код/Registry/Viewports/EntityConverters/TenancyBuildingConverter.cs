using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancyBuildingConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_assoc"],
                row["id_building"], 
                true, 
                row["rent_total_area"],
                row["rent_living_area"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static TenancyBuildingAssoc FromRow(DataRow row)
        {
            return new TenancyBuildingAssoc
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building"),
                RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area"),
                RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area")
            };
        }

        public static TenancyBuildingAssoc FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }
    }
}
