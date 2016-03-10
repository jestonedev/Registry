using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyPersonsDataModel : DataModel
    {
        private static TenancyPersonsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_persons WHERE deleted = 0";
        private const string TableName = "tenancy_persons";

        private TenancyPersonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyPersonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyPersonsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_person"] };
            Table.Columns["id_document_type"].DefaultValue = 255;
        }

        protected override void ConfigureRelations()
        {
            AddRelation("tenancy_processes", "id_process", TableName, "id_process");
            AddRelation("kinships", "id_kinship", TableName, "id_kinship");
            AddRelation("document_types", "id_document_type", TableName, "id_document_type");
            AddRelation("documents_issued_by", "id_document_issued_by", TableName, "id_document_issued_by");
            
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_persons SET deleted = 1 WHERE id_person = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_person", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_persons
                            (id_process, id_kinship, surname, name, patronymic, date_of_birth, id_document_type, 
                            date_of_document_issue, document_num, document_seria, id_document_issued_by,
                            registration_id_street, registration_house, registration_flat, registration_room,
                            residence_id_street, residence_house, residence_flat, residence_room, personal_account, include_date, exclude_date, registration_date)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            var tenancyPerson = (TenancyPerson) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyPerson.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("id_kinship", tenancyPerson.IdKinship));
            command.Parameters.Add(DBConnection.CreateParameter("surname", tenancyPerson.Surname));
            command.Parameters.Add(DBConnection.CreateParameter("name", tenancyPerson.Name));
            command.Parameters.Add(DBConnection.CreateParameter("patronymic", tenancyPerson.Patronymic));
            command.Parameters.Add(DBConnection.CreateParameter("date_of_birth", tenancyPerson.DateOfBirth));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_type", tenancyPerson.IdDocumentType));
            command.Parameters.Add(DBConnection.CreateParameter("date_of_document_issue", tenancyPerson.DateOfDocumentIssue));
            command.Parameters.Add(DBConnection.CreateParameter("document_num", tenancyPerson.DocumentNum));
            command.Parameters.Add(DBConnection.CreateParameter("document_seria", tenancyPerson.DocumentSeria));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_issued_by", tenancyPerson.IdDocumentIssuedBy));
            command.Parameters.Add(DBConnection.CreateParameter("registration_id_street", tenancyPerson.RegistrationIdStreet));
            command.Parameters.Add(DBConnection.CreateParameter("registration_house", tenancyPerson.RegistrationHouse));
            command.Parameters.Add(DBConnection.CreateParameter("registration_flat", tenancyPerson.RegistrationFlat));
            command.Parameters.Add(DBConnection.CreateParameter("registration_room", tenancyPerson.RegistrationRoom));
            command.Parameters.Add(DBConnection.CreateParameter("residence_id_street", tenancyPerson.ResidenceIdStreet));
            command.Parameters.Add(DBConnection.CreateParameter("residence_house", tenancyPerson.ResidenceHouse));
            command.Parameters.Add(DBConnection.CreateParameter("residence_flat", tenancyPerson.ResidenceFlat));
            command.Parameters.Add(DBConnection.CreateParameter("residence_room", tenancyPerson.ResidenceRoom));
            command.Parameters.Add(DBConnection.CreateParameter("personal_account", tenancyPerson.PersonalAccount));
            command.Parameters.Add(DBConnection.CreateParameter("include_date", tenancyPerson.IncludeDate));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_date", tenancyPerson.ExcludeDate));
            command.Parameters.Add(DBConnection.CreateParameter("registration_date", tenancyPerson.RegistrationDate));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_persons SET id_process = ?, id_kinship = ?, surname = ?, 
                            name = ?, patronymic = ?, date_of_birth = ?, id_document_type = ?, date_of_document_issue = ?, 
                            document_num = ?, document_seria = ?, id_document_issued_by = ?, registration_id_street = ?, 
                            registration_house = ?, registration_flat = ?, registration_room = ?, residence_id_street = ?,
                            residence_house = ?, residence_flat = ?, residence_room = ? , personal_account = ?,
                            include_date = ?, exclude_date = ? WHERE id_person = ?";
            var tenancyPerson = (TenancyPerson)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancyPerson.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("id_kinship", tenancyPerson.IdKinship));
            command.Parameters.Add(DBConnection.CreateParameter("surname", tenancyPerson.Surname));
            command.Parameters.Add(DBConnection.CreateParameter("name", tenancyPerson.Name));
            command.Parameters.Add(DBConnection.CreateParameter("patronymic", tenancyPerson.Patronymic));
            command.Parameters.Add(DBConnection.CreateParameter("date_of_birth", tenancyPerson.DateOfBirth));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_type", tenancyPerson.IdDocumentType));
            command.Parameters.Add(DBConnection.CreateParameter("date_of_document_issue", tenancyPerson.DateOfDocumentIssue));
            command.Parameters.Add(DBConnection.CreateParameter("document_num", tenancyPerson.DocumentNum));
            command.Parameters.Add(DBConnection.CreateParameter("document_seria", tenancyPerson.DocumentSeria));
            command.Parameters.Add(DBConnection.CreateParameter("id_document_issued_by", tenancyPerson.IdDocumentIssuedBy));
            command.Parameters.Add(DBConnection.CreateParameter("registration_id_street", tenancyPerson.RegistrationIdStreet));
            command.Parameters.Add(DBConnection.CreateParameter("registration_house", tenancyPerson.RegistrationHouse));
            command.Parameters.Add(DBConnection.CreateParameter("registration_flat", tenancyPerson.RegistrationFlat));
            command.Parameters.Add(DBConnection.CreateParameter("registration_room", tenancyPerson.RegistrationRoom));
            command.Parameters.Add(DBConnection.CreateParameter("residence_id_street", tenancyPerson.ResidenceIdStreet));
            command.Parameters.Add(DBConnection.CreateParameter("residence_house", tenancyPerson.ResidenceHouse));
            command.Parameters.Add(DBConnection.CreateParameter("residence_flat", tenancyPerson.ResidenceFlat));
            command.Parameters.Add(DBConnection.CreateParameter("residence_room", tenancyPerson.ResidenceRoom));
            command.Parameters.Add(DBConnection.CreateParameter("personal_account", tenancyPerson.PersonalAccount));
            command.Parameters.Add(DBConnection.CreateParameter("include_date", tenancyPerson.IncludeDate));
            command.Parameters.Add(DBConnection.CreateParameter("exclude_date", tenancyPerson.ExcludeDate));
            command.Parameters.Add(DBConnection.CreateParameter("id_person", tenancyPerson.IdPerson));
            // No reg date, because user can't modify it. Only import from msp for information
        }
    }
}
