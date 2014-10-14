using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public sealed class PersonsDataModel: DataModel
    {
        private static PersonsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM persons WHERE deleted = 0";
        private static string tableName = "persons";

        private PersonsDataModel(ToolStripProgressBar progressBar, int incrementor)
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

        public static PersonsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static PersonsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new PersonsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
