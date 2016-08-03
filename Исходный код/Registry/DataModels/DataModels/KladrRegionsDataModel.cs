using System;

namespace Registry.DataModels.DataModels
{
    public sealed class KladrRegionsDataModel : DataModel
    {
        private static KladrRegionsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM v_kladr_regions";
        private const string TableName = "kladr_regions";

        private KladrRegionsDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static KladrRegionsDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new KladrRegionsDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_region"] };
        }
    }
}
