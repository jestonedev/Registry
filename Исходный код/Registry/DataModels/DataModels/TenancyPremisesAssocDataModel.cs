using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyPremisesAssocDataModel : DataModel
    {
        private static TenancyPremisesAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_premises_assoc WHERE deleted = 0";
        private const string TableName = "tenancy_premises_assoc";

        private TenancyPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyPremisesAssocDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_assoc"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("premises", "id_premises", TableName, "id_premises");
            AddRelation("tenancy_processes", "id_process", TableName, "id_process");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_premises_assoc SET deleted = 1 WHERE id_assoc = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_premises_assoc (id_premises, id_process, rent_total_area, rent_living_area) VALUES (?,?,?,?)";
            var tenancyObject = (TenancyObject) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", tenancyObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("rent_living_area", tenancyObject.RentLivingArea));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_premises_assoc SET id_premises = ?, id_process = ?, rent_total_area = ?, rent_living_area = ? WHERE id_assoc = ?";
            var tenancyObject = (TenancyObject)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_premises", tenancyObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("rent_living_area", tenancyObject.RentLivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("id_assoc", tenancyObject.IdAssoc));
        }
    }
}
