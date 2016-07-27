using System;
using System.Data.Common;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancySubPremisesAssocDataModel : DataModel
    {
        private static TenancySubPremisesAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_sub_premises_assoc WHERE deleted = 0";
        private const string TableName = "tenancy_sub_premises_assoc";

        private TenancySubPremisesAssocDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static TenancySubPremisesAssocDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new TenancySubPremisesAssocDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_assoc"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("sub_premises", "id_sub_premises", TableName, "id_sub_premises");
            AddRelation("tenancy_processes", "id_process", TableName, "id_process");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_sub_premises_assoc SET deleted = 1 WHERE id_assoc = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_assoc", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_sub_premises_assoc SET id_sub_premises = ?, id_process = ?, rent_total_area = ? WHERE id_assoc = ?";
            var tenancyObject = (TenancyObject) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_sub_premises", tenancyObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
            command.Parameters.Add(DBConnection.CreateParameter("id_assoc", tenancyObject.IdAssoc));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_sub_premises_assoc (id_sub_premises, id_process, rent_total_area) VALUES (?,?,?)";
            var tenancyObject = (TenancyObject)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_sub_premises", tenancyObject.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyObject.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("rent_total_area", tenancyObject.RentTotalArea));
        }
    }
}
