using System;
using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class BuildingsDataModel : DataModel
    {
        private static BuildingsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM buildings b WHERE deleted = 0";
        private const string TableName = "buildings";

        private BuildingsDataModel(ToolStripProgressBar progressBar, int incrementor): base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;      
        }

        public static BuildingsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new BuildingsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_building"] };
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
            Table.Columns["plumbing"].DefaultValue = true;
            Table.Columns["hot_water_supply"].DefaultValue = true;
            Table.Columns["canalization"].DefaultValue = true;
            Table.Columns["electricity"].DefaultValue = true;
            Table.Columns["radio_network"].DefaultValue = false;
            Table.Columns["id_heating_type"].DefaultValue = 4;
            Table.Columns["BTI_rooms"].DefaultValue = DBNull.Value;
            Table.Columns["id_structure_type"].DefaultValue = 1;
        }

        protected override void ConfigureRelations()
        {
            AddRelation("kladr", "id_street", TableName, "id_street");
            AddRelation("object_states", "id_state", TableName, "id_state");
            AddRelation("structure_types", "id_structure_type", TableName, "id_structure_type");
            AddRelation(TableName, "id_building", "premises", "id_building");
            AddRelation(TableName, "id_building", "restrictions_buildings_assoc", "id_building");
            AddRelation(TableName, "id_building", "ownership_buildings_assoc", "id_building");
            AddRelation(TableName, "id_building", "funds_buildings_assoc", "id_building");
            AddRelation(TableName, "id_building", "tenancy_buildings_assoc", "id_building");
            AddRelation(TableName, "id_building", "resettle_buildings_from_assoc", "id_building");
            AddRelation(TableName, "id_building", "resettle_buildings_to_assoc", "id_building");
            AddRelation("heating_type", "id_heating_type", TableName, "id_heating_type");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE buildings SET deleted = 1 WHERE id_building = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_building", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE buildings SET id_state = ?, id_structure_type = ?, id_street = ?, 
                            house = ?, floors = ?, num_premises = ?, num_rooms = ?,
                            num_apartments = ?, num_shared_apartments = ?, total_area = ?, living_area = ?, cadastral_num = ?, 
                            cadastral_cost = ?, balance_cost = ?, description = ?, startup_year = ?, 
                            improvement = ?, elevator = ?, rubbish_chute = ?, wear = ?, state_date = ?, plumbing = ?,
                             hot_water_supply = ?, canalization = ?, electricity = ?, radio_network = ?, id_heating_type = ?, BTI_rooms = ?, housing_cooperative = ?
                            WHERE id_building = ?";
            var building = (Building) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_state", building.IdState));
            command.Parameters.Add(DBConnection.CreateParameter("id_structure_type", building.IdStructureType));
            command.Parameters.Add(DBConnection.CreateParameter("id_street", building.IdStreet));
            command.Parameters.Add(DBConnection.CreateParameter("house", building.House));
            command.Parameters.Add(DBConnection.CreateParameter("floors", building.Floors));
            command.Parameters.Add(DBConnection.CreateParameter("num_premises", building.NumPremises));
            command.Parameters.Add(DBConnection.CreateParameter("num_rooms", building.NumRooms));
            command.Parameters.Add(DBConnection.CreateParameter("num_apartments", building.NumApartments));
            command.Parameters.Add(DBConnection.CreateParameter("num_shared_apartments", building.NumSharedApartments));
            command.Parameters.Add(DBConnection.CreateParameter("total_area", building.TotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("living_area", building.LivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_num", building.CadastralNum));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_cost", building.CadastralCost));
            command.Parameters.Add(DBConnection.CreateParameter("balance_cost", building.BalanceCost));
            command.Parameters.Add(DBConnection.CreateParameter("description", building.Description));
            command.Parameters.Add(DBConnection.CreateParameter("startup_year", building.StartupYear));
            command.Parameters.Add(DBConnection.CreateParameter("improvement", building.Improvement));
            command.Parameters.Add(DBConnection.CreateParameter("elevator", building.Elevator));
            command.Parameters.Add(DBConnection.CreateParameter("rubbish_chute", building.RubbishChute));
            command.Parameters.Add(DBConnection.CreateParameter("wear", building.Wear));
            command.Parameters.Add(DBConnection.CreateParameter("state_date", building.StateDate));          
            command.Parameters.Add(DBConnection.CreateParameter("plumbing", building.Plumbing));
            command.Parameters.Add(DBConnection.CreateParameter("hot_water_supply", building.HotWaterSupply));
            command.Parameters.Add(DBConnection.CreateParameter("canalization", building.Canalization));
            command.Parameters.Add(DBConnection.CreateParameter("electricity", building.Electricity));
            command.Parameters.Add(DBConnection.CreateParameter("radio_network", building.RadioNetwork));
            command.Parameters.Add(DBConnection.CreateParameter("id_heating_type", building.IdHeatingType));
            command.Parameters.Add(DBConnection.CreateParameter("BTI_rooms", building.RoomsBTI));
            command.Parameters.Add(DBConnection.CreateParameter("housing_cooperative", building.HousingCooperative));
            command.Parameters.Add(DBConnection.CreateParameter("id_building", building.IdBuilding));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO buildings
                            (id_state, id_structure_type, id_street, house
                             , floors, num_premises, num_rooms, num_apartments
                             , num_shared_apartments, total_area, living_area, cadastral_num
                             , cadastral_cost, balance_cost, description, startup_year
                             , improvement, elevator, rubbish_chute, wear, state_date,plumbing,hot_water_supply,
                                canalization,electricity,radio_network,id_heating_type,BTI_rooms,housing_cooperative)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,?,?)";
            var building = (Building)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_state", building.IdState));
            command.Parameters.Add(DBConnection.CreateParameter("id_structure_type", building.IdStructureType));
            command.Parameters.Add(DBConnection.CreateParameter("id_street", building.IdStreet));
            command.Parameters.Add(DBConnection.CreateParameter("house", building.House));
            command.Parameters.Add(DBConnection.CreateParameter("floors", building.Floors));
            command.Parameters.Add(DBConnection.CreateParameter("num_premises", building.NumPremises));
            command.Parameters.Add(DBConnection.CreateParameter("num_rooms", building.NumRooms));
            command.Parameters.Add(DBConnection.CreateParameter("num_apartments", building.NumApartments));
            command.Parameters.Add(DBConnection.CreateParameter("num_shared_apartments", building.NumSharedApartments));
            command.Parameters.Add(DBConnection.CreateParameter("total_area", building.TotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("living_area", building.LivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_num", building.CadastralNum));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_cost", building.CadastralCost));
            command.Parameters.Add(DBConnection.CreateParameter("balance_cost", building.BalanceCost));
            command.Parameters.Add(DBConnection.CreateParameter("description", building.Description));
            command.Parameters.Add(DBConnection.CreateParameter("startup_year", building.StartupYear));
            command.Parameters.Add(DBConnection.CreateParameter("improvement", building.Improvement));
            command.Parameters.Add(DBConnection.CreateParameter("elevator", building.Elevator));
            command.Parameters.Add(DBConnection.CreateParameter("rubbish_chute", building.RubbishChute));
            command.Parameters.Add(DBConnection.CreateParameter("wear", building.Wear));
            command.Parameters.Add(DBConnection.CreateParameter("state_date", building.StateDate));
            command.Parameters.Add(DBConnection.CreateParameter("plumbing", building.Plumbing));
            command.Parameters.Add(DBConnection.CreateParameter("hot_water_supply", building.HotWaterSupply));
            command.Parameters.Add(DBConnection.CreateParameter("canalization", building.Canalization));
            command.Parameters.Add(DBConnection.CreateParameter("electricity", building.Electricity));
            command.Parameters.Add(DBConnection.CreateParameter("radio_network", building.RadioNetwork));            
            command.Parameters.Add(DBConnection.CreateParameter("id_heating_type", building.IdHeatingType));
            command.Parameters.Add(DBConnection.CreateParameter("BTI_rooms", building.RoomsBTI));
            command.Parameters.Add(DBConnection.CreateParameter("housing_cooperative", building.HousingCooperative));
        }
    }
}
