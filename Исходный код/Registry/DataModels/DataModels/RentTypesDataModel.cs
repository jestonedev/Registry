using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public sealed class RentTypesDataModel: DataModel
    {
        private static RentTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM rent_types rt ORDER BY rt.rent_type DESC";
        private static string tableName = "rent_types";

        private RentTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["id_rent_type"] };
        }

        public static RentTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static RentTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            if (dataModel == null)
                dataModel = new RentTypesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
