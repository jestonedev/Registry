using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Registry.DataModels;
using System.Globalization;
using Registry.Entities;

namespace Registry.CalcDataModels
{
    public sealed class CalcDataModelTenancyPremisesInfo : CalcDataModel
    {

        private static string tableName = "tenancy_premises_info";

        private CalcDataModelTenancyPremisesInfo()
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
            table.Columns.Add("total_area").DataType = typeof(double);
            table.Columns.Add("living_area").DataType = typeof(double);
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelTenancyPremisesInfo");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Вычисляем агрегационную информацию
            var assoc_sub_premises = DataModelHelper.FilterRows(TenancySubPremisesAssocDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var assoc_premises = DataModelHelper.FilterRows(TenancyPremisesAssocDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var assoc_buildings = DataModelHelper.FilterRows(TenancyBuildingsAssocDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var kladr_street = DataModelHelper.FilterRows(KladrStreetsDataModel.GetInstance().Select());
            var buildings = DataModelHelper.FilterRows(BuildingsDataModel.GetInstance().Select());
            var premises = DataModelHelper.FilterRows(PremisesDataModel.GetInstance().Select());
            var sub_premises = DataModelHelper.FilterRows(SubPremisesDataModel.GetInstance().Select());

            var a_sub_premises = from assoc_sub_premises_row in assoc_sub_premises
                                    join sub_premises_row in sub_premises
                                    on assoc_sub_premises_row.Field<int>("id_sub_premises") equals sub_premises_row.Field<int>("id_sub_premises")
                                    join premises_row in premises
                                    on sub_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                    join buildings_row in buildings
                                    on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    join kladr_street_row in kladr_street
                                    on buildings_row.Field<string>("id_street") equals kladr_street_row.Field<string>("id_street")
                                    select new
                                    {
                                        id_process = assoc_sub_premises_row.Field<int>("id_process"),
                                        address = kladr_street_row.Field<string>("street_name") + ", дом " + buildings_row.Field<string>("house") +
                                            (premises_row.Field<int>("id_premises_type") == 2 ? " ком. " : " кв. ") + premises_row.Field<string>("premises_num") +
                                            " ком. " + sub_premises_row.Field<string>("sub_premises_num"),
                                        total_area = sub_premises_row.Field<double>("total_area"),
                                        living_area = sub_premises_row.Field<double>("living_area")
                                    };
            var a_premises = from assoc_premises_row in assoc_premises
                                    join premises_row in premises
                                    on assoc_premises_row.Field<int>("id_premises") equals premises_row.Field<int>("id_premises")
                                    join buildings_row in buildings
                                    on premises_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                                    join kladr_street_row in kladr_street
                                    on buildings_row.Field<string>("id_street") equals kladr_street_row.Field<string>("id_street")
                                    select new
                                    {
                                        id_process = assoc_premises_row.Field<int>("id_process"),
                                        address = kladr_street_row.Field<string>("street_name") + ", дом " + buildings_row.Field<string>("house") +
                                            (premises_row.Field<int>("id_premises_type") == 2 ? " ком. " : " кв. ") + premises_row.Field<string>("premises_num"),
                                        total_area = premises_row.Field<double>("total_area"),
                                        living_area = premises_row.Field<double>("living_area")
                                    };
            var a_buildings = from assoc_buildings_row in assoc_buildings
                             join buildings_row in buildings
                             on assoc_buildings_row.Field<int>("id_building") equals buildings_row.Field<int>("id_building")
                             join kladr_street_row in kladr_street
                             on buildings_row.Field<string>("id_street") equals kladr_street_row.Field<string>("id_street")
                             select new
                             {
                                 id_process = assoc_buildings_row.Field<int>("id_process"),
                                 address = kladr_street_row.Field<string>("street_name") + ", дом " + buildings_row.Field<string>("house"),
                                 total_area = buildings_row.Field<double>("total_area"),
                                 living_area = buildings_row.Field<double>("living_area")
                             };
            var result = a_buildings.Union(a_premises).Union(a_sub_premises);

            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_process, 
                    x.address, 
                    x.total_area,
                    x.living_area
                });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelTenancyPremisesInfo GetInstance()
        {
            return new CalcDataModelTenancyPremisesInfo();
        }

        public static bool HasInstance()
        {
            return false;
        }
    }
}
