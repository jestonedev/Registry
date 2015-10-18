using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public sealed class TenancyNotifiesDataModel: DataModel
    {
        private static TenancyNotifiesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_notifies WHERE deleted <> 1";
        private static string tableName = "tenancy_notifies";

        private TenancyNotifiesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_notify"] };
        }

        public static TenancyNotifiesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyNotifiesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyNotifiesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
