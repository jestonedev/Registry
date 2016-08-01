using System;

namespace Registry.DataModels.DataModels
{
    internal sealed class KladrStreetsDataModel : DataModel
    {
        private static KladrStreetsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM v_kladr_streets";
        private const string TableName = "kladr";

        private KladrStreetsDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static KladrStreetsDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new KladrStreetsDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_street"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_street", "buildings", "id_street");
        }
    }
}
