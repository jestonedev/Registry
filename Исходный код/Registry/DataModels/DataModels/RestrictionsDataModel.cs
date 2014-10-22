using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Registry.Entities;
using System.Data.Odbc;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class RestrictionsDataModel : DataModel
    {
        private static RestrictionsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restrictions";
        private static string deleteQuery = "UPDATE restrictions SET deleted = 1 WHERE id_restriction = ?";
        private static string insertQuery = @"INSERT INTO restrictions
                            (id_restriction_type, number, `date`, description)
                            VALUES (?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE restrictions SET id_restriction_type = ?,
                            number= ?, `date` = ?, description = ? WHERE id_restriction = ?";
        private static string tableName = "restrictions";

        private RestrictionsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_restriction"] };
        }

        public static RestrictionsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static RestrictionsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new RestrictionsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Insert(Entities.Restriction restriction, ParentTypeEnum ParentType, int id_parent)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_restriction_type", restriction.id_restriction_type));
            command.Parameters.Add(connection.CreateParameter<string>("number", restriction.number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date", restriction.date));
            command.Parameters.Add(connection.CreateParameter<string>("description", restriction.description));

            DbCommand command_assoc = connection.CreateCommand();
            if (ParentType == ParentTypeEnum.Building)
                command_assoc.CommandText = "INSERT INTO restrictions_buildings_assoc (id_building, id_restriction) VALUES (?, ?)";
            else
                if (ParentType == ParentTypeEnum.Premises)
                    command_assoc.CommandText = "INSERT INTO restrictions_premises_assoc (id_premises, id_restriction) VALUES (?, ?)";
                else
                {
                    MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
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
                command_assoc.Parameters.Add(connection.CreateParameter<int?>("id_object", id_parent));
                command_assoc.Parameters.Add(connection.CreateParameter<int?>("id_restriction", Convert.ToInt32(last_id.Rows[0][0])));
                connection.SqlModifyQuery(command_assoc);
                connection.SqlCommitTransaction();
                return Convert.ToInt32(last_id.Rows[0][0]);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить наименование реквизита в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Entities.Restriction restriction)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_restriction_type", restriction.id_restriction_type));
            command.Parameters.Add(connection.CreateParameter<string>("number", restriction.number));
            command.Parameters.Add(connection.CreateParameter<DateTime?>("date", restriction.date));
            command.Parameters.Add(connection.CreateParameter<string>("description", restriction.description));
            command.Parameters.Add(connection.CreateParameter<int?>("id_restriction", restriction.id_restriction));
            DbCommand command_assoc = connection.CreateCommand();
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить наименование реквизита в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_restriction", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить реквизит из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
