using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class RestrictionsDataModel : DataModel
    {
        private static RestrictionsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM restrictions";
        private const string TableName = "restrictions";

        private RestrictionsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static RestrictionsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new RestrictionsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_restriction"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("restriction_types", "id_restriction_type", TableName, "id_restriction_type");
            AddRelation(TableName, "id_restriction", "restrictions_buildings_assoc", "id_restriction");
            AddRelation(TableName, "id_restriction", "restrictions_premises_assoc", "id_restriction");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE restrictions SET deleted = 1 WHERE id_restriction = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_restriction", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO restrictions
                            (id_restriction_type, number, `date`, description)
                            VALUES (?, ?, ?, ?)";
            var restriction = (Restriction) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_restriction_type", restriction.IdRestrictionType));
            command.Parameters.Add(DBConnection.CreateParameter("number", restriction.Number));
            command.Parameters.Add(DBConnection.CreateParameter("date", restriction.Date));
            command.Parameters.Add(DBConnection.CreateParameter("description", restriction.Description));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE restrictions SET id_restriction_type = ?,
                            number= ?, `date` = ?, description = ? WHERE id_restriction = ?";
            var restriction = (Restriction)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_restriction_type", restriction.IdRestrictionType));
            command.Parameters.Add(DBConnection.CreateParameter("number", restriction.Number));
            command.Parameters.Add(DBConnection.CreateParameter("date", restriction.Date));
            command.Parameters.Add(DBConnection.CreateParameter("description", restriction.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_restriction", restriction.IdRestriction));
        }
    }
}
