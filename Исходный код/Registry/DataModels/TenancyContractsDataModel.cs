using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class TenancyContractsDataModel: DataModel
    {
        private static TenancyContractsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_contracts WHERE deleted = 0";
        private static string tableName = "tenancy_contracts";

        private TenancyContractsDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static TenancyContractsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyContractsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyContractsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
