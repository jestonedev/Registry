using System.Data;
using Registry.DataModels.DataModels;

namespace Registry.DataModels
{
    public static class DataStorage
    {
        public static DataSet DataSet { get; private set; }

        static DataStorage()
        {
            DataSet = new DataSet();
        }

        public static bool ContainTable(DataTable table)
        {
            return ContainTable(table.TableName);
        }

        public static bool ContainTable(string tableName)
        {
            return DataSet.Tables.Contains(tableName);
        }

        public static void AddTable(DataTable table)
        {
            DataSet.Tables.Add(table);
        }

        public static DataModel GetDataModel(string tableName)
        {
            return DataSet.Tables[tableName].ExtendedProperties["model"] as DataModel;
        }

        public static bool ContainRelation(string relationName)
        {
            return DataSet.Relations.Contains(relationName);
        }
    }
}
