using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class OwnershipBuildingsAssocDataModel : DataModel
    {
        private static OwnershipBuildingsAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM ownership_buildings_assoc WHERE deleted = 0";
        private static string tableName = "ownership_buildings_assoc";

        private OwnershipBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_ownership_right"] };
        }

        public static OwnershipBuildingsAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static OwnershipBuildingsAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new OwnershipBuildingsAssocDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
