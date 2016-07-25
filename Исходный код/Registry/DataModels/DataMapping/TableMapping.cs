using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.DataModels.DataMapping
{
    public abstract class TableMapping
    {
        public virtual string TableName { get; set; }

        public abstract List<ColumnMapping> GetPrimaryKeyColumnsMapping();

        public abstract List<ColumnMapping> GetColumnsMapping();

        public abstract List<RelationMapping> GetRelationsMapping();

        public virtual string GetSelectQuery()
        {
            return "";
        }

        public virtual string GetInsertQuery()
        {
            return "";
        }

        public virtual string GetUpdateQuery()
        {
            return "";
        }

        public virtual string GetDeleteQuery()
        {
            return "";
        }
    }
}
