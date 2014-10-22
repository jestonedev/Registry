using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public sealed class PersonsDataModel: DataModel
    {
        private static PersonsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM persons WHERE deleted = 0";
        private static string deleteQuery = "UPDATE persons SET deleted = 1 WHERE id_person = ?";
        private static string insertQuery = @"INSERT INTO persons
                            (id_contract, id_kinship, surname, name, patronymic, date_of_birth, id_document_type, 
                            date_of_document_issue, document_num, document_seria, id_document_issued_by,
                            registration_id_street, registration_house, registration_flat, registration_room,
                            residence_id_street, residence_house, residence_flat, residence_room, personal_account)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE persons SET id_contract = ?, id_kinship = ?, surname = ?, 
                            name = ?, patronymic = ?, date_of_birth = ?, id_document_type = ?, date_of_document_issue = ?, 
                            document_num = ?, document_seria = ?, id_document_issued_by = ?, registration_id_street = ?, 
                            registration_house = ?, registration_flat = ?, registration_room = ?, residence_id_street = ?,
                            residence_house = ?, residence_flat = ?, residence_room = ? , personal_account = ? WHERE id_person = ?";
        private static string tableName = "persons";

        private PersonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_persons"] };
        }

        public static PersonsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static PersonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new PersonsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public bool EditingNewRecord { get; set; }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_person", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить участника договора из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Person person)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", person.id_contract));
            command.Parameters.Add(connection.CreateParameter<int?>("id_kinship", person.id_kinship));
            command.Parameters.Add(connection.CreateParameter<string>("surname", person.surname));
            command.Parameters.Add(connection.CreateParameter<string>("name", person.name));
            command.Parameters.Add(connection.CreateParameter<string>("patronymic", person.patronymic));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date_of_birth", person.date_of_birth));
            command.Parameters.Add(connection.CreateParameter<int?>("id_document_type", person.id_document_type));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date_of_document_issue", person.date_of_document_issue));
            command.Parameters.Add(connection.CreateParameter<string>("document_num", person.document_num));
            command.Parameters.Add(connection.CreateParameter<string>("document_seria", person.document_seria));
            command.Parameters.Add(connection.CreateParameter<int?>("id_document_issued_by", person.id_document_issued_by));
            command.Parameters.Add(connection.CreateParameter<string>("registration_id_street", person.registration_id_street));
            command.Parameters.Add(connection.CreateParameter<string>("registration_house", person.registration_house));
            command.Parameters.Add(connection.CreateParameter<string>("registration_flat", person.registration_flat));
            command.Parameters.Add(connection.CreateParameter<string>("registration_room", person.registration_room));
            command.Parameters.Add(connection.CreateParameter<string>("residence_id_street", person.residence_id_street));
            command.Parameters.Add(connection.CreateParameter<string>("residence_house", person.residence_house));
            command.Parameters.Add(connection.CreateParameter<string>("residence_flat", person.residence_flat));
            command.Parameters.Add(connection.CreateParameter<string>("residence_room", person.residence_room));
            command.Parameters.Add(connection.CreateParameter<string>("personal_account", person.personal_account));

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
                MessageBox.Show(String.Format("Не удалось добавить участника найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Person person)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", person.id_contract));
            command.Parameters.Add(connection.CreateParameter<int?>("id_kinship", person.id_kinship));
            command.Parameters.Add(connection.CreateParameter<string>("surname", person.surname));
            command.Parameters.Add(connection.CreateParameter<string>("name", person.name));
            command.Parameters.Add(connection.CreateParameter<string>("patronymic", person.patronymic));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date_of_birth", person.date_of_birth));
            command.Parameters.Add(connection.CreateParameter<int?>("id_document_type", person.id_document_type));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date_of_document_issue", person.date_of_document_issue));
            command.Parameters.Add(connection.CreateParameter<string>("document_num", person.document_num));
            command.Parameters.Add(connection.CreateParameter<string>("document_seria", person.document_seria));
            command.Parameters.Add(connection.CreateParameter<int?>("id_document_issued_by", person.id_document_issued_by));
            command.Parameters.Add(connection.CreateParameter<string>("registration_id_street", person.registration_id_street));
            command.Parameters.Add(connection.CreateParameter<string>("registration_house", person.registration_house));
            command.Parameters.Add(connection.CreateParameter<string>("registration_flat", person.registration_flat));
            command.Parameters.Add(connection.CreateParameter<string>("registration_room", person.registration_room));
            command.Parameters.Add(connection.CreateParameter<string>("residence_id_street", person.residence_id_street));
            command.Parameters.Add(connection.CreateParameter<string>("residence_house", person.residence_house));
            command.Parameters.Add(connection.CreateParameter<string>("residence_flat", person.residence_flat));
            command.Parameters.Add(connection.CreateParameter<string>("residence_room", person.residence_room));
            command.Parameters.Add(connection.CreateParameter<string>("personal_account", person.personal_account));
            command.Parameters.Add(connection.CreateParameter<int?>("id_person", person.id_person));

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить информацию об участнике найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
