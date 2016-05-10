using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class HeatingTypesDataModel : DataModel
    {
        private static HeatingTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM heating_type WHERE deleted <> 1";
        private const string TableName = "heating_type";

        private HeatingTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static HeatingTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new HeatingTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_heating_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_heating_type", "buildings", "id_heating_type");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE heating_type SET deleted = 1 WHERE id_heating_type = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_heating_type", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO heating_type (heating_type) VALUES (?)";
            var structureType = (HeatingType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("heating_type", structureType.HeatingTypeName));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE heating_type SET heating_type = ? WHERE id_heating_type = ?";
            var heatingType = (HeatingType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("heating_type", heatingType.HeatingTypeName));
            command.Parameters.Add(DBConnection.CreateParameter("id_heating_type", heatingType.IdHeatingType));
        }
    }
}
