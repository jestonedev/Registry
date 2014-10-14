using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class ContractReasonsDataModel: DataModel
    {
        private static ContractReasonsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM contract_reasons WHERE deleted = 0";
        private static string tableName = "contract_reasons";

        private ContractReasonsDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static ContractReasonsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ContractReasonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ContractReasonsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
