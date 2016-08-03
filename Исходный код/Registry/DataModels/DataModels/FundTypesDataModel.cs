using System;

namespace Registry.DataModels.DataModels
{
    public sealed class FundTypesDataModel : DataModel
    {
        private static FundTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM fund_types ft ORDER BY CASE ft.id_fund_type WHEN 4 THEN 0 ELSE ft.fund_type END DESC";
        private const string TableName = "fund_types";

        private FundTypesDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static FundTypesDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new FundTypesDataModel(afterLoadHandler));
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
