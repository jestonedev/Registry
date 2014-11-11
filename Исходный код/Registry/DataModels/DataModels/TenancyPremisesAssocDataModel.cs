using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.Entities;
using System.Data.Common;
using System.Data.Odbc;
using System.Data;
using System.Globalization;

namespace Registry.DataModels
{
    public sealed class TenancyPremisesAssocDataModel: DataModel
    {
        private static TenancyPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_premises_assoc WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_premises_assoc SET deleted = 1 WHERE id_assoc = ?";
        private static string insertQuery = @"INSERT INTO tenancy_premises_assoc (id_premises, id_process, rent_total_area, rent_living_area) VALUES (?,?,?,?)";
        private static string updateQuery = @"UPDATE tenancy_premises_assoc SET id_premises = ?, id_process = ?, rent_total_area = ?, rent_living_area = ? WHERE id_assoc = ?";
        private static string tableName = "tenancy_premises_assoc";

        private TenancyPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_assoc"] };
        }

        public static TenancyPremisesAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyPremisesAssocDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Insert(TenancyObject tenancyObject)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (tenancyObject == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на сущность объекта найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises", tenancyObject.id_object));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyObject.id_process));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("rent_total_area", tenancyObject.rent_total_area));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("rent_living_area", tenancyObject.rent_living_area));
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    connection.SqlCommitTransaction();
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return -1;
                    }
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.CurrentCulture);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось добавить связь помещения с процессом найма в базу данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }

        public static int Update(TenancyObject tenancyObject)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (tenancyObject == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на сущность объекта найма", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises", tenancyObject.id_object));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_process", tenancyObject.id_process));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("rent_total_area", tenancyObject.rent_total_area));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("rent_living_area", tenancyObject.rent_living_area));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", tenancyObject.id_assoc));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
                        "Не удалось изменить связь помещения с процессом найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
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
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.CurrentCulture,
                        "Не удалось удалить связь помещения с процесссом найма из базы данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return -1;
                }
            }
        }
    }
}
