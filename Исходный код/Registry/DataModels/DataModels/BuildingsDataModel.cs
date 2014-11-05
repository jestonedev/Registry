using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;
using Registry.Entities;
using System.Data.Odbc;
using System.Threading;

namespace Registry.DataModels
{
    public sealed class BuildingsDataModel : DataModel
    {
        private static BuildingsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM buildings b WHERE deleted = 0";
        private static string deleteQuery = "UPDATE buildings SET deleted = 1 WHERE id_building = ?";
        private static string insertQuery = @"INSERT INTO buildings
                            (id_state, id_structure_type, id_street, house
                             , floors, num_premises, num_rooms, num_apartments
                             , num_shared_apartments, total_area, living_area, cadastral_num
                             , cadastral_cost, balance_cost, description, startup_year
                             , improvement, elevator)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE buildings SET id_state = ?, id_structure_type = ?, id_street = ?, 
                            house = ?, floors = ?, num_premises = ?, num_rooms = ?,
                            num_apartments = ?, num_shared_apartments = ?, total_area = ?, living_area = ?, cadastral_num = ?, 
                            cadastral_cost = ?, balance_cost = ?, description = ?, startup_year = ?, 
                            improvement = ?, elevator = ? WHERE id_building = ?";
        private static string tableName = "buildings";

        public bool EditingNewRecord { get; set; }

        private BuildingsDataModel(ToolStripProgressBar progressBar, int incrementor): base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_building"] };
            table.Columns["id_state"].DefaultValue = 1;
            table.Columns["num_premises"].DefaultValue = 0;
            table.Columns["num_rooms"].DefaultValue = 0;
            table.Columns["num_apartments"].DefaultValue = 0;
            table.Columns["num_shared_apartments"].DefaultValue = 0;
            table.Columns["startup_year"].DefaultValue = DateTime.Now.Year;
            table.Columns["improvement"].DefaultValue = true;
            table.Columns["elevator"].DefaultValue = false;
            table.Columns["living_area"].DefaultValue = 0;
            table.Columns["total_area"].DefaultValue = 0;
            table.Columns["floors"].DefaultValue = 5;
            table.Columns["cadastral_cost"].DefaultValue = 0;
            table.Columns["balance_cost"].DefaultValue = 0;
        }

        public static BuildingsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static BuildingsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new BuildingsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public int Delete(int id)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            command.Parameters.Add(connection.CreateParameter<int?>("id_building", id));
            try
            {
                return connection.SqlModifyQuery(command);  
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить здание из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Building building)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_state", building.id_state));
            command.Parameters.Add(connection.CreateParameter<int?>("id_structure_type", building.id_structure_type));
            command.Parameters.Add(connection.CreateParameter<string>("id_street", building.id_street));
            command.Parameters.Add(connection.CreateParameter<string>("house", building.house));
            command.Parameters.Add(connection.CreateParameter<short?>("floors", building.floors));
            command.Parameters.Add(connection.CreateParameter<int?>("num_premises", building.num_premises));
            command.Parameters.Add(connection.CreateParameter<int?>("num_rooms", building.num_rooms));
            command.Parameters.Add(connection.CreateParameter<int?>("num_apartments", building.num_apartments));
            command.Parameters.Add(connection.CreateParameter<int?>("num_shared_apartments", building.num_shared_apartments));
            command.Parameters.Add(connection.CreateParameter<double?>("total_area", building.total_area));
            command.Parameters.Add(connection.CreateParameter<double?>("living_area", building.living_area));
            command.Parameters.Add(connection.CreateParameter<string>("cadastral_num", building.cadastral_num));
            command.Parameters.Add(connection.CreateParameter<decimal?>("cadastral_cost", building.cadastral_cost));
            command.Parameters.Add(connection.CreateParameter<decimal?>("balance_cost", building.balance_cost));
            command.Parameters.Add(connection.CreateParameter<string>("description", building.description));
            command.Parameters.Add(connection.CreateParameter<int?>("startup_year", building.startup_year));
            command.Parameters.Add(connection.CreateParameter<bool?>("improvement", building.improvement));
            command.Parameters.Add(connection.CreateParameter<bool?>("elevator", building.elevator));
            command.Parameters.Add(connection.CreateParameter<int?>("id_building", building.id_building));

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (OdbcException e)
            {
                MessageBox.Show(String.Format("Не удалось изменить данные о здание. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Building building)
        {
            DBConnection connection = new DBConnection();
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;

            command.Parameters.Add(connection.CreateParameter<int?>("id_state", building.id_state));
            command.Parameters.Add(connection.CreateParameter<int?>("id_structure_type", building.id_structure_type));
            command.Parameters.Add(connection.CreateParameter<string>("id_street", building.id_street));
            command.Parameters.Add(connection.CreateParameter<string>("house", building.house));
            command.Parameters.Add(connection.CreateParameter<short?>("floors", building.floors));
            command.Parameters.Add(connection.CreateParameter<int?>("num_premises", building.num_premises));
            command.Parameters.Add(connection.CreateParameter<int?>("num_rooms", building.num_rooms));
            command.Parameters.Add(connection.CreateParameter<int?>("num_apartments", building.num_apartments));
            command.Parameters.Add(connection.CreateParameter<int?>("num_shared_apartments", building.num_shared_apartments));
            command.Parameters.Add(connection.CreateParameter<double?>("total_area", building.total_area));
            command.Parameters.Add(connection.CreateParameter<double?>("living_area", building.living_area));
            command.Parameters.Add(connection.CreateParameter<string>("cadastral_num", building.cadastral_num));
            command.Parameters.Add(connection.CreateParameter<decimal?>("cadastral_cost", building.cadastral_cost));
            command.Parameters.Add(connection.CreateParameter<decimal?>("balance_cost", building.balance_cost));
            command.Parameters.Add(connection.CreateParameter<string>("description", building.description));
            command.Parameters.Add(connection.CreateParameter<int?>("startup_year", building.startup_year));
            command.Parameters.Add(connection.CreateParameter<bool?>("improvement", building.improvement));
            command.Parameters.Add(connection.CreateParameter<bool?>("elevator", building.elevator));

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
                MessageBox.Show(String.Format("Не удалось добавить здание в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
