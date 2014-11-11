using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using Registry.Entities;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class TenancyReasonsDataModel: DataModel
    {
        private static TenancyReasonsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_reasons WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_reasons SET deleted = 1 WHERE id_reason = ?";
        private static string insertQuery = @"INSERT INTO tenancy_reasons
                            (id_process, id_reason_type, reason_number, reason_date, reason_prepared)
                            VALUES (?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE tenancy_reasons SET id_process = ?, id_reason_type = ?, reason_number = ?, 
                            reason_date = ?, reason_prepared = ? WHERE id_reason = ?";
        private static string tableName = "tenancy_reasons";

        private TenancyReasonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_reason"] };
            table.Columns["reason_date"].DefaultValue = DateTime.Now.Date;
        }

        public static TenancyReasonsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyReasonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyReasonsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(TenancyReason tenancyReason)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (tenancyReason == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность основания найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyReason.id_process));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_reason_type", tenancyReason.id_reason_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("reason_number", tenancyReason.reason_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("reason_date", tenancyReason.reason_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("reason_prepared", tenancyReason.reason_prepared));
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    connection.SqlCommitTransaction();
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return -1;
                    }
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить основание найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(TenancyReason tenancyReason)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (tenancyReason == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность основания найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyReason.id_process));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_reason_type", tenancyReason.id_reason_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("reason_number", tenancyReason.reason_number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("reason_date", tenancyReason.reason_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("reason_prepared", tenancyReason.reason_prepared));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_reason", tenancyReason.id_reason));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить основание найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_reason", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить основание найма из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
