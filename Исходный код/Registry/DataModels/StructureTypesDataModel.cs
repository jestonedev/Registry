using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Text;

namespace Registry.DataModels
{
    public class StructureTypesDataModel: DataModel
    {
        private static StructureTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM structure_types";
        private static string tableName = "structure_types";

        private StructureTypesDataModel()
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

        public static StructureTypesDataModel GetInstance()
        {
            if (dataModel == null)
                dataModel = new StructureTypesDataModel();
            DataSetManager.AddModel(dataModel);
            return dataModel;
        }
    }
}
