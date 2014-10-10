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
    public sealed class OwnershipsRightsDataModel : DataModel
    {
        private static OwnershipsRightsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_rights";
        private static string deleteQuery = "UPDATE ownership_rights SET deleted = 1 WHERE id_ownership_right = ?";
        private static string insertQuery = @"INSERT INTO ownership_rights
                            (id_ownership_right_type, number, `date`, description)
                            VALUES (?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE ownership_rights SET id_ownership_right_type = ?,
                            number= ?, `date` = ?, description = ? WHERE id_ownership_right = ?";
        private static string tableName = "ownership_rights";

        private OwnershipsRightsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_ownership_right"] };
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static OwnershipsRightsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static OwnershipsRightsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new OwnershipsRightsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Insert(Entities.OwnershipRight ownershipRight, ParentTypeEnum ParentType, int id_parent)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            DbParameter id_ownership_right_type = connection.CreateParameter();
            id_ownership_right_type.ParameterName = "id_ownership_right_type";
            id_ownership_right_type.Value = (ownershipRight.id_ownership_right_type == null) ? DBNull.Value : (Object)ownershipRight.id_ownership_right_type;
            command.Parameters.Add(id_ownership_right_type);

            DbParameter number = connection.CreateParameter();
            number.ParameterName = "number";
            number.DbType = DbType.String;
            number.Value = (ownershipRight.number == null) ? DBNull.Value : (Object)ownershipRight.number;
            command.Parameters.Add(number);

            DbParameter date = connection.CreateParameter();
            date.ParameterName = "date";
            date.Value = (ownershipRight.date == null) ? DBNull.Value : (Object)ownershipRight.date;
            command.Parameters.Add(date);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            description.Value = (ownershipRight.description == null) ? DBNull.Value : (Object)ownershipRight.description;
            command.Parameters.Add(description);

            DbCommand command_assoc = connection.CreateCommand();
            if (ParentType == ParentTypeEnum.Building)
                command_assoc.CommandText = "INSERT INTO ownership_buildings_assoc (id_building, id_ownership_right) VALUES (?, ?)";
            else
            if (ParentType == ParentTypeEnum.Premises)
                command_assoc.CommandText = "INSERT INTO ownership_premises_assoc (id_premises, id_ownership_right) VALUES (?, ?)";
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

                DbParameter id_object = connection.CreateParameter();
                id_object.ParameterName = "id_object";
                id_object.Value = id_parent;
                command_assoc.Parameters.Add(id_object);

                DbParameter id_ownership_right = connection.CreateParameter();
                id_ownership_right.ParameterName = "id_ownership_right";
                id_ownership_right.Value = last_id.Rows[0][0];
                command_assoc.Parameters.Add(id_ownership_right);

                connection.SqlModifyQuery(command_assoc);


                connection.SqlCommitTransaction();

                return Convert.ToInt32(last_id.Rows[0][0]);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить наименование ограничения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Entities.OwnershipRight ownershipRight)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter id_ownership_right_type = connection.CreateParameter();
            id_ownership_right_type.ParameterName = "id_ownership_right_type";
            id_ownership_right_type.Value = (ownershipRight.id_ownership_right_type == null) ? DBNull.Value : (Object)ownershipRight.id_ownership_right_type;
            command.Parameters.Add(id_ownership_right_type);

            DbParameter number = connection.CreateParameter();
            number.ParameterName = "number";
            number.DbType = DbType.String;
            number.Value = (ownershipRight.number == null) ? DBNull.Value : (Object)ownershipRight.number;
            command.Parameters.Add(number);

            DbParameter date = connection.CreateParameter();
            date.ParameterName = "date";
            date.Value = (ownershipRight.date == null) ? DBNull.Value : (Object)ownershipRight.date;
            command.Parameters.Add(date);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            description.Value = (ownershipRight.description == null) ? DBNull.Value : (Object)ownershipRight.description;
            command.Parameters.Add(description);

            DbParameter id_ownership_right = connection.CreateParameter();
            id_ownership_right.ParameterName = "id_ownership_right";
            id_ownership_right.Value = (ownershipRight.id_ownership_right == null) ? DBNull.Value : (Object)ownershipRight.id_ownership_right;
            command.Parameters.Add(id_ownership_right);

            DbCommand command_assoc = connection.CreateCommand();
            
            try
            {
                return connection.SqlModifyQuery(command);               
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить наименование ограничения в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Delete(int id)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_ownership_right = connection.CreateParameter();
            id_ownership_right.ParameterName = "id_ownership_right";
            id_ownership_right.Value = id;
            command.Parameters.Add(id_ownership_right);
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить ограничение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
