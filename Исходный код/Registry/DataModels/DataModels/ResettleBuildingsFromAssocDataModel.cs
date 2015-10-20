using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ResettleBuildingsFromAssocDataModel : DataModel
    {
        private static ResettleBuildingsFromAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM resettle_buildings_from_assoc WHERE deleted = 0";
        private const string TableName = "resettle_buildings_from_assoc";

        private ResettleBuildingsFromAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static ResettleBuildingsFromAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ResettleBuildingsFromAssocDataModel(progressBar, incrementor));
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
            command.CommandText = "UPDATE resettle_buildings_from_assoc SET deleted = 1 WHERE id_assoc = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO resettle_buildings_from_assoc (id_building, id_process) VALUES (?,?)";
            var resettleObject = (ResettleObject) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", resettleObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", resettleObject.IdProcess));
        }
    }
}
