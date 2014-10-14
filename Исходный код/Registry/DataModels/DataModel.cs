using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Data.Odbc;

namespace Registry.DataModels
{
    public abstract class DataModel
    {
        protected DataTable table = null;
        protected DBConnection connection = null;

        public DataModelLoadState dmLoadState = DataModelLoadState.BeforeLoad;
        public DataModelLoadSyncType dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная

        private static object lock_obj = new object();

        protected DataModel()
        {
        }

        protected DataModel(ToolStripProgressBar progressBar, int incrementor, string selectQuery, string tableName)
        {
            dmLoadType = DataModelLoadSyncType.Asyncronize;
            ThreadPool.QueueUserWorkItem((progress) =>
            {
                try
                {
                    dmLoadState = DataModelLoadState.Loading;
                    connection = new DBConnection();
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = selectQuery;
                    Interlocked.Exchange<DataTable>(ref table, connection.SqlSelectTable(tableName, (DbCommand)command));
                    ConfigureTable();
                    lock (lock_obj)
                    {
                        DataSetManager.AddTable(table);
                    }
                    dmLoadState = DataModelLoadState.SuccessLoad;
                    if (progress != null)
                        ((ToolStripProgressBar)progress).GetCurrentParent().Invoke(
                            new MethodInvoker(delegate()
                        {
                            progressBar.Value += incrementor;
                            if (progressBar.Value == progressBar.Maximum)
                                progressBar.Visible = false;
                        }));
                }
                catch (OdbcException e)
                {
                    lock (lock_obj)
                    {
                        MessageBox.Show(String.Format("Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dmLoadState = DataModelLoadState.ErrorLoad;
                        Application.Exit();
                    }
                }
                catch (DataModelException e)
                {
                    MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dmLoadState = DataModelLoadState.ErrorLoad;
                }
            }, progressBar); 
        }

        protected virtual void ConfigureTable()
        {
        }

        public virtual DataTable Select()
        {
            if (dmLoadType == DataModelLoadSyncType.Syncronize)
                return table;
            while (dmLoadState != DataModelLoadState.SuccessLoad)
            {
                if (dmLoadState == DataModelLoadState.ErrorLoad)
                {
                    lock (lock_obj)
                    {
                        MessageBox.Show("Произошла ошибка при загрузке данных из базы данных. Дальнейшая работа приложения невозможна", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return null;
                    }
                }
            }
            return table;
        }
    }
}
