using System;
using System.Data.Common;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class OwnershipPremisesAssocDataModel : DataModel
    {
        private static OwnershipPremisesAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM ownership_premises_assoc WHERE deleted = 0";
        private const string TableName = "ownership_premises_assoc";

        private OwnershipPremisesAssocDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static OwnershipPremisesAssocDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new OwnershipPremisesAssocDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_ownership_right"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("ownership_rights", "id_ownership_right", TableName, "id_ownership_right");
            AddRelation("premises", "id_premises", TableName, "id_premises");
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = "INSERT INTO ownership_premises_assoc (id_premises, id_ownership_right) VALUES (?, ?)";
            var ownershipRightObjectAssoc = (OwnershipRightObjectAssoc)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", ownershipRightObjectAssoc.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_ownership_right", ownershipRightObjectAssoc.IdOwnershipRight));
        }
    }
}
