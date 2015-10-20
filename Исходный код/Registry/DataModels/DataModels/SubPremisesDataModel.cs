using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class SubPremisesDataModel : DataModel
    {
        private static SubPremisesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM sub_premises WHERE deleted = 0";
        private const string TableName = "sub_premises";

        private SubPremisesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static SubPremisesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SubPremisesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_sub_premises"] };
            Table.Columns["total_area"].DefaultValue = 0;
            Table.Columns["living_area"].DefaultValue = 0;
            Table.Columns["deleted"].DefaultValue = 0;
            Table.Columns["id_state"].DefaultValue = 1;
        }

        protected override void ConfigureRelations()
        {
            AddRelation("premises", "id_premises", TableName, "id_premises");
            AddRelation("object_states", "id_state", TableName, "id_state");
            AddRelation(TableName, "id_sub_premises", "funds_sub_premises_assoc", "id_sub_premises");
            AddRelation(TableName, "id_sub_premises", "tenancy_sub_premises_assoc", "id_sub_premises");
            AddRelation(TableName, "id_sub_premises", "resettle_sub_premises_from_assoc", "id_sub_premises");
            AddRelation(TableName, "id_sub_premises", "resettle_sub_premises_to_assoc", "id_sub_premises");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE sub_premises SET deleted = 1 WHERE id_sub_premises = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_sub_premises", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE sub_premises SET id_premises = ?, id_state = ?, sub_premises_num = ?, 
                            total_area = ?, living_area = ?, description = ?, state_date = ? WHERE id_sub_premises = ?";
            var subPremise = (SubPremise) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", subPremise.IdPremises));
            command.Parameters.Add(DBConnection.CreateParameter("id_state", subPremise.IdState));
            command.Parameters.Add(DBConnection.CreateParameter("sub_premises_num", subPremise.SubPremisesNum));
            command.Parameters.Add(DBConnection.CreateParameter("total_area", subPremise.TotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("living_area", subPremise.LivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("description", subPremise.Description));
            command.Parameters.Add(DBConnection.CreateParameter("state_date", subPremise.StateDate));
            command.Parameters.Add(DBConnection.CreateParameter("id_sub_premises", subPremise.IdSubPremises));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO sub_premises 
                            (id_premises, id_state, sub_premises_num, total_area, living_area, description, state_date)
                            VALUES (?, ?, ?, ?, ?, ?, ?)";
            var subPremise = (SubPremise)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", subPremise.IdPremises));
            command.Parameters.Add(DBConnection.CreateParameter("id_state", subPremise.IdState));
            command.Parameters.Add(DBConnection.CreateParameter("sub_premises_num", subPremise.SubPremisesNum));
            command.Parameters.Add(DBConnection.CreateParameter("total_area", subPremise.TotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("living_area", subPremise.LivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("description", subPremise.Description));
            command.Parameters.Add(DBConnection.CreateParameter("state_date", subPremise.StateDate));
        }
    }
}
