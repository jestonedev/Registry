using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class OwnershipRightTypesDataModel: DataModel
    {
        private static OwnershipRightTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_right_types";
        private static string tableName = "ownership_right_types";

        private OwnershipRightTypesDataModel()
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

        public static OwnershipRightTypesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new OwnershipRightTypesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
