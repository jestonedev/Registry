using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.Entities;
using System.Data.Odbc;
using System.Data.Common;

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
            table.PrimaryKey = new DataColumn[] { table.Columns["id_assoc"] };
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

        public int Insert(TenancyObject tenancyObject)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_sub_premises", tenancyObject.id_object));
            command.Parameters.Add(connection.CreateParameter<int?>("id_process", tenancyObject.id_process));
            command.Parameters.Add(connection.CreateParameter<double?>("rent_total_area", tenancyObject.rent_total_area));
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
                MessageBox.Show(String.Format("Не удалось добавить связь комнату с процессом найма в базу данных. Подробная ошибка: {0}",
                    e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(TenancyObject tenancyObject)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_sub_premises", tenancyObject.id_object));
            command.Parameters.Add(connection.CreateParameter<int?>("id_process", tenancyObject.id_process));
            command.Parameters.Add(connection.CreateParameter<double?>("rent_total_area", tenancyObject.rent_total_area));
            command.Parameters.Add(connection.CreateParameter<int?>("id_assoc", tenancyObject.id_assoc));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить связь комнату с процессом найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_assoc", id));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить связь комнату с процесссом найма из базы данных. Подробная ошибка: {0}",
                    e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
