using Registry.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public sealed class ResettlePersonsDataModel: DataModel
    {
        private static ResettlePersonsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM resettle_persons WHERE deleted = 0";
        private static string deleteQuery = "UPDATE resettle_persons SET deleted = 1 WHERE id_person = ?";
        private static string insertQuery = @"INSERT INTO resettle_persons
                            (id_process, surname, name, patronymic)
                            VALUES (?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE resettle_persons SET id_process = ?, surname = ?, 
                            name = ?, patronymic = ? WHERE id_person = ?";
        private static string tableName = "resettle_persons";

        private ResettlePersonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_person"] };
        }

        public static ResettlePersonsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ResettlePersonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ResettlePersonsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public bool EditingNewRecord { get; set; }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_person", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить участника процесса переселения из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(ResettlePerson resettlePerson)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (resettlePerson == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность участника процесса переселения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("id_process", resettlePerson.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter("surname", resettlePerson.Surname));
                command.Parameters.Add(DBConnection.CreateParameter("name", resettlePerson.Name));
                command.Parameters.Add(DBConnection.CreateParameter("patronymic", resettlePerson.Patronymic));

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
                        "Не удалось добавить участника процесса переселения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(ResettlePerson resettlePerson)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (resettlePerson == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность участника процесса найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("id_process", resettlePerson.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter("surname", resettlePerson.Surname));
                command.Parameters.Add(DBConnection.CreateParameter("name", resettlePerson.Name));
                command.Parameters.Add(DBConnection.CreateParameter("patronymic", resettlePerson.Patronymic));
                command.Parameters.Add(DBConnection.CreateParameter("id_person", resettlePerson.IdPerson));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить информацию об участнике переселения в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
