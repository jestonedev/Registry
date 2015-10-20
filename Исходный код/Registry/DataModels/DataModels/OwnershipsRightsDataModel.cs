using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class OwnershipsRightsDataModel : DataModel
    {
        private static OwnershipsRightsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM ownership_rights";
        private const string TableName = "ownership_rights";

        private OwnershipsRightsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static OwnershipsRightsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new OwnershipsRightsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_ownership_right"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("ownership_right_types", "id_ownership_right_type", TableName, "id_ownership_right_type");
            AddRelation(TableName, "id_ownership_right", "ownership_premises_assoc", "id_ownership_right");
            AddRelation(TableName, "id_ownership_right", "ownership_buildings_assoc", "id_ownership_right");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE ownership_rights SET deleted = 1 WHERE id_ownership_right = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_ownership_right", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE ownership_rights SET id_ownership_right_type = ?,
                            number= ?, `date` = ?, description = ? WHERE id_ownership_right = ?";
            var ownershipRight = (OwnershipRight) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_ownership_right_type", ownershipRight.IdOwnershipRightType));
            command.Parameters.Add(DBConnection.CreateParameter("number", ownershipRight.Number));
            command.Parameters.Add(DBConnection.CreateParameter("date", ownershipRight.Date));
            command.Parameters.Add(DBConnection.CreateParameter("description", ownershipRight.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_ownership_right", ownershipRight.IdOwnershipRight));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO ownership_rights (id_ownership_right_type, number, `date`, description) VALUES (?, ?, ?, ?)";
            var ownershipRight = (OwnershipRight)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_ownership_right_type", ownershipRight.IdOwnershipRightType));
            command.Parameters.Add(DBConnection.CreateParameter("number", ownershipRight.Number));
            command.Parameters.Add(DBConnection.CreateParameter("date", ownershipRight.Date));
            command.Parameters.Add(DBConnection.CreateParameter("description", ownershipRight.Description));
        }
    }
}
