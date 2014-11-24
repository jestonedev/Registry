﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Data.Odbc;
using System.Globalization;

namespace Registry.DataModels
{
    public abstract class DataModel
    {
        private DataTable table = null;
        private DataModelLoadState dmLoadState = DataModelLoadState.BeforeLoad;
        private DataModelLoadSyncType dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная

        public DataModelLoadState DMLoadState { get { return dmLoadState; } set { dmLoadState = value; } }
        public DataModelLoadSyncType DMLoadType { get { return dmLoadType; } set { dmLoadType = value; } }
        protected DataTable Table { get { return table; } set { table = value; } }

        private static object lock_obj = new object();
        // Не больше MaxDBConnectionCount потоков одновременно делают запросы к БД
        private static Semaphore db_access_semaphore = new Semaphore(Registry.RegistrySettings.MaxDBConnectionCount,
            Registry.RegistrySettings.MaxDBConnectionCount);

        protected DataModel()
        {
        }

        protected DataModel(ToolStripProgressBar progressBar, int incrementor, string selectQuery, string tableName)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            DMLoadType = DataModelLoadSyncType.Asyncronize;
            ThreadPool.QueueUserWorkItem((progress) =>
            {
                try
                {
                    DMLoadState = DataModelLoadState.Loading;
                    using (DBConnection connection = new DBConnection())
                    using (DbCommand command = DBConnection.CreateCommand())
                    {
                        command.CommandText = selectQuery;
                        db_access_semaphore.WaitOne();
                        Interlocked.Exchange<DataTable>(ref table, connection.SqlSelectTable(tableName, (DbCommand)command));
                    }
                    db_access_semaphore.Release();
                    ConfigureTable();
                    lock (lock_obj)
                    {
                        DataSetManager.AddTable(Table);
                    }
                    DMLoadState = DataModelLoadState.SuccessLoad;
                    if (progress != null)
                    {
                        context.Post(_ => {
                            progressBar.Value += incrementor;
                            if (progressBar.Value == progressBar.Maximum)
                            {
                                progressBar.Visible = false;
                                //Если мы загрузили все данные, то запускаем CallbackUpdater
                                DataModelCallbackUpdater.GetInstance().Run();
                            }
                        }, null);
                    }
                }
                catch (OdbcException e)
                {
                    lock (lock_obj)
                    {
                        MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                            "Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        DMLoadState = DataModelLoadState.ErrorLoad;
                        Application.Exit();
                    }
                }
                catch (DataModelException e)
                {
                    MessageBox.Show(e.Message, "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    DMLoadState = DataModelLoadState.ErrorLoad;
                }
            }, progressBar); 
        }

        protected virtual void ConfigureTable()
        {
        }

        public virtual DataTable Select()
        {
            if (DMLoadType == DataModelLoadSyncType.Syncronize)
                return Table;
            while (DMLoadState != DataModelLoadState.SuccessLoad)
            {
                if (DMLoadState == DataModelLoadState.ErrorLoad)
                {
                    lock (lock_obj)
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
    }
}
