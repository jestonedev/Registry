using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels
{
    public sealed class OwnershipRightTypesDataModel : DataModel
    {
        private static OwnershipRightTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_right_types";
        private static string deleteQuery = "DELETE FROM ownership_right_types WHERE id_ownership_right_type = ?";
        private static string insertQuery = @"INSERT INTO ownership_right_types
                            (ownership_right_type)
                            VALUES (?)";
        private static string updateQuery = @"UPDATE ownership_right_types SET ownership_right_type = ? WHERE id_ownership_right_type = ?";
        private static string tableName = "ownership_right_types";

        private OwnershipRightTypesDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_ownership_right_type"] };
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static OwnershipRightTypesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new OwnershipRightTypesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }

        public int Delete(int id)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_ownership_right_type = connection.CreateParameter();
            id_ownership_right_type.ParameterName = "id_ownership_right_type";
            id_ownership_right_type.Value = id;
            command.Parameters.Add(id_ownership_right_type);
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить наименование ограничения из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(OwnershipRightType ownershipRightType)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            DbParameter ownership_right_type = connection.CreateParameter();
            ownership_right_type.ParameterName = "structure_type";
            ownership_right_type.DbType = DbType.String;
            ownership_right_type.Value = (ownershipRightType.ownership_right_type == null) ? DBNull.Value : (Object)ownershipRightType.ownership_right_type;
            command.Parameters.Add(ownership_right_type);
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
                MessageBox.Show(String.Format("Не удалось добавить наименование ограничения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(OwnershipRightType ownershipRightType)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter ownership_right_type = connection.CreateParameter();
            ownership_right_type.ParameterName = "ownership_right_type";
            ownership_right_type.DbType = DbType.String;
            ownership_right_type.Value = (ownershipRightType.ownership_right_type == null) ? DBNull.Value : (Object)ownershipRightType.ownership_right_type;
            command.Parameters.Add(ownership_right_type);

            DbParameter id_ownership_right_type = connection.CreateParameter();
            id_ownership_right_type.ParameterName = "id_ownership_right_type";
            id_ownership_right_type.Value = ownershipRightType.id_ownership_right_type;
            command.Parameters.Add(id_ownership_right_type);
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
    }
}
