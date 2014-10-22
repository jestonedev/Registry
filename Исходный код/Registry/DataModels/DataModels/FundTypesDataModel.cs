using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class FundTypesDataModel : DataModel
    {
        private static FundTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM fund_types";
        private static string tableName = "fund_types";

        private FundTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_fund_type"] };
        }

        public static FundTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static FundTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new FundTypesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
