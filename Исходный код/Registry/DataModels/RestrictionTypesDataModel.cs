using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class RestrictionTypesDataModel: DataModel
    {
        private static RestrictionTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restriction_types";
        private static string tableName = "restriction_types";

        private RestrictionTypesDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }


        public static RestrictionTypesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new RestrictionTypesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
