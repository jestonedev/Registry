using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class FundsHistoryDataModel: DataModel
    {
        private static FundsHistoryDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM funds_history WHERE deleted = 0";
        private static string tableName = "funds_history";

        private FundsHistoryDataModel()
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

        public static FundsHistoryDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new FundsHistoryDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
