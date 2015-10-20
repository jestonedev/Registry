using System;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class PremisesDataModel : DataModel
    {
        private static PremisesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM premises WHERE deleted = 0";
        private const string TableName = "premises";

        private PremisesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;            
        }

        public static PremisesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new PremisesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_premises"] };
            Table.Columns["id_state"].DefaultValue = 1;
            Table.Columns["living_area"].DefaultValue = 0;
            Table.Columns["total_area"].DefaultValue = 0;
            Table.Columns["height"].DefaultValue = 0;
            Table.Columns["num_rooms"].DefaultValue = 0;
            Table.Columns["num_beds"].DefaultValue = 0;
            Table.Columns["id_premises_type"].DefaultValue = 1;
            Table.Columns["id_premises_kind"].DefaultValue = 1;
            Table.Columns["floor"].DefaultValue = 0;
            Table.Columns["cadastral_cost"].DefaultValue = 0;
            Table.Columns["balance_cost"].DefaultValue = 0;
            Table.Columns["is_memorial"].DefaultValue = false;
        }

        protected override void ConfigureRelations()
        {
            AddRelation("buildings", "id_building", TableName, "id_building");
            AddRelation("premises_types", "id_premises_type", TableName, "id_premises_type");
            AddRelation("premises_kinds", "id_premises_kind", TableName, "id_premises_kind");
            AddRelation("object_states", "id_state", TableName, "id_state");
            AddRelation(TableName, "id_premises", "sub_premises", "id_premises");
            AddRelation(TableName, "id_premises", "restrictions_premises_assoc", "id_premises");
            AddRelation(TableName, "id_premises", "ownership_premises_assoc", "id_premises");
            AddRelation(TableName, "id_premises", "funds_premises_assoc", "id_premises");
            AddRelation(TableName, "id_premises", "tenancy_premises_assoc", "id_premises");
            AddRelation(TableName, "id_premises", "resettle_premises_from_assoc", "id_premises");
            AddRelation(TableName, "id_premises", "resettle_premises_to_assoc", "id_premises");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE premises SET deleted = 1 WHERE id_premises = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_premises", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO premises
                            (id_building, id_state, id_premises_kind, id_premises_type, premises_num, floor
                             , num_rooms, num_beds, total_area, living_area, height
                             , cadastral_num, cadastral_cost
                             , balance_cost, description, reg_date, is_memorial, account, state_date)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            var premise = (Premise)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", premise.IdBuilding));
            command.Parameters.Add(DBConnection.CreateParameter("id_state", premise.IdState));
            command.Parameters.Add(DBConnection.CreateParameter("id_premises_kind", premise.IdPremisesKind));
            command.Parameters.Add(DBConnection.CreateParameter("id_premises_type", premise.IdPremisesType));
            command.Parameters.Add(DBConnection.CreateParameter("premises_num", premise.PremisesNum));
            command.Parameters.Add(DBConnection.CreateParameter("floor", premise.Floor));
            command.Parameters.Add(DBConnection.CreateParameter("num_rooms", premise.NumRooms));
            command.Parameters.Add(DBConnection.CreateParameter("num_beds", premise.NumBeds));
            command.Parameters.Add(DBConnection.CreateParameter("total_area", premise.TotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("living_area", premise.LivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("height", premise.Height));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_num", premise.CadastralNum));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_cost", premise.CadastralCost));
            command.Parameters.Add(DBConnection.CreateParameter("balance_cost", premise.BalanceCost));
            command.Parameters.Add(DBConnection.CreateParameter("description", premise.Description));
            command.Parameters.Add(DBConnection.CreateParameter("reg_date", premise.RegDate));
            command.Parameters.Add(DBConnection.CreateParameter("is_memorial", premise.IsMemorial));
            command.Parameters.Add(DBConnection.CreateParameter("account", premise.Account));
            command.Parameters.Add(DBConnection.CreateParameter("state_date", premise.StateDate));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE premises SET id_building = ?, id_state = ?, id_premises_kind = ?, 
                            id_premises_type = ?,premises_num = ?, floor = ?, num_rooms = ?, num_beds = ?, 
                            total_area = ?, living_area = ?, height = ?, 
                            cadastral_num = ?, cadastral_cost = ?, 
                            balance_cost = ?, description = ?, reg_date = ?, is_memorial = ?, account = ?, state_date = ? WHERE id_premises = ?";
            var premise = (Premise) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", premise.IdBuilding));
            command.Parameters.Add(DBConnection.CreateParameter("id_state", premise.IdState));
            command.Parameters.Add(DBConnection.CreateParameter("id_premises_kind", premise.IdPremisesKind));
            command.Parameters.Add(DBConnection.CreateParameter("id_premises_type", premise.IdPremisesType));
            command.Parameters.Add(DBConnection.CreateParameter("premises_num", premise.PremisesNum));
            command.Parameters.Add(DBConnection.CreateParameter("floor", premise.Floor));
            command.Parameters.Add(DBConnection.CreateParameter("num_rooms", premise.NumRooms));
            command.Parameters.Add(DBConnection.CreateParameter("num_beds", premise.NumBeds));
            command.Parameters.Add(DBConnection.CreateParameter("total_area", premise.TotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("living_area", premise.LivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("height", premise.Height));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_num", premise.CadastralNum));
            command.Parameters.Add(DBConnection.CreateParameter("cadastral_cost", premise.CadastralCost));
            command.Parameters.Add(DBConnection.CreateParameter("balance_cost", premise.BalanceCost));
            command.Parameters.Add(DBConnection.CreateParameter("description", premise.Description));
            command.Parameters.Add(DBConnection.CreateParameter("reg_date", premise.RegDate));
            command.Parameters.Add(DBConnection.CreateParameter("is_memorial", premise.IsMemorial));
            command.Parameters.Add(DBConnection.CreateParameter("account", premise.Account));
            command.Parameters.Add(DBConnection.CreateParameter("state_date", premise.StateDate));
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", premise.IdPremises));
        }
    }
}
