using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Registry.DataModels
{
    public sealed class FundTypesDataModel : DataModel
    {
        private static FundTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM fund_types";
        private static string tableName = "fund_types";

        private FundTypesDataModel()
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;
            table = connection.SqlSelectTable(tableName, command);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_fund_type"] };
            table.RowDeleted += new DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static FundTypesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new FundTypesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
