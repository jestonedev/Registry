using System.Windows.Forms;

namespace Registry.DataModels.DataModels
{
    public sealed class FundTypesDataModel : DataModel
    {
        private const string SelectQuery = "SELECT * FROM fund_types ft ORDER BY CASE ft.id_fund_type WHEN 4 THEN 0 ELSE ft.fund_type END DESC";
        private const string TableName = "fund_types";

        private FundTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_fund_type"] };
        }
    }
}
