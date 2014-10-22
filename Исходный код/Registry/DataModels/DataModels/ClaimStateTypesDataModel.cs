using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class ClaimStateTypesDataModel: DataModel
    {
        private static ClaimStateTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claim_state_types";

        private static string tableName = "claim_state_types";

        private ClaimStateTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_state_type"] };
        }

        public static ClaimStateTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimStateTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimStateTypesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
