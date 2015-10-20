using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class FundsSubPremisesAssocDataModel : DataModel
    {
        private static FundsSubPremisesAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM funds_sub_premises_assoc WHERE deleted = 0";
        private const string TableName = "funds_sub_premises_assoc";

        private FundsSubPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static FundsSubPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new FundsSubPremisesAssocDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_fund"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("funds_history", "id_fund", TableName, "id_fund");
            AddRelation("sub_premises", "id_sub_premises", TableName, "id_sub_premises");
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = "INSERT INTO funds_sub_premises_assoc (id_sub_premises, id_fund) VALUES (?, ?)";
            var fundObjectAssoc = (FundObjectAssoc) entity;
            command.Parameters.Add(DBConnection.CreateParameter("id_sub_premises", fundObjectAssoc.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_fund", fundObjectAssoc.IdFund));
        }
    }
}
