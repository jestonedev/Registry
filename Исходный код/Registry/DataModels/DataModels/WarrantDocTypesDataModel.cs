﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public sealed class WarrantDocTypesDataModel: DataModel
    {
        private static WarrantDocTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM warrant_doc_types";
        private static string tableName = "warrant_doc_types";

        private WarrantDocTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_warrant_doc_type"] };
        }


        public static WarrantDocTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static WarrantDocTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new WarrantDocTypesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
