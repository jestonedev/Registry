using System.Reflection;

namespace Registry.DataModels.DataMapping
{
    public sealed class ColumnMapping
    {
        public string DbColumn { get; set; }
        public PropertyInfo EntityColumn { get; set; }
        public object DefaultValue { get; set; }
    }
}
