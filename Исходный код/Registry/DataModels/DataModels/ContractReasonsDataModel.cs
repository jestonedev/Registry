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
    public sealed class ContractReasonsDataModel: DataModel
    {
        private static ContractReasonsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM contract_reasons WHERE deleted = 0";
        private static string deleteQuery = "UPDATE contract_reasons SET deleted = 1 WHERE id_contract_reason = ?";
        private static string insertQuery = @"INSERT INTO contract_reasons
                            (id_contract, id_reason_type, reason_number, reason_date, contract_reason_prepared)
                            VALUES (?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE contract_reasons SET id_contract = ?, id_reason_type = ?, reason_number = ?, 
                            reason_date = ?, contract_reason_prepared = ? WHERE id_contract_reason = ?";
        private static string tableName = "contract_reasons";

        private ContractReasonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_contract_reason"] };
            table.Columns["reason_date"].DefaultValue = DateTime.Now.Date;
        }

        public static ContractReasonsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ContractReasonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ContractReasonsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Insert(ContractReason contractReason)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", contractReason.id_contract));
            command.Parameters.Add(connection.CreateParameter<int?>("id_reason_type", contractReason.id_reason_type));
            command.Parameters.Add(connection.CreateParameter<string>("reason_number", contractReason.reason_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("reason_date", contractReason.reason_date));
            command.Parameters.Add(connection.CreateParameter<string>("contract_reason_prepared", contractReason.contract_reason_prepared));

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
                MessageBox.Show(String.Format("Не удалось добавить основание найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Entities.ContractReason contractReason)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", contractReason.id_contract));
            command.Parameters.Add(connection.CreateParameter<int?>("id_reason_type", contractReason.id_reason_type));
            command.Parameters.Add(connection.CreateParameter<string>("reason_number", contractReason.reason_number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("reason_date", contractReason.reason_date));
            command.Parameters.Add(connection.CreateParameter<string>("contract_reason_prepared", contractReason.contract_reason_prepared));
            command.Parameters.Add(connection.CreateParameter<int?>("id_contract_reason", contractReason.id_contract_reason));

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить основание найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_contract_reason", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить основание найма из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
