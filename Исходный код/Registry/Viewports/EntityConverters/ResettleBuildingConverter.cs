using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ResettleBuildingConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_assoc"],
                row["id_building"], 
                true
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static ResettleBuildingFromAssoc FromRow(DataRow row)
        {
            return new ResettleBuildingFromAssoc
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdBuilding = ViewportHelper.ValueOrNull<int>(row, "id_building")
            };
        }

        public static ResettleBuildingFromAssoc FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static ResettleBuildingToAssoc CastFromToAssoc(ResettleBuildingFromAssoc assoc)
        {
            return new ResettleBuildingToAssoc
            {
                IdAssoc = assoc.IdAssoc,
                IdBuilding = assoc.IdBuilding,
                IdProcess = assoc.IdProcess
            };
        }
    }
}
