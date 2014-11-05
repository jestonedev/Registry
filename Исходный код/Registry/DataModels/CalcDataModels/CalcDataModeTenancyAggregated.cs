using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModeTenancyAggregated : CalcDataModel
    {
        private static CalcDataModeTenancyAggregated dataModel = null;

        private static string tableName = "tenancy_aggregated";

        private CalcDataModeTenancyAggregated()
        {
            table = InitializeTable();
            Refresh(CalcDataModelFilterEnity.All, null);            
        }

        private DataTable InitializeTable()
        {
            DataTable table = table = new DataTable(tableName);
            table.Columns.Add("id_process").DataType = typeof(int);
            table.Columns.Add("address").DataType = typeof(string);
            table.Columns.Add("tenant").DataType = typeof(string);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_process"] };
            return table;
        }

        private class PremisesAggregateSurrogate
        {
            public int? id_premises { get; set; }
            public string premises_num { get; set; }
            public string result_str { get; set; }

            public PremisesAggregateSurrogate(int? id_premises, string premises_num, string result_str)
            {
                this.id_premises = id_premises;
                this.premises_num = premises_num;
                this.result_str = result_str;
            }
        }

        private class BuildingsAggregateSurrogate
        {
            public int? id_building { get; set; }
            public string building_full_address { get; set; }
            public string result_str { get; set; }

            public BuildingsAggregateSurrogate(int? id_building, string building_full_address, string result_str)
            {
                this.id_building = id_building;
                this.building_full_address = building_full_address;
                this.result_str = result_str;
            }
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            dmLoadState = DataModelLoadState.Loading;
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var tenancies = from tenancies_row in DataModelHelper.FilterRows(TenancyProcessesDataModel.GetInstance().Select()) 
                            where (config.Entity == CalcDataModelFilterEnity.Tenancy ?
                                  tenancies_row.Field<int>("id_process") == config.IdObject : true)
                            select tenancies_row;
            var tenancy_sub_premises = DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select());
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
            var tenants = from tenancy_persons_row in tenancy_persons.AsEnumerable()
                          where tenancy_persons_row.Field<int?>("id_kinship") == 1
                          group tenancy_persons_row by tenancy_persons_row.Field<int>("id_process") into gs
                          select new 
                          {
                              id_process = gs.Key,
                              tenant = (gs.First().Field<string>("surname")+" "+gs.First().Field<string>("name")+" "+gs.First().Field<string>("patronymic")).Trim()
                          };
            var t_sub_premises_gc = from tenancy_sub_premises_row in tenancy_sub_premises.AsEnumerable()
                                    where tenancy_sub_premises_row.RowState != DataRowState.Detached &&
                                          tenancy_sub_premises_row.RowState != DataRowState.Deleted
                                    join sub_premises_row in sub_premises.AsEnumerable()
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
            var t_premises = from tenancy_premises_row in tenancy_premises.AsEnumerable()
                             where tenancy_premises_row.RowState != DataRowState.Detached &&
                                    tenancy_premises_row.RowState != DataRowState.Deleted
                                join premises_row in premises.AsEnumerable()
                                on tenancy_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                             select new
                                {
                                    id_process = tenancy_premises_row.Field<int>("id_process"),
                                    id_building = premises_row.Field<int>("id_building"),
                                    id_premises = tenancy_premises_row.Field<int>("id_premises"),
                                    result_str = " пом " + premises_row.Field<string>("premises_num")
                                };
            var t_buildings = from tenancy_buildings_row in tenancy_buildings.AsEnumerable()
                              where tenancy_buildings_row.RowState != DataRowState.Detached &&
                                    tenancy_buildings_row.RowState != DataRowState.Deleted
                              select new 
                              {
                                  id_process = tenancy_buildings_row.Field<int>("id_process"),
                                  id_building = tenancy_buildings_row.Field<int>("id_building"),
                                  result_str = ""
                              };
            var tenancy_premises_gc = from tenancy_premises_row in t_premises.Union(t_sub_premises_gc)
                                      join premises_row in premises.AsEnumerable()
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
                         join buildings_row in buildings.AsEnumerable()
                         on tenancy_row.id_building equals buildings_row.Field<int>("id_building")
                         join kladr_row in kladr_street.AsEnumerable()
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
            var result = from tenancies_row in tenancies.AsEnumerable()
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

        public static CalcDataModeTenancyAggregated GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModeTenancyAggregated();
            return dataModel;
        }
    }
}
