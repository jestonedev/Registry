using System;

namespace Registry.DataModels.DataModels
{
    public class ObjectStatesDataModel : DataModel
    {
        private static ObjectStatesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM object_states";
        private const string TableName = "object_states";

        private ObjectStatesDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        public static ObjectStatesDataModel GetInstance(Action afterLoadHandler)
        {
            return _dataModel ?? (_dataModel = new ObjectStatesDataModel(afterLoadHandler));
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new [] { Table.Columns["id_state"] };
        }

        protected override void ConfigureRelations()
        {
            AddRelation(TableName, "id_state", "premises", "id_state");
            AddRelation(TableName, "id_state", "sub_premises", "id_state");
            AddRelation(TableName, "id_state", "buildings", "id_state");
        }
    }
}
