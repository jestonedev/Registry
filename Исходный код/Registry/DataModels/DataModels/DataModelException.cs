using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.DataModels
{
    [Serializable]
    public class DataModelException: Exception
    {
        public DataModelException(string message)
            : base(message)
        {
        }
    }
}
