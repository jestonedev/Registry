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
    public sealed class TenancyProcessesDataModel: DataModel
    {
        private static TenancyProcessesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_processes WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_processes SET deleted = 1 WHERE id_process = ?";
        private static string insertQuery = @"INSERT INTO tenancy_processes
                            (id_rent_type, id_warrant, registration_num
                             , registration_date, issue_date, begin_date, end_date
                             , residence_warrant_num, residence_warrant_date
                             , protocol_num, protocol_date, id_executor, description)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE tenancy_processes SET id_rent_type = ?, id_warrant = ?, registration_num = ?, 
                            registration_date = ?, issue_date = ?, begin_date = ?, end_date = ?,
                            residence_warrant_num = ?, residence_warrant_date = ?, protocol_num = ?, 
                            protocol_date = ?, id_executor = ?, description = ? WHERE id_process = ?";
        private static string tableName = "tenancy_processes";

        public bool EditingNewRecord { get; set; }

        private TenancyProcessesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_process"] };
            Table.Columns["id_rent_type"].DefaultValue = 1;
            Table.Columns["registration_date"].DefaultValue = DateTime.Now.Date;
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
                        "Не удалось удалить процесс найма из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(TenancyProcess tenancy)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (tenancy == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность процесса найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_rent_type", tenancy.IdRentType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant", tenancy.IdWarrant));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_num", tenancy.RegistrationNum));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("registration_date", tenancy.RegistrationDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("issue_date", tenancy.IssueDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("begin_date", tenancy.BeginDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("end_date", tenancy.EndDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_warrant_num", tenancy.ResidenceWarrantNum));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("residence_warrant_date", tenancy.ResidenceWarrantDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("protocol_num", tenancy.ProtocolNum));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("protocol_date", tenancy.ProtocolDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_executor", tenancy.IdExecutor));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", tenancy.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancy.IdProcess));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о процессе найма. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(TenancyProcess tenancy)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (tenancy == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность процесса найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_rent_type", tenancy.IdRentType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant", tenancy.IdWarrant));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_num", tenancy.RegistrationNum));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("registration_date", tenancy.RegistrationDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("issue_date", tenancy.IssueDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("begin_date", tenancy.BeginDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("end_date", tenancy.EndDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_warrant_num", tenancy.ResidenceWarrantNum));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("residence_warrant_date", tenancy.ResidenceWarrantDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("protocol_num", tenancy.ProtocolNum));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("protocol_date", tenancy.ProtocolDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_executor", tenancy.IdExecutor));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", tenancy.Description));
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
                        "Не удалось добавить информацию о процессе найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
