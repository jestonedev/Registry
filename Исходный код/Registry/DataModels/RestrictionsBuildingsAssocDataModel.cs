﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class RestrictionsBuildingsAssocDataModel : DataModel
    {        
        private static RestrictionsBuildingsAssocDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM restrictions_buildings_assoc WHERE deleted = 0";
        private static string tableName = "restrictions_buildings_assoc";

        private RestrictionsBuildingsAssocDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.RowDeleted += new System.Data.DataRowChangeEventHandler(table_RowDeleted);
        }

        void table_RowDeleted(object sender, System.Data.DataRowChangeEventArgs e)
        {
            table.AcceptChanges();
        }

        public static RestrictionsBuildingsAssocDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static RestrictionsBuildingsAssocDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new RestrictionsBuildingsAssocDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
