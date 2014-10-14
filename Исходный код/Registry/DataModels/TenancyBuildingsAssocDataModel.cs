using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class TenancyBuildingsAssocDataModel: DataModel
    {
        private static TenancyBuildingsAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_buildings_assoc WHERE deleted = 0";
        private static string tableName = "tenancy_buildings_assoc";

        private TenancyBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static TenancyBuildingsAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancyBuildingsAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancyBuildingsAssocDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
