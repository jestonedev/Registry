using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Registry.DataModels
{
    public sealed class OwnershipPremisesAssocDataModel : DataModel
    {
        private static OwnershipPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_premises_assoc WHERE deleted = 0";
        private static string tableName = "ownership_premises_assoc";

        private OwnershipPremisesAssocDataModel()
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


        public static OwnershipPremisesAssocDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new OwnershipPremisesAssocDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
