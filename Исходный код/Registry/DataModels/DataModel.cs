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
        protected DBConnection connection = new DBConnection();

        public DataModelLoadState dmLoadState = DataModelLoadState.BeforeLoad;
        public DataModelLoadSyncType dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная

        protected DataModel()
        {
        }

        protected DataModel(ToolStripProgressBar progressBar, int incrementor, string selectQuery, string tableName)
        {
            dmLoadType = DataModelLoadSyncType.Asyncronize;
            ThreadPool.QueueUserWorkItem((progress) =>
            {
                dmLoadState = DataModelLoadState.Loading;
                DbCommand command = connection.CreateCommand();
                command.CommandText = selectQuery;
                try
                {
                    Interlocked.Exchange<DataTable>(ref table, connection.SqlSelectTable(tableName, (DbCommand)command));
                    ConfigureTable();
                    lock (DataSetManager.GetDataSet())
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
                    MessageBox.Show(String.Format("Произошла ошибка при загрузке зданий из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dmLoadState = DataModelLoadState.ErrorLoad;
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
                    MessageBox.Show("Во время загрузки зданий из базы данных произошла ошибка. Дальнейшая работа приложения невозможна", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                Thread.Sleep(100);  // Ожидание загрузки (чтобы сильно не нагружать процессор)
            }
            return table;
        }
    }
}
