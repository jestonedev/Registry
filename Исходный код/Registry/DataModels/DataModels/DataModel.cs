using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Registry.Entities;
using Settings;

namespace Registry.DataModels.DataModels
{
    public class DataModel
    {

        private DataModelLoadState _dmLoadState = DataModelLoadState.BeforeLoad;
        public DataModelLoadState DmLoadState { get { return _dmLoadState; } set { _dmLoadState = value; } }

        private DataModelLoadSyncType _dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная
        public DataModelLoadSyncType DmLoadType { get { return _dmLoadType; } set { _dmLoadType = value; } }

        private DataTable _table;
        protected DataTable Table { get { return _table; } set { _table = value; } }

        private static readonly object LockObj = new object();
        // Не больше MaxDBConnectionCount потоков одновременно делают запросы к БД
        private static readonly Semaphore DbAccessSemaphore = new Semaphore(RegistrySettings.MaxDbConnectionCount, RegistrySettings.MaxDbConnectionCount);
        public bool EditingNewRecord { get; set; }

        protected DataModel()
        {
            EditingNewRecord = false;
        }

        protected DataModel(string selectQuery, string tableName, Action afterLoadHandler)
        {
            EditingNewRecord = false;
            var context = SynchronizationContext.Current;
            DmLoadType = DataModelLoadSyncType.Asyncronize;
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    DmLoadState = DataModelLoadState.Loading;
                    using (var connection = new DBConnection())
                    using (var command = DBConnection.CreateCommand())
                    {
                        command.CommandText = selectQuery;
                        DbAccessSemaphore.WaitOne();
                        Interlocked.Exchange(ref _table, connection.SqlSelectTable(tableName, command));
                    }
                    DbAccessSemaphore.Release();
                    ConfigureTable();
                    Table.ExtendedProperties.Add("model", this);
                    lock (LockObj)
                    {
                        if (!DataStorage.ContainTable(Table))
                        {
                            DataStorage.AddTable(Table);
                            ConfigureRelations();
                        }
                        else
                        {
                            Table = DataStorage.DataSet.Tables[tableName];
                        }
                    }
                    DmLoadState = DataModelLoadState.SuccessLoad;
                    if (afterLoadHandler != null)
                    {
                        context.Post(_ =>
                        {
                            afterLoadHandler();
                        }, null);
                    }
                }
                catch (OdbcException e)
                {
                    lock (LockObj)
                    {
                        MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
                            "Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        DmLoadState = DataModelLoadState.ErrorLoad;
                        Application.Exit();
                    }
                }
                catch (DataModelException e)
                {
                    MessageBox.Show(e.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    DmLoadState = DataModelLoadState.ErrorLoad;
                }
            }, null);
        }

        public static DataModel GetLoadedInstance(string tableName)
        {
            DataModel dm = null;
            lock (LockObj)
            {
                if (DataStorage.ContainTable(tableName))
                {
                    dm = DataStorage.GetDataModel(tableName);
                }
            }
            return dm;
        }              

        protected static void AddRelation(string masterTableName, string masterColumnName, string slaveTableName,
            string slaveColumnName)
        {
            if (!DataStorage.ContainTable(masterTableName)) return;
            if (!DataStorage.ContainTable(slaveTableName)) return;
            if (DataStorage.ContainRelation(masterTableName + "_" + slaveTableName)) return;

            var relation = new DataRelation(masterTableName + "_" + slaveTableName,
                DataStorage.DataSet.Tables[masterTableName].Columns[masterColumnName],
                DataStorage.DataSet.Tables[slaveTableName].Columns[slaveColumnName], true);
            DataStorage.DataSet.Relations.Add(relation);
        }

        protected virtual void ConfigureRelations()
        {
            
        }

        protected virtual void ConfigureTable()
        {
        }

        public virtual DataTable Select()
        {
            if (DmLoadType == DataModelLoadSyncType.Syncronize)
                return Table;
            while (DmLoadState != DataModelLoadState.SuccessLoad)
            {
                if (DmLoadState == DataModelLoadState.ErrorLoad)
                {
                    lock (LockObj)
                    {
                        MessageBox.Show("Произошла ошибка при загрузке данных из базы данных. Дальнейшая работа приложения невозможна", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        Application.Exit();
                        return null;
                    }
                }
                Application.DoEvents();
            }
            return Table;
        }

        public virtual int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                ConfigureDeleteCommand(command, id);
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
                        "Не удалось удалить объект из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        protected virtual void ConfigureDeleteCommand(DbCommand command, int id)
        {
            throw new DataModelException("Необходимо переопределеить метод ConfigureDeleteCommand");
        }

        public virtual int Update(Entity entity)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                if (entity == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                ConfigureUpdateCommand(command, entity);
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (OdbcException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось изменить данные о здание. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        protected virtual void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
            throw new DataModelException("Необходимо переопределеить метод ConfigureUpdateCommand");
        }

        public virtual int Insert(Entity entity)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            using (var lastIdCommand = DBConnection.CreateCommand())
            {
                if (entity == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }

                lastIdCommand.CommandText = "SELECT LAST_INSERT_ID()";
                ConfigureInsertCommand(command, entity);
                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    var lastId = connection.SqlSelectTable("last_id", lastIdCommand);
                    connection.SqlCommitTransaction();
                    if (lastId.Rows.Count != 0) 
                        return Convert.ToInt32(lastId.Rows[0][0], CultureInfo.InvariantCulture);
                    MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                catch (OdbcException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
                        "Не удалось добавить объект в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        protected virtual void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
            throw new DataModelException("Необходимо переопределеить метод ConfigureInsertCommand");
        }

        public IEnumerable<DataRow> FilterDeletedRows()
        {
            return from tableRow in Select().AsEnumerable()
                   where (tableRow.RowState != DataRowState.Deleted) &&
                         (tableRow.RowState != DataRowState.Detached)
                   select tableRow;
        }
    
        public static DataModel GetInstance<T>() where T : DataModel
        {            
             return GetInstance<T>(null);
        }

        public static DataModel GetInstance<T>(Action afterLoadHandler) where T :  DataModel
        {
            var currentDataModel = typeof(T);            
            if(typeof(T) == typeof(PaymentsPremiseHistoryDataModel) ||
               typeof(T) == typeof(PaymentsAccountHistoryDataModel) || 
               typeof(T) == typeof(SelectableSigners))
            {
                var method = currentDataModel.GetMethod("GetInstance",new Type[] {});
                var instanceDm = (T)method.Invoke(null, new object[] { });
                return instanceDm;
            }
            else
            {
                var method = currentDataModel.GetMethod("GetInstance", new[] { typeof(Action) });
                var instanceDm = (T)method.Invoke(null, new object[] { afterLoadHandler });
                return instanceDm;
            }                            
        }
    }
}

