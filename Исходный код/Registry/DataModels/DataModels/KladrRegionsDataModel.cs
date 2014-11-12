using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class KladrRegionsDataModel : DataModel
    {
        private static KladrRegionsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM v_kladr_regions";
        private static string tableName = "kladr_regions";

        private KladrRegionsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_region"] };
        }


        public static KladrRegionsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static KladrRegionsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new KladrRegionsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
