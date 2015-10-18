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
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public sealed class WarrantsDataModel: DataModel
    {
        private static WarrantsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM warrants WHERE deleted <> 1";
        private static string deleteQuery = "UPDATE warrants SET deleted = 1 WHERE id_warrant = ?";
        private static string insertQuery = @"INSERT INTO warrants
                            (id_warrant_doc_type, registration_num, 
                            registration_date, on_behalf_of, notary,
                            notary_district, description) VALUES (?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE warrants SET id_warrant_doc_type = ?,
                            registration_num = ?, registration_date = ?, on_behalf_of = ?, notary = ?,
                            notary_district = ?, description = ? WHERE id_warrant = ?";
        private static string tableName = "warrants";

        private WarrantsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_warrant"] };
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
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Не удалось удалить доверенность из базы данных. Подробная ошибка: {0}", 
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("id_warrant_doc_type", warrant.IdWarrantDocType));
                command.Parameters.Add(DBConnection.CreateParameter("registration_num", warrant.RegistrationNum));
                command.Parameters.Add(DBConnection.CreateParameter("registration_date", warrant.RegistrationDate));
                command.Parameters.Add(DBConnection.CreateParameter("on_behalf_of", warrant.OnBehalfOf));
                command.Parameters.Add(DBConnection.CreateParameter("notary", warrant.Notary));
                command.Parameters.Add(DBConnection.CreateParameter("notary_district", warrant.NotaryDistrict));
                command.Parameters.Add(DBConnection.CreateParameter("description", warrant.Description));
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);

                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, 
                            MessageBoxDefaultButton.Button1);
                        connection.SqlRollbackTransaction();
                        return -1;
                    }
                    connection.SqlCommitTransaction();

                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Не удалось добавить запись о доверенности в базу данных. Подробная ошибка: {0}", 
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.CommandText = updateQuery;
                command.Parameters.Add(DBConnection.CreateParameter("id_warrant_doc_type", warrant.IdWarrantDocType));
                command.Parameters.Add(DBConnection.CreateParameter("registration_num", warrant.RegistrationNum));
                command.Parameters.Add(DBConnection.CreateParameter("registration_date", warrant.RegistrationDate));
                command.Parameters.Add(DBConnection.CreateParameter("on_behalf_of", warrant.OnBehalfOf));
                command.Parameters.Add(DBConnection.CreateParameter("notary", warrant.Notary));
                command.Parameters.Add(DBConnection.CreateParameter("notary_district", warrant.NotaryDistrict));
                command.Parameters.Add(DBConnection.CreateParameter("description", warrant.Description));
                command.Parameters.Add(DBConnection.CreateParameter("id_warrant", warrant.IdWarrant));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить запись о доверенности в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }               
            }
        }
    }
}
