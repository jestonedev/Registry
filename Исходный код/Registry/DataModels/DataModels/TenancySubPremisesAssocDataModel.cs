using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Odbc;
using System.Data.Common;
using System.Globalization;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public sealed class TenancySubPremisesAssocDataModel: DataModel
    {
        private static TenancySubPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_sub_premises_assoc WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_sub_premises_assoc SET deleted = 1 WHERE id_assoc = ?";
        private static string insertQuery = @"INSERT INTO tenancy_sub_premises_assoc (id_sub_premises, id_process, rent_total_area) VALUES (?,?,?)";
        private static string updateQuery = @"UPDATE tenancy_sub_premises_assoc SET id_sub_premises = ?, id_process = ?, rent_total_area = ? WHERE id_assoc = ?";
        private static string tableName = "tenancy_sub_premises_assoc";

        private TenancySubPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_assoc"] };
        }

        public static TenancySubPremisesAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancySubPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancySubPremisesAssocDataModel(progressBar, incrementor);
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
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("id_sub_premises", tenancyObject.IdObject));
                command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
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
                        "Не удалось добавить связь комнаты с процессом найма в базу данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("id_sub_premises", tenancyObject.IdObject));
                command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
                command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
                command.Parameters.Add(DBConnection.CreateParameter("id_assoc", tenancyObject.IdAssoc));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить связь комнаты с процессом найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
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
                        "Не удалось удалить связь комнаты с процесссом найма из базы данных. Подробная ошибка: {0}",
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
