using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class TenancySubPremiseConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_assoc"],
                row["id_premises"], 
                true, 
                row["rent_total_area"]
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static TenancySubPremisesAssoc FromRow(DataRow row)
        {
            return new TenancySubPremisesAssoc
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises"),
                RentTotalArea = ViewportHelper.ValueOrNull<double>(row, "rent_total_area")
            };
        }

        public static TenancySubPremisesAssoc FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }
    }
}
