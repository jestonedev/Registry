using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public class StatesDataModel: DataModel
    {
        private static StatesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM states";
        private static string tableName = "states";

        private StatesDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static StatesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static StatesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new StatesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
