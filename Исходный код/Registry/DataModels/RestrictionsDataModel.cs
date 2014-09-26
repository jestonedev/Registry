using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class RestrictionsDataModel: DataModel
    {
        private static RestrictionsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restrictions";
        private static string tableName = "restrictions";

        private RestrictionsDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
        }


        public static RestrictionsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new RestrictionsDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
