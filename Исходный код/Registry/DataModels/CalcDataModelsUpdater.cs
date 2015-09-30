using Registry.CalcDataModels;
using Registry.Entities;
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
                            CalcDataModelBuildingsCurrentFunds.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelBuildingsCurrentFunds.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelPremisesCurrentFunds.HasInstance() && CalcDataModelPremisesCurrentFunds.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelPremisesCurrentFunds.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelPremisesCurrentFunds.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelSubPremisesCurrentFunds.HasInstance() && CalcDataModelSubPremisesCurrentFunds.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelSubPremisesCurrentFunds.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelSubPremisesCurrentFunds.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelBuildingsPremisesFunds.HasInstance() && CalcDataModelBuildingsPremisesFunds.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelBuildingsPremisesFunds.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelBuildingsPremisesFunds.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelPremiseSubPremisesSumArea.HasInstance() && CalcDataModelPremiseSubPremisesSumArea.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelPremiseSubPremisesSumArea.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelPremiseSubPremisesSumArea.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelBuildingsPremisesSumArea.HasInstance() && CalcDataModelBuildingsPremisesSumArea.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelBuildingsPremisesSumArea.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelBuildingsPremisesSumArea.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelTenancyAggregated.HasInstance() && CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelTenancyAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelTenancyAggregated.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelResettleAggregated.HasInstance() && CalcDataModelResettleAggregated.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelResettleAggregated.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelResettleAggregated.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelTenancyNotifiesMaxDate.HasInstance() && CalcDataModelTenancyNotifiesMaxDate.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelTenancyNotifiesMaxDate.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelTenancyNotifiesMaxDate.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelPremisesTenanciesInfo.HasInstance() && CalcDataModelPremisesTenanciesInfo.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelPremisesTenanciesInfo.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelPremisesTenanciesInfo.GetInstance().DefferedUpdate = false;
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
