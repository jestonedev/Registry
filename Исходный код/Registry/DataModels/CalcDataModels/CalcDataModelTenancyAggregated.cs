using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;
using Registry.DataModels.DataModels;
using Registry.Entities;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelTenancyAggregated : CalcDataModel
    {
        private static CalcDataModelTenancyAggregated dataModel = null;

        private static string tableName = "tenancy_aggregated";

        private CalcDataModelTenancyAggregated()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);            
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("address").DataType = typeof(string);
            table.Columns.Add("tenant").DataType = typeof(string);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_process"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModeTenancyAggregated");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var tenancies = DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select(), config.Entity, config.IdObject);    
            var tenancy_persons = DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            // Вычисляем агрегационную информацию
            var tenants = from tenancy_persons_row in tenancy_persons
                          where tenancy_persons_row.Field<int?>("id_kinship") == 1
                          group tenancy_persons_row by tenancy_persons_row.Field<int>("id_process") into gs
                          select new 
                          {
                              id_process = gs.Key,
                              tenant = (gs.First().Field<string>("surname")+" "+gs.First().Field<string>("name")+" "+gs.First().Field<string>("patronymic")).Trim()
                          };         
            var addresses = DataModelHelper.AggregateAddressByIdProcess(TenancyBuildingsAssocDataModel.GetInstance(), TenancyPremisesAssocDataModel.GetInstance(),
                TenancySubPremisesAssocDataModel.GetInstance(), config.Entity == EntityType.TenancyProcess ? config.IdObject : null);
            var result = from tenancies_row in tenancies
                         join tenants_row in tenants
                         on tenancies_row.Field<int>("id_process") equals tenants_row.id_process into sp_t
                         from sp_t_row in sp_t.DefaultIfEmpty()
                         join addresses_row in addresses
                         on tenancies_row.Field<int>("id_process") equals addresses_row.IdProcess into sp_a
                         from sp_a_row in sp_a.DefaultIfEmpty()
                         select new
                         {
                             id_process = tenancies_row.Field<int>("id_process"),
                             address = (sp_a_row == null) ? "" : sp_a_row.Address,
                             tenant = (sp_t_row == null) ? "" : sp_t_row.tenant
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_process, 
                    x.address, 
                    x.tenant
                });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelTenancyAggregated GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelTenancyAggregated();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
