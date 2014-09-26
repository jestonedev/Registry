using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public class BuildingsDataModel: DataModel
    {
        private static BuildingsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM buildings b WHERE deleted = 0";
        private static string deleteQuery = "UPDATE buildings SET deleted = 1 WHERE id_building = ?";
        private static string insertQuery = @"INSERT INTO buildings
                            (id_structure_type, id_street, house
                             , floors, num_premises, num_rooms, num_apartments
                             , num_shared_apartments, living_area, cadastral_num
                             , cadastral_cost, balance_cost, description, startup_year
                             , improvement, elevator)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        private static string updateQuery = @"UPDATE buildings SET id_structure_type = ?, id_street = ?, 
                            house = ?, floors = ?, num_premises = ?, num_rooms = ?,
                            num_apartments = ?, num_shared_apartments = ?, living_area = ?, cadastral_num = ?, 
                            cadastral_cost = ?, balance_cost = ?, description = ?, startup_year = ?, 
                            improvement = ?, elevator = ? WHERE id_building = ?";
        private static string tableName = "buildings";

        public bool EditingNewRecord { get; set; }


        private BuildingsDataModel()
        {
            EditingNewRecord = false;
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_building"] };
            table.Columns["num_premises"].DefaultValue = 0;
            table.Columns["num_rooms"].DefaultValue = 0;
            table.Columns["num_apartments"].DefaultValue = 0;
            table.Columns["num_shared_apartments"].DefaultValue = 0;
            table.Columns["startup_year"].DefaultValue = DateTime.Now.Year;
            table.Columns["improvement"].DefaultValue = true;
            table.Columns["elevator"].DefaultValue = false;
            table.Columns["living_area"].DefaultValue = 0;
            table.Columns["floors"].DefaultValue = 5;
            table.Columns["cadastral_cost"].DefaultValue = 0;
            table.Columns["balance_cost"].DefaultValue = 0;
            table.RowDeleted += new DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static BuildingsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new BuildingsDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }

        public int Delete(int id)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = deleteQuery;
            DbParameter id_building = connection.CreateParameter();
            id_building.ParameterName = "id_building";
            id_building.Value = id;
            command.Parameters.Add(id_building);
            try
            {
                return connection.SqlModifyQuery(command);  
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(String.Format("Не удалось удалить здание из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Update(Building building)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = updateQuery;

            DbParameter id_structure_type = connection.CreateParameter();
            id_structure_type.ParameterName = "id_structure_type";
            id_structure_type.Value = building.id_structure_type;
            command.Parameters.Add(id_structure_type);

            DbParameter id_street = connection.CreateParameter();
            id_street.ParameterName = "id_street";
            id_street.Value = building.id_street;
            command.Parameters.Add(id_street);

            DbParameter house = connection.CreateParameter();
            house.ParameterName = "house";
            house.Value = building.house;
            command.Parameters.Add(house);

            DbParameter floors = connection.CreateParameter();
            floors.ParameterName = "floors";
            floors.Value = building.floors;
            command.Parameters.Add(floors);

            DbParameter num_premises = connection.CreateParameter();
            num_premises.ParameterName = "num_premises";
            num_premises.Value = building.num_premises;
            command.Parameters.Add(num_premises);

            DbParameter num_rooms = connection.CreateParameter();
            num_rooms.ParameterName = "num_rooms";
            num_rooms.Value = building.num_rooms;
            command.Parameters.Add(num_rooms);

            DbParameter num_apartments = connection.CreateParameter();
            num_apartments.ParameterName = "num_apartments";
            num_apartments.Value = building.num_apartments;
            command.Parameters.Add(num_apartments);

            DbParameter num_shared_apartments = connection.CreateParameter();
            num_shared_apartments.ParameterName = "num_shared_apartments";
            num_shared_apartments.Value = building.num_shared_apartments;
            command.Parameters.Add(num_shared_apartments);

            DbParameter living_area = connection.CreateParameter();
            living_area.ParameterName = "living_area";
            living_area.Value = building.living_area;
            command.Parameters.Add(living_area);

            DbParameter cadastral_num = connection.CreateParameter();
            cadastral_num.ParameterName = "cadastral_num";
            cadastral_num.Value = (building.cadastral_num == null) ? DBNull.Value : (Object)building.cadastral_num;
            command.Parameters.Add(cadastral_num);

            DbParameter cadastral_cost = connection.CreateParameter();
            cadastral_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)cadastral_cost).Scale = 2;
            ((IDbDataParameter)cadastral_cost).Precision = 12;
            cadastral_cost.ParameterName = "cadastral_cost";
            cadastral_cost.Value = building.cadastral_cost;
            command.Parameters.Add(cadastral_cost);

            DbParameter balance_cost = connection.CreateParameter();
            balance_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)balance_cost).Scale = 2;
            ((IDbDataParameter)balance_cost).Precision = 12;
            balance_cost.ParameterName = "balance_cost";
            balance_cost.Value = building.balance_cost;
            command.Parameters.Add(balance_cost);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            description.Value = (building.description == null) ? DBNull.Value : (Object)building.description;
            command.Parameters.Add(description);

            DbParameter startup_year = connection.CreateParameter();
            startup_year.ParameterName = "startup_year";
            startup_year.Value = building.startup_year;
            command.Parameters.Add(startup_year);

            DbParameter improvement = connection.CreateParameter();
            improvement.ParameterName = "improvement";
            improvement.Value = building.improvement;
            command.Parameters.Add(improvement);

            DbParameter elevator = connection.CreateParameter();
            elevator.ParameterName = "elevator";
            elevator.Value = building.elevator;
            command.Parameters.Add(elevator);

            DbParameter id_building = connection.CreateParameter();
            id_building.ParameterName = "id_building";
            id_building.Value = building.id_building;
            command.Parameters.Add(id_building);

            try
            {
                return connection.SqlModifyQuery(command);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(String.Format("Не удалось изменить данные о здание. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public int Insert(Building building)
        {
            DbCommand command = connection.CreateCommand();
            DbCommand last_id_command = connection.CreateCommand();
            last_id_command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandText = insertQuery;
            
            DbParameter id_structure_type = connection.CreateParameter();
            id_structure_type.ParameterName = "id_structure_type";
            id_structure_type.Value = building.id_structure_type;
            command.Parameters.Add(id_structure_type);

            DbParameter id_street = connection.CreateParameter();
            id_street.ParameterName = "id_street";
            id_street.Value = building.id_street;
            command.Parameters.Add(id_street);

            DbParameter house = connection.CreateParameter();
            house.ParameterName = "house";
            house.Value = building.house;
            command.Parameters.Add(house);

            DbParameter floors = connection.CreateParameter();
            floors.ParameterName = "floors";
            floors.Value = building.floors;
            command.Parameters.Add(floors);

            DbParameter num_premises = connection.CreateParameter();
            num_premises.ParameterName = "num_premises";
            num_premises.Value = building.num_premises;
            command.Parameters.Add(num_premises);

            DbParameter num_rooms = connection.CreateParameter();
            num_rooms.ParameterName = "num_rooms";
            num_rooms.Value = building.num_rooms;
            command.Parameters.Add(num_rooms);

            DbParameter num_apartments = connection.CreateParameter();
            num_apartments.ParameterName = "num_apartments";
            num_apartments.Value = building.num_apartments;
            command.Parameters.Add(num_apartments);

            DbParameter num_shared_apartments = connection.CreateParameter();
            num_shared_apartments.ParameterName = "num_shared_apartments";
            num_shared_apartments.Value = building.num_shared_apartments;
            command.Parameters.Add(num_shared_apartments);

            DbParameter living_area = connection.CreateParameter();
            living_area.ParameterName = "living_area";
            living_area.Value = building.living_area;
            command.Parameters.Add(living_area);

            DbParameter cadastral_num = connection.CreateParameter();
            cadastral_num.ParameterName = "cadastral_num";
            cadastral_num.DbType = DbType.String;
            cadastral_num.Value = (building.cadastral_num == null) ? DBNull.Value : (Object)building.cadastral_num;
            command.Parameters.Add(cadastral_num);

            DbParameter cadastral_cost = connection.CreateParameter();
            cadastral_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)cadastral_cost).Scale = 2;
            ((IDbDataParameter)cadastral_cost).Precision = 12;
            cadastral_cost.ParameterName = "cadastral_cost";
            cadastral_cost.Value = building.cadastral_cost;
            command.Parameters.Add(cadastral_cost);

            DbParameter balance_cost = connection.CreateParameter();
            balance_cost.DbType = DbType.Decimal;
            ((IDbDataParameter)balance_cost).Scale = 2;
            ((IDbDataParameter)balance_cost).Precision = 12;
            balance_cost.ParameterName = "balance_cost";
            balance_cost.Value = building.balance_cost;
            command.Parameters.Add(balance_cost);

            DbParameter description = connection.CreateParameter();
            description.ParameterName = "description";
            cadastral_num.DbType = DbType.String;
            description.Value = (building.description == null) ? DBNull.Value : (Object)building.description;
            command.Parameters.Add(description);

            DbParameter startup_year = connection.CreateParameter();
            startup_year.ParameterName = "startup_year";
            startup_year.Value = building.startup_year;
            command.Parameters.Add(startup_year);

            DbParameter improvement = connection.CreateParameter();
            improvement.ParameterName = "improvement";
            improvement.Value = building.improvement;
            command.Parameters.Add(improvement);

            DbParameter elevator = connection.CreateParameter();
            elevator.ParameterName = "elevator";
            elevator.Value = building.elevator;
            command.Parameters.Add(elevator);

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
            catch (InvalidOperationException e)
            {
                connection.SqlRollbackTransaction();
                MessageBox.Show(String.Format("Не удалось добавить здание в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
