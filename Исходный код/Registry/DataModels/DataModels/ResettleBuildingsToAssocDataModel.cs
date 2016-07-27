using System;
using System.Data.Common;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ResettleBuildingsToAssocDataModel : DataModel
    {
        private static ResettleBuildingsToAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM resettle_buildings_to_assoc WHERE deleted = 0";
        private const string TableName = "resettle_buildings_to_assoc";

        private ResettleBuildingsToAssocDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static ResettleBuildingsToAssocDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new ResettleBuildingsToAssocDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_assoc"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("resettle_processes", "id_process", TableName, "id_process");
            AddRelation("buildings", "id_building", TableName, "id_building");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE resettle_buildings_to_assoc SET deleted = 1 WHERE id_assoc = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO resettle_buildings_to_assoc (id_building, id_process) VALUES (?,?)";
            var resettleObject = (ResettleObject) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", resettleObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", resettleObject.IdProcess));
        }
    }
}
