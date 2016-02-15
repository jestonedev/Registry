using System.Data;
using System.Data.Common;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    public class ClaimFilesDataModel : DataModel
    {
        private const string SelectQuery = @"SELECT * FROM claim_files WHERE id_claim = {0}";
        private const string TableName = "claim_files";

        public override DataTable Select()
        {
            return null;
        }

        private ClaimFilesDataModel()
        {
        }

        public static ClaimFilesDataModel GetInstance()
        {
            return new ClaimFilesDataModel();
        }

        public DataTable Select(int idClaim)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format(SelectQuery, idClaim);
                return connection.SqlSelectTable(TableName, command);
            }
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO claim_files
                            (id_claim, file_name, display_name) VALUES (?, ?, ?)";
            var claimFile = (ClaimFile)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_claim", claimFile.IdClaim));
            command.Parameters.Add(DBConnection.CreateParameter("file_name", claimFile.FileName));
            command.Parameters.Add(DBConnection.CreateParameter("display_name", claimFile.DisplayName));
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "DELETE FROM claim_files WHERE id_file = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_file", id));
        }
    }
}
