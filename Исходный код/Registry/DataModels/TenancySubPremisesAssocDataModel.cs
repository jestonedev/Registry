using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class TenancySubPremisesAssocDataModel: DataModel
    {
        private static TenancySubPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM tenancy_sub_premises_assoc WHERE deleted = 0";
        private static string tableName = "tenancy_sub_premises_assoc";

        private TenancySubPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static TenancySubPremisesAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static TenancySubPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new TenancySubPremisesAssocDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
