using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using System.Data.Odbc;

namespace Security
{
    public static class AccessControl
    {
        private static int priveleges;
        private static string query = @"SELECT f_user_privileges('{0}')";

        public static void LoadPriveleges()
        {
            using (DBConnection connection = new DBConnection())
            {
                DbCommand command = DBConnection.CreateCommand();
                command.CommandText = String.Format(query, System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("'", "").Replace("\\", "\\\\"));
                try
                {
                    DataTable table = connection.SqlSelectTable("privileges", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул привелегии для данного пользователя", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        priveleges = 0;
                    }
                    priveleges = Convert.ToInt32(table.Rows[0][0]);
                }
                catch (OdbcException e)
                {
                    priveleges = 0;
                    MessageBox.Show(String.Format("Не удалось загрузить привелегии пользователя", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static bool HasPrivelege(Priveleges privelege)
        {
            return ((int)priveleges & (int)privelege) == (int)privelege;
        }

        public static bool HasNoPriveleges()
        {
            return (int)priveleges == 0;
        }
    }
}
