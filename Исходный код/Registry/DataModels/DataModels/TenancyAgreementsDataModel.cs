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
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_agreement"] };
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

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_agreement", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить соглашение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(TenancyAgreement tenancyAgreement)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (tenancyAgreement == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект соглашения процесса найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyAgreement.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("agreement_date", tenancyAgreement.AgreementDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("agreement_content", tenancyAgreement.AgreementContent));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_executor", tenancyAgreement.IdExecutor));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant", tenancyAgreement.IdWarrant));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_agreement", tenancyAgreement.IdAgreement));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить информацию о соглашении в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(TenancyAgreement tenancyAgreement)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (tenancyAgreement == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект соглашения процесса найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyAgreement.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("agreement_date", tenancyAgreement.AgreementDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("agreement_content", tenancyAgreement.AgreementContent));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_executor", tenancyAgreement.IdExecutor));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant", tenancyAgreement.IdWarrant));
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
                        "Не удалось добавить соглашение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
