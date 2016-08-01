using System.Data;
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

        public static TenancyPremisesAssoc FromRow(DataRow row)
        {
            return new TenancyPremisesAssoc
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area"),
                RentLivingArea = ViewportHelper.ValueOrNull<double>(row, "rent_living_area")
            };
        }

        public static TenancyPremisesAssoc FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }
    }
}
