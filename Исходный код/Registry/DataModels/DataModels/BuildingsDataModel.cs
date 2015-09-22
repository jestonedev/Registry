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
using System.Globalization;

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
                             , improvement, elevator, rubbish_chute, wear, state_date)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE buildings SET id_state = ?, id_structure_type = ?, id_street = ?, 
                            house = ?, floors = ?, num_premises = ?, num_rooms = ?,
                            num_apartments = ?, num_shared_apartments = ?, total_area = ?, living_area = ?, cadastral_num = ?, 
                            cadastral_cost = ?, balance_cost = ?, description = ?, startup_year = ?, 
                            improvement = ?, elevator = ?, rubbish_chute = ?, wear = ?, state_date = ? WHERE id_building = ?";
        private static string tableName = "buildings";

        private BuildingsDataModel(ToolStripProgressBar progressBar, int incrementor): base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_building"] };
            Table.Columns["id_state"].DefaultValue = 1;
            Table.Columns["num_premises"].DefaultValue = 0;
            Table.Columns["num_rooms"].DefaultValue = 0;
            Table.Columns["num_apartments"].DefaultValue = 0;
            Table.Columns["num_shared_apartments"].DefaultValue = 0;
            Table.Columns["startup_year"].DefaultValue = DateTime.Now.Year;
            Table.Columns["improvement"].DefaultValue = true;
            Table.Columns["elevator"].DefaultValue = false;
            Table.Columns["rubbish_chute"].DefaultValue = false;
            Table.Columns["living_area"].DefaultValue = 0;
            Table.Columns["total_area"].DefaultValue = 0;
            Table.Columns["floors"].DefaultValue = 5;
            Table.Columns["cadastral_cost"].DefaultValue = 0;
            Table.Columns["balance_cost"].DefaultValue = 0;
            Table.Columns["wear"].DefaultValue = 0;
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

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_building", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить здание из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(Building building)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (building == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект здания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", building.IdState));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_structure_type", building.IdStructureType));
                command.Parameters.Add(DBConnection.CreateParameter<string>("id_street", building.IdStreet));
                command.Parameters.Add(DBConnection.CreateParameter<string>("house", building.House));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("floors", building.Floors));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_premises", building.NumPremises));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_rooms", building.NumRooms));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_apartments", building.NumApartments));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_shared_apartments", building.NumSharedApartments));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("total_area", building.TotalArea));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("living_area", building.LivingArea));
                command.Parameters.Add(DBConnection.CreateParameter<string>("cadastral_num", building.CadastralNum));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("cadastral_cost", building.CadastralCost));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("balance_cost", building.BalanceCost));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", building.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("startup_year", building.StartupYear));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("improvement", building.Improvement));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("elevator", building.Elevator));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("rubbish_chute", building.RubbishChute));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("wear", building.Wear));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("state_date", building.StateDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_building", building.IdBuilding));

                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о здание. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(Building building)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
                command.CommandText = insertQuery;
                if (building == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект здания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state", building.IdState));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("id_structure_type", building.IdStructureType));
                command.Parameters.Add(DBConnection.CreateParameter<string>("id_street", building.IdStreet));
                command.Parameters.Add(DBConnection.CreateParameter<string>("house", building.House));
                command.Parameters.Add(DBConnection.CreateParameter<short?>("floors", building.Floors));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_premises", building.NumPremises));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_rooms", building.NumRooms));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_apartments", building.NumApartments));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("num_shared_apartments", building.NumSharedApartments));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("total_area", building.TotalArea));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("living_area", building.LivingArea));
                command.Parameters.Add(DBConnection.CreateParameter<string>("cadastral_num", building.CadastralNum));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("cadastral_cost", building.CadastralCost));
                command.Parameters.Add(DBConnection.CreateParameter<decimal?>("balance_cost", building.BalanceCost));
                command.Parameters.Add(DBConnection.CreateParameter<string>("description", building.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("startup_year", building.StartupYear));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("improvement", building.Improvement));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("elevator", building.Elevator));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("rubbish_chute", building.RubbishChute));
                command.Parameters.Add(DBConnection.CreateParameter<double?>("wear", building.Wear));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("state_date", building.StateDate));

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
                        "Не удалось добавить здание в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
