using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class RestrictionsBuildingsAssocDataModel: DataModel
    {        
        private static RestrictionsBuildingsAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restrictions_buildings_assoc WHERE deleted = 0";
        private static string tableName = "restrictions_buildings_assoc";

        private RestrictionsBuildingsAssocDataModel()
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

        public static RestrictionsBuildingsAssocDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new RestrictionsBuildingsAssocDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
