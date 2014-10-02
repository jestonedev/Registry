using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Registry.DataModels
{
    public sealed class KladrDataModel : DataModel
    {
        private static KladrDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM v_kladr_streets";
        private static string tableName = "kladr";

        private KladrDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_street"] };
        }


        public static KladrDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new KladrDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
