using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class FundsPremisesAssocDataModel : DataModel
    {
        private static FundsPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM funds_premises_assoc WHERE deleted = 0";
        private static string tableName = "funds_premises_assoc";

        private FundsPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_fund"] };
        }

        public static FundsPremisesAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static FundsPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new FundsPremisesAssocDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
