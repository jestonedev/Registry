using Registry.CalcDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Registry.DataModels
{      
    internal sealed class CalcDataModelsUpdater
    {
        private static CalcDataModelsUpdater instance;
        public void Run()
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    context.Send(__ =>
                    {
                        if (CalcDataModelBuildingsCurrentFunds.HasInstance() && CalcDataModelBuildingsCurrentFunds.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelBuildingsCurrentFunds.GetInstance().Refresh(CalcDataModelFilterEnity.All, null, true);
                            CalcDataModelBuildingsCurrentFunds.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelBuildingsPremisesFunds.HasInstance() && CalcDataModelBuildingsPremisesFunds.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(CalcDataModelFilterEnity.All, null, true);
                            CalcDataModelBuildingsPremisesFunds.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelBuildingsPremisesSumArea.HasInstance() && CalcDataModelBuildingsPremisesSumArea.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(CalcDataModelFilterEnity.All, null, true);
                            CalcDataModelBuildingsPremisesSumArea.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelPremisesCurrentFunds.HasInstance() && CalcDataModelPremisesCurrentFunds.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelPremisesCurrentFunds.GetInstance().Refresh(CalcDataModelFilterEnity.All, null, true);
                            CalcDataModelPremisesCurrentFunds.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelTenancyAggregated.HasInstance() && CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelTenancyAggregated.GetInstance().Refresh(CalcDataModelFilterEnity.All, null, true);
                            CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate = false;
                        }
                    }, null);
                    //Обновление делаем примерно каждые CalcDataModelsUpdateTimeout милисекунд
                    Thread.Sleep(RegistrySettings.CalcDataModelsUpdateTimeout);
                }
            }, null);
        }

        public static CalcDataModelsUpdater GetInstance()
        {
            if (instance == null)
                instance = new CalcDataModelsUpdater();
            return instance;
        }
    }
}
