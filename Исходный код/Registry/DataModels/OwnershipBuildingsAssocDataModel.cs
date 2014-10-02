using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public sealed class OwnershipBuildingsAssocDataModel : DataModel
    {
        private static OwnershipBuildingsAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_buildings_assoc WHERE deleted = 0";
        private static string tableName = "ownership_buildings_assoc";

        private OwnershipBuildingsAssocDataModel()
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


        public static OwnershipBuildingsAssocDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new OwnershipBuildingsAssocDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
