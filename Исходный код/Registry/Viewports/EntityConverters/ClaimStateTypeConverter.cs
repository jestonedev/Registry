using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ClaimStateTypeConverter
    {
        public static ClaimStateType FromRow(DataRow row)
        {
            return new ClaimStateType
            {
                IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type"),
                StateType = ViewportHelper.ValueOrNull(row, "state_type"),
                IsStartStateType = ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type") == true
            };
        }

        public static ClaimStateType FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static ClaimStateType FromRow(DataGridViewRow row)
        {
            return new ClaimStateType
            {
                IdStateType = ViewportHelper.ValueOrNull<int>(row, "id_state_type"),
                StateType = ViewportHelper.ValueOrNull(row, "state_type"),
                IsStartStateType = ViewportHelper.ValueOrNull<bool>(row, "is_start_state_type") == true
            };
        }

        public static object[] ToArray(DataRowView dataRowView)
        {
            return new[]
            {
                dataRowView["id_state_type"],
                dataRowView["state_type"],
                ViewportHelper.ValueOrNull<bool>(dataRowView, "is_start_state_type") == true
            };
        }
    }
}
