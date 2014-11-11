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
    public sealed class ClaimStatesDataModel: DataModel
    {
        private static ClaimStatesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claim_states c WHERE deleted = 0";
        private static string deleteQuery = "UPDATE claim_states SET deleted = 1 WHERE id_state = ?";
        private static string insertQuery = @"INSERT INTO claim_states
                            (id_claim, id_state_type, date_start_state, date_end_state, document_num,
                            document_date, description) VALUES (?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE claim_states SET id_claim = ?,
                            id_state_type = ?, date_start_state = ?, date_end_state = ?, document_num = ?,
                            document_date = ?, description = ? WHERE id_state = ?";

        private static string tableName = "claim_states";

        public bool EditingNewRecord { get; set; }

        private ClaimStatesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_state"] };
        }

        public static ClaimStatesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimStatesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimStatesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(ClaimState claimState)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;

                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_claim", claimState.id_claim));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state_type", claimState.id_state_type));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_start_state", claimState.date_start_state));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_end_state", claimState.date_end_state));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_num", claimState.document_num));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("document_date", claimState.document_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", claimState.description));

                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        connection.SqlRollbackTransaction();
                        return -1;
                    }
                    connection.SqlCommitTransaction();
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить запись о состоянии претензионно-исковой работы в базу данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(ClaimState claimState)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;

                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_claim", claimState.id_claim));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state_type", claimState.id_state_type));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_start_state", claimState.date_start_state));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_end_state", claimState.date_end_state));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_num", claimState.document_num));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("document_date", claimState.document_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", claimState.description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", claimState.id_state));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить запись о состоянии претензионно-исковой работы в базе данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить запись о состоянии претензионно-исковой работы из базы данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
