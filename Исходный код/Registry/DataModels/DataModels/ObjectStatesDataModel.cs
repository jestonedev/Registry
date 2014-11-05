using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Windows.Forms;
using System.Data;

namespace Registry.DataModels
{
    public class ObjectStatesDataModel: DataModel
    {
        private static ObjectStatesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM object_states";
        private static string tableName = "object_states";

        private ObjectStatesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            table.PrimaryKey = new DataColumn[] { table.Columns["id_state"] };
        }

        public static ObjectStatesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static ObjectStatesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new ObjectStatesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
