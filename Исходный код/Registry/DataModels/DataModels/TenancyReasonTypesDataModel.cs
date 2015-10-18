using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.Odbc;
using Registry.Entities;
using System.Data;
using System.Globalization;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public sealed class TenancyReasonTypesDataModel: DataModel
    {
        private static TenancyReasonTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_reason_types WHERE deleted <> 1";
        private static string deleteQuery = "UPDATE tenancy_reason_types SET deleted = 1 WHERE id_reason_type = ?";
        private static string insertQuery = @"INSERT INTO tenancy_reason_types
                            (reason_name, reason_template) VALUES (?, ?)";
        private static string updateQuery = @"UPDATE tenancy_reason_types SET reason_name = ?, reason_template = ? WHERE id_reason_type = ?";
        private static string tableName = "tenancy_reason_types";

        private TenancyReasonTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_reason_type"] };
        }

        public static TenancyReasonTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyReasonTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyReasonTypesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Update(ReasonType reasonType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (reasonType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность вида основания найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("reason_name", reasonType.ReasonName));
                command.Parameters.Add(DBConnection.CreateParameter("reason_template", reasonType.ReasonTemplate));
                command.Parameters.Add(DBConnection.CreateParameter("id_reason_type", reasonType.IdReasonType));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить запись о виде основания найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(ReasonType reasonType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (reasonType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность вида основания найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("reason_name", reasonType.ReasonName));
                command.Parameters.Add(DBConnection.CreateParameter("reason_template", reasonType.ReasonTemplate));

                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);

                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        connection.SqlRollbackTransaction();
                        return -1;
                    }
                    connection.SqlCommitTransaction();

                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить запись о виде основания найма в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_reason_type", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить вид основания из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
