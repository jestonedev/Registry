using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Registry.DataModels.DataModels;
using Registry.Entities;
using Settings;

namespace Registry.DataModels.CalcDataModels
{
    public class CalcDataModel: DataModel, IDisposable
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        public event EventHandler<EventArgs> RefreshEvent;
        private static List<WeakReference> _calcedDataModels = new List<WeakReference>();
        private static readonly object LockObj = new object();
        
        // Метка отложенного обновления. Данные будут обновлены полностью при следующем проходе диспетчера обновления вычисляемых моделей
        public bool DefferedUpdate { get; set; }

        protected CalcDataModel()
        {
            DmLoadType = DataModelLoadSyncType.Asyncronize;
            _worker.DoWork += Calculate;
            _worker.RunWorkerCompleted += CalculationComplete;
            lock (LockObj)
                _calcedDataModels.Add(new WeakReference(this));
        }

        public static void RunRefreshWalker()
        {
            var context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    context.Send(__ =>
                    {
                        lock (LockObj)
                        {
                            foreach (var reference in _calcedDataModels)
                            {
                                var modelRef = reference.Target;
                                if (modelRef == null) continue;
                                var model = ((CalcDataModel)modelRef);
                                if (!model.DefferedUpdate) continue;
                                model.Refresh();
                                model.DefferedUpdate = false;
                            }
                            _calcedDataModels = _calcedDataModels.Where(model => model.IsAlive).ToList();
                        }
                    }, null);
                    //Обновление делаем примерно каждые CalcDataModelsUpdateTimeout милисекунд
                    Thread.Sleep(RegistrySettings.CalcDataModelsUpdateTimeout);
                }
            }, null);
        }

        public static CalcDataModel GetInstance(CalcDataModelType dataModelType)
        {
            switch (dataModelType)
            {
                case CalcDataModelType.CalcDataModelBuildingsCurrentFunds:
                    return CalcDataModelBuildingsCurrentFunds.GetInstance();
                case CalcDataModelType.CalcDataModelBuildingsPremisesFunds:
                    return CalcDataModelBuildingsPremisesFunds.GetInstance();
                case CalcDataModelType.CalcDataModelBuildingsPremisesSumArea:
                    return CalcDataModelBuildingsPremisesSumArea.GetInstance();
                case CalcDataModelType.CalcDataModelPremisesCurrentFunds:
                    return CalcDataModelPremisesCurrentFunds.GetInstance();
                case CalcDataModelType.CalcDataModelPremisesTenanciesInfo:
                    return CalcDataModelPremisesTenanciesInfo.GetInstance();
                case CalcDataModelType.CalcDataModelPremiseSubPremisesSumArea:
                    return CalcDataModelPremiseSubPremisesSumArea.GetInstance();
                case CalcDataModelType.CalcDataModelResettleAggregated:
                    return CalcDataModelResettleAggregated.GetInstance();
                case CalcDataModelType.CalcDataModelSubPremisesCurrentFunds:
                    return CalcDataModelSubPremisesCurrentFunds.GetInstance();
                case CalcDataModelType.CalcDataModelTenancyAggregated:
                    return CalcDataModelTenancyAggregated.GetInstance();
                case CalcDataModelType.CalcDataModelTenancyNotifiesMaxDate:
                    return CalcDataModelTenancyNotifiesMaxDate.GetInstance();
                case CalcDataModelType.CalcDataModelTenancyPremisesInfo:
                    return CalcDataModelTenancyPremisesInfo.GetInstance(); 
                default:
                    throw new DataModelException("Неизвестный тип модели");
            }
        }

        public void Refresh()
        {
            while (_worker.IsBusy)
                Application.DoEvents();
            Table.Clear();
            _worker.RunWorkerAsync();
        }

        protected void RefreshOnTableModify(DataTable table)
        {
            table.RowChanged += (sender, e) => { DefferedUpdate = true; };
            table.RowDeleted += (sender, e) => { DefferedUpdate = true; };
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
                DmLoadState = DataModelLoadState.ErrorLoad;
                return;
            }
            var table = e.Result as DataTable;
            if (table != null)
                Table.Merge(table);
            DmLoadState = DataModelLoadState.SuccessLoad;
            if (RefreshEvent != null)
                RefreshEvent(this, new EventArgs());
        }

        public void Dispose()
        {
            _worker.Dispose();
            GC.SuppressFinalize(this);
        }

        protected sealed override void ConfigureInsertCommand(DbCommand command, Entity entity)
        {
        }

        protected sealed override void ConfigureDeleteCommand(DbCommand command, int id)
        {
        }

        protected sealed override void ConfigureUpdateCommand(DbCommand command, Entity entity)
        {
        }

        public override int Delete(int id)
        {
            throw new  DataModelException("Нельзя удалить запись из вычислемой модели");
        }

        public override int Update(Entity entity)
        {
            throw new DataModelException("Нельзя обновить запись в вычислемой модели");
        }

        public override int Insert(Entity entity)
        {
            throw new DataModelException("Нельзя добавить запись в вычислемую модель");
        }
    }
}
