﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using Registry.Entities;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class RestrictionTypesDataModel : DataModel
    {
        private static RestrictionTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restriction_types WHERE deleted <> 1";
        private static string deleteQuery = "UPDATE restriction_types SET deleted = 1 WHERE id_restriction_type = ?";
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
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_restriction_type"] };
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

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_restriction_type", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить наименование реквизита из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(RestrictionType restrictionType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (restrictionType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект типа реквизита", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("restriction_type", restrictionType.RestrictionTypeName));
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
                        "Не удалось добавить наименование реквизита в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(RestrictionType restrictionType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (restrictionType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект типа реквизита", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("restriction_type", restrictionType.RestrictionTypeName));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_restriction_type", restrictionType.IdRestrictionType));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить наименование реквизита в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
