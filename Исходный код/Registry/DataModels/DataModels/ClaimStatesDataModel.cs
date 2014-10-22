using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class ClaimStatesDataModel: DataModel
    {
        private static ClaimStatesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claim_states c WHERE deleted = 0";

        private static string tableName = "claim_states";

        public bool EditingNewRecord { get; set; }

        private ClaimStatesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_state"] };
            table.Columns["date_start_state"].DefaultValue = DateTime.Now.Date;
            table.Columns["date_end_state"].DefaultValue = DateTime.Now.Date;
            table.Columns["document_date"].DefaultValue = DateTime.Now.Date;
        }

        public static ClaimStatesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimStatesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimStatesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
