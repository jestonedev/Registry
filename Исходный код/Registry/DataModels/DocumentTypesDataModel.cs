using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class DocumentTypesDataModel: DataModel
    {
        private static DocumentTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM document_types";
        private static string tableName = "document_types";

        private DocumentTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static DocumentTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static DocumentTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new DocumentTypesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
