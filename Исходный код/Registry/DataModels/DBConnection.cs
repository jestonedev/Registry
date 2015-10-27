using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Settings;

namespace Registry.DataModels
{
    public sealed class DBConnection: IDisposable
    {
        private const string ProviderName = "ODBC";
        private static readonly DbProviderFactory Factory = DbProviderFactories.GetFactory(ParseProviderName(ProviderName));

        private DbTransaction _transaction;
        private readonly DbConnection _connection;

        public DBConnection()
        {
            _connection = Factory.CreateConnection();
            if (_connection == null) return;
            _connection.ConnectionString = RegistrySettings.ConnectionString;
            if (_connection.State != ConnectionState.Closed) return;
            try
            {
                _connection.Open();
            }
            catch(OdbcException e)
            {
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                    "Произошла ошибка при установке соединения с базой данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Application.Exit();
            }
        }

        public static DbCommand CreateCommand()
        {
            return Factory.CreateCommand();
        }

        public static DbParameter CreateParameter<T>(string name, T value) 
        {
            var parameter = Factory.CreateParameter();
            if (parameter == null) return null;
            parameter.ParameterName = name;
            parameter.Value = value == null ? DBNull.Value : (object)value;
            return parameter;
        }

        private static string ParseProviderName(string name)
        {
            var dt = DbProviderFactories.GetFactoryClasses();
            var providers = (from DataRow row in dt.Rows select row["InvariantName"].ToString()).ToList();
            foreach (var provider in providers.Where(provider => Regex.IsMatch(provider, name, RegexOptions.IgnoreCase)))
            {
                return provider;
            }
            throw new DataModelException(string.Format(CultureInfo.InvariantCulture, "Провайдер {0} не найден", name));
        }

        public DataTable SqlSelectTable(string resultTableName, DbCommand command)
        {
            if (command == null)
                throw new DataModelException("Не передана ссылка на исполняемую команду SQL");
            command.Connection = _connection;
            if (_transaction != null)
                command.Transaction = _transaction;
            if (_connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение с базой данных прервано по неизвестным причинам");
            var adapter = Factory.CreateDataAdapter();
            if (adapter == null) return null;
            adapter.SelectCommand = command;
            var dt = new DataTable(resultTableName) {Locale = CultureInfo.InvariantCulture};
            adapter.Fill(dt);
            return dt;
        }

        public int SqlModifyQuery(DbCommand command)
        {
            if (command == null)
                throw new DataModelException("Не передана ссылка на исполняемую команду SQL");
            command.Connection = _connection;
            if (_transaction != null)
                command.Transaction = _transaction;
            if (_connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение прервано по неизвестным причинам");
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (OdbcException)
            {
                SqlRollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// Начать выполнение транзакции
        /// </summary>
        public void SqlBeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// Подтверждение транзакции
        /// </summary>
        public void SqlCommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        /// <summary>
        /// Откат транзакции
        /// </summary>
        public void SqlRollbackTransaction()
        {
            if (_transaction == null) return;
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
