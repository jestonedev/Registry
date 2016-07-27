using System;
using System.Data.Common;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class RestrictionsBuildingsAssocDataModel : DataModel
    {
        private static RestrictionsBuildingsAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM restrictions_buildings_assoc WHERE deleted = 0";
        private const string TableName = "restrictions_buildings_assoc";

        private RestrictionsBuildingsAssocDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static RestrictionsBuildingsAssocDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new RestrictionsBuildingsAssocDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_restriction"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("restrictions", "id_restriction", TableName, "id_restriction");
            AddRelation("buildings", "id_building", TableName, "id_building");
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = "INSERT INTO restrictions_buildings_assoc (id_building, id_restriction) VALUES (?, ?)";
            var restrictionObject = (RestrictionObjectAssoc)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", restrictionObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_restriction", restrictionObject.IdRestriction));
        }
    }
}
