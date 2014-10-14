using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class ReasonTypesDataModel: DataModel
    {
        private static ReasonTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM reason_types";
        private static string tableName = "reason_types";

        private ReasonTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static ReasonTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ReasonTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ReasonTypesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
