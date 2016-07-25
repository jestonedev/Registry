using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.DataModels.DataMapping
{
    public class RelationMapping
    {
        public string MasterTableName { get; set; }
        public string MasterColumnName { get; set; }
        public string SlaveTableName { get; set; }
        public string SlaveColumnName { get; set; }
    }
}
