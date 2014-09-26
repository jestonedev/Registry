using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Registry.DataModels
{
    public abstract class DataModel
    {
        protected DataTable table = null;
        protected DBConnection connection = DBConnection.GetInstance();

        public virtual DataTable Select()
        {
            return table;
        }
    }
}
