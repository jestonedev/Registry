using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.Entities;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public sealed class TenancyBuildingsAssocDataModel: DataModel
    {
        private static TenancyBuildingsAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_buildings_assoc WHERE deleted = 0";
        private static string deleteQuery = "UPDATE tenancy_buildings_assoc SET deleted = 1 WHERE id_assoc = ?";
        private static string insertQuery = @"INSERT INTO tenancy_buildings_assoc (id_building, id_contract, beds) VALUES (?,?,?)";
        private static string updateQuery = @"UPDATE tenancy_buildings_assoc SET id_building = ?, id_contract = ?, beds = ? WHERE id_assoc = ?";
        private static string tableName = "tenancy_buildings_assoc";

        private TenancyBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_assoc"] };
        }

        public static TenancyBuildingsAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyBuildingsAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyBuildingsAssocDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Insert(TenancyObject tenancyObject)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_building", tenancyObject.id_object));
            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", tenancyObject.id_contract));
            command.Parameters.Add(connection.CreateParameter<string>("beds", tenancyObject.beds));
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
                MessageBox.Show(String.Format("Не удалось добавить связь здания с процессом найма в базу данных. Подробная ошибка: {0}", 
                    e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(TenancyObject tenancyObject)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_building", tenancyObject.id_object));
            command.Parameters.Add(connection.CreateParameter<int?>("id_contract", tenancyObject.id_contract));
            command.Parameters.Add(connection.CreateParameter<string>("beds", tenancyObject.beds));
            command.Parameters.Add(connection.CreateParameter<int?>("id_assoc", tenancyObject.id_assoc));
            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось изменить связь здания с процессом найма в базе данных. Подробная ошибка: {0}", e.Message), "Ошибка",
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
                MessageBox.Show(String.Format("Не удалось удалить связь здания с процесссом найма из базы данных. Подробная ошибка: {0}", 
                    e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
