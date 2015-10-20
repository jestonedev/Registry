using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ClaimStateTypesRelationsDataModel : DataModel
    {
        private static ClaimStateTypesRelationsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM claim_state_types_relations WHERE deleted <> 1";
        private const string TableName = "claim_state_types_relations";

        private ClaimStateTypesRelationsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {  
        }

        public static ClaimStateTypesRelationsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ClaimStateTypesRelationsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_relation"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("claim_state_types", "id_state_type", TableName, "id_state_from");
            AddRelation("claim_state_types", "id_state_type", TableName, "id_state_to");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE claim_state_types_relations SET deleted = 1 WHERE id_relation = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_relation", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO claim_state_types_relations
                            (id_state_from, id_state_to)
                            VALUES (?, ?)";
            var claimStateTypeRelation = (ClaimStateTypeRelation) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_state_from", claimStateTypeRelation.IdStateFrom));
            command.Parameters.Add(DBConnection.CreateParameter("id_state_to", claimStateTypeRelation.IdStateTo));
        }
    }
}
