using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class FundsPremisesAssocDataModel: DataModel
    {
        private static FundsPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM funds_premises_assoc WHERE deleted = 0";
        private static string tableName = "funds_premises_assoc";

        private FundsPremisesAssocDataModel()
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

        public static FundsPremisesAssocDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new FundsPremisesAssocDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
