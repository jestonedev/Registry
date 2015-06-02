using Registry.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public class ResettleProcessesDataModel: DataModel
    {
        private static ResettleProcessesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM resettle_processes WHERE deleted = 0";
        private static string deleteQuery = "UPDATE resettle_processes SET deleted = 1 WHERE id_process = ?";
        private static string insertQuery = @"INSERT INTO resettle_processes
                            (resettle_date, id_document_residence, debts, description)
                            VALUES (?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE resettle_processes SET resettle_date = ?, id_document_residence = ?, debts = ?, 
                            description = ? WHERE id_process = ?";
        private static string tableName = "resettle_processes";

        private ResettleProcessesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_process"] };
            Table.Columns["debts"].DefaultValue = 0;
        }

        public static ResettleProcessesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ResettleProcessesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ResettleProcessesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить процесс переселения из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(ResettleProcess resettle)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (resettle == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность процесса переселения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("resettle_date", resettle.ResettleDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_residence", resettle.IdDocumentResidence));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("debts", resettle.Debts));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", resettle.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", resettle.IdProcess));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о процессе переселения. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(ResettleProcess resettle)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (resettle == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность процесса переселения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("resettle_date", resettle.ResettleDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_residence", resettle.IdDocumentResidence));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("debts", resettle.Debts));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", resettle.Description));
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    connection.SqlCommitTransaction();
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return -1;
                    }
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить информацию о процессе переселения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
