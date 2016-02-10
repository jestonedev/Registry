using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class ResettlePersonsDataModel : DataModel
    {
        private static ResettlePersonsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM resettle_persons WHERE deleted = 0";
        private const string TableName = "resettle_persons";

        private ResettlePersonsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static ResettlePersonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new ResettlePersonsDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_person"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("resettle_processes", "id_process", TableName, "id_process");
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE resettle_persons SET deleted = 1 WHERE id_person = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_person", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO resettle_persons (id_process, surname, name, patronymic, document_num, document_seria)
                            VALUES (?, ?, ?, ?, ?, ?)";
            var resettlePerson = (ResettlePerson) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", resettlePerson.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("surname", resettlePerson.Surname));
            command.Parameters.Add(DBConnection.CreateParameter("name", resettlePerson.Name));
            command.Parameters.Add(DBConnection.CreateParameter("patronymic", resettlePerson.Patronymic));
            command.Parameters.Add(DBConnection.CreateParameter("document_num", resettlePerson.DocumentNum));
            command.Parameters.Add(DBConnection.CreateParameter("document_seria", resettlePerson.DocumentSeria));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE resettle_persons SET id_process = ?, surname = ?, 
                            name = ?, patronymic = ?, document_num = ?, document_seria = ? WHERE id_person = ?";
            var resettlePerson = (ResettlePerson)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", resettlePerson.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("surname", resettlePerson.Surname));
            command.Parameters.Add(DBConnection.CreateParameter("name", resettlePerson.Name));
            command.Parameters.Add(DBConnection.CreateParameter("patronymic", resettlePerson.Patronymic));
            command.Parameters.Add(DBConnection.CreateParameter("document_num", resettlePerson.DocumentNum));
            command.Parameters.Add(DBConnection.CreateParameter("document_seria", resettlePerson.DocumentSeria));
            command.Parameters.Add(DBConnection.CreateParameter("id_person", resettlePerson.IdPerson));
        }
    }
}
