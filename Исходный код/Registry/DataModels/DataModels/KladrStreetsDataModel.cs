using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class KladrStreetsDataModel : DataModel
    {
        private static KladrStreetsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM v_kladr_streets";
        private static string tableName = "kladr";

        private KladrStreetsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_street"] };
        }


        public static KladrStreetsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static KladrStreetsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new KladrStreetsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
