using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ResettlePremiseConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_assoc"],
                row["id_premises"], 
                true
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static ResettleObject FromRow(DataRow row)
        {
            return new ResettleObject
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdObject = ViewportHelper.ValueOrNull<int>(row, "id_premises")
            };
        }

        public static ResettleObject FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }
    }
}
