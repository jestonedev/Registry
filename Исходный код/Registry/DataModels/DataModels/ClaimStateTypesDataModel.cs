﻿using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ClaimStateTypesDataModel : DataModel
    {
        private static ClaimStateTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM claim_state_types WHERE deleted <> 1";
        private const string TableName = "claim_state_types";

        private ClaimStateTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static ClaimStateTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ClaimStateTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_state_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_state_type", "claim_states", "id_state_type");
            AddRelation(TableName, "id_state_type", "claim_state_types_relations", "id_state_from");
            AddRelation(TableName, "id_state_type", "claim_state_types_relations", "id_state_to");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE claim_state_types SET deleted = 1 WHERE id_state_type = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_state_type", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE claim_state_types SET state_type = ?, is_start_state_type = ? WHERE id_state_type = ?";
            var claimStateType = (ClaimStateType) entity;
            command.Parameters.Add(DBConnection.CreateParameter("state_type", claimStateType.StateType));
            command.Parameters.Add(DBConnection.CreateParameter("is_start_state_type", claimStateType.IsStartStateType));
            command.Parameters.Add(DBConnection.CreateParameter("id_state_type", claimStateType.IdStateType));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO claim_state_types
                            (state_type, is_start_state_type)
                            VALUES (?, ?)";
            var claimStateType = (ClaimStateType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("state_type", claimStateType.StateType));
            command.Parameters.Add(DBConnection.CreateParameter("is_start_state_type", claimStateType.IsStartStateType));
        }
    }
}
