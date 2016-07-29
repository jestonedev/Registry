using System;
using System.Linq;

namespace Registry.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataTableAttribute : Attribute
    {
        public string Name { get; set; }
        public bool HasDeletedMark { get; set; }
        public string CustomSelectQuery { get; set; }
    }
}
