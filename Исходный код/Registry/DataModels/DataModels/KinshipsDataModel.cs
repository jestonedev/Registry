using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class KinshipsDataModel: DataModel
    {
        private static KinshipsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM kinships";
        private static string tableName = "kinships";

        private KinshipsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_kinship"] };
        }

        public static KinshipsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static KinshipsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new KinshipsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
