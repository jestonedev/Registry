using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Registry.Entities;
using System.Windows.Forms;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public sealed class SubPremisesDataModel : DataModel
    {
        private static SubPremisesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM sub_premises WHERE deleted = 0";
        private static string deleteQuery = "UPDATE sub_premises SET deleted = 1 WHERE id_sub_premises = ?";
        private static string insertQuery = @"INSERT INTO sub_premises
                            (id_premises, sub_premises_num, total_area)
                            VALUES (?, ?, ?)";
        private static string updateQuery = @"UPDATE sub_premises SET id_premises = ?, sub_premises_num = ?, 
                            total_area = ? WHERE id_sub_premises = ?";

        private static string tableName = "sub_premises";

        private SubPremisesDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_sub_premises"] };
            table.Columns["total_area"].DefaultValue = 0;
            table.Columns["deleted"].DefaultValue = 0;
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static SubPremisesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new SubPremisesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }

        public int Delete(int id)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_sub_premises = connection.CreateParameter();
            id_sub_premises.ParameterName = "id_sub_premises";
            id_sub_premises.Value = id;
            command.Parameters.Add(id_sub_premises);
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить комнату из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(SubPremise subPremise)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            DbParameter id_premise = connection.CreateParameter();
            id_premise.ParameterName = "id_premises";
            id_premise.Value = (subPremise.id_premises == null) ? DBNull.Value : (Object)subPremise.id_premises;
            command.Parameters.Add(id_premise);

            DbParameter sub_premises_num = connection.CreateParameter();
            sub_premises_num.ParameterName = "sub_premises_num";
            sub_premises_num.DbType = DbType.String;
            sub_premises_num.Value = (subPremise.sub_premises_num == null) ? DBNull.Value : (Object)subPremise.sub_premises_num;
            command.Parameters.Add(sub_premises_num);

            DbParameter total_area = connection.CreateParameter();
            total_area.ParameterName = "total_area";
            total_area.Value = subPremise.total_area;
            command.Parameters.Add(total_area);

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
                MessageBox.Show(String.Format("Не удалось добавить комнату в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(SubPremise subPremise)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter id_premise = connection.CreateParameter();
            id_premise.ParameterName = "id_premise";
            id_premise.Value = subPremise.id_premises;
            command.Parameters.Add(id_premise);

            DbParameter sub_premises_num = connection.CreateParameter();
            sub_premises_num.ParameterName = "sub_premises_num";
            sub_premises_num.DbType = DbType.String;
            sub_premises_num.Value = (subPremise.sub_premises_num == null) ? DBNull.Value : (Object)subPremise.sub_premises_num;
            command.Parameters.Add(sub_premises_num);

            DbParameter total_area = connection.CreateParameter();
            total_area.ParameterName = "total_area";
            total_area.Value = subPremise.total_area;
            command.Parameters.Add(total_area);

            DbParameter id_sub_premise = connection.CreateParameter();
            id_sub_premise.ParameterName = "id_sub_premise";
            id_sub_premise.Value = subPremise.id_sub_premises;
            command.Parameters.Add(id_sub_premise);

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить комнату в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
