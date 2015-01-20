﻿using Registry.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class ResettleBuildingsFromAssocDataModel: DataModel
    {
        private static ResettleBuildingsFromAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM resettle_buildings_from_assoc WHERE deleted = 0";
        private static string deleteQuery = "UPDATE resettle_buildings_from_assoc SET deleted = 1 WHERE id_assoc = ?";
        private static string insertQuery = @"INSERT INTO resettle_buildings_from_assoc (id_building, id_process) VALUES (?,?)";
        private static string updateQuery = @"UPDATE resettle_buildings_from_assoc SET id_building = ?, id_process = ? WHERE id_assoc = ?";
        private static string tableName = "resettle_buildings_from_assoc";

        private ResettleBuildingsFromAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_assoc"] };
        }

        public static ResettleBuildingsFromAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ResettleBuildingsFromAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ResettleBuildingsFromAssocDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(ResettleObject resettleObject)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (resettleObject == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на ассоциативный объект процесса переселения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_building", resettleObject.IdObject));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", resettleObject.IdProcess));
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
                        "Не удалось добавить связь здания с процессом переселения в базу данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(ResettleObject resettleObject)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (resettleObject == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на ассоциативный объект процесса переселения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_building", resettleObject.IdObject));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", resettleObject.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", resettleObject.IdAssoc));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить связь здания с процессом переселения в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить связь здания с процесссом переселения из базы данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
