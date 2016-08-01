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

        public static ResettlePremisesFromAssoc FromRow(DataRow row)
        {
            return new ResettlePremisesFromAssoc
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdPremises = ViewportHelper.ValueOrNull<int>(row, "id_premises")
            };
        }

        public static ResettlePremisesFromAssoc FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static ResettlePremisesToAssoc CastFromToAssoc(ResettlePremisesFromAssoc assoc)
        {
            return new ResettlePremisesToAssoc
            {
                IdAssoc = assoc.IdAssoc,
                IdPremises = assoc.IdPremises,
                IdProcess = assoc.IdProcess
            };
        }
    }
}
