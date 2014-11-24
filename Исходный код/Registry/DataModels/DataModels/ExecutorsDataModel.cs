using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class ExecutorsDataModel: DataModel
    {
        private static ExecutorsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM executors WHERE deleted <> 1";
        private static string deleteQuery = "UPDATE executors SET deleted = 1 WHERE id_executor = ?";
        private static string insertQuery = @"INSERT INTO executors
                            (executor_name, executor_login, phone, is_inactive) VALUES (?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE executors SET executor_name = ?, executor_login = ?, phone = ?, is_inactive = ? WHERE id_executor = ?";
        private static string tableName = "executors";

        private ExecutorsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_executor"] };
            Table.Columns["is_inactive"].DefaultValue = false;
        }

        public static ExecutorsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ExecutorsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ExecutorsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(Executor executor)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (executor == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект исполнителя", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("executor_name", executor.ExecutorName));
                command.Parameters.Add(DBConnection.CreateParameter<string>("executor_login", executor.ExecutorLogin));
                command.Parameters.Add(DBConnection.CreateParameter<string>("phone", executor.Phone));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("is_inactive", executor.IsInactive));

                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);

                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        connection.SqlRollbackTransaction();
                        return -1;
                    }
                    connection.SqlCommitTransaction();

                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить запись об исполнителе в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(Executor executor)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (executor == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект исполнителя", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("executor_name", executor.ExecutorName));
                command.Parameters.Add(DBConnection.CreateParameter<string>("executor_login", executor.ExecutorLogin));
                command.Parameters.Add(DBConnection.CreateParameter<string>("phone", executor.Phone));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("is_inactive", executor.IsInactive));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_executor", executor.IdExecutor));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить запись об исполнителе в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_executor", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить исполнителя из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
