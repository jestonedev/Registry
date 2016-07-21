using System.Data;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ClaimStateTypeRelationConverter
    {
        public static object[] ToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["id_relation"], 
                dataRowView["id_state_from"],
                dataRowView["id_state_to"],
                true    // checked
            };
        }

        public static ClaimStateTypeRelation FromRow(DataRow row)
        {
            return new ClaimStateTypeRelation
            {
                IdRelation = ViewportHelper.ValueOrNull<int>(row, "id_relation"),
                IdStateFrom = ViewportHelper.ValueOrNull<int>(row, "id_state_from"),
                IdStateTo = ViewportHelper.ValueOrNull<int>(row, "id_state_to")
            };
        }

        public static ClaimStateTypeRelation FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

    }
}
