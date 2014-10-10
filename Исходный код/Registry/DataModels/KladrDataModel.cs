using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class KladrDataModel : DataModel
    {
        private static KladrDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM v_kladr_streets";
        private static string tableName = "kladr";

        private KladrDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_street"] };
        }


        public static KladrDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static KladrDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new KladrDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
