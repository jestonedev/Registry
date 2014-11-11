using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using Registry.Entities;
using System.Data.Odbc;
using System.Windows.Forms;
using System.Globalization;

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

        public static int Insert(Entities.OwnershipRight ownershipRight, ParentTypeEnum parentType, int idParent)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand command_assoc = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;

                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_ownership_right_type", ownershipRight.id_ownership_right_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("number", ownershipRight.number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date", ownershipRight.date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", ownershipRight.description));

                if (parentType == ParentTypeEnum.Building)
                    command_assoc.CommandText = "INSERT INTO ownership_buildings_assoc (id_building, id_ownership_right) VALUES (?, ?)";
                else
                    if (parentType == ParentTypeEnum.Premises)
                        command_assoc.CommandText = "INSERT INTO ownership_premises_assoc (id_premises, id_ownership_right) VALUES (?, ?)";
                    else
                    {
                        MessageBox.Show("Неизвестный родительский элемент. Если вы видите это сообщение, обратитесь к администратору",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return -1;
                    }
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        connection.SqlRollbackTransaction();
                        return -1;
                    }
                    command_assoc.Parameters.Add(DBConnection.CreateParameter<int?>("id_object", idParent));
                    command_assoc.Parameters.Add(DBConnection.CreateParameter<int?>("id_ownership_right", 
                        Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture)));
                    connection.SqlModifyQuery(command_assoc);
                    connection.SqlCommitTransaction();
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture,
                        "Не удалось добавить наименование ограничения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(Entities.OwnershipRight ownershipRight)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;

                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_ownership_right_type", ownershipRight.id_ownership_right_type));
                command.Parameters.Add(DBConnection.CreateParameter<string>("number", ownershipRight.number));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("date", ownershipRight.date));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", ownershipRight.description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_ownership_right", ownershipRight.id_ownership_right));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить наименование ограничения в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_ownership_right", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось удалить ограничение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
