﻿using System;
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
    public sealed class RestrictionTypesDataModel : DataModel
    {
        private static RestrictionTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restriction_types";
        private static string deleteQuery = "DELETE FROM restriction_types WHERE id_restriction_type = ?";
        private static string insertQuery = @"INSERT INTO restriction_types
                            (restriction_type)
                            VALUES (?)";
        private static string updateQuery = @"UPDATE restriction_types SET restriction_type = ? WHERE id_restriction_type = ?";
        private static string tableName = "restriction_types";

        private RestrictionTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_restriction_type"] };
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static RestrictionTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static RestrictionTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new RestrictionTypesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Delete(int id)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_restriction_type = connection.CreateParameter();
            id_restriction_type.ParameterName = "id_restriction_type";
            id_restriction_type.Value = id;
            command.Parameters.Add(id_restriction_type);
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить наименование реквизита из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(RestrictionType restrictionType)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            DbParameter restriction_type = connection.CreateParameter();
            restriction_type.ParameterName = "restriction_type";
            restriction_type.DbType = DbType.String;
            restriction_type.Value = (restrictionType.restriction_type == null) ? DBNull.Value : (Object)restrictionType.restriction_type;
            command.Parameters.Add(restriction_type);

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
                MessageBox.Show(String.Format("Не удалось добавить наименование реквизита в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(RestrictionType restrictionType)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter restriction_type = connection.CreateParameter();
            restriction_type.ParameterName = "restriction_type";
            restriction_type.DbType = DbType.String;
            restriction_type.Value = (restrictionType.restriction_type == null) ? DBNull.Value : (Object)restrictionType.restriction_type;
            command.Parameters.Add(restriction_type);

            DbParameter id_restriction_type = connection.CreateParameter();
            id_restriction_type.ParameterName = "id_restriction_type";
            id_restriction_type.Value = restrictionType.id_restriction_type;
            command.Parameters.Add(id_restriction_type);

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
    }
}
