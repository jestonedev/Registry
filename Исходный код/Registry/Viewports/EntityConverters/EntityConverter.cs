using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.Viewport.EntityConverters
{
    internal class EntityConverter<T>
        where T: Entity
    {
        protected static PropertyInfo[] Properties
        {
            get { return typeof(T).GetProperties(); }
        }

        public static T FromRow(DataRow row)
        {
            var instance = Activator.CreateInstance(typeof(T));
            foreach (var property in Properties)
            {
                var attribute = property.GetCustomAttributes(false).OfType<DataColumnAttribute>().FirstOrDefault();
                var columnName = ConvertPropertyToColumnName(property.Name);
                if (attribute != null && attribute.Name != null)
                {
                    columnName = attribute.Name;
                }
                var value = row[columnName];
                if (value == DBNull.Value)
                {
                    value = null;
                }
                if (value != null)
                {
                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    value = Convert.ChangeType(value, type);
                }
                property.SetValue(instance, value, null);
            }
            return (T) instance;
        }

        public static T FromRow(DataGridViewRow row)
        {
            var instance = Activator.CreateInstance(typeof(T));
            foreach (var property in Properties)
            {
                var attribute = property.GetCustomAttributes(false).OfType<DataColumnAttribute>().FirstOrDefault();
                var columnName = ConvertPropertyToColumnName(property.Name);
                if (attribute != null && attribute.Name != null)
                {
                    columnName = attribute.Name;
                }
                var value = row.Cells[columnName].Value;
                if (value == DBNull.Value)
                {
                    value = null;
                }
                if (value != null)
                {
                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    value = Convert.ChangeType(value, type);
                }
                property.SetValue(instance, value, null);
            }
            return (T)instance;
        }

        public static T FromRow(DataRowView row)
        {
            return FromRow(row.Row);
        }

        public static void FillRow(T entity, DataRow row)
        {
            row.BeginEdit();
            foreach (var property in Properties)
            {
                var attribute = property.GetCustomAttributes(false).OfType<DataColumnAttribute>().FirstOrDefault();
                var columnName = ConvertPropertyToColumnName(property.Name);
                if (attribute != null && attribute.Name != null)
                {
                    columnName = attribute.Name;
                }
                row[columnName] = ViewportHelper.ValueOrDbNull(property.GetValue(entity, null));
            }
            row.EndEdit();
        }

        public static void FillRow(T entity, DataRowView row)
        {
            row.BeginEdit();
            foreach (var property in Properties)
            {
                var attribute = property.GetCustomAttributes(false).OfType<DataColumnAttribute>().FirstOrDefault();
                var columnName = ConvertPropertyToColumnName(property.Name);
                if (attribute != null && attribute.Name != null)
                {
                    columnName = attribute.Name;
                }
                row[columnName] = ViewportHelper.ValueOrDbNull(property.GetValue(entity, null));
            }
            row.EndEdit();
        }

        public static object[] ToArray(DataRow row)
        {
            return(object[]) row.ItemArray.Clone();
        }

        public static object[] ToArray(DataRowView row)
        {
            return ToArray(row.Row);
        }

        private static string ConvertPropertyToColumnName(string propertyName)
        {
            return string.IsNullOrEmpty(propertyName) ?
                propertyName :
                Regex.Replace(propertyName, "([A-Z])", "_$0").TrimStart('_').ToLowerInvariant();
        }
    }
}
