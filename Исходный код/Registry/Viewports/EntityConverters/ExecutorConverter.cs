using System.Data;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal sealed class ExecutorConverter
    {
        public static object[] ToArray(DataRow row)
        {
            return new[] { 
                row["id_executor"], 
                row["executor_name"],
                row["executor_login"],
                row["phone"],
                ViewportHelper.ValueOrNull<bool>(row, "is_inactive") == true
            };
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        public static Executor FromRow(DataRow row)
        {
            return new Executor
            {
                IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor"),
                ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name"),
                ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login"),
                Phone = ViewportHelper.ValueOrNull(row, "phone"),
                IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive") == true
            };
        }

        public static Executor FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static Executor FromRow(DataGridViewRow row)
        {
            return new Executor
            {
                IdExecutor = ViewportHelper.ValueOrNull<int>(row, "id_executor"),
                ExecutorName = ViewportHelper.ValueOrNull(row, "executor_name"),
                ExecutorLogin = ViewportHelper.ValueOrNull(row, "executor_login"),
                Phone = ViewportHelper.ValueOrNull(row, "phone"),
                IsInactive = ViewportHelper.ValueOrNull<bool>(row, "is_inactive") == true
            };
        }

        public static void FillRow(Executor executor, DataRow row)
        {
            row.BeginEdit();
            row["id_executor"] = ViewportHelper.ValueOrDbNull(executor.IdExecutor);
            row["executor_name"] = ViewportHelper.ValueOrDbNull(executor.ExecutorName);
            row["executor_login"] = ViewportHelper.ValueOrDbNull(executor.ExecutorLogin);
            row["phone"] = ViewportHelper.ValueOrDbNull(executor.Phone);
            row["is_inactive"] = ViewportHelper.ValueOrDbNull(executor.IsInactive);
            row.EndEdit();
        }
    }
}
