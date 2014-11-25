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
    public class CalcDataModel: DataModel, IDisposable
    {
        private BackgroundWorker worker = new BackgroundWorker();
        public event EventHandler<EventArgs> RefreshEvent;
        
        // Метка отложенного обновления. Данные будут обновлены полностью при следующем проходе диспетчера обновления вычисляемых моделей
        public bool DefferedUpdate { get; set; }

        protected CalcDataModel()
        {
            DMLoadType = DataModelLoadSyncType.Asyncronize;
            worker.DoWork += new DoWorkEventHandler(Calculate);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CalculationComplete);
        }

        public void Refresh(CalcDataModelFilterEnity entity, int? idObject, bool removeDependenceEntities = false)
        {
            while (worker.IsBusy)
                Application.DoEvents();
            if (removeDependenceEntities)
            {
                if (entity == CalcDataModelFilterEnity.All)
                    Table.Clear();
                else
                {
                    var remove_rows = (from row in Table.AsEnumerable()
                                       where entity == CalcDataModelFilterEnity.Building ? row.Field<int?>("id_building") == idObject :
                                             entity == CalcDataModelFilterEnity.Premise ? row.Field<int?>("id_premises") == idObject :
                                             entity == CalcDataModelFilterEnity.SubPremise ? row.Field<int?>("id_sub_premises") == idObject :
                                             entity == CalcDataModelFilterEnity.Tenancy ? row.Field<int?>("id_process") == idObject : false
                                       select row);
                    for (int i = remove_rows.Count() - 1; i >= 0; i--)
                        remove_rows.ElementAt(i).Delete();
                }
            }
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
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект RunWorkerCompletedEventArgs в классе CalcDataModel");
            if (e.Error != null)
            {
                DMLoadState = DataModelLoadState.ErrorLoad;
                return;
            }
            if (e.Result is DataTable)
                Table.Merge((DataTable)e.Result);
            DMLoadState = DataModelLoadState.SuccessLoad;
            if (RefreshEvent != null)
                RefreshEvent(this, new EventArgs());
        }

        public void Dispose()
        {
            worker.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
