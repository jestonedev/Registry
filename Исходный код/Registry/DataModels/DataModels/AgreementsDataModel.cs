using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using Registry.Entities;

namespace Registry.DataModels
{
    public sealed class AgreementsDataModel: DataModel
    {
        private static AgreementsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM agreements WHERE deleted = 0";
        private static string deleteQuery = "UPDATE agreements SET deleted = 1 WHERE id_agreement = ?";
        private static string insertQuery = @"INSERT INTO agreements
                            (id_contract, agreement_date, agreement_content, id_executor, id_warrant)
                            VALUES (?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE agreements SET id_contract = ?, agreement_date = ?, agreement_content = ?, 
                            id_executor = ?, id_warrant = ? WHERE id_agreement = ?";
        private static string tableName = "agreements";

        private AgreementsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_agreement"] };
        }

        public static AgreementsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static AgreementsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new AgreementsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public bool EditingNewRecord { get; set; }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_agreement", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить соглашение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Agreement agreement)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", agreement.id_contract));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("agreement_date", agreement.agreement_date));
            command.Parameters.Add(connection.CreateParameter<string>("agreement_content", agreement.agreement_content));
            command.Parameters.Add(connection.CreateParameter<int?>("id_executor", agreement.id_executor));
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", agreement.id_warrant));
            command.Parameters.Add(connection.CreateParameter<int?>("id_agreement", agreement.id_agreement));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить информацию о соглашении в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Agreement agreement)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", agreement.id_contract));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("agreement_date", agreement.agreement_date));
            command.Parameters.Add(connection.CreateParameter<string>("agreement_content", agreement.agreement_content));
            command.Parameters.Add(connection.CreateParameter<int?>("id_executor", agreement.id_executor));
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", agreement.id_warrant));
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
                MessageBox.Show(String.Format("Не удалось добавить соглашение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
