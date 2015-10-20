using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyBuildingsAssocDataModel : DataModel
    {
        private static TenancyBuildingsAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_buildings_assoc WHERE deleted = 0";
        private const string TableName = "tenancy_buildings_assoc";

        private TenancyBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyBuildingsAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyBuildingsAssocDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_assoc"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("building", "id_building", TableName, "id_building");
            AddRelation("tenancy_processes", "id_process", TableName, "id_process");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_buildings_assoc SET deleted = 1 WHERE id_assoc = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_buildings_assoc SET id_building = ?, id_process = ?, rent_total_area = ?, rent_living_area = ? 
                                    WHERE id_assoc = ?";
            var tenancyObject = (TenancyObject) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", tenancyObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("rent_living_area", tenancyObject.RentLivingArea));
            command.Parameters.Add(DBConnection.CreateParameter("id_assoc", tenancyObject.IdAssoc));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_buildings_assoc (id_building, id_process, rent_total_area, rent_living_area) VALUES (?,?,?,?)";
            var tenancyObject = (TenancyObject)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_building", tenancyObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("rent_living_area", tenancyObject.RentLivingArea));
        }
    }
}
