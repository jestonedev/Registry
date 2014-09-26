using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class PremisesKindsDataModel: DataModel
    {
        private static PremisesKindsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM premises_kinds";
        private static string tableName = "premises_kinds";

        private PremisesKindsDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
        }

        public static PremisesKindsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new PremisesKindsDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
