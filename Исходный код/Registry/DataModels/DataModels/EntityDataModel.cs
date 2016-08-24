using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Registry.Entities;
using Registry.Entities.Infrastructure;

namespace Registry.DataModels.DataModels
{
    public class EntityDataModel<T>: DataModel
        where T: Entity
    {
        private static EntityDataModel<T> _dataModel;
        private static readonly object LockObj = new object();

        protected static string SelectQuery
        {
            get
            {
                var attribute = typeof(T).GetCustomAttributes(false).OfType<DataTableAttribute>().FirstOrDefault();
                if (attribute == null)
                {
                    throw new DataModelException(string.Format("Не указан обязательный атрибут DataTableAttribute у класса {0}", typeof(T)));
                }
                var customSqlQuery = attribute.CustomSelectQuery;
                if (!string.IsNullOrEmpty(customSqlQuery))
                {
                    return customSqlQuery;
                }
                var tableName = TableName;
                var hasDeletedMark = attribute.HasDeletedMark;
                var template = "SELECT * FROM {0}";
                if (hasDeletedMark)
                {
                    template += " WHERE deleted <> 1";
                }
                return string.Format(template, tableName);
            }
        }

        protected static string TableName
        {
            get
            {
                var attribute = typeof(T).GetCustomAttributes(false).OfType<DataTableAttribute>().FirstOrDefault();
                if (attribute == null)
                {
                    throw new DataModelException(string.Format("Не указан обязательный атрибут DataTableAttribute у класса {0}", typeof(T)));
                }
                if (string.IsNullOrEmpty(attribute.Name))
                {
                    throw new DataModelException(string.Format("Не указан обязательный параметр Name атрибута DataTableAttribute у класса {0}", typeof(T)));
                }
                return attribute.Name;
            }
        }

        private static PropertyInfo PrimaryKeyProperty
        {
            get
            {
                var properties = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(false).
                    OfType<DataColumnAttribute>().Any(a => a.IsPrimaryKey)).ToList();
                if (properties.Count != 1)
                {
                    throw new DataModelException(string.Format("Параметр IsPrimaryKey должен быть установлен ровно у одного свойства класса {0}", typeof(T)));
                }
                return typeof(T).GetProperties().First(p => p.GetCustomAttributes(false).
                    OfType<DataColumnAttribute>().Any(a => a.IsPrimaryKey));
            }
        }

        private static IEnumerable<PropertyInfo> NonPrimaryKeyProperty
        {
            get
            {
                return typeof(T).GetProperties().Except(new List<PropertyInfo> { PrimaryKeyProperty });
            }
        }

        protected EntityDataModel(Action afterLoadHandler)
            : base(SelectQuery, TableName, afterLoadHandler)
        {
        }

        protected EntityDataModel()
        {
        }

        public static EntityDataModel<T> GetInstance(Action afterLoadHandler)
        {
            lock (LockObj)
            {
                return _dataModel ?? (_dataModel = new EntityDataModel<T>(afterLoadHandler));
            }
        }

        public static EntityDataModel<T> GetInstance()
        {
            lock (LockObj)
            {
                return _dataModel ?? (_dataModel = new EntityDataModel<T>(null));
            }
        }

        private static string ConvertPropertyToColumnName(string propertyName)
        {
            return string.IsNullOrEmpty(propertyName) ? 
                propertyName : 
                Regex.Replace(propertyName, "([A-Z])", "_$0").TrimStart('_').ToLowerInvariant();
        }

        protected override void ConfigureTable()
        {
            var columnName = PrimaryKeyProperty.GetCustomAttributes(false).OfType<DataColumnAttribute>().First().Name ??
                             ConvertPropertyToColumnName(PrimaryKeyProperty.Name);
            Table.PrimaryKey = new[] { Table.Columns[columnName] };
            var propertiesWithDefaults =
                typeof(T).GetProperties()
                    .Where(
                        p => p.GetCustomAttributes(false).OfType<DataColumnAttribute>().Any(a => a.DefaultValue != null));
            foreach (var property in propertiesWithDefaults)
            {
                var attribute = property.GetCustomAttributes(false).OfType<DataColumnAttribute>().First();
                Table.Columns[attribute.Name ?? ConvertPropertyToColumnName(property.Name)].DefaultValue =
                    attribute.DefaultValue;
            }
        }

