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
            table.Columns.Add("id_contract").DataType = typeof(int);
            table.Columns.Add("address").DataType = typeof(string);
            table.Columns.Add("tenant").DataType = typeof(string);
            table.PrimaryKey = new DataColumn[] { table.Columns["id_contract"] };
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
            var tenancies = from tenancies_row in TenancyContractsDataModel.GetInstance().Select().AsEnumerable()
                            where (tenancies_row.RowState != DataRowState.Deleted) &&
                                  (tenancies_row.RowState != DataRowState.Detached) &&
                                  (config.Entity == CalcDataModelFilterEnity.Tenancy ? 
                                  tenancies_row.Field<int>("id_contract") == config.IdObject : true)
                            select tenancies_row;
            var tenancy_sub_premises = from tenancy_sub_premises_row in TenancySubPremisesAssocDataModel.GetInstance().Select().AsEnumerable()
                                       where (tenancy_sub_premises_row.RowState != DataRowState.Deleted) &&
                                             (tenancy_sub_premises_row.RowState != DataRowState.Detached)
                                       select tenancy_sub_premises_row; 
            var tenancy_premises = from tenancy_premises_row in TenancyPremisesAssocDataModel.GetInstance().Select().AsEnumerable()
                                   where (tenancy_premises_row.RowState != DataRowState.Deleted) &&
                                         (tenancy_premises_row.RowState != DataRowState.Detached) &&
                                          (config.Entity == CalcDataModelFilterEnity.Premise ?
                                          tenancy_premises_row.Field<int>("id_premises") == config.IdObject : true)
                                   select tenancy_premises_row; 
            var tenancy_buildings = from tenancy_buildings_row in TenancyBuildingsAssocDataModel.GetInstance().Select().AsEnumerable()
                                    where (tenancy_buildings_row.RowState != DataRowState.Deleted) &&
                                          (tenancy_buildings_row.RowState != DataRowState.Detached) &&
                                          (config.Entity == CalcDataModelFilterEnity.Building ? 
                                          tenancy_buildings_row.Field<int>("id_building") == config.IdObject : true)
                                    select tenancy_buildings_row; 
            var buildings = from buildings_row in BuildingsDataModel.GetInstance().Select().AsEnumerable()
                            where (buildings_row.RowState != DataRowState.Deleted) &&
                                  (buildings_row.RowState != DataRowState.Detached)
                            select buildings_row; 
            var premises = from premises_row in PremisesDataModel.GetInstance().Select().AsEnumerable()
                           where (premises_row.RowState != DataRowState.Deleted) &&
                                 (premises_row.RowState != DataRowState.Detached)
                           select premises_row;
            var sub_premises = from sub_premises_row in SubPremisesDataModel.GetInstance().Select().AsEnumerable()
                               where (sub_premises_row.RowState != DataRowState.Deleted) &&
                                     (sub_premises_row.RowState != DataRowState.Detached)
                               select sub_premises_row;
            var persons = from persons_row in PersonsDataModel.GetInstance().Select().AsEnumerable()
                          where (persons_row.RowState != DataRowState.Deleted) &&
                                (persons_row.RowState != DataRowState.Detached)
                          select persons_row;
            var kladr_street = from kladr_street_row in KladrStreetsDataModel.GetInstance().Select().AsEnumerable()
                               where (kladr_street_row.RowState != DataRowState.Deleted) &&
                                     (kladr_street_row.RowState != DataRowState.Detached)
                               select kladr_street_row;
            // Вычисляем агрегационную информацию
            var tenants = from persons_row in persons.AsEnumerable()
                          where persons_row.Field<int?>("id_kinship") == 1
                          group persons_row by persons_row.Field<int>("id_contract") into gs
                          select new 
                          {
                              id_contract = gs.Key,
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
                                            id_contract = tenancy_sub_premises_row.Field<int>("id_contract"),
                                            id_building = premises_row.Field<int>("id_building"),
                                            id_premises = sub_premises_row.Field<int>("id_premises"),
                                            premises_num = premises_row.Field<string>("premises_num")
                                        } into gs
                                    select new
                                        {
                                            id_contract = gs.Key.id_contract,
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
                                    id_contract = tenancy_premises_row.Field<int>("id_contract"),
                                    id_building = premises_row.Field<int>("id_building"),
                                    id_premises = tenancy_premises_row.Field<int>("id_premises"),
                                    result_str = " пом " + premises_row.Field<string>("premises_num")
                                };
            var t_buildings = from tenancy_buildings_row in tenancy_buildings.AsEnumerable()
                              where tenancy_buildings_row.RowState != DataRowState.Detached &&
                                    tenancy_buildings_row.RowState != DataRowState.Deleted
                              select new 
                              {
                                  id_contract = tenancy_buildings_row.Field<int>("id_contract"),
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
                                              tenancy_premises_row.id_contract,
                                              tenancy_premises_row.id_building
                                          } into gs
                                      select new
                                          {
                                              id_contract = gs.Key.id_contract,
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
                         by tenancy_row.id_contract into gs
                         select new
                             {
                                 id_contract = gs.Key,
                                 address = gs.Aggregate((a, b) =>
                                 {
                                     return a + ", " + b.Trim();
                                 })
                             };
            var result = from tenancies_row in tenancies.AsEnumerable()
                         join tenants_row in tenants
                         on tenancies_row.Field<int>("id_contract") equals tenants_row.id_contract into sp_t
                         from sp_t_row in sp_t.DefaultIfEmpty()
                         join addresses_row in addresses
                         on tenancies_row.Field<int>("id_contract") equals addresses_row.id_contract into sp_a
                         from sp_a_row in sp_a.DefaultIfEmpty()
                         select new
                         {
                             id_contract = tenancies_row.Field<int>("id_contract"),
                             address = (sp_a_row == null) ? "" : sp_a_row.address,
                             tenant = (sp_t_row == null) ? "" : sp_t_row.tenant
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_contract, 
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
