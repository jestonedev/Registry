using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;
using System.Data;

namespace Registry.DataModels
{
    public sealed class WarrantsDataModel: DataModel
    {
        private static WarrantsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM warrants";
        private static string deleteQuery = "DELETE FROM warrants WHERE id_warrant = ?";
        private static string insertQuery = @"INSERT INTO warrants
                            (id_warrant_doc_type, registration_num, 
                            registration_date, on_behalf_of, notary,
                            notary_district, description) VALUES (?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE warrants SET id_warrant_doc_type = ?,
                            registration_num = ?, registration_date = ?, on_behalf_of = ?, notary = ?,
                            notary_district = ?, description = ? WHERE id_warrant = ?";
        private static string tableName = "warrants";

        public bool EditingNewRecord { get; set; }

        private WarrantsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_warrant"] };
        }


        public static WarrantsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static WarrantsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new WarrantsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить доверенность из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Warrant warrant)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant_doc_type", warrant.id_warrant_doc_type));
            command.Parameters.Add(connection.CreateParameter<string>("registration_num", warrant.registration_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("registration_date", warrant.registration_date));
            command.Parameters.Add(connection.CreateParameter<string>("on_behalf_of", warrant.on_behalf_of));
            command.Parameters.Add(connection.CreateParameter<string>("notary", warrant.notary));
            command.Parameters.Add(connection.CreateParameter<string>("notary", warrant.notary));
            command.Parameters.Add(connection.CreateParameter<string>("notary_district", warrant.notary_district));
            command.Parameters.Add(connection.CreateParameter<string>("description", warrant.description));

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
                MessageBox.Show(String.Format("Не удалось добавить запись о доверенности в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Warrant warrant)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant_doc_type", warrant.id_warrant_doc_type));
            command.Parameters.Add(connection.CreateParameter<string>("registration_num", warrant.registration_num));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("registration_date", warrant.registration_date));
            command.Parameters.Add(connection.CreateParameter<string>("on_behalf_of", warrant.on_behalf_of));
            command.Parameters.Add(connection.CreateParameter<string>("notary", warrant.notary));
            command.Parameters.Add(connection.CreateParameter<string>("notary", warrant.notary));
            command.Parameters.Add(connection.CreateParameter<string>("notary_district", warrant.notary_district));
            command.Parameters.Add(connection.CreateParameter<string>("description", warrant.description));
            command.Parameters.Add(connection.CreateParameter<int?>("id_warrant", warrant.id_warrant));

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить запись о доверенности в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