        protected override void ConfigureRelations()
        {
            var relations = typeof(T).GetCustomAttributes(false).OfType<RelationAttribute>();
            var primaryKey = PrimaryKeyProperty.GetCustomAttributes(false).OfType<DataColumnAttribute>().First().Name ?? 
                ConvertPropertyToColumnName(PrimaryKeyProperty.Name);
            foreach (var relation in relations)
            {
                if (relation.MasterTableName == null && relation.SlaveTableName == null)
                {
                    throw new DataModelException(string.Format("В атрибуте RelationAttribute класса {0} необходимо указать как минимум одно из свойств MasterTableName или SlaveTableName", typeof(T)));
                }
                AddRelation(relation.MasterTableName ?? TableName,
                    relation.MasterFieldName ?? primaryKey, 
                    relation.SlaveTableName ?? TableName, 
                    relation.SlaveFieldName ?? relation.MasterFieldName ?? primaryKey);
            }
        }

        protected override void ConfigureDeleteCommand(DbCommand command, int id)
        {
            var attribute = typeof(T).GetCustomAttributes(false).OfType<DataTableAttribute>().FirstOrDefault();
            if (attribute == null)
            {
                throw new DataModelException(string.Format("Не указан обязательный атрибут DataTableAttribute у класса {0}", typeof(T)));
            }
            var commandText = "UPDATE {0} SET deleted = 1 WHERE {1} = ?";
            if (!attribute.HasDeletedMark)
            {
                commandText = "DELETE FROM {0} WHERE {1} = ?";
            }
            var primaryKey = PrimaryKeyProperty.GetCustomAttributes(false).OfType<DataColumnAttribute>().First().Name ?? 
                ConvertPropertyToColumnName(PrimaryKeyProperty.Name);
            command.CommandText = string.Format(commandText, TableName, primaryKey);
            command.Parameters.Add(DBConnection.CreateParameter(primaryKey, id));
        }

        protected override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            const string commandTemplate = "UPDATE {0} SET {1} WHERE {2}";
            const string columnTemplate = "{0} = ?";
            var setStatement = "";

            foreach (var property in NonPrimaryKeyProperty)
            {
                var attribute = property.GetCustomAttributes(false).OfType<DataColumnAttribute>().FirstOrDefault();
                if (attribute != null && !attribute.IncludeIntoUpdate)
                {
                    continue;
                }
                var columnName = ConvertPropertyToColumnName(property.Name);
                if (attribute != null && attribute.Name != null)
                {
                    columnName = attribute.Name;
                }
                setStatement += string.Format(columnTemplate, columnName) + ",";
                command.Parameters.Add(DBConnection.CreateParameter(columnName, property.GetValue(entity, null)));
            }
            setStatement = setStatement.TrimEnd(',');
            var primaryKeyColumn =
                PrimaryKeyProperty.GetCustomAttributes(false).OfType<DataColumnAttribute>().First().Name ??
                ConvertPropertyToColumnName(PrimaryKeyProperty.Name);
            var whereStatement = string.Format(columnTemplate, primaryKeyColumn);
            command.Parameters.Add(DBConnection.CreateParameter(primaryKeyColumn,
                PrimaryKeyProperty.GetValue(entity, null)));
            command.CommandText = string.Format(commandTemplate, TableName, setStatement, whereStatement);
        }

        protected override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            const string commandTemplate = "INSERT INTO {0} ({1}) VALUES({2})";
            const string columnTemplate = "{0}";
            var intoStatement = "";
            var valuesStatement = "";

            foreach (var property in NonPrimaryKeyProperty)
            {
                var columnName = ConvertPropertyToColumnName(property.Name);
                var attribute = property.GetCustomAttributes(false).OfType<DataColumnAttribute>().FirstOrDefault();
                if (attribute != null && !attribute.IncludeIntoInsert)
                {
                    continue;
                }
                if (attribute != null && attribute.Name != null)
                {
                    columnName = attribute.Name;
                }
                intoStatement += string.Format(columnTemplate, columnName) + ",";
                valuesStatement += "?,";
                command.Parameters.Add(DBConnection.CreateParameter(columnName, property.GetValue(entity, null)));
            }
            if (PrimaryKeyProperty.GetCustomAttributes(false)
                .OfType<DataColumnAttribute>()
                .Any(a => a.IncludeIntoInsert))
            {
                var primaryKeyColumn =
                    PrimaryKeyProperty.GetCustomAttributes(false).OfType<DataColumnAttribute>().First().Name ??
                    ConvertPropertyToColumnName(PrimaryKeyProperty.Name);
                intoStatement += string.Format(columnTemplate, primaryKeyColumn) + ",";
                valuesStatement += "?,";
                command.Parameters.Add(DBConnection.CreateParameter(primaryKeyColumn, PrimaryKeyProperty.GetValue(entity, null)));
            }
            intoStatement = intoStatement.TrimEnd(',');
            valuesStatement = valuesStatement.TrimEnd(',');
            command.CommandText = string.Format(commandTemplate, TableName, intoStatement, valuesStatement);
        }
    }
}
