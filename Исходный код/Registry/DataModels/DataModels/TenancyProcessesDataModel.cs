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
    public sealed class TenancyProcessesDataModel: DataModel
    {
        private static TenancyProcessesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_processes WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_processes SET deleted = 1 WHERE id_process = ?";
        private static string insertQuery = @"INSERT INTO tenancy_processes
                            (id_rent_type, id_warrant, registration_num
                             , registration_date, issue_date, begin_date, end_date
                             , residence_warrant_num, residence_warrant_date
                             , kumi_order_num, kumi_order_date, id_executor, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE tenancy_processes SET id_rent_type = ?, id_warrant = ?, registration_num = ?, 
                            registration_date = ?, issue_date = ?, begin_date = ?, end_date = ?,
                            residence_warrant_num = ?, residence_warrant_date = ?, kumi_order_num = ?, 
                            kumi_order_date = ?, id_executor = ?, description = ? WHERE id_process = ?";
        private static string tableName = "tenancy_processes";

        public bool EditingNewRecord { get; set; }

        private TenancyProcessesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_process"] };
            table.Columns["id_rent_type"].DefaultValue = 1;
            table.Columns["registration_date"].DefaultValue = DateTime.Now.Date;
        }

        public static TenancyProcessesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyProcessesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyProcessesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_process", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить процесс найма из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(TenancyProcess tenancy)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_rent_type", tenancy.id_rent_type));
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", tenancy.id_warrant));
            command.Parameters.Add(connection.CreateParameter<string>("registration_num", tenancy.registration_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("registration_date", tenancy.registration_date));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("issue_date", tenancy.issue_date));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("begin_date", tenancy.begin_date));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("end_date", tenancy.end_date));
            command.Parameters.Add(connection.CreateParameter<string>("residence_warrant_num", tenancy.residence_warrant_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("residence_warrant_date", tenancy.residence_warrant_date));
            command.Parameters.Add(connection.CreateParameter<string>("kumi_order_num", tenancy.kumi_order_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("kumi_order_date", tenancy.kumi_order_date));
            command.Parameters.Add(connection.CreateParameter<int?>("id_executor", tenancy.id_executor));
            command.Parameters.Add(connection.CreateParameter<string>("description", tenancy.description));
            command.Parameters.Add(connection.CreateParameter<int?>("id_process", tenancy.id_process));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось изменить данные о процессе найма. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(TenancyProcess tenancy)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_rent_type", tenancy.id_rent_type));
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", tenancy.id_warrant));
            command.Parameters.Add(connection.CreateParameter<string>("registration_num", tenancy.registration_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("registration_date", tenancy.registration_date));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("issue_date", tenancy.issue_date));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("begin_date", tenancy.begin_date));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("end_date", tenancy.end_date));
            command.Parameters.Add(connection.CreateParameter<string>("residence_warrant_num", tenancy.residence_warrant_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("residence_warrant_date", tenancy.residence_warrant_date));
            command.Parameters.Add(connection.CreateParameter<string>("kumi_order_num", tenancy.kumi_order_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("kumi_order_date", tenancy.kumi_order_date));
            command.Parameters.Add(connection.CreateParameter<int?>("id_executor", tenancy.id_executor));
            command.Parameters.Add(connection.CreateParameter<string>("description", tenancy.description));
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
                MessageBox.Show(String.Format("Не удалось добавить информацию о процессе найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
