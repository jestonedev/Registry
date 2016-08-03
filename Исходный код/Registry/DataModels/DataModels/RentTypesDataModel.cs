using System;

namespace Registry.DataModels.DataModels
{
    public sealed class RentTypesDataModel : DataModel
    {
        private static RentTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM rent_types rt ORDER BY rt.rent_type DESC";
        private const string TableName = "rent_types";

        private RentTypesDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static RentTypesDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new RentTypesDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_rent_type"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_rent_type", "tenancy_processes", "id_rent_type");
        }
    }
}
