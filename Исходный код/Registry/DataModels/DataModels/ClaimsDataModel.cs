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
    public sealed class ClaimsDataModel: DataModel
    {
        private static ClaimsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claims c WHERE deleted = 0";
        private static string deleteQuery = "UPDATE claims SET deleted = 1 WHERE id_claim = ?";
        private static string insertQuery = @"INSERT INTO claims
                            (id_process, date_of_transfer, amount_of_debt_rent, 
                            amount_of_debt_fine, at_date, amount_of_rent,
                            amount_of_fine, amount_of_rent_recover, amount_of_fine_recover, start_dept_period, end_dept_period, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE claims SET id_process = ?,
                            date_of_transfer = ?, amount_of_debt_rent = ?, amount_of_debt_fine = ?, at_date = ?,
                            amount_of_rent = ?, amount_of_fine = ?, amount_of_rent_recover = ?, 
                            amount_of_fine_recover = ?, start_dept_period = ?, end_dept_period = ?, description = ? WHERE id_claim = ?";

        private static string tableName = "claims";

        public bool EditingNewRecord { get; set; }

        private ClaimsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_claim"] };
            table.Columns["id_process"].DefaultValue = 0;
            table.Columns["amount_of_debt_rent"].DefaultValue = 0;
            table.Columns["amount_of_debt_fine"].DefaultValue = 0;
            table.Columns["amount_of_rent"].DefaultValue = 0;
            table.Columns["amount_of_fine"].DefaultValue = 0;
            table.Columns["amount_of_rent_recover"].DefaultValue = 0;
            table.Columns["amount_of_fine_recover"].DefaultValue = 0;
        }

        public static ClaimsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_claim", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить запись о претензионно-исковой работе из базы данных. Подробная ошибка: {0}", 
                    e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Claim claim)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_process", claim.id_process));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date_of_transfer", claim.date_of_transfer));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_debt_rent", claim.amount_of_debt_rent));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_debt_fine", claim.amount_of_debt_fine));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("at_date", claim.at_date));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_rent", claim.amount_of_rent));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_fine", claim.amount_of_fine));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_rent_recover", claim.amount_of_rent_recover));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_fine_recover", claim.amount_of_fine_recover));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("start_dept_period", claim.start_dept_period));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("end_dept_period", claim.end_dept_period));
            command.Parameters.Add(connection.CreateParameter<string>("description", claim.description));
            command.Parameters.Add(connection.CreateParameter<int?>("id_claim", claim.id_claim));

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить запись о претензионно-исковой работе в базе данных. Подробная ошибка: {0}", 
                    e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Claim claim)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_process", claim.id_process));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date_of_transfer", claim.date_of_transfer));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_debt_rent", claim.amount_of_debt_rent));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_debt_fine", claim.amount_of_debt_fine));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("at_date", claim.at_date));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_rent", claim.amount_of_rent));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_fine", claim.amount_of_fine));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_rent_recover", claim.amount_of_rent_recover));
            command.Parameters.Add(connection.CreateParameter<decimal?>("amount_of_fine_recover", claim.amount_of_fine_recover));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("start_dept_period", claim.start_dept_period));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("end_dept_period", claim.end_dept_period));
            command.Parameters.Add(connection.CreateParameter<string>("description", claim.description));

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
                MessageBox.Show(String.Format("Не удалось добавить запись о претензионно-исковой работе в базу данных. Подробная ошибка: {0}", 
                    e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
