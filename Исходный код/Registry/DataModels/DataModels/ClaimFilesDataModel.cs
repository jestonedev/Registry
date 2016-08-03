using System.Data;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    public sealed class ClaimFilesDataModel : EntityDataModel<ClaimFile>
    {

        public override DataTable Select()
        {
            throw new DataModelException("Попытка вызова неразрешенного метода класса ClaimFilesDataModel");
        }

        private ClaimFilesDataModel()
        {
        }

        public new static ClaimFilesDataModel GetInstance()
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
    }
}
