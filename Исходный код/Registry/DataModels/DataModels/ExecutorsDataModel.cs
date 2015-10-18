using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    public sealed class ExecutorsDataModel: DataModel
    {
        private const string SelectQuery = "SELECT * FROM executors WHERE deleted <> 1";
        private const string TableName = "executors";

        private ExecutorsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_executor"] };
            Table.Columns["is_inactive"].DefaultValue = false;
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE executors SET deleted = 1 WHERE id_executor = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_executor", id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE executors SET executor_name = ?, executor_login = ?, phone = ?, is_inactive = ? WHERE id_executor = ?";
            var executor = (Executor) entity;
            command.Parameters.Add(DBConnection.CreateParameter("executor_name", executor.ExecutorName));
            command.Parameters.Add(DBConnection.CreateParameter("executor_login", executor.ExecutorLogin));
            command.Parameters.Add(DBConnection.CreateParameter("phone", executor.Phone));
            command.Parameters.Add(DBConnection.CreateParameter("is_inactive", executor.IsInactive));
            command.Parameters.Add(DBConnection.CreateParameter("id_executor", executor.IdExecutor));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO executors (executor_name, executor_login, phone, is_inactive) VALUES (?, ?, ?, ?)";
            var executor = (Executor)entity;
            command.Parameters.Add(DBConnection.CreateParameter("executor_name", executor.ExecutorName));
            command.Parameters.Add(DBConnection.CreateParameter("executor_login", executor.ExecutorLogin));
            command.Parameters.Add(DBConnection.CreateParameter("phone", executor.Phone));
            command.Parameters.Add(DBConnection.CreateParameter("is_inactive", executor.IsInactive));
        }
    }
}
