using System.Data;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    public sealed class TenancyFilesDataModel : EntityDataModel<TenancyFile>
    {

        public override DataTable Select()
        {
            throw new DataModelException("Попытка вызова неразрешенного метода класса TenancyFilesDataModel");
        }

        private TenancyFilesDataModel()
        {
        }

        public new static TenancyFilesDataModel GetInstance()
        {
            return new TenancyFilesDataModel();
        }

        public DataTable Select(int idProcess)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = string.Format(SelectQuery, idProcess);
                return connection.SqlSelectTable(TableName, command);
            }
        }
    }
}
