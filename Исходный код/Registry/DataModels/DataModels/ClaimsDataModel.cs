using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class ClaimsDataModel: DataModel
    {
        private static ClaimsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM claims c WHERE deleted = 0";

        private static string tableName = "claims";

        public bool EditingNewRecord { get; set; }

        private ClaimsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_claim"] };
            table.Columns["amount_of_debt_rent"].DefaultValue = 0;
            table.Columns["amount_of_debt_fine"].DefaultValue = 0;
            table.Columns["amount_of_rent"].DefaultValue = 0;
            table.Columns["amount_of_fine"].DefaultValue = 0;
            table.Columns["amount_of_rent_recover"].DefaultValue = 0;
            table.Columns["amount_of_fine_recover"].DefaultValue = 0;
        }

        public static ClaimsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ClaimsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ClaimsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
