using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registry.DataModels;
using System.Data;
using System.ComponentModel;
using System.Windows.Forms;

namespace Registry.CalcDataModels
{
    public class CalcDataModel: DataModel
    {
        private BackgroundWorker worker = new BackgroundWorker();
        public event EventHandler<EventArgs> RefreshEvent;

        protected CalcDataModel()
        {
            dmLoadType = DataModelLoadSyncType.Asyncronize;
            worker.DoWork += new DoWorkEventHandler(Calculate);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CalculationComplete);
        }

        public void Refresh(CalcDataModelFilterEnity entity, int? idObject)
        {
            while (worker.IsBusy)
                Application.DoEvents();
            worker.RunWorkerAsync(new CalcAsyncConfig(entity, idObject));
        }

        protected virtual void Calculate(object sender, DoWorkEventArgs e)
        {
            // Здесь располагается асинхронно выполняющийся код вычисления модели
            // e.Argument - объект типа CalculationAsyncConfig, хранит информацию о сущности DataModelCalculateEnity и id объекта, который необходимо пересчитать
        }

        protected void CalculationComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // Делаем слияние результатов с текущей таблицей
            if (e.Error != null)
            {
                dmLoadState = DataModelLoadState.ErrorLoad;
                return;
            }
            if (e.Result is DataTable)
                table.Merge((DataTable)e.Result);
            dmLoadState = DataModelLoadState.SuccessLoad;
            if (RefreshEvent != null)
                RefreshEvent(this, new EventArgs());
        }
    }
}
