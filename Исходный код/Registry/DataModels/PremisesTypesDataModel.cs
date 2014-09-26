using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class PremisesTypesDataModel: DataModel
    {
        private static PremisesTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM premises_types";
        private static string tableName = "premises_types";

        private PremisesTypesDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
        }

        public static PremisesTypesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new PremisesTypesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
