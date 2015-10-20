﻿using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using Registry.DataModels.DataModels;

namespace Registry.DataModels.CalcDataModels
{
    internal sealed class CalcDataModelTenancyAggregated : CalcDataModel
    {
        private static CalcDataModelTenancyAggregated _dataModel;

        private const string TableName = "tenancy_aggregated";

        private CalcDataModelTenancyAggregated()
        {
            Table = InitializeTable();
            Refresh();      
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancyProcessesDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancyPersonsDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancyBuildingsAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancyPremisesAssocDataModel).Select());
            RefreshOnTableModify(DataModel.GetInstance(DataModelType.TenancySubPremisesAssocDataModel).Select());
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("address").DataType = typeof(string);
            table.Columns.Add("tenant").DataType = typeof(string);
            table.PrimaryKey = new [] { table.Columns["id_process"] };
            return table;
        }

        protected override void Calculate(object sender, DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModeTenancyAggregated");
            // Фильтруем удаленные строки
            var tenancies = GetInstance(DataModelType.TenancyProcessesDataModel).FilterDeletedRows();
            var tenancyPersons = GetInstance(DataModelType.TenancyPersonsDataModel).FilterDeletedRows();
            // Вычисляем агрегационную информацию
            var tenants = from tenancyPersonsRow in tenancyPersons
                          where tenancyPersonsRow.Field<int?>("id_kinship") == 1
                          group tenancyPersonsRow by tenancyPersonsRow.Field<int>("id_process") into gs
                          select new 
                          {
                              id_process = gs.Key,
                              tenant = (gs.First().Field<string>("surname")+" "+gs.First().Field<string>("name")+" "+gs.First().Field<string>("patronymic")).Trim()
                          };
            var addresses = DataModelHelper.AggregateAddressByIdProcess(
                GetInstance(DataModelType.TenancyBuildingsAssocDataModel).FilterDeletedRows(),
                GetInstance(DataModelType.TenancyPremisesAssocDataModel).FilterDeletedRows(),
                GetInstance(DataModelType.TenancySubPremisesAssocDataModel).FilterDeletedRows());
            var result = from tenanciesRow in tenancies
                         join tenantsRow in tenants
                         on tenanciesRow.Field<int>("id_process") equals tenantsRow.id_process into spT
                         from spTRow in spT.DefaultIfEmpty()
                         join addressesRow in addresses
                         on tenanciesRow.Field<int>("id_process") equals addressesRow.IdProcess into spA
                         from spARow in spA.DefaultIfEmpty()
                         select new
                         {
                             id_process = tenanciesRow.Field<int>("id_process"),
                             address = (spARow == null) ? "" : spARow.Address,
                             tenant = (spTRow == null) ? "" : spTRow.tenant
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_process, x.address, x.tenant);
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelTenancyAggregated GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelTenancyAggregated());
        }
    }
}
