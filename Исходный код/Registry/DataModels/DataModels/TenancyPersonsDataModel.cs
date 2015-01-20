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
                            residence_id_street, residence_house, residence_flat, residence_room, personal_account, include_date, exclude_date)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE tenancy_persons SET id_process = ?, id_kinship = ?, surname = ?, 
                            name = ?, patronymic = ?, date_of_birth = ?, id_document_type = ?, date_of_document_issue = ?, 
                            document_num = ?, document_seria = ?, id_document_issued_by = ?, registration_id_street = ?, 
                            registration_house = ?, registration_flat = ?, registration_room = ?, residence_id_street = ?,
                            residence_house = ?, residence_flat = ?, residence_room = ? , personal_account = ?,
                            include_date = ?, exclude_date = ? WHERE id_person = ?";
        private static string tableName = "tenancy_persons";

        private TenancyPersonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_person"] };
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
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить участника найма из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                if (tenancyPerson == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность участника процесса найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyPerson.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_kinship", tenancyPerson.IdKinship));
                command.Parameters.Add(DBConnection.CreateParameter<string>("surname", tenancyPerson.Surname));
                command.Parameters.Add(DBConnection.CreateParameter<string>("name", tenancyPerson.Name));
                command.Parameters.Add(DBConnection.CreateParameter<string>("patronymic", tenancyPerson.Patronymic));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_birth", tenancyPerson.DateOfBirth));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_type", tenancyPerson.IdDocumentType));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_document_issue", tenancyPerson.DateOfDocumentIssue));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_num", tenancyPerson.DocumentNum));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_seria", tenancyPerson.DocumentSeria));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_issued_by", tenancyPerson.IdDocumentIssuedBy));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_id_street", tenancyPerson.RegistrationIdStreet));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_house", tenancyPerson.RegistrationHouse));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_flat", tenancyPerson.RegistrationFlat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_room", tenancyPerson.RegistrationRoom));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_id_street", tenancyPerson.ResidenceIdStreet));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_house", tenancyPerson.ResidenceHouse));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_flat", tenancyPerson.ResidenceFlat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_room", tenancyPerson.ResidenceRoom));
                command.Parameters.Add(DBConnection.CreateParameter<string>("personal_account", tenancyPerson.PersonalAccount));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("include_date", tenancyPerson.IncludeDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("exclude_date", tenancyPerson.ExcludeDate));

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
                        "Не удалось добавить участника найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                if (tenancyPerson == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность участника процесса найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyPerson.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_kinship", tenancyPerson.IdKinship));
                command.Parameters.Add(DBConnection.CreateParameter<string>("surname", tenancyPerson.Surname));
                command.Parameters.Add(DBConnection.CreateParameter<string>("name", tenancyPerson.Name));
                command.Parameters.Add(DBConnection.CreateParameter<string>("patronymic", tenancyPerson.Patronymic));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_birth", tenancyPerson.DateOfBirth));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_type", tenancyPerson.IdDocumentType));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date_of_document_issue", tenancyPerson.DateOfDocumentIssue));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_num", tenancyPerson.DocumentNum));
                command.Parameters.Add(DBConnection.CreateParameter<string>("document_seria", tenancyPerson.DocumentSeria));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_document_issued_by", tenancyPerson.IdDocumentIssuedBy));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_id_street", tenancyPerson.RegistrationIdStreet));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_house", tenancyPerson.RegistrationHouse));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_flat", tenancyPerson.RegistrationFlat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("registration_room", tenancyPerson.RegistrationRoom));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_id_street", tenancyPerson.ResidenceIdStreet));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_house", tenancyPerson.ResidenceHouse));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_flat", tenancyPerson.ResidenceFlat));
                command.Parameters.Add(DBConnection.CreateParameter<string>("residence_room", tenancyPerson.ResidenceRoom));
                command.Parameters.Add(DBConnection.CreateParameter<string>("personal_account", tenancyPerson.PersonalAccount));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("include_date", tenancyPerson.IncludeDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("exclude_date", tenancyPerson.ExcludeDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_person", tenancyPerson.IdPerson));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить информацию об участнике найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
