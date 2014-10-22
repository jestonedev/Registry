using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.Odbc;
using Registry.Entities;
using System.Data;

namespace Registry.DataModels
{
    public sealed class ReasonTypesDataModel: DataModel
    {
        private static ReasonTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM reason_types";
        private static string deleteQuery = "DELETE FROM reason_types WHERE id_reason_type = ?";
        private static string insertQuery = @"INSERT INTO reason_types
                            (reason_name, reason_template) VALUES (?, ?)";
        private static string updateQuery = @"UPDATE reason_types SET reason_name = ?, reason_template = ? WHERE id_reason_type = ?";
        private static string tableName = "reason_types";

        private ReasonTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_reason_type"] };
        }

        public static ReasonTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ReasonTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ReasonTypesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Update(ReasonType reasonType)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<string>("reason_name", reasonType.reason_name));
            command.Parameters.Add(connection.CreateParameter<string>("reason_template", reasonType.reason_template));
            command.Parameters.Add(connection.CreateParameter<int?>("id_reason_type", reasonType.id_reason_type));

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить запись о виде основания в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(ReasonType reasonType)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<string>("reason_name", reasonType.reason_name));
            command.Parameters.Add(connection.CreateParameter<string>("reason_template", reasonType.reason_template));

            try
            {
                connection.SqlBeginTransaction();
                connection.SqlModifyQuery(command);
                DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);

                if (last_id.Rows.Count == 0)
                {
                    MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connection.SqlRollbackTransaction();
                    return -1;
                }
                connection.SqlCommitTransaction();

                return Convert.ToInt32(last_id.Rows[0][0]);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить запись о виде основания в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_reason_type", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить вид основания из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
