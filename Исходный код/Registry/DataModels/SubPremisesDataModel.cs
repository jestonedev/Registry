using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public class SubPremisesDataModel: DataModel
    {
        private static SubPremisesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM sub_premises WHERE deleted = 0";
        private static string tableName = "sub_premises";

        private SubPremisesDataModel()
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

        public static SubPremisesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new SubPremisesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
