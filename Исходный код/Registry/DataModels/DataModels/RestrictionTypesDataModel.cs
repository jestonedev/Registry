using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class RestrictionTypesDataModel : DataModel
    {
        private static RestrictionTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM restriction_types WHERE deleted <> 1";
        private const string TableName = "restriction_types";

        private RestrictionTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static RestrictionTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new RestrictionTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_restriction_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_restriction_type", "restrictions", "id_restriction_type");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE restriction_types SET deleted = 1 WHERE id_restriction_type = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_restriction_type", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO restriction_types (restriction_type) VALUES (?)";
            var restrictionType = (RestrictionType) entity;
            command.Parameters.Add(DBConnection.CreateParameter("restriction_type", restrictionType.RestrictionTypeName));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE restriction_types SET restriction_type = ? WHERE id_restriction_type = ?";
            var restrictionType = (RestrictionType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("restriction_type", restrictionType.RestrictionTypeName));
            command.Parameters.Add(DBConnection.CreateParameter("id_restriction_type", restrictionType.IdRestrictionType));
        }
    }
}
