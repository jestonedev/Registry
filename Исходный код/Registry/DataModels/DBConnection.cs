using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using Registry.DataModels.Properties;

namespace Registry.DataModels
{
    public class DBConnection: IDisposable
    {
        private static string ProviderName = "ODBC";
        private static System.Data.Common.DbConnection connection = null;
        private static DbProviderFactory factory = null;

        private System.Data.Common.DbTransaction transaction = null;

        private DBConnection()
        {
        }

        static DBConnection()
        {
            if (factory == null)
                factory = System.Data.Common.DbProviderFactories.GetFactory(ParseProviderName(ProviderName));
            connection = factory.CreateConnection();
            connection.ConnectionString = Settings.Default["ConnectionString"].ToString();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        public DbCommand CreateCommand()
        {
            DbCommand command = factory.CreateCommand();
            command.Connection = connection;
            return command;
        }

        public DbParameter CreateParameter()
        {
            return factory.CreateParameter();
        }

        public static DBConnection GetInstance()
        {
            return new DBConnection();
        }

        private static string ParseProviderName(string name)
        {
            DataTable dt = DbProviderFactories.GetFactoryClasses();
            List<string> providers = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                providers.Add(row["InvariantName"].ToString());
            }
            foreach (string provider in providers)
            {
                if (Regex.IsMatch(provider, name, RegexOptions.IgnoreCase))
                    return provider;
            }
            throw new DataModelException(String.Format("Провайдер {0} не найден", name));
        }

        public DataTable SqlSelectTable(string resultTableName, DbCommand command)
        {
            if (transaction != null)
                command.Transaction = transaction;
            if (connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение с базой данных прервано по неизвестным причинам");
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            DataTable dt = new DataTable(resultTableName);
            adapter.Fill(dt);
            return dt;
        }

        public int SqlModifyQuery(DbCommand command)
        {
            if (transaction != null)
                command.Transaction = transaction;
            if (connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение прервано по неизвестным причинам");
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (InvalidOperationException e)
            {
                SqlRollbackTransaction();
                throw e;
            }
        }

        /// <summary>
        /// Начать выполнение транзакции
        /// </summary>
        public void SqlBeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }

        /// <summary>
        /// Подтверждение транзакции
        /// </summary>
        public void SqlCommitTransaction()
        {
            transaction.Commit();
            transaction.Dispose();
            transaction = null;
        }

        /// <summary>
        /// Откат транзакции
        /// </summary>
        public void SqlRollbackTransaction()
        {
            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
