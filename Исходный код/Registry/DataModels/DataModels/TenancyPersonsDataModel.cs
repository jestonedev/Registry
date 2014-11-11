using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class TenancyPersonsDataModel: DataModel
    {
        private static TenancyPersonsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_persons WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_persons SET deleted = 1 WHERE id_person = ?";
        private static string insertQuery = @"INSERT INTO tenancy_persons
                            (id_process, id_kinship, surname, name, patronymic, date_of_birth, id_document_type, 
                            date_of_document_issue, document_num, document_seria, id_document_issued_by,
                            registration_id_street, registration_house, registration_flat, registration_room,
                            residence_id_street, residence_house, residence_flat, residence_room, personal_account)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE tenancy_persons SET id_process = ?, id_kinship = ?, surname = ?, 
                            name = ?, patronymic = ?, date_of_birth = ?, id_document_type = ?, date_of_document_issue = ?, 
                            document_num = ?, document_seria = ?, id_document_issued_by = ?, registration_id_street = ?, 
                            registration_house = ?, registration_flat = ?, registration_room = ?, residence_id_street = ?,
                            residence_house = ?, residence_flat = ?, residence_room = ? , personal_account = ? WHERE id_person = ?";
        private static string tableName = "tenancy_persons";

        private TenancyPersonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_persons"] };
        }

        public static TenancyPersonsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyPersonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyPersonsDataModel(progressBar, incrementor);
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
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить участника договора из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Insert(TenancyPerson tenancyPerson)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;

                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyPerson.id_process));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_kinship", tenancyPerson.id_kinship));
                command.Parameters.Add(DBConnection.CreateParameter<string>("surname", tenancyPerson.surname));
                command.Parameters.Add(DBConnection.CreateParameter<string>("name", tenancyPerson.name));
                command.Parameters.Add(DBConnection.CreateParameter<string>("patronymic", tenancyPerson.patronymic));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_birth", tenancyPerson.date_of_birth));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_type", tenancyPerson.id_document_type));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_document_issue", tenancyPerson.date_of_document_issue));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_num", tenancyPerson.document_num));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_seria", tenancyPerson.document_seria));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_issued_by", tenancyPerson.id_document_issued_by));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_id_street", tenancyPerson.registration_id_street));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_house", tenancyPerson.registration_house));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_flat", tenancyPerson.registration_flat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_room", tenancyPerson.registration_room));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_id_street", tenancyPerson.residence_id_street));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_house", tenancyPerson.residence_house));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_flat", tenancyPerson.residence_flat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_room", tenancyPerson.residence_room));
                command.Parameters.Add(DBConnection.CreateParameter<string>("personal_account", tenancyPerson.personal_account));

                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    connection.SqlCommitTransaction();
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return -1;
                    }
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить участника найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(TenancyPerson tenancyPerson)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;

                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyPerson.id_process));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_kinship", tenancyPerson.id_kinship));
                command.Parameters.Add(DBConnection.CreateParameter<string>("surname", tenancyPerson.surname));
                command.Parameters.Add(DBConnection.CreateParameter<string>("name", tenancyPerson.name));
                command.Parameters.Add(DBConnection.CreateParameter<string>("patronymic", tenancyPerson.patronymic));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_birth", tenancyPerson.date_of_birth));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_type", tenancyPerson.id_document_type));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_document_issue", tenancyPerson.date_of_document_issue));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_num", tenancyPerson.document_num));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_seria", tenancyPerson.document_seria));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_issued_by", tenancyPerson.id_document_issued_by));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_id_street", tenancyPerson.registration_id_street));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_house", tenancyPerson.registration_house));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_flat", tenancyPerson.registration_flat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_room", tenancyPerson.registration_room));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_id_street", tenancyPerson.residence_id_street));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_house", tenancyPerson.residence_house));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_flat", tenancyPerson.residence_flat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_room", tenancyPerson.residence_room));
                command.Parameters.Add(DBConnection.CreateParameter<string>("personal_account", tenancyPerson.personal_account));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_person", tenancyPerson.id_person));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить информацию об участнике найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
