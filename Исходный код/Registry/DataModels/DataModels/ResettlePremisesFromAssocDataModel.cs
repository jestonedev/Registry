using System;
using System.Data.Common;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ResettlePremisesFromAssocDataModel : DataModel
    {
        private static ResettlePremisesFromAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM resettle_premises_from_assoc WHERE deleted = 0";
        private const string TableName = "resettle_premises_from_assoc";

        private ResettlePremisesFromAssocDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static ResettlePremisesFromAssocDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new ResettlePremisesFromAssocDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_assoc"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("resettle_processes", "id_process", TableName, "id_process");
            AddRelation("premises", "id_premises", TableName, "id_premises");
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO resettle_premises_from_assoc (id_premises, id_process) VALUES (?,?)";
            var resettleObject = (ResettleObject) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", resettleObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", resettleObject.IdProcess));
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE resettle_premises_from_assoc SET deleted = 1 WHERE id_assoc = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
        }
    }
}
