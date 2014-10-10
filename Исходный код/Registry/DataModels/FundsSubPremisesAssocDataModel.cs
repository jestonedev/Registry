using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class FundsSubPremisesAssocDataModel : DataModel
    {
        private static FundsSubPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM funds_sub_premises_assoc WHERE deleted = 0";
        private static string tableName = "funds_sub_premises_assoc";

        private FundsSubPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static FundsSubPremisesAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static FundsSubPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new FundsSubPremisesAssocDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
