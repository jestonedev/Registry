using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Registry.Entities;
using Registry.DataModels.CalcDataModels;
using Settings;

namespace Registry.DataModels.DataModels
{
    public class DataModel
    {
        private static readonly DataSet dataSet = new DataSet();
        public static DataSet DataSet { get { return dataSet; } }

        private DataTable _table;
        private DataModelLoadState _dmLoadState = DataModelLoadState.BeforeLoad;
        private DataModelLoadSyncType _dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная

        public DataModelLoadState DmLoadState { get { return _dmLoadState; } set { _dmLoadState = value; } }
        public DataModelLoadSyncType DmLoadType { get { return _dmLoadType; } set { _dmLoadType = value; } }
        protected DataTable Table { get { return _table; } set { _table = value; } }

        private static readonly object LockObj = new object();
        // Не больше MaxDBConnectionCount потоков одновременно делают запросы к БД
        private static readonly Semaphore DbAccessSemaphore = new Semaphore(RegistrySettings.MaxDbConnectionCount, RegistrySettings.MaxDbConnectionCount);
        public bool EditingNewRecord { get; set; }

        protected DataModel()
        {
        }

        protected DataModel(ToolStripProgressBar progressBar, int incrementor, string selectQuery, string tableName)
        {
            var context = SynchronizationContext.Current;
            DmLoadType = DataModelLoadSyncType.Asyncronize;
            ThreadPool.QueueUserWorkItem(progress =>
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
                    lock (LockObj)
                    {
                        if (!dataSet.Tables.Contains(Table.TableName))
                        {
                            Table.ExtendedProperties.Add("model", this);
                            dataSet.Tables.Add(Table);
                            ConfigureRelations();
                        }
                    }
                    DmLoadState = DataModelLoadState.SuccessLoad;
                    if (progress != null)
                    {
                        context.Post(_ => {
                            progressBar.Value += incrementor;
                            if (progressBar.Value != progressBar.Maximum) return;
                            progressBar.Visible = false;
                            //Если мы загрузили все данные, то запускаем CallbackUpdater
                            DataModelsCallbackUpdater.GetInstance().Run();
                            CalcDataModel.RunRefreshWalker();
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
            }, progressBar); 
        }

        public static DataModel GetLoadedInstance(string tableName)
        {
            DataModel dm = null;
            lock (LockObj)
            {
                if (dataSet.Tables.Contains(tableName))
                    dm = (DataModel)dataSet.Tables[tableName].ExtendedProperties["model"];
            }
            return dm;
        }              

        protected static void AddRelation(string masterTableName, string masterColumnName, string slaveTableName,
            string slaveColumnName)
        {
            if (!dataSet.Tables.Contains(masterTableName)) return;
            if (!dataSet.Tables.Contains(slaveTableName)) return;
            if (dataSet.Relations.Contains(masterTableName + "_" + slaveTableName)) return;
            var relation = new DataRelation(masterTableName + "_" + slaveTableName,
                dataSet.Tables[masterTableName].Columns[masterColumnName],
                dataSet.Tables[slaveTableName].Columns[slaveColumnName], true);
            dataSet.Relations.Add(relation);
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
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                        "Не удалось удалить объект из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        protected virtual void ConfigureDeleteCommand(DbCommand command, int id)
        {
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
        }

        public IEnumerable<DataRow> FilterDeletedRows()
        {
            return from tableRow in Select().AsEnumerable()
                   where (tableRow.RowState != DataRowState.Deleted) &&
                         (tableRow.RowState != DataRowState.Detached)
                   select tableRow;
        }
    
          //public static DataModel GetInstance(DataModelType dataModelType)
        public static DataModel GetInstance<T>() where T : DataModel
        {            
             return GetInstance<T>(null,0);
        }

        public static DataModel GetInstance<T>(ToolStripProgressBar progressBar, int incrementor) where T :  DataModel
        {
            Type currentDataModel = typeof(T);            
            if(typeof(T) == typeof(PaymentsDataModel) || typeof(T) == typeof(SelectableSigners))
            {
                var method = currentDataModel.GetMethod("GetInstance",new Type[] {});
                var instanceDM = (T)method.Invoke(null, new object[] { });
                return instanceDM;
            }
            else
            {
                var method = currentDataModel.GetMethod("GetInstance", new Type[] { typeof(ToolStripProgressBar), typeof(int) });
                var instanceDM = (T)method.Invoke(null, new object[] { progressBar, incrementor });
                return instanceDM;
            }            
                //case DataModelType.PaymentsDataModel:
                //    return PaymentsDataModel.GetInstance();
                //case DataModelType.SelectableHeadHousingDepDataModel:
                //    return SelectableSigners.GetInstance();
                //default:
                //    throw new DataModelException("Неизвестный тип модели");
        }
    }
}

