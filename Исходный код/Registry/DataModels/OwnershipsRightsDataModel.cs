using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class OwnershipsRightsDataModel: DataModel
    {
        private static OwnershipsRightsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_rights";
        private static string tableName = "ownership_rights";

        private OwnershipsRightsDataModel()
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

        public static OwnershipsRightsDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new OwnershipsRightsDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
