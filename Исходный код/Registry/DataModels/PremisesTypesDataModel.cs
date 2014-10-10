using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class PremisesTypesDataModel : DataModel
    {
        private static PremisesTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM premises_types";
        private static string tableName = "premises_types";

        private PremisesTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {          
        }

        protected override void ConfigureTable()
        {
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static PremisesTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static PremisesTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new PremisesTypesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
