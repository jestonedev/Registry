using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class ClaimStateTypesRelationsDataModel: DataModel
    {
        private static ClaimStateTypesRelationsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claim_state_types_relations";

        private static string tableName = "claim_state_types_relations";

        private ClaimStateTypesRelationsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {  
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_state_from"], table.Columns["id_state_to"] };
        }

        public static ClaimStateTypesRelationsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimStateTypesRelationsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimStateTypesRelationsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
