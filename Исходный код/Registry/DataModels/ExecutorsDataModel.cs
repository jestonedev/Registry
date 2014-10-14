using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class ExecutorsDataModel: DataModel
    {
        private static ExecutorsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM executors WHERE deleted = 0";
        private static string tableName = "executors";

        private ExecutorsDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static ExecutorsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ExecutorsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ExecutorsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
