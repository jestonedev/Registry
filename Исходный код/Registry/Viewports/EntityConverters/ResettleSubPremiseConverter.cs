using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ResettleSubPremiseConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_assoc"],
                row["id_sub_premises"], 
                true
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static ResettleSubPremisesFromAssoc FromRow(DataRow row)
        {
            return new ResettleSubPremisesFromAssoc
            {
                IdAssoc = ViewportHelper.ValueOrNull<int>(row, "id_assoc"),
                IdProcess = ViewportHelper.ValueOrNull<int>(row, "id_process"),
                IdSubPremises = ViewportHelper.ValueOrNull<int>(row, "id_sub_premises")
            };
        }

        public static ResettleSubPremisesFromAssoc FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static ResettleSubPremisesToAssoc CastFromToAssoc(ResettleSubPremisesFromAssoc assoc)
        {
            return new ResettleSubPremisesToAssoc
            {
                IdAssoc = assoc.IdAssoc,
                IdSubPremises = assoc.IdSubPremises,
                IdProcess = assoc.IdProcess
            };
        }
    }
}
