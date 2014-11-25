using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelTenancyAggregated : CalcDataModel
    {
        private static CalcDataModelTenancyAggregated dataModel = null;

        private static string tableName = "tenancy_aggregated";

        private CalcDataModelTenancyAggregated()
        {
            Table = InitializeTable();
            Refresh(CalcDataModelFilterEnity.All, null);            
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
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModeTenancyAggregated");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var tenancies = from tenancies_row in DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select()) 
                            where (config.Entity == CalcDataModelFilterEnity.Tenancy ?
                                  tenancies_row.Field<int>("id_process") == config.IdObject : true)
                            select tenancies_row;
            var tenancy_sub_premises = from tenancy_sub_premises_row in DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select())
                                       where (config.Entity == CalcDataModelFilterEnity.SubPremise ?
                                              tenancy_sub_premises_row.Field<int>("id_sub_premises") == config.IdObject : true)
                                       select tenancy_sub_premises_row; 
            var tenancy_premises = from tenancy_premises_row in DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select())
                                   where (config.Entity == CalcDataModelFilterEnity.Premise ?
                                          tenancy_premises_row.Field<int>("id_premises") == config.IdObject : true)
                                   select tenancy_premises_row; 
            var tenancy_buildings = from tenancy_buildings_row in DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select())
                                    where (config.Entity == CalcDataModelFilterEnity.Building ? 
                                           tenancy_buildings_row.Field<int>("id_building") == config.IdObject : true)
                                    select tenancy_buildings_row; 
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());
            var tenancy_persons = DataModelHelper.FilterRows(TenancyPersonsDataModel.GetInstance().Select());
            var kladr_street = DataModelHelper.FilterRows(KladrStreetsDataModel.GetInstance().Select());
            // Вычисляем агрегационную информацию
            var tenants = from tenancy_persons_row in tenancy_persons
                          where tenancy_persons_row.Field<int?>("id_kinship") == 1
                          group tenancy_persons_row by tenancy_persons_row.Field<int>("id_process") into gs
                          select new 
                          {
                              id_process = gs.Key,
                              tenant = (gs.First().Field<string>("surname")+" "+gs.First().Field<string>("name")+" "+gs.First().Field<string>("patronymic")).Trim()
                          };
            var t_sub_premises_gc = from tenancy_sub_premises_row in tenancy_sub_premises
                                    join sub_premises_row in sub_premises
                                    on tenancy_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                    join premises_row in premises.AsEnumerable()
                                    on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                    group sub_premises_row.Field<string>("sub_premises_num") by
                                        new {
                                            id_process = tenancy_sub_premises_row.Field<int>("id_process"),
                                            id_building = premises_row.Field<int>("id_building"),
                                            id_premises = sub_premises_row.Field<int>("id_premises"),
                                            premises_num = premises_row.Field<string>("premises_num")
                                        } into gs
                                    select new
                                        {
                                            id_process = gs.Key.id_process,
                                            id_building = gs.Key.id_building,
                                            id_premises = gs.Key.id_premises,
                                            result_str = " пом " + gs.Key.premises_num + ((gs.Count() > 0) ? 
                                            (" ком "+gs.Aggregate((a, b) => { 
                                                return a + ", " + b; 
                                            })) : "")
                                        };
            var t_premises = from tenancy_premises_row in tenancy_premises
                                join premises_row in premises
                                on tenancy_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                             select new
                                {
                                    id_process = tenancy_premises_row.Field<int>("id_process"),
                                    id_building = premises_row.Field<int>("id_building"),
                                    id_premises = tenancy_premises_row.Field<int>("id_premises"),
                                    result_str = " пом " + premises_row.Field<string>("premises_num")
                                };
            var t_buildings = from tenancy_buildings_row in tenancy_buildings
                              select new 
                              {
                                  id_process = tenancy_buildings_row.Field<int>("id_process"),
                                  id_building = tenancy_buildings_row.Field<int>("id_building"),
                                  result_str = ""
                              };
            var tenancy_premises_gc = from tenancy_premises_row in t_premises.Union(t_sub_premises_gc)
                                      join premises_row in premises
                                      on tenancy_premises_row.id_premises equals premises_row.Field<int>("id_premises")
                                      group 
                                          tenancy_premises_row.result_str
                                          by new
                                          {
                                              tenancy_premises_row.id_process,
                                              tenancy_premises_row.id_building
                                          } into gs
                                      select new
                                          {
                                              id_process = gs.Key.id_process,
                                              id_building = gs.Key.id_building,
                                              result_str = gs.Aggregate((a, b) =>
                                              {
                                                  return a + ", " + b.Trim();
                                              })
                                          };
            var addresses = from tenancy_row in t_buildings.Union(tenancy_premises_gc)
                         join buildings_row in buildings
                         on tenancy_row.id_building equals buildings_row.Field<int>("id_building")
                         join kladr_row in kladr_street
                         on buildings_row.Field<string>("id_street") equals kladr_row.Field<string>("id_street")
                         group 
                            kladr_row.Field<string>("street_name") + ", дом " + buildings_row.Field<string>("house") + tenancy_row.result_str
                         by tenancy_row.id_process into gs
                         select new
                             {
                                 id_process = gs.Key,
                                 address = gs.Aggregate((a, b) =>
                                 {
                                     return a + ", " + b.Trim();
                                 })
                             };
            var result = from tenancies_row in tenancies
                         join tenants_row in tenants
                         on tenancies_row.Field<int>("id_process") equals tenants_row.id_process into sp_t
                         from sp_t_row in sp_t.DefaultIfEmpty()
                         join addresses_row in addresses
                         on tenancies_row.Field<int>("id_process") equals addresses_row.id_process into sp_a
                         from sp_a_row in sp_a.DefaultIfEmpty()
                         select new
                         {
                             id_process = tenancies_row.Field<int>("id_process"),
                             address = (sp_a_row == null) ? "" : sp_a_row.address,
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
