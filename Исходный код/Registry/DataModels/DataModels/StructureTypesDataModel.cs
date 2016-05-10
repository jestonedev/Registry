using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class StructureTypesDataModel : DataModel
    {
        private static StructureTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM structure_types WHERE deleted <> 1";
        private const string TableName = "structure_types";

        private StructureTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static StructureTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new StructureTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_structure_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_structure_type", "buildings", "id_structure_type");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE structure_types SET deleted = 1 WHERE id_structure_type = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_structure_type", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO structure_types (structure_type) VALUES (?)";
            var structureType = (StructureType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("structure_type", structureType.StructureTypeName));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE structure_types SET structure_type = ? WHERE id_structure_type = ?";
            var structureType = (StructureType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("structure_type", structureType.StructureTypeName));
            command.Parameters.Add(DBConnection.CreateParameter("id_structure_type", structureType.IdStructureType));
        }
    }
}
