using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class WarrantsDataModel : DataModel
    {
        private static WarrantsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM warrants WHERE deleted <> 1";
        private const string TableName = "warrants";

        private WarrantsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static WarrantsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new WarrantsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_warrant"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_warrant", "tenancy_agreements", "id_warrant");
            AddRelation(TableName, "id_warrant", "tenancy_processes", "id_warrant");
            AddRelation("warrant_doc_types", "id_warrant_doc_type", TableName, "id_warrant_doc_type");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE warrants SET deleted = 1 WHERE id_warrant = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_warrant", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE warrants SET id_warrant_doc_type = ?,
                            registration_num = ?, registration_date = ?, on_behalf_of = ?, notary = ?,
                            notary_district = ?, description = ? WHERE id_warrant = ?";
            var warrant = (Warrant) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_warrant_doc_type", warrant.IdWarrantDocType));
            command.Parameters.Add(DBConnection.CreateParameter("registration_num", warrant.RegistrationNum));
            command.Parameters.Add(DBConnection.CreateParameter("registration_date", warrant.RegistrationDate));
            command.Parameters.Add(DBConnection.CreateParameter("on_behalf_of", warrant.OnBehalfOf));
            command.Parameters.Add(DBConnection.CreateParameter("notary", warrant.Notary));
            command.Parameters.Add(DBConnection.CreateParameter("notary_district", warrant.NotaryDistrict));
            command.Parameters.Add(DBConnection.CreateParameter("description", warrant.Description));
            command.Parameters.Add(DBConnection.CreateParameter("id_warrant", warrant.IdWarrant));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO warrants
                            (id_warrant_doc_type, registration_num, 
                            registration_date, on_behalf_of, notary,
                            notary_district, description) VALUES (?, ?, ?, ?, ?, ?, ?)";
            var warrant = (Warrant)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_warrant_doc_type", warrant.IdWarrantDocType));
            command.Parameters.Add(DBConnection.CreateParameter("registration_num", warrant.RegistrationNum));
            command.Parameters.Add(DBConnection.CreateParameter("registration_date", warrant.RegistrationDate));
            command.Parameters.Add(DBConnection.CreateParameter("on_behalf_of", warrant.OnBehalfOf));
            command.Parameters.Add(DBConnection.CreateParameter("notary", warrant.Notary));
            command.Parameters.Add(DBConnection.CreateParameter("notary_district", warrant.NotaryDistrict));
            command.Parameters.Add(DBConnection.CreateParameter("description", warrant.Description));
        }
    }
}
