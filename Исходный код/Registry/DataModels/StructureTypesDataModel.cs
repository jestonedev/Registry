using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public sealed class StructureTypesDataModel : DataModel
    {
        private static StructureTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM structure_types";
        private static string deleteQuery = "DELETE FROM structure_types WHERE id_structure_type = ?";
        private static string insertQuery = @"INSERT INTO structure_types
                            (structure_type)
                            VALUES (?)";
        private static string updateQuery = @"UPDATE structure_types SET structure_type = ? WHERE id_structure_type = ?";
        private static string tableName = "structure_types";

        private StructureTypesDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_structure_type"] };
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static StructureTypesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new StructureTypesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }

        public int Delete(int id)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_structure_type = connection.CreateParameter();
            id_structure_type.ParameterName = "id_structure_type";
            id_structure_type.Value = id;
            command.Parameters.Add(id_structure_type);
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить наименование структуры из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(StructureType structureType)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            DbParameter structure_type = connection.CreateParameter();
            structure_type.ParameterName = "structure_type";
            structure_type.DbType = DbType.String;
            structure_type.Value = (structureType.structure_type == null) ? DBNull.Value : (Object)structureType.structure_type;
            command.Parameters.Add(structure_type);

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
                MessageBox.Show(String.Format("Не удалось добавить наименование структуры в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(StructureType structureType)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter structure_type = connection.CreateParameter();
            structure_type.ParameterName = "structure_type";
            structure_type.DbType = DbType.String;
            structure_type.Value = (structureType.structure_type == null) ? DBNull.Value : (Object)structureType.structure_type;
            command.Parameters.Add(structure_type);

            DbParameter id_structure_type = connection.CreateParameter();
            id_structure_type.ParameterName = "id_structure_type";
            id_structure_type.Value = structureType.id_structure_type;
            command.Parameters.Add(id_structure_type);

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить наименование структуры в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
