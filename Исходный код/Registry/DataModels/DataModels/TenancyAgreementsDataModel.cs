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
    public sealed class TenancyAgreementsDataModel: DataModel
    {
        private static TenancyAgreementsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_agreements WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_agreements SET deleted = 1 WHERE id_agreement = ?";
        private static string insertQuery = @"INSERT INTO tenancy_agreements
                            (id_process, agreement_date, agreement_content, id_executor, id_warrant)
                            VALUES (?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE tenancy_agreements SET id_process = ?, agreement_date = ?, agreement_content = ?, 
                            id_executor = ?, id_warrant = ? WHERE id_agreement = ?";
        private static string tableName = "tenancy_agreements";

        private TenancyAgreementsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_agreement"] };
        }

        public static TenancyAgreementsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyAgreementsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyAgreementsDataModel(progressBar, incrementor);
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

        public int Update(TenancyAgreement tenancyAgreement)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_process", tenancyAgreement.id_process));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("agreement_date", tenancyAgreement.agreement_date));
            command.Parameters.Add(connection.CreateParameter<string>("agreement_content", tenancyAgreement.agreement_content));
            command.Parameters.Add(connection.CreateParameter<int?>("id_executor", tenancyAgreement.id_executor));
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", tenancyAgreement.id_warrant));
            command.Parameters.Add(connection.CreateParameter<int?>("id_agreement", tenancyAgreement.id_agreement));
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

        public int Insert(TenancyAgreement tenancyAgreement)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_process", tenancyAgreement.id_process));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("agreement_date", tenancyAgreement.agreement_date));
            command.Parameters.Add(connection.CreateParameter<string>("agreement_content", tenancyAgreement.agreement_content));
            command.Parameters.Add(connection.CreateParameter<int?>("id_executor", tenancyAgreement.id_executor));
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", tenancyAgreement.id_warrant));
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
