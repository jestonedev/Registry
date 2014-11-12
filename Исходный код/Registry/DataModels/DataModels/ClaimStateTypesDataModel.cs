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
    public sealed class ClaimStateTypesDataModel: DataModel
    {
        private static ClaimStateTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claim_state_types WHERE deleted <> 1";
        private static string deleteQuery = "UPDATE claim_state_types SET deleted = 1 WHERE id_state_type = ?";
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
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_state_type"] };
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

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state_type", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить наименование вида состояния претензионно-исковой рабоыт. Подробная ошибка: {0}",
                        e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(ClaimStateType claimStateType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (claimStateType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект типа состояния претензионно-исковой работы", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("state_type", claimStateType.StateType));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("is_start_state_type", claimStateType.IsStartStateType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state_type", claimStateType.IdStateType));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить наименование вида состояния претензионно-исковой работы в базе данных. " +
                        "Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(ClaimStateType claimStateType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (claimStateType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект типа состояния претензионно-исковой работы", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("state_type", claimStateType.StateType));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("is_start_state_type", claimStateType.IsStartStateType));
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
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить наименование вида состояния претензионно-исковой работы в базу данных. " +
                        "Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
