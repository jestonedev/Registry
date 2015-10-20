using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class OwnershipRightTypesDataModel : DataModel
    {
        private static OwnershipRightTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM ownership_right_types WHERE deleted <> 1";
        private const string TableName = "ownership_right_types";

        private OwnershipRightTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static OwnershipRightTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new OwnershipRightTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_ownership_right_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_ownership_right_type", "ownership_rights", "id_ownership_right_type"); 
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE ownership_right_types SET deleted = 1 WHERE id_ownership_right_type = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_ownership_right_type", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE ownership_right_types SET ownership_right_type = ? WHERE id_ownership_right_type = ?";
            var ownershipRightType = (OwnershipRightType) entity;
            command.Parameters.Add(DBConnection.CreateParameter("ownership_right_type", ownershipRightType.OwnershipRightTypeName));
            command.Parameters.Add(DBConnection.CreateParameter("id_ownership_right_type", ownershipRightType.IdOwnershipRightType));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO ownership_right_types (ownership_right_type) VALUES (?)";
            var ownershipRightType = (OwnershipRightType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("ownership_right_type", ownershipRightType.OwnershipRightTypeName));
        }
    }
}
