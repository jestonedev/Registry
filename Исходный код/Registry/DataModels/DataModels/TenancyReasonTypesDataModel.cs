using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyReasonTypesDataModel : DataModel
    {
        private static TenancyReasonTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_reason_types WHERE deleted <> 1";
        private const string TableName = "tenancy_reason_types";

        private TenancyReasonTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyReasonTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyReasonTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_reason_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_reason_type", "tenancy_reasons", "id_reason_type");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_reason_types SET deleted = 1 WHERE id_reason_type = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_reason_type", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_reason_types SET reason_name = ?, reason_template = ? WHERE id_reason_type = ?";
            var reasonType = (ReasonType) entity;
            command.Parameters.Add(DBConnection.CreateParameter("reason_name", reasonType.ReasonName));
            command.Parameters.Add(DBConnection.CreateParameter("reason_template", reasonType.ReasonTemplate));
            command.Parameters.Add(DBConnection.CreateParameter("id_reason_type", reasonType.IdReasonType));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_reason_types (reason_name, reason_template) VALUES (?, ?)";
            var reasonType = (ReasonType)entity;
            command.Parameters.Add(DBConnection.CreateParameter("reason_name", reasonType.ReasonName));
            command.Parameters.Add(DBConnection.CreateParameter("reason_template", reasonType.ReasonTemplate));
        }
    }
}
