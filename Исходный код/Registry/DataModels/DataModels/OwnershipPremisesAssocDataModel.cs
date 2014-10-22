using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class OwnershipPremisesAssocDataModel : DataModel
    {
        private static OwnershipPremisesAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_premises_assoc WHERE deleted = 0";
        private static string tableName = "ownership_premises_assoc";

        private OwnershipPremisesAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
        }

        public static OwnershipPremisesAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static OwnershipPremisesAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new OwnershipPremisesAssocDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
