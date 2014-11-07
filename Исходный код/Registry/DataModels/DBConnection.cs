using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.Odbc;
using Registry.Entities;
using System.Windows.Forms;

namespace Registry.DataModels
{
    public class DBConnection: IDisposable
    {
        private static string ProviderName = "ODBC";
        private static DbProviderFactory factory = null;

        private System.Data.Common.DbTransaction transaction = null;
        private DbConnection connection = null;

        public DBConnection()
        {
            connection = factory.CreateConnection();
            connection.ConnectionString = RegistrySettings.ConnectionString;
            if (connection.State == System.Data.ConnectionState.Closed)
                try
                {
                    connection.Open();
                }
                catch(OdbcException e)
                {
                    MessageBox.Show(String.Format("Произошла ошибка при установке соединения с базой данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
        }

        static DBConnection()
        {
            if (factory == null)
                factory = System.Data.Common.DbProviderFactories.GetFactory(ParseProviderName(ProviderName));
        }

        public DbCommand CreateCommand()
        {
            return factory.CreateCommand();
        }

        public DbParameter CreateParameter<T>(string name, T value) 
        {
            DbParameter parameter = factory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value == null ? DBNull.Value : (Object)value;
            return parameter;
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
            command.Connection = connection;
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
            command.Connection = connection;
            if (transaction != null)
                command.Transaction = transaction;
            if (connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение прервано по неизвестным причинам");
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (OdbcException e)
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
            if (transaction != null)
            {
                transaction.Rollback();
                transaction.Dispose();
                transaction = null;
            }
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
