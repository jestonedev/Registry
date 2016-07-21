using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class TenancyRentPeriodsHistoryDataModel: DataModel
    {
        private static TenancyRentPeriodsHistoryDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM tenancy_rent_periods_history WHERE deleted = 0";
        private const string TableName = "tenancy_rent_periods_history";

        private TenancyRentPeriodsHistoryDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static TenancyRentPeriodsHistoryDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new TenancyRentPeriodsHistoryDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["id_rent_period"] };
            Table.Columns["until_dismissal"].DefaultValue = false;
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            command.CommandText = "UPDATE tenancy_rent_periods_history SET deleted = 1 WHERE id_rent_period = ?";
            command.Parameters.Add(DBConnection.CreateParameter<int?>("id_rent_period", id));
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"INSERT INTO tenancy_rent_periods_history
                            (id_process, begin_date, end_date, until_dismissal)
                            VALUES (?, ?, ?, ?)";
            var tenancy = (TenancyRentPeriod)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancy.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("begin_date", tenancy.BeginDate));
            command.Parameters.Add(DBConnection.CreateParameter("end_date", tenancy.EndDate));
            command.Parameters.Add(DBConnection.CreateParameter("until_dismissal", tenancy.UntilDismissal));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            command.CommandText = @"UPDATE tenancy_rent_periods_history SET id_process = ?, begin_date = ?,
                                    end_date = ?, until_dismissal = ? WHERE id_rent_period = ?";
            var tenancy = (TenancyRentPeriod)entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_process", tenancy.IdProcess));
            command.Parameters.Add(DBConnection.CreateParameter("begin_date", tenancy.BeginDate));
            command.Parameters.Add(DBConnection.CreateParameter("end_date", tenancy.EndDate));
            command.Parameters.Add(DBConnection.CreateParameter("until_dismissal", tenancy.UntilDismissal));
            command.Parameters.Add(DBConnection.CreateParameter("id_rent_period", tenancy.IdRentPeriod));
        }
    }
}
