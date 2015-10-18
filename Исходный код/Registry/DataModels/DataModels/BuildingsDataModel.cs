﻿using System;
using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class BuildingsDataModel : DataModel
    {
        private const string SelectQuery = "SELECT * FROM buildings b WHERE deleted = 0";
        private const string TableName = "buildings";

        public BuildingsDataModel(ToolStripProgressBar progressBar, int incrementor): base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;      
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
                            improvement = ?, elevator = ?, rubbish_chute = ?, wear = ?, state_date = ? WHERE id_building = ?";
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
            command.Parameters.Add(DBConnection.CreateParameter("id_building", building.IdBuilding));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO buildings
                            (id_state, id_structure_type, id_street, house
                             , floors, num_premises, num_rooms, num_apartments
                             , num_shared_apartments, total_area, living_area, cadastral_num
                             , cadastral_cost, balance_cost, description, startup_year
                             , improvement, elevator, rubbish_chute, wear, state_date)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
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
        }
    }
}
