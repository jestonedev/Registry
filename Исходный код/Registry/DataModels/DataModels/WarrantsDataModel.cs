using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;
using System.Data;
using System.Globalization;

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

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, "Не удалось удалить доверенность из базы данных. Подробная ошибка: {0}", 
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Insert(Warrant warrant)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {   
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (warrant == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность доверенности", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant_doc_type", warrant.id_warrant_doc_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_num", warrant.registration_num));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("registration_date", warrant.registration_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("on_behalf_of", warrant.on_behalf_of));
                command.Parameters.Add(DBConnection.CreateParameter<string>("notary", warrant.notary));
                command.Parameters.Add(DBConnection.CreateParameter<string>("notary", warrant.notary));
                command.Parameters.Add(DBConnection.CreateParameter<string>("notary_district", warrant.notary_district));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", warrant.description));
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);

                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, 
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        connection.SqlRollbackTransaction();
                        return -1;
                    }
                    connection.SqlCommitTransaction();

                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, "Не удалось добавить запись о доверенности в базу данных. Подробная ошибка: {0}", 
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(Warrant warrant)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                if (warrant == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность доверенности", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
                command.CommandText = updateQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant_doc_type", warrant.id_warrant_doc_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_num", warrant.registration_num));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("registration_date", warrant.registration_date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("on_behalf_of", warrant.on_behalf_of));
                command.Parameters.Add(DBConnection.CreateParameter<string>("notary", warrant.notary));
                command.Parameters.Add(DBConnection.CreateParameter<string>("notary", warrant.notary));
                command.Parameters.Add(DBConnection.CreateParameter<string>("notary_district", warrant.notary_district));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", warrant.description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant", warrant.id_warrant));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить запись о доверенности в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
                
            }
        }
    }
}
