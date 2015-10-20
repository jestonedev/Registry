using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    internal sealed class FundTypesDataModel : DataModel
    {
        private static FundTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM fund_types ft ORDER BY CASE ft.id_fund_type WHEN 4 THEN 0 ELSE ft.fund_type END DESC";
        private const string TableName = "fund_types";

        private FundTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        public static FundTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new FundTypesDataModel(progressBar, incrementor));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_fund_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_fund_type", "funds_history", "id_fund_type");
        }
    }
}
