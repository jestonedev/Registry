using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class FundsBuildingsAssocDataModel: DataModel
    {
        private static FundsBuildingsAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM funds_buildings_assoc WHERE deleted = 0";
        private static string tableName = "funds_buildings_assoc";

        private FundsBuildingsAssocDataModel()
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

        public static FundsBuildingsAssocDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new FundsBuildingsAssocDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
