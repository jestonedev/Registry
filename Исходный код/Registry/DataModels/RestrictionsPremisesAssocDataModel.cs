using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public sealed class RestrictionsPremisesAssocDataModel : DataModel
    {
        private static RestrictionsPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restrictions_premises_assoc WHERE deleted = 0";
        private static string tableName = "restrictions_premises_assoc";

        private RestrictionsPremisesAssocDataModel()
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

        public static RestrictionsPremisesAssocDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new RestrictionsPremisesAssocDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
