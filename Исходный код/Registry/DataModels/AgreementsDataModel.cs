using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class AgreementsDataModel: DataModel
    {
        private static AgreementsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM agreements WHERE deleted = 0";
        private static string tableName = "agreements";

        private AgreementsDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static AgreementsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static AgreementsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new AgreementsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
