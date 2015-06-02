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

        private ClaimsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_claim"] };
            Table.Columns["amount_of_debt_rent"].DefaultValue = 0;
            Table.Columns["amount_of_debt_fine"].DefaultValue = 0;
            Table.Columns["amount_of_rent"].DefaultValue = 0;
            Table.Columns["amount_of_fine"].DefaultValue = 0;
            Table.Columns["amount_of_rent_recover"].DefaultValue = 0;
            Table.Columns["amount_of_fine_recover"].DefaultValue = 0;
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

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_claim", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить запись о претензионно-исковой работе из базы данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(Claim claim)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (claim == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект претензионно-исковой работы", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", claim.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_transfer", claim.DateOfTransfer));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_debt_rent", claim.AmountOfDebtRent));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_debt_fine", claim.AmountOfDebtFine));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("at_date", claim.AtDate));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_rent", claim.AmountOfRent));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_fine", claim.AmountOfFine));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_rent_recover", claim.AmountOfRentRecover));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_fine_recover", claim.AmountOfFineRecover));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("start_dept_period", claim.StartDeptPeriod));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("end_dept_period", claim.EndDeptPeriod));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", claim.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_claim", claim.IdClaim));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось изменить запись о претензионно-исковой работе в базе данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(Claim claim)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (claim == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект претензионно-исковой работы", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", claim.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_transfer", claim.DateOfTransfer));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_debt_rent", claim.AmountOfDebtRent));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_debt_fine", claim.AmountOfDebtFine));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("at_date", claim.AtDate));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_rent", claim.AmountOfRent));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_fine", claim.AmountOfFine));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_rent_recover", claim.AmountOfRentRecover));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("amount_of_fine_recover", claim.AmountOfFineRecover));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("start_dept_period", claim.StartDeptPeriod));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("end_dept_period", claim.EndDeptPeriod));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", claim.Description));

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
                        "Не удалось добавить запись о претензионно-исковой работе в базу данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
