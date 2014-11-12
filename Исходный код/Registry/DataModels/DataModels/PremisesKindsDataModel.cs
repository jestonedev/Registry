using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class PremisesKindsDataModel : DataModel
    {
        private static PremisesKindsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM premises_kinds";
        private static string tableName = "premises_kinds";

        private PremisesKindsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_premises_kind"] };
        }

        public static PremisesKindsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static PremisesKindsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new PremisesKindsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
