using System;

namespace Registry.Entities.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RelationAttribute : Attribute
    {
        public string MasterTableName { get; set; }
        public string MasterFieldName { get; set; }
        public string SlaveTableName { get; set; }
        public string SlaveFieldName { get; set; }
    }
}
