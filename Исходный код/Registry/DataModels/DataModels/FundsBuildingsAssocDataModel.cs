using System.Data.Common;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    internal sealed class FundsBuildingsAssocDataModel : DataModel
    {
        private static FundsBuildingsAssocDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM funds_buildings_assoc WHERE deleted = 0";
        private const string TableName = "funds_buildings_assoc";

        private FundsBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static FundsBuildingsAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new FundsBuildingsAssocDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_fund"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation("funds_history", "id_fund", TableName, "id_fund");
            AddRelation("buildings", "id_building", TableName, "id_building");
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            command.CommandText = "INSERT INTO funds_buildings_assoc (id_building, id_fund) VALUES (?, ?)";
            var fundObjectAssoc = (FundObjectAssoc) entity;          
            command.Parameters.Add(DBConnection.CreateParameter("id_building", fundObjectAssoc.IdObject));
            command.Parameters.Add(DBConnection.CreateParameter("id_fund", fundObjectAssoc.IdFund));
        }
    }
}
