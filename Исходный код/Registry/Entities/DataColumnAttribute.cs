using System;

namespace Registry.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public object DefaultValue { get; set; }
        public bool IncludeIntoUpdate { get; set; }
    }
}
