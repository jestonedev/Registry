using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Registry.Entities;

namespace Registry.DataModels.DataModels
{
    public class DataModel
    {
        private static DataModel _dataModel;

        private DataTable _table;
        private DataModelLoadState _dmLoadState = DataModelLoadState.BeforeLoad;
        private DataModelLoadSyncType _dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная

        public DataModelLoadState DmLoadState { get { return _dmLoadState; } set { _dmLoadState = value; } }
        public DataModelLoadSyncType DmLoadType { get { return _dmLoadType; } set { _dmLoadType = value; } }
        protected DataTable Table { get { return _table; } set { _table = value; } }

        private static readonly object LockObj = new object();
        // Не больше MaxDBConnectionCount потоков одновременно делают запросы к БД
        private static readonly Semaphore DbAccessSemaphore = new Semaphore(RegistrySettings.MaxDBConnectionCount, RegistrySettings.MaxDBConnectionCount);
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
                        DataSetManager.AddTable(Table);
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
                            CalcDataModelsUpdater.GetInstance().Run();
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

        public DataModel GetInstance(DataModelType dataModeltype)
        {
            return GetInstance(null, 0, dataModeltype);
        }

        public DataModel GetInstance(ToolStripProgressBar progressBar, int incrementor, DataModelType dataModelType)
        {
            if (_dataModel != null)
                return _dataModel;
            switch (dataModelType)
            {
                case DataModelType.BuildingsDataModel:
                    _dataModel = new BuildingsDataModel(progressBar, incrementor);
                    break;
                case DataModelType.ClaimsDataModel:
                    _dataModel = new ClaimsDataModel(progressBar, incrementor);
                    break;
                case DataModelType.ClaimStatesDataModel:
                    _dataModel = new ClaimStatesDataModel(progressBar, incrementor);
                    break;
                default:
                    throw new DataModelException("Неизвестный тип модели");
            }
            return _dataModel;
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

        public int Delete(int id)
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

        public int Update(Entity entity)
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

        public int Insert(Entity entity)
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
                    if (lastId.Rows.Count != 0) return Convert.ToInt32(lastId.Rows[0][0], CultureInfo.InvariantCulture);
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
    }
}
