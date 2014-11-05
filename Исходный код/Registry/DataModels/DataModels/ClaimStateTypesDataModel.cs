using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public sealed class ClaimStateTypesDataModel: DataModel
    {
        private static ClaimStateTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claim_state_types";
        private static string deleteQuery = "DELETE FROM claim_state_types WHERE id_state_type = ?";
        private static string insertQuery = @"INSERT INTO claim_state_types
                            (state_type, is_start_state_type)
                            VALUES (?, ?)";
        private static string updateQuery = @"UPDATE claim_state_types SET state_type = ?, is_start_state_type = ? WHERE id_state_type = ?";

        private static string tableName = "claim_state_types";

        private ClaimStateTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_state_type"] };
        }

        public static ClaimStateTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimStateTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimStateTypesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_state_type", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить наименование вида состояния претензионно-исковой рабоыт. Подробная ошибка: {0}", 
                    e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(ClaimStateType claimStateType)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;
            command.Parameters.Add(connection.CreateParameter<string>("state_type", claimStateType.state_type));
            command.Parameters.Add(connection.CreateParameter<bool?>("is_start_state_type", claimStateType.is_start_state_type));
            command.Parameters.Add(connection.CreateParameter<int?>("id_state_type", claimStateType.id_state_type));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить наименование вида состояния претензионно-исковой работы в базе данных. "+
                    "Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(ClaimStateType claimStateType)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;
            command.Parameters.Add(connection.CreateParameter<string>("state_type", claimStateType.state_type));
            command.Parameters.Add(connection.CreateParameter<bool?>("is_start_state_type", claimStateType.is_start_state_type));
            try
            {
                connection.SqlBeginTransaction();
                connection.SqlModifyQuery(command);
                DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                connection.SqlCommitTransaction();
                if (last_id.Rows.Count == 0)
                {
                    MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                return Convert.ToInt32(last_id.Rows[0][0]);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить наименование вида состояния претензионно-исковой работы в базу данных. "+
                    "Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
